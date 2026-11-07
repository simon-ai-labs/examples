using System.Text.Json;
using System.Text.Json.Nodes;

namespace MinimumTaxServices.Samples.Services;

/// <summary>
/// Builds request JSON from a JSON Schema at runtime, so the sample needs no request or response classes.
/// The REST service passes schemas from the OpenAPI description that /docs points to, the MCP service
/// passes each tool's input schema. Values are addressed by dotted property paths such as
/// <c>document.expectedRepresentation.syntax</c>; the schema decides the nesting, the property names,
/// the allowed enum values and which properties are required.
/// </summary>
public sealed class SchemaRequestBuilder(JsonNode root)
{
    public JsonObject Build(JsonNode schema, IReadOnlyDictionary<string, object?> values)
    {
        var request = new JsonObject();
        var supplied = new HashSet<JsonNode>(ReferenceEqualityComparer.Instance);
        foreach (var (path, value) in values)
        {
            Set(request, schema, path.Split('.'), 0, path, value, supplied);
        }

        CheckRequired(request, schema, "", supplied);
        return request;
    }

    private void Set(JsonObject target, JsonNode schema, string[] segments, int index, string path, object? value, HashSet<JsonNode> supplied)
    {
        var name = segments[index];
        var propertySchema = Resolve(schema)["properties"]?[name]
            ?? throw new InvalidOperationException($"'{path}' is not described by the schema.");

        if (index < segments.Length - 1)
        {
            if (target[name] is not JsonObject child)
            {
                child = new JsonObject();
                target[name] = child;
            }

            Set(child, propertySchema, segments, index + 1, path, value, supplied);
            return;
        }

        var node = ToNode(Resolve(propertySchema), path, value);
        if (node is not null && value is JsonNode)
        {
            supplied.Add(node);
        }

        target[name] = node;
    }

    private static JsonNode? ToNode(JsonNode schema, string path, object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is JsonNode node)
        {
            return node.DeepClone();
        }

        if (value is byte[] bytes)
        {
            return JsonValue.Create(Convert.ToBase64String(bytes));
        }

        if (schema["enum"] is JsonArray allowed && value is string text)
        {
            // The description owns the spelling: "xrechnung" becomes "xRechnung" if that is what it lists.
            var match = allowed.FirstOrDefault(candidate =>
                string.Equals(candidate?.ToString(), text, StringComparison.OrdinalIgnoreCase));
            return match?.DeepClone() ?? throw new InvalidOperationException(
                $"'{text}' is not allowed for '{path}'. Allowed: {string.Join(", ", allowed)}.");
        }

        return JsonSerializer.SerializeToNode(value);
    }

    private void CheckRequired(JsonObject target, JsonNode schema, string path, HashSet<JsonNode> supplied)
    {
        var resolved = Resolve(schema);
        foreach (var required in resolved["required"]?.AsArray() ?? [])
        {
            var name = required!.GetValue<string>();
            if (!target.ContainsKey(name) && !IsNullable(resolved["properties"]?[name]))
            {
                throw new InvalidOperationException($"The schema requires '{path}{name}'.");
            }
        }

        foreach (var (name, value) in target)
        {
            if (value is JsonObject child && !supplied.Contains(child) && resolved["properties"]?[name] is { } childSchema)
            {
                CheckRequired(child, childSchema, $"{path}{name}.", supplied);
            }
        }
    }

    /// <summary>Follows <c>$ref</c> and picks the non-null branch of <c>oneOf</c>/<c>anyOf</c>.</summary>
    private JsonNode Resolve(JsonNode schema)
    {
        while (true)
        {
            if (schema["$ref"]?.GetValue<string>() is { } reference)
            {
                schema = Pointer(reference);
                continue;
            }

            var branches = (schema["oneOf"] ?? schema["anyOf"])?.AsArray()
                .Where(branch => branch?["type"]?.ToString() != "null")
                .ToList();
            if (branches is [{ } single])
            {
                schema = single;
                continue;
            }

            return schema;
        }
    }

    private JsonNode Pointer(string reference)
    {
        if (!reference.StartsWith("#/", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Only local schema references are supported: {reference}");
        }

        var node = root;
        foreach (var segment in reference[2..].Split('/'))
        {
            node = node[segment.Replace("~1", "/").Replace("~0", "~")]
                ?? throw new InvalidOperationException($"The schema reference {reference} cannot be resolved.");
        }

        return node;
    }

    private bool IsNullable(JsonNode? schema)
    {
        if (schema is null)
        {
            return true;
        }

        if (schema["nullable"]?.GetValue<bool>() == true
            || schema["type"] is JsonArray types && types.Any(type => type?.ToString() == "null")
            || (schema["oneOf"] ?? schema["anyOf"]) is JsonArray branches && branches.Any(branch => branch?["type"]?.ToString() == "null"))
        {
            return true;
        }

        return schema["$ref"] is not null && IsNullable(Pointer(schema["$ref"]!.GetValue<string>()));
    }
}

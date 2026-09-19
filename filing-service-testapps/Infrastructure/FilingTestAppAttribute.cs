namespace FilingService.TestApps.Infrastructure;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class FilingTestAppAttribute(string name, string routePrefix) : Attribute
{
    public string Name { get; } = name;
    public string RoutePrefix { get; } = routePrefix;
}


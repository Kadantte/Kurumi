using System.Reflection;

namespace nhitomi;

public static class VersionHelper
{
    private static Assembly Assembly => typeof(Startup).Assembly;

    public static Version Version => Assembly.GetName().Version ?? new Version(3, 4);

    public static string Codename =>
        Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Heresta";
}

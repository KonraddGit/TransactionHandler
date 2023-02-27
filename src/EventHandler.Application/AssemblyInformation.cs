using System.Reflection;

namespace EventHandler.Application
{
    public static class AssemblyInformation
    {
        private static readonly AssemblyName? _assembly = Assembly.GetEntryAssembly()?.GetName();

        public static string? SoftwareVersion
            => _assembly?.Version?.ToString();

        public static string? SoftwareName
            => _assembly?.Name;
    }
}

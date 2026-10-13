using Parcs;

namespace BentleyOttmannModule
{
    // Simple implementation of IModuleOptions.
    // For production pass ServerIp of the Azure HostServer VM.
    public class ModuleOptions : IModuleOptions
    {
        public int    Priority  { get; set; } = 0;
        public string Username  { get; set; } = "student";
        public string ServerIp  { get; set; } = null;
    }
}

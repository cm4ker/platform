namespace ZenPlatform.Configuration.Structure
{
    public interface IXCPlatformRule
    {
        bool IsDataAdministrator { get; set; }
        bool CanUpdateDatabase { get; set; }
        bool CanMonopolisticMode { get; set; }
        bool IsActiveUser { get; set; }
        bool CanActionLogView { get; set; }
        bool CanUseThinClient { get; set; }
        bool CanUseWebClient { get; set; }
        bool CanUseExternalConnection { get; set; }
        bool CanOpenExternalModules { get; set; }
        IXCRole Parent { get; }
    }
}
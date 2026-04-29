using MediaBrowser.Model.Plugins;

namespace RandomUnwatchedShowPlugin.Configuration;

public enum WatchedStatusFilter
{
    Any,
    Partial,
    New
}

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        WatchedStatusFilter = WatchedStatusFilter.New;
    }
    
    public string? Library { get; set; }
    
    public WatchedStatusFilter WatchedStatusFilter { get; set; }
}
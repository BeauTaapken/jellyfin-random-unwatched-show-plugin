using System.Globalization;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using RandomUnwatchedShowPlugin.Configuration;
using RandomUnwatchedShowPlugin.FileTransformation;

namespace RandomUnwatchedShowPlugin;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    private readonly IApplicationPaths _appPaths;
    private readonly ILogger<Plugin> _logger;
    
    private string CurrentVersion => GetType().Assembly.GetName().Version?.ToString() ?? "1.0.0.0";

    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger) 
        : base(applicationPaths, xmlSerializer)
    {
        _appPaths = applicationPaths;
        _logger = logger;
        
        Injector injector = new Injector();
        Task.Delay(5000).ContinueWith(_ => injector.InjectScript(CurrentVersion, _appPaths, _logger));
    }

    
    
    public override string Name => "RandomShowSelector";
    public override Guid Id => Guid.Parse("3c913dbb-2219-4e15-92b7-22f004d10a7d");
    
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return
        [
            new PluginPageInfo
            {
                Name = "Random unwatched show selector",
                EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
            }
        ];
    }
 
}
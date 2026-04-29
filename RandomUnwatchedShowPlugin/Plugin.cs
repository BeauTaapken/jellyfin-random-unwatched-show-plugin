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
    private readonly ILogger _logger;
    
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogger<Plugin> logger) : base(applicationPaths,
        xmlSerializer)
    {
        Instance = this;
        _logger = logger;
        
        FileTransformationHandler fileTransformationHandler = new FileTransformationHandler();
        Task.Delay(1000).ContinueWith(_ => fileTransformationHandler.RegisterWithFileTransformation(_logger));
    }
    
    
    public override string Name => "RandomShowSelector";
    public override Guid Id => Guid.Parse("3c913dbb-2219-4e15-92b7-22f004d10a7d");
    
    public static Plugin? Instance { get; set; }

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
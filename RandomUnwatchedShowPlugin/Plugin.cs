using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Serialization;
using RandomUnwatchedShowPlugin.Configuration;

namespace RandomUnwatchedShowPlugin;

internal class Plugin : BasePlugin<PluginConfiguration>
{ 
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer){}
    public override string Name => "Random unwatched show selector";
    public override Guid Id => Guid.Parse("3c913dbb-2219-4e15-92b7-22f004d10a7d"); 
}
using System.Text.RegularExpressions;
using MediaBrowser.Common.Configuration;
using Microsoft.Extensions.Logging;

namespace RandomUnwatchedShowPlugin.FileTransformation;

public class Injector
{
    public void InjectScript(string currentVersion, IApplicationPaths appPaths, ILogger<Plugin> logger)
    {
        string startComment = $"<!-- RandomShowSelector Start v{currentVersion} -->";
        const string endComment = "<!-- RandomShowSelector End -->"; 
        
        var indexPath = Path.Combine(appPaths.WebPath, "index.html");
        
        if (!File.Exists(indexPath))
        {
            logger.LogError($"index.html not found at {indexPath}");
            return;
        }

        JsHelper jsHelper = new JsHelper();
        string script = $"{startComment}\n{jsHelper.GetJavascriptToInject()}\n{endComment}";

        try
        {
            var content = File.ReadAllText(indexPath);
            
            if (content.Contains(startComment))
            {
                logger.LogDebug("Script version {Version} already injected, skipping", currentVersion);
                return;
            }
            
            var regex = new Regex(@"<!-- RandomShowSelector Start v[\d\.]+ -->.*?<!-- RandomShowSelector End -->",
                RegexOptions.Singleline); 
            content = regex.Replace(content, "");
            
            content = content.Replace("</body>", script + "\n</body>");
            File.WriteAllText(indexPath, content);
            logger.LogInformation("Successfully injected button!");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to inject script");
        }
    }
}
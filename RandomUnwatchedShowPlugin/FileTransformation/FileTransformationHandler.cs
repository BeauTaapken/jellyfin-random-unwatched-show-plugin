using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace RandomUnwatchedShowPlugin.FileTransformation;

public class FileTransformationHandler
{
    public void RegisterWithFileTransformation(ILogger logger)
    {
        try
        {
            logger.LogInformation("Attempting to register with File Transformation...");
        
            // Find the File Transformation assembly
            var fileTransformationAssembly = AssemblyLoadContext.All
                .SelectMany(x => x.Assemblies)
                .FirstOrDefault(x => x.FullName?.Contains(".FileTransformation") == true);

            if (fileTransformationAssembly == null)
            {
                logger.LogWarning("File Transformation plugin not found. Button injection disabled.");
                return;
            }

            var pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");
            if (pluginInterfaceType == null)
            {
                logger.LogWarning("Could not find PluginInterface type");
                return;
            }

            // Build the payload according to the docs
            var payload = JObject.FromObject(new
            {
                id = Guid.NewGuid().ToString(), // Unique ID for this transformation
                fileNamePattern = "index\\.html", // Regex pattern for files to patch
                callbackAssembly = GetType().Assembly.FullName, // Your assembly
                callbackClass = "RandomUnwatchedShowPlugin.Plugin", // Full class name
                callbackMethod = "TransformHtml" // Method that will be called
            });

            // Register with File Transformation using reflection
            var registerMethod = pluginInterfaceType.GetMethod("RegisterTransformation");
            if (registerMethod != null)
            {
                registerMethod.Invoke(null, new object?[] { payload });
                logger.LogInformation("Successfully registered with File Transformation");
            }
            else
            {
                logger.LogWarning("Could not find RegisterTransformation method");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register with File Transformation");
        }
    }
    
    // This method will be called by File Transformation for every HTML file matching the pattern
    public static object TransformHtml(object request)
    {
        Console.WriteLine("TransformHtml was called!");
        try
        {
            // The request should have a "contents" property
            var requestType = request.GetType();
            var contentsProperty = requestType.GetProperty("contents");
            
            if (contentsProperty == null)
            {
                return request;
            }
            
            var contents = contentsProperty.GetValue(request) as string;
            if (string.IsNullOrEmpty(contents))
            {
                return request;
            }
        
            // Build the script to inject
            var script = @"
                <script>
                (function() {
                    function addButton() {
                        var buttonContainer = document.querySelector('.headerRight, .skinHeader .headerRight');
                        if (!buttonContainer) {
                            setTimeout(addButton, 500);
                            return;
                        }

                               const searchButton = buttonContainer.querySelector('.btnSearch, .button-search, [title=""Search""], [aria-label=""Search""]');
                        
                        if (document.getElementById('randomShowButton')) return;
                        
                        var button = document.createElement('button');
                        button.id = 'randomShowButton';
                        button.className = 'paper-icon-button-light emby-button';
                        button.setAttribute('title', 'Random Show');
                        button.setAttribute('aria-label', 'Random Show');
                        
                        var icon = document.createElement('span');
                        icon.className = 'material-icons';
                        icon.textContent = 'shuffle';
                        button.appendChild(icon);
                        
                        button.onclick = function() {
                            alert('Random Show Selector - Your custom action here!');
                        };
                        buttonContainer.insertBefore(button, searchButton);
                    }
                    
                    if (document.readyState === 'loading') {
                        document.addEventListener('DOMContentLoaded', addButton);
                    } else {
                        addButton();
                    }
                })();
                </script>";
            
            // Inject the script before </body>
            if (contents.Contains("</body>"))
            {
                var modifiedContents = contents.Replace("</body>", script + "\n</body>");
                
                // Return the modified contents in the expected format
                return new
                {
                    contents = modifiedContents
                };
            }
            
            return request;
        }
        catch (Exception ex)
        {
            // Log error (you'll need access to logger - consider using static logger or Console)
            Console.WriteLine($"TransformHtml error: {ex.Message}");
            return request;
        }
    }
}
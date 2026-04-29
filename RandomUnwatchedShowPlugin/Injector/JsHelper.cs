using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace RandomUnwatchedShowPlugin.FileTransformation;

public class JsHelper
{
    public string GetJavascriptToInject()
    {
        return @"<script>
            (function() {
                function addButton() {
                    var buttonContainer = document.querySelector('.headerRight, .skinHeader .headerRight');
                    if (!buttonContainer) {
                        setTimeout(addButton, 500);
                        return;
                    }

                    var searchButton = buttonContainer.querySelector('.btnSearch, .button-search, [title=""Search""], [aria-label=""Search""]');
                    
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
                        fetch('/Items?IsPlayed=false&Recursive=true&IncludeItemTypes=Series')
                            .then(r => r.json())
                            .then(data => {
                                var items = data.Items || [];
                                if (items.length) {
                                    var random = items[Math.floor(Math.random() * items.length)];
                                    location.href = '/web/index.html#!/item?id=' + random.Id;
                                }
                            });
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
    }
}
using Playnite.SDK;
using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace ThemeOptions
{

    //based on code from lacro59 from
    //https://github.com/Lacro59/playnite-plugincommon/blob/master/Localization.cs
    //
    public class Localization
    {
        private static readonly ILogger Logger = LogManager.GetLogger();

        public static void Load(string folder, string language = "en_US")
        {
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var langFile = Path.Combine(folder, "Localization", language + ".xaml");

            // Load localization
            if (File.Exists(langFile))
            {
                ResourceDictionary res;
                try
                {
                    using (var stream = new StreamReader(langFile))
                    {
                        res = (ResourceDictionary)XamlReader.Load(stream.BaseStream);
                        res.Source = new Uri(langFile, UriKind.Absolute);
                    }

                    foreach (var key in res.Keys)
                    {
                        if (res[key] is string locString && string.IsNullOrEmpty(locString))
                        {
                            res.Remove(key);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"Failed to parse localization file {langFile}.");
                    return;
                }

                dictionaries.Add(res);
            }
            else
            {
                Logger.Warn($"File {langFile} not found.");
            }
        }
    }
}

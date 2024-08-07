using System;
using System.IO;
using System.Collections.Generic;

using System.Linq;

using Playnite.SDK;
using Playnite.SDK.Data;

namespace ThemeOptions.Models
{
    public class Theme
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public Options Options { get; set; }

        public List<Preset> PresetList { get => Options.Presets?.Values.ToList() ?? new List<Preset>(); }
        public List<Variable> VariablesList { get => Options.Variables?.Values.ToList() ?? new List<Variable>(); }

        private static readonly ILogger logger = LogManager.GetLogger();
        public static Theme FromFile(string path)
        {
            if (path == null)
            {
                return null;
            }

            path = path.EndsWith("theme.yaml") ? path : System.IO.Path.Combine(path, "theme.yaml");

            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                var theme = Serialization.FromYamlFile<Theme>(path);
                theme.Path =  System.IO.Path.GetDirectoryName(path);
                theme.Options = Options.FromFile(System.IO.Path.Combine(theme.Path, "options.yaml"));
                return theme;
            }
            catch (Exception e)
            {
                logger.Error($"Error loading theme {path}: \n{e.Message}");
                return null;
            }
        }
        public static Theme FromId(string id)
        {
            return FromFile(GetThemePath(id));
        }

        private static string GetThemePath(string id)
        {
            var theme = FindTheme(id);
            return theme?.Path;
        }
        public static IEnumerable<Theme> EnumThemes()
        {
            var themesRoot = new List<string>();
            if (!ThemeOptions.PlayniteAPI.ApplicationInfo.IsPortable)
            {
                themesRoot.Add(ThemeOptions.PlayniteAPI.Paths.ConfigurationPath);
            }

            themesRoot.AddMissing(ThemeOptions.PlayniteAPI.Paths.ApplicationPath);

            var modes = new List<string> { "Fullscreen", "Desktop" };

            foreach (var root in themesRoot)
            {
                foreach (var mode in modes)
                {
                    var themesFolder = System.IO.Path.Combine(root, "Themes", mode);
                    if (Directory.Exists(themesFolder))
                    {
                        foreach (var themePath in Directory.EnumerateDirectories(themesFolder))
                        {
                            Theme theme = Theme.FromFile(System.IO.Path.Combine(themePath, "theme.yaml"));
                            if (theme != null && theme.Options != null) yield return theme;
                        }
                    }
                }
            }
        }
        private static Theme FindTheme(string themeId) => EnumThemes().FirstOrDefault(theme => theme.Id == themeId);

        public void TranslateOptions()
        {
            Options?.Translate(Path, ThemeOptions.PlayniteAPI.ApplicationSettings.Language);
        }
    }
}
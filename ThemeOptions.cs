using Playnite;
using Playnite.SDK;
using Playnite.SDK.Plugins;
using Playnite.SDK.Events;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using ThemeOptions.Models;
using ThemeOptions.Views;


namespace ThemeOptions
{
    public class ThemeOptions: GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private static readonly string PluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static IPlayniteAPI PlayniteAPI { get; private set; }
        public static string ThemesPath { get; private set; }

        public static SettingsViewModel Settings { get; set; }

        public override Guid Id { get; } = Guid.Parse("904cbf3b-573f-48f8-9642-0a09d05c64ef");

        public void LoadResource(string resourceFile, bool mandatory=true)
        {
                if (File.Exists(resourceFile))
                {
                    try
                    {
                        using (var stream = new StreamReader(resourceFile))
                        {
                            ResourceDictionary resource = (ResourceDictionary)XamlReader.Load(stream.BaseStream);
                            resource.Source = new Uri(resourceFile, UriKind.Absolute);
                            Application.Current.Resources.MergedDictionaries.Add(resource);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error($"Error: '{e.Message}' during loading resource {resourceFile} ");
                    }
                }
                else if (mandatory)
                {
                    logger.Error($"File not found {resourceFile}");
                }
                else
                {
                    logger.Info($"Optional file not found {resourceFile}");
                }
        }
        public void LoadThemeOption()
        {
            string themeId =
                PlayniteAPI.ApplicationInfo.Mode == ApplicationMode.Fullscreen
                    ? PlayniteAPI.ApplicationSettings.FullscreenTheme
                    : PlayniteAPI.ApplicationSettings.DesktopTheme;

            Theme theme = Theme.FromId(themeId);
            if (theme == null) return;

            if (theme.Options != null)
            {
                List<string> selectedPresets = Settings.ThemePresets(theme.Id);
                if (selectedPresets?.Count > 0)
                {
                    var presets = String.Join(", ", selectedPresets);
                    logger.Info($"Loading presets {presets}");
                    foreach (var presetResource in theme.Options.Presets.GetResourceFiles(selectedPresets))
                    {
                        logger.Info($"Loading resource {presetResource}");
                        LoadResource(Path.Combine(theme.Path, presetResource));
                    }
                }
            }

            if (PlayniteAPI.ApplicationSettings.Language != "en_US")
            {
                logger.Info($"Loading theme localization {PlayniteAPI.ApplicationSettings.Language}");
                Localization.Load(theme.Path, PlayniteAPI.ApplicationSettings.Language);
            }

            // TODO: Generate and loads user defined Resources
        }

        public ThemeOptions(IPlayniteAPI api) : base(api)
        {
            PlayniteAPI = api;
            ThemesPath = Path.Combine(PlayniteAPI.Paths.ConfigurationPath,"Themes");
            Settings = new SettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            Localization.Load(PluginFolder, PlayniteAPI.ApplicationSettings.Language);
            LoadThemeOption();
        }

        public Options FromFile(string optionsFile)
        {
            if (!File.Exists(optionsFile)) return null;

            try
            {
                return Serialization.FromYamlFile<Options>(optionsFile);
            }
            catch (Exception e)
            {
                logger.Error($"Error loading theme option file {optionsFile}: \n{e.Message}");
                return null;
            }
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new SettingsView();
        }
    }
}
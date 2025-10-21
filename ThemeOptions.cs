using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;

using Playnite.SDK;
using Playnite.SDK.Plugins;
using Playnite.SDK.Data;

using ThemeOptions.Models;
using ThemeOptions.Views;
using ThemeOptions.Controls;
using Playnite.SDK.Events;
using ThemeOptions.Tools;


namespace ThemeOptions
{
    public class ThemeOptions : GenericPlugin
    {
        private static readonly ILogger logger = LogManager.GetLogger();

        public static string PluginVersion;
        private static readonly string PluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static IPlayniteAPI PlayniteAPI { get; private set; }

        public static SettingsViewModel Settings { get; set; }
        public Views.SettingsView SettingsView { get; private set; }
        public static Window CustomWindow { get; private set; }

        public string CurrentThemeId;

        public Extensions MissingExtensions { get; private set; } = new Extensions();

        public override Guid Id { get; } = Guid.Parse("904cbf3b-573f-48f8-9642-0a09d05c64ef");
        List<ResourceDictionary> themeResources = new List<ResourceDictionary>();

        public ResourceDictionary LoadResource(string resourceFile, bool mandatory = true)
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
                        return resource;
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
            return null;
        }
        public void LoadThemeOption()
        {
            foreach (ResourceDictionary resource in themeResources)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resource);
            }
            themeResources.Clear();

            Theme theme = Theme.FromId(CurrentThemeId);
            if (theme == null) return;

            if (theme.Options?.Presets?.Count > 0)
            {
                List<string> selectedPresets = Settings.ThemePresets(theme.Id);
                if (selectedPresets?.Count > 0)
                {
                    var presets = String.Join(", ", selectedPresets);
                    logger.Info($"Loading presets {presets}");
                    foreach (var presetResource in theme.Options.Presets.GetResourceFiles(selectedPresets))
                    {
                        logger.Info($"Loading resource {presetResource}");
                        if (LoadResource(Path.Combine(theme.Path, presetResource)) is ResourceDictionary resource)
                        {
                            themeResources.Add(resource);
                        }
                    }
                }
            }


            var missingRequired = new List<string>();
            var missingRecommended = new List<string>();

            var installedExtensions = PlayniteAPI.Addons.Addons;
            if (theme.Extensions?.Required?.Count > 0)
            {
                foreach (var ext in theme.Extensions.Required)
                {
                    if (!installedExtensions.Exists(a => a.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        missingRequired.Add(ext);
                    }
                }
            }

            if(theme.Extensions?.Recommended?.Count > 0)
            {
                foreach (var ext in theme.Extensions.Recommended)
                {
                    if (!installedExtensions.Exists(a => a.Equals(ext, StringComparison.OrdinalIgnoreCase)))
                    {
                        missingRecommended.Add(ext);
                    }
                }
            }

            MissingExtensions = new Extensions
            {
                Required = missingRequired,
                Recommended = missingRecommended
            };

            if (PlayniteAPI.ApplicationSettings.Language != "en_US")
            {
                logger.Info($"Loading theme localization {PlayniteAPI.ApplicationSettings.Language}");
                Localization.Load(theme.Path, PlayniteAPI.ApplicationSettings.Language);
            }

            if (theme.Options != null)
            {
                var themeSettings = Settings.ThemeSettings(theme.Id);
                if (themeSettings != null && themeSettings.Count > 0)
                {
                    try
                    {
                        var settingsText = themeSettings.FormatResourceDictionary();
                        logger.Debug("FormatResourceDictionary:\n" + settingsText);
                        ResourceDictionary resource = (ResourceDictionary)XamlReader.Parse(settingsText);
                        Application.Current.Resources.MergedDictionaries.Add(resource);
                        themeResources.Add(resource);
                    }
                    catch (Exception e)
                    {
                        logger.Error($"Error while parsing settings: {e.Message}");
                    }
                }
            }
        }

        public static void OpenWindow(string styleName)
        {
            var parent = PlayniteAPI.Dialogs.GetCurrentAppWindow();
            CustomWindow = PlayniteAPI.Dialogs.CreateWindow(new WindowCreationOptions
            {
                ShowMinimizeButton = false
            });

            CustomWindow.Height = parent.Height;
            CustomWindow.Width = parent.Width;
            CustomWindow.Title = "MoData";

            string xamlString = $@"
            <Viewbox Stretch=""Uniform"" 
                     xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                     xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                     xmlns:pbeh=""clr-namespace:Playnite.Behaviors;assembly=Playnite"">
                <Grid Width=""1920"" Height=""1080"">
                    <ContentControl x:Name=""CustomWindow""
                                    Focusable=""False""
                                    Style=""{{DynamicResource {styleName}}}"" />
                </Grid>
            </Viewbox>";

            // Parse the XAML string
            var element = (FrameworkElement)XamlReader.Parse(xamlString);


            CustomWindow.Content = element;

            CustomWindow.DataContext = parent.DataContext;

            CustomWindow.Owner = parent;
            CustomWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            CustomWindow.PreviewKeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                {
                    CustomWindow.Close();
                    CustomWindow = null;
                    e.Handled = true;
                }
            };

            CustomWindow.ShowDialog();
        }

        public ThemeOptions(IPlayniteAPI api) : base(api)
        {
            PlayniteAPI = api;
            CurrentThemeId =
                PlayniteAPI.ApplicationInfo.Mode == ApplicationMode.Fullscreen
                    ? PlayniteAPI.ApplicationSettings.FullscreenTheme
                    : PlayniteAPI.ApplicationSettings.DesktopTheme;

            Settings = new SettingsViewModel(this);
            Properties = new GenericPluginProperties
            {
                HasSettings = true
            };
            Localization.Load(PluginFolder, PlayniteAPI.ApplicationSettings.Language);
            LoadThemeOption();
            if (PlayniteAPI.ApplicationInfo.Mode == ApplicationMode.Fullscreen)
            {
                FullscreenSettingsView.Init();
            }

            AddSettingsSupport(new AddSettingsSupportArgs
            {
                SourceName = "ThemeOptions",
                SettingsRoot = $"{nameof(Settings)}.{nameof(Settings.Settings)}"
            });


            AddCustomElementSupport(new AddCustomElementSupportArgs
            {
                SourceName = "ThemeOptions",
                ElementList = new List<string>
                {
                    "Command",
                    "GamepadAdd",
                    "GamepadAlt",
                    "GamepadAddGlobal",
                    "GamepadAltGlobal"
                }
            });
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
            if (SettingsView == null)
                SettingsView = new Views.SettingsView();
            return SettingsView as UserControl;
        }

        public override Control GetGameViewControl(GetGameViewControlArgs args)
        {
            var strArgs = args.Name.Split('_');

            var controlType = strArgs[0];

            switch (controlType)
            {
                case "Command":
                    return new CommandControl();
                case "GamepadAdd":
                    return new GamepadAltControl(suppressDefaults: false);
                case "GamepadAlt":
                    return new GamepadAltControl(suppressDefaults: true);
                case "GamepadAddGlobal":
                    return new GamepadAltControl(suppressDefaults: false, global: true);
                case "GamepadAltGlobal":
                    return new GamepadAltControl(suppressDefaults: true, global: true);
                default:
                    throw new ArgumentException($"Unrecognized controlType '{controlType}' for request '{args.Name}'");
            }
        }

        public override void OnApplicationStarted(OnApplicationStartedEventArgs args)
        {
            if (MissingExtensions.Required.Count > 0)
            {
                var result = PlayniteAPI.Dialogs.ShowMessage(
                    $"You are missing {MissingExtensions.Required.Count} required extensions for this theme. Would you like to open their addon pages?",
                    "Missing Extensions",
                    MessageBoxButton.YesNo
                );
                if (result == MessageBoxResult.Yes)
                {
                    foreach (var ext in MissingExtensions.Required)
                    {
                        System.Diagnostics.Process.Start($"https://playnite.link/addons.html#{ext}");
                    }
                }
            }  

            if (MissingExtensions.Recommended.Count > 0)
            {
                PlayniteAPI.Notifications.Add(
                    new NotificationMessage(
                        "ThemeOptions-MissingExtensions",
                        $"You are missing {MissingExtensions.Recommended.Count} recommended extensions for this theme.",
                        NotificationType.Info,
                        () =>
                        {
                            var result = PlayniteAPI.Dialogs.ShowMessage(
                                $"You are missing {MissingExtensions.Recommended.Count} recommended extensions for this theme. Would you like to open their addon pages?",
                                "Missing Extensions",
                                MessageBoxButton.YesNo
                            );
                            if (result == MessageBoxResult.Yes)
                            {
                                foreach (var ext in MissingExtensions.Recommended)
                                {
                                    System.Diagnostics.Process.Start($"https://playnite.link/addons.html#{ext}");
                                }
                            }
                        }
                    )
                );
            }
        }

        public override void OnApplicationStopped(OnApplicationStoppedEventArgs args)
        {
            Settings.Settings.OptionsToTheme(CurrentThemeId);
            SavePluginSettings(Settings.Settings);
        }
        public override void OnControllerButtonStateChanged(OnControllerButtonStateChangedArgs args)
        {

            if (CustomWindow != null && CustomWindow.IsVisible && args.Button == ControllerInput.B && args.State == ControllerInputState.Pressed)
            {
                CustomWindow.Close();
                CustomWindow = null;
            }
            else
            {
                Gamepad.SetState(args);
            }
        }
    }
}
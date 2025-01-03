using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.ComponentModel;

using Playnite.SDK;

using ThemeOptions.Models;


namespace ThemeOptions.Views
{
    static public class Extension
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent, string typeName=null) where T : DependencyObject
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var queue = new Queue<DependencyObject>(new[] { parent });

            while (queue.Any())
            {
                var reference = queue.Dequeue();
                var count = VisualTreeHelper.GetChildrenCount(reference);

                for (var i = 0; i < count; i++)
                {
                    var child = VisualTreeHelper.GetChild(reference, i);
                    if (child is T && (typeName == null || child.GetType().FullName == typeName))
                        yield return child as T;

                    queue.Enqueue(child);
                }
            }
        }
    }

    public class FullscreenSettingsView
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        private static readonly string PluginFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        static private Theme CurrentTheme;
        public static void Init()
        {
            CurrentTheme = Theme.FromId(ThemeOptions.PlayniteAPI.ApplicationSettings.FullscreenTheme);
            if (CurrentTheme?.Options == null)
                return;

            EventManager.RegisterClassHandler(
                typeof(Window),
                Window.LoadedEvent,
                new RoutedEventHandler((object sender, RoutedEventArgs e)=>
                    {
                        if (sender.GetType().Name != "SettingsWindow") return;
                        try
                        {
                            Load(sender as DependencyObject);
                        }
                        catch (Exception exception)
                        {
                            logger.Error(exception.Message);
                        };
                    }
                ));
        }

        public static void Load(DependencyObject parent)
        {
            dynamic ctx = (parent as Window).DataContext;
            dynamic sectionViews = ctx
                .GetType()
                .GetField("sectionViews", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(ctx);

            var assembly = Application.Current.GetType().Assembly;
            Type type = assembly.GetType("Playnite.FullscreenApp.Controls.SettingsSections.SettingsSectionControl");
            dynamic hostControl = Activator.CreateInstance(type);

            Uri resourceUri = new Uri("pack://application:,,,/ThemeOptions;component/Views/FullscreenSettingsView.xaml");

            using (Stream stream = Application.GetResourceStream(resourceUri).Stream)
            {
                UserControl control = (UserControl)XamlReader.Load(stream);

                foreach (string style in new string[] {
                    "ThemeOptionsPreviewImageGridStyle",
                    "ThemeOptionsPreviewImageStyle",
                    "SettingsSectionInputBoxStyle",
                    "SettingsSectionInputBoxTemplate"
                })
                {
                    if ( Application.Current.TryFindResource(style) != null )
                    {
                        control.Resources.Remove(style);
                    }
                }

                control.DataContext = ThemeOptions.Settings;
                hostControl.Content=control;
            }

            int nextKey = (sectionViews.Keys as IEnumerable<int>).ToList().Max() + 1;
            sectionViews[nextKey] = hostControl;

            StackPanel stack = parent.FindVisualChildren<DependencyObject>("System.Windows.Controls.StackPanel").FirstOrDefault() as StackPanel;

            DataTemplate menuTemplate = Application.Current.TryFindResource("SettingsMenuThemeOptionsButtonTemplate") as DataTemplate
                ?? ( hostControl.Content.Resources.Contains("SettingsMenuThemeOptionsButtonTemplate")
                        ? hostControl.Content.Resources["SettingsMenuThemeOptionsButtonTemplate"] as DataTemplate
                        : null);

            Type ButtonExType = assembly.GetType("Playnite.FullscreenApp.Controls.ButtonEx");
            dynamic newBtn = Activator.CreateInstance(ButtonExType);
            {
                newBtn.Content = (string)Application.Current.TryFindResource("LOCThemeOptionsSettingsMenu") ?? "Theme Options";
                newBtn.Style = Application.Current.TryFindResource("SettingsMenuButton") as Style;
                if (menuTemplate != null)  newBtn.ContentTemplate = menuTemplate;
                newBtn.Command = ctx.OpenSectionCommand;
                newBtn.CommandParameter = nextKey.ToString();
            }
            ThemeOptions.Settings.BeginFullscreenEdit(CurrentTheme.Id);

            (hostControl as UserControl).Loaded += OnLoad;
            (hostControl as UserControl).Unloaded += OnUnload;

            stack.Children.Insert(3, newBtn);
        }

        public static void OnLoad(object sender, RoutedEventArgs e)
        {
            (sender as DependencyObject)
                .FindVisualChildren<Control>()
                .FirstOrDefault(ctrl => ctrl.Focusable && ctrl.IsVisible)
                ?.Focus();
        }

        public static void OnUnload(object sender, RoutedEventArgs e)
        {
            UserControl control = (sender as UserControl).Content as UserControl;
            SettingsViewModel settings = control.DataContext as SettingsViewModel;
            settings.EndEdit();
        }

        static private Variable currentInput;
        static private dynamic InputWindow;
        public static void TextInput(Variable input)
        {
            var assembly = Application.Current.GetType().Assembly;
            Type type = assembly.GetType("Playnite.FullscreenApp.Windows.TextInputWindow");
            InputWindow = Activator.CreateInstance(type);
            currentInput = input;
            var oldValue = input.Value;
            (InputWindow as INotifyPropertyChanged).PropertyChanged += InputWindow_PropertyChanged;
            var result = InputWindow.ShowInput(ThemeOptions.PlayniteAPI.Dialogs.GetCurrentAppWindow(), input.Title, "", input.Value);
            (InputWindow as INotifyPropertyChanged).PropertyChanged -= InputWindow_PropertyChanged;
            input.Value = result.Result ? result.SelectedString : oldValue;
            currentInput = null;
            InputWindow = null;
        }

        static private void InputWindow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "InputText")
            {
                currentInput.Value = InputWindow.InputText;
            }
        }
    }
}

using Playnite.SDK;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Animation;
using ThemeOptions.Tools;

namespace ThemeOptions.Controls
{

    public partial class CommandControl
    {

        public class MainMenuCommands
        {
            const string MainMenuWindowFactory = "Playnite.FullscreenApp.Windows.MainMenuWindowFactory";
            const string MainMenuModel = "Playnite.FullscreenApp.ViewModels.MainMenuViewModel";

            dynamic FakeModel() => FakeWindow.GetModel( MainMenuWindowFactory, MainMenuModel);
            public RelayCommand CloseCommand => new RelayCommand(() => FakeModel().CloseCommand.Execute());
            public RelayCommand ExitCommand => new RelayCommand(() => FakeModel().ExitCommand.Execute());
            public RelayCommand SwitchToDesktopCommand => new RelayCommand(() => FakeModel().SwitchToDesktopCommand.Execute());
            public RelayCommand OpenSettingsCommand => new RelayCommand(() => FakeModel().OpenSettingsCommand.Execute());
            public RelayCommand SelectRandomGameCommand => new RelayCommand(() => FakeModel().SelectRandomGameCommand.Execute());
            public RelayCommand OpenPatreonCommand => new RelayCommand(() => FakeModel().OpenPatreonCommand.Execute());
            public RelayCommand OpenKofiCommand => new RelayCommand(() => FakeModel().OpenKofiCommand.Execute());
            public RelayCommand ShutdownSystemCommand => new RelayCommand(() => FakeModel().ShutdownSystemCommand.Execute());
            public RelayCommand HibernateSystemCommand => new RelayCommand(() => FakeModel().HibernateSystemCommand.Execute());
            public RelayCommand SleepSystemCommand => new RelayCommand(() => FakeModel().SleepSystemCommand.Execute());
            public RelayCommand RestartSystemCommand => new RelayCommand(() => FakeModel().RestartSystemCommand.Execute());
            public RelayCommand LockSystemCommand => new RelayCommand(() => FakeModel().LockSystemCommand.Execute());
            public RelayCommand LogoutUserCommand => new RelayCommand(() => FakeModel().LogoutUserCommand.Execute());
            public RelayCommand UpdateGamesCommand => new RelayCommand(() => FakeModel().UpdateGamesCommand.Execute());
            public RelayCommand CancelProgressCommand => new RelayCommand(() => FakeModel().CancelProgressCommand.Execute());
            public RelayCommand OpenHelpCommand => new RelayCommand(() => FakeModel().OpenHelpCommand.Execute());
            public RelayCommand MinimizeCommand => new RelayCommand(() => FakeModel().MinimizeCommand.Execute());
        }
        public MainMenuCommands MainMenu { get; } = new MainMenuCommands();

    }
}
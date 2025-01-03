using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Automation;

namespace ThemeOptions.Tools
{
    public class FakeWindow
    {
        public static dynamic GetModel(string factoryName, string modelName)
        {

            Assembly playnite = Assembly.LoadFrom(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "playnite.dll"));

            Type windowBaseType = playnite.GetType("Playnite.Controls.WindowBase");
            var windowsBase = Activator.CreateInstance(windowBaseType);

            Assembly assembly = Application.Current.GetType().Assembly;

            Type fakeFactoryType = assembly.GetType(factoryName);
            var fakeFactory = Activator.CreateInstance(fakeFactoryType);

            PropertyInfo windowProperty = fakeFactoryType.BaseType.GetProperty("Window", BindingFlags.Public | BindingFlags.Instance);
            windowProperty.GetSetMethod(true).Invoke(fakeFactory, new[] { windowsBase });

            PropertyInfo initFinishedEventProperty = fakeFactoryType.BaseType.GetProperty("initFinishedEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            AutoResetEvent initFinishedEvent = initFinishedEventProperty.GetValue(fakeFactory) as AutoResetEvent;

            initFinishedEvent.Set();

            Type fakeModelType = assembly.GetType(modelName);
            return Activator.CreateInstance(fakeModelType, new[] { fakeFactory,  Application.Current.MainWindow.DataContext});
        }
    }
}
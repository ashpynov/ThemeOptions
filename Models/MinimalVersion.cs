using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Playnite.SDK.Data;
using ThemeOptions.Tools;

namespace ThemeOptions.Models
{
    public class MinimalVersion: Dictionary<string, object>
    {
        static private string GetPluginVersion()
        {
            string pluginManifestFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Extension.yaml");
            var info = Serialization.FromYamlFile<Dictionary<string, object>>(pluginManifestFile);
            return info["Version"].ToString();
        }
        static public string PluginVersion = GetPluginVersion();

        public new object this[string version]
        {
            get => VersionComparer.MinimalVersion(version, PluginVersion);
            set { }
        }
    }

}
using System;
using System.IO;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Data;
using System.Linq;
using System.CodeDom;
using System.Runtime.InteropServices;
using System.Windows.Controls;


namespace ThemeOptions.Models
{
    public class DictNoCase<TValue> : Dictionary<string, TValue>
    {
        public DictNoCase() : base(StringComparer.OrdinalIgnoreCase) { }
    }

    public class Options
    {
        public Presets Presets { get; set; }
        public Variables Variables { get; set; }

        private static readonly ILogger logger = LogManager.GetLogger();
        static public Options FromFile(string path)
        {
            if (!File.Exists(path)) return null;

            try
            {
                var res = Serialization.FromYamlFile<Options>(path);
                res?.Presets?.PostLoad(Path.GetDirectoryName(path));
                return res;
            }
            catch (Exception e)
            {
                logger.Error($"Error loading theme option file {path}: \n{e.Message}");
                return null;
            }
        }
    }
}
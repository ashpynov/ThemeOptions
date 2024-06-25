using System;
using System.IO;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Data;
using System.Windows;


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
        public void Translate(string themePath, string language)
        {
            ResourceDictionary locale = language != "en_US" ? Localization.LoadDictionary(themePath, language) : null;
            if (locale == null) return;
            if (Variables?.Count > 0)
            {
                foreach (var v in Variables)
                {
                    if (v.Value.LocKey != null && locale.Contains(v.Value.LocKey) )
                    {
                        v.Value.Title = locale[v.Value.LocKey] as string;
                    }
                }
            }
            if (Presets?.Count > 0)
            {
                Presets.Enumerate().ForEach(p => {
                    if (p.Value.LocKey != null && locale.Contains(p.Value.LocKey))
                    {
                        p.Value.Name = locale[p.Value.LocKey] as string;
                    }
                });
            }
        }
    }
}
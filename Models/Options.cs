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
    public class Presets : DictNoCase<Preset>
    {
        /*  Gets end suboption node by dot delemeted path  */
        public Preset GetItem(string path, Func<Preset, bool> callback = null)
        {
            if (string.IsNullOrEmpty(path)) return null;
            Presets suboptions = this;
            Preset option = null;
            foreach (var p in path.Split('.'))
            {
                option = suboptions?[p];
                suboptions = option?.Presets;
                if (option == null || callback?.Invoke(option) == false)
                    break;
            }
            return option;
        }

        /* Get list of files to loaded by commas options */
        public List<string> GetResourceFiles(List<string> presets)
        {
            List<string> paths = new List<string>();
            foreach (var p in presets)
            {
                GetItem(p.Trim(), (option) =>
                {
                    if (option?.Files != null)
                    {
                        paths.RemoveAll(f => option.Files.Contains(f));
                        paths.AddRange(option.Files);
                    }
                    return true;
                });
            }
            return paths;
        }

        public void PostLoad(string themePath)
        {
            List<Preset> toProcess = new List<Preset>();

            void ProcessSubPresets( string rootId, Presets presets, List<Preset> outList)
            {
                if (presets == null) return;

                foreach(var k in presets.Keys)
                {
                    if (presets[k].Id == null)
                    {
                        presets[k].Id = rootId != null ? $"{rootId}.{k}" : k;
                    }
                    if (presets[k].Preview != null)
                    {
                        var imagePath = Path.Combine(themePath, presets[k].Preview);
                        presets[k].Preview = File.Exists(imagePath) ? imagePath : null;
                    }

                    outList.Add(presets[k]);
                }
            }

            ProcessSubPresets(null, this, toProcess);

            for (int index = 0; index < toProcess.Count; index++)
            {
                ProcessSubPresets(toProcess[index].Id, toProcess[index].Presets, toProcess);
            }
        }
    }

    public class Preset
    {
#pragma warning disable 0649
        public string Id { get; set; }
        public string Name { get; set; }
        public string Preview { get; set; }
        public Presets Presets { get; set; }
        public List<string> Files { get; set; }
#pragma warning restore 0649

        [DontSerialize]
        public List<Preset> OptionsList { get
            {
                var list = Presets.Values.ToList();
                if (list.FirstOrDefault(p => p.Id.ToLower().EndsWith("default")) == null)
                {
                    list.Insert(0, new Preset() { Id = "default", Name = "Default" });
                }
                return list;
            }
        }

        [DontSerialize]

        private Preset defaultOption { get => OptionsList.FirstOrDefault(p => p.Id.ToLower().EndsWith("default")); }

        private Preset selected;
        public Preset Selected
        {
            get => selected ?? defaultOption;
            set { selected = value; }
        }
    }

    public class Options
    {
        public Presets Presets { get; set; }

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
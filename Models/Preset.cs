
using System.Collections.Generic;
using System.Linq;

using Playnite.SDK.Data;


namespace ThemeOptions.Models
{
    public class Preset
    {
#pragma warning disable 0649
        public string Id { get; set; }
        public string Name { get; set; }
        public string LocKey { get; set; }
        public string Preview { get; set; }
        public Presets Presets { get; set; }
        public List<string> Files { get; set; }
        public VariablesValues Constants { get; set; }

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
}
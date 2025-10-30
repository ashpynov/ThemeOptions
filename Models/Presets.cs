using System;
using System.IO;
using System.Collections.Generic;



namespace ThemeOptions.Models
{
    using PresetPair = KeyValuePair<string, Preset>;
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
                option = suboptions?.Get(p);
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


        public VariablesValues GetConstants(List<string> presets)
        {
            VariablesValues variables= new VariablesValues();

            foreach (var p in presets ?? new List<string>())
            {
                GetItem(p.Trim(), (option) =>
                {
                    if (option?.Constants != null)
                    {
                        foreach (var c in option.Constants)
                            variables[c.Key] = c.Value;
                    }
                    return true;
                });
            }
            return variables;
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

        public IEnumerable<PresetPair>Enumerate()
        {
            Queue<PresetPair> toProcess = new Queue<PresetPair>();

            if (this != null )
                foreach(var p in this)
                    toProcess.Enqueue(p);

            while (toProcess.Count > 0)
            {
                PresetPair preset = toProcess.Dequeue();
                if (preset.Value.Presets != null)
                    foreach(var p in preset.Value.Presets)
                        toProcess.Enqueue(p);

                yield return preset;
            }
        }
    }
}
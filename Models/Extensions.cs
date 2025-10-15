using Playnite.SDK;
using Playnite.SDK.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThemeOptions.Models
{
    public class Extensions
    {
        private static readonly ILogger logger = LogManager.GetLogger();
        public List<string> Required { get; set; }
        public List<string> Recommended { get; set; }
        static public Extensions FromFile(string path)
        {
            if (!File.Exists(path)) return null;

            try
            {
                var res = Serialization.FromYamlFile<Extensions>(path);
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

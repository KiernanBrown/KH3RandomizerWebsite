using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UE4DataTableInterpreter.Enums;

namespace KH3Randomizer.Models
{
    public class SettingsFile
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, bool> AvailablePools { get; set; }
        public Dictionary<string, Extra> AvailableExtras { get; set; }
        public Dictionary<string, Dictionary<string, bool>> AvailableOptions { get; set; }
        public Dictionary<DataTableEnum, Dictionary<string, bool>> Replacements { get; set; }
        public List<string> KeyAbilities { get; set; }
        public List<string> DuplicateAbilities { get; set; }
        public List<string> ImportantChecks { get; set; }
        public string HintType { get; set; }
    }
}

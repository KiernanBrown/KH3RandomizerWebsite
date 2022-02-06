using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KH3Randomizer.Models
{
    public class Extra
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public bool DefaultEnabled { get; set; }
        public string RequiredPool { get; set; }
        public string Description { get; set; }

        public Extra (string name, bool value, string pool, string description)
        {
            Name = name;
            Enabled = DefaultEnabled = value;
            RequiredPool = pool;
            Description = description;
        }
    }
}

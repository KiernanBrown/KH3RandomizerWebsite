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

        public Extra (string Name, bool Enabled, string RequiredPool, string Description)
        {
            this.Name = Name;
            this.Enabled = DefaultEnabled = Enabled;
            this.RequiredPool = RequiredPool;
            this.Description = Description;
        }
    }
}

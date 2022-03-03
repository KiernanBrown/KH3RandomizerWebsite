using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KH3Randomizer.Models
{
    public class HintContainer
    {
        public string Type { get; set; }
        public List<string> ImportantChecks { get; set; }
        public byte[] Hints { get; set; }
    }
}

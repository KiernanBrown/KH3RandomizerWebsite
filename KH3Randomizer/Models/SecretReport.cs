using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KH3Randomizer.Data;

namespace KH3Randomizer.Models
{
    public class SecretReport
    {
        public string Name { get; set; }
        public List<string> Hints { get; set; }

        // Returns a list of hints that is shuffled and padded
        public List<string> GetHintList (int hintCount, Random rng)
        {
            List<string> hintList = new List<string>(Hints);
            hintList.Shuffle(rng);
            for (int i = 0; i < hintCount - Hints.Count; i++)
            {
                hintList.Add(null);
            }
            return hintList;
        }
    }
}

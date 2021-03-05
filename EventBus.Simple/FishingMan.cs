using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Simple
{
    public class FishingMan
    {
        public string Name { get; set; }

        public int FishCount { get; set; }

        public FishingMan(string name)
        {
            this.Name = name;
        }

        public FishingRod FishingRod { get; set; }

        public void Fishing()
        {
            this.FishingRod.ThrowHook(this);
        }
    }
}

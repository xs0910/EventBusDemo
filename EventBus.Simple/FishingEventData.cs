using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Simple
{
    public class FishingEventData : EventData
    {
        public FishType FishType { get; set; }
        public FishingMan FishingMan { get; set; }
    }
}

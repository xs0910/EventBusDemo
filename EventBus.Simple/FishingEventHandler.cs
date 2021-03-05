using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus.Simple
{
    public class FishingEventHandler : IEventHandler<FishingEventData>
    {
        public void HandleEvent(FishingEventData eventData)
        {
            eventData.FishingMan.FishCount++;
            Console.WriteLine($"{eventData.FishingMan.Name }:钓到一条{eventData.FishType}，已经钓到{eventData.FishingMan.FishCount}条鱼了");
        }
    }
}

using System;
using System.Reflection;
using System.Threading;

namespace EventBus.Simple
{
    class Program
    {
        static void Main(string[] args)
        {
            EventBus.Default.RegisterAllEventHandlerFromAssembly(Assembly.GetExecutingAssembly());

            FishingRod fishingRod = new FishingRod();

            FishingMan fishingMan = new FishingMan("渔夫");
            fishingMan.FishingRod = fishingRod;

            //fishingRod.FishingEvent += new FishingEventHandler().HandleEvent;

            while (fishingMan.FishCount < 5)
            {
                fishingMan.Fishing();
                Console.WriteLine("----------------");
                Thread.Sleep(3000);
            }

            Console.ReadKey();
        }
    }
}

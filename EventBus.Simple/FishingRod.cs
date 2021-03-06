using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventBus.Simple
{
    public class FishingRod
    {
        //public delegate void FishingHandler(FishingEventData eventData);  // 声明委托
        //public event FishingHandler FishingEvent;            // 声明事件

        //public FishingRod()
        //{
        //    Assembly assembly = Assembly.GetExecutingAssembly();

        //    foreach (var type in assembly.GetTypes())
        //    {
        //        // 判断当前类型是否实现了IEventHandler接口
        //        if (typeof(IEventHandler).IsAssignableFrom(type))
        //        {
        //            // 获取该类实现的泛型接口
        //            Type handlerInterface = type.GetInterface("IEventHandler`1");

        //            // 获取泛型接口指定的参数类型
        //            Type eventDataType = handlerInterface?.GetGenericArguments()[0];

        //            // 如果参数类型是FishingEventData，则说明事件源匹配
        //            if (eventDataType == typeof(FishingEventData))
        //            {
        //                var handler = Activator.CreateInstance(type) as IEventHandler<FishingEventData>;
        //                // 注册事件
        //                FishingEvent += handler.HandleEvent;
        //            }
        //        }
        //    }
        //}

        public void ThrowHook(FishingMan man)
        {
            Console.WriteLine("开始下钩");

            if (new Random().Next() % 2 == 0)
            {
                var type = (FishType)new Random().Next(0, 5);
                Console.WriteLine("铃铛：叮叮叮，鱼儿上钩了");
                FishingEventData eventData = new FishingEventData() { FishingMan = man, FishType = type };

                //if (FishingEvent != null)
                //{
                //    FishingEvent(eventData);   //不再需要通过事件委托触发
                //}

                // 直接通过事件总线触发
                EventBus.Default.Trigger(eventData);
                //EventBus.Default.Register<EventData>(actionEventData =>
                //{
                //    Console.WriteLine(actionEventData.GetType());
                //});

            }

        }

    }
}

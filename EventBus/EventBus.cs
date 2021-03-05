using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventBus
{
    public class EventBus
    {
        public static EventBus Default => new EventBus();

        private readonly ConcurrentDictionary<Type, List<Type>> _handlers;

        public EventBus()
        {
            _handlers = new ConcurrentDictionary<Type, List<Type>>();
            MapEventToHandler();
        }

        private void MapEventToHandler()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (var type in assembly.GetTypes())
            {
                // 判断当前类型是否实现了IEventHandler接口
                if (typeof(IEventHandler).IsAssignableFrom(type))
                {
                    // 获取该类实现的泛型接口
                    Type handlerInterface = type.GetInterface("IEventHandler`1");
                    // 获取泛型接口指定的参数类型
                    Type eventDataType = handlerInterface.GetGenericArguments()[0];

                    if (_handlers.ContainsKey(eventDataType))
                    {
                        List<Type> handlerTypes = _handlers[eventDataType];
                        handlerTypes.Add(type);
                        _handlers[eventDataType] = handlerTypes;
                    }
                    else
                    {
                        var handlerTypes = new List<Type>();
                        handlerTypes.Add(type);
                        _handlers[eventDataType] = handlerTypes;
                    }
                }
            }
        }

        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            List<Type> handlers = new List<Type>();
            _handlers.TryGetValue(typeof(EventData), out handlers);

            if (handlers.Count > 0)
            {
                foreach (var handler in handlers)
                {
                    MethodInfo methodInfo = handler.GetMethod("HandleEvent");
                    if (methodInfo != null)
                    {
                        object obj = Activator.CreateInstance(handler);
                        methodInfo.Invoke(obj, new object[] { eventData });
                    }
                }
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EventBus
{
    public interface IEventBus
    {
        void Register<TEventData>(IEventHandler eventHandler);
        void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData;

        void Register(Type eventType, Type eventHandler);

        void RegisterAllEventHandlerFromAssembly(Assembly assembly);

        void UnRegister<TEventData>(Type eventHandler);
        void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData;
    }
}

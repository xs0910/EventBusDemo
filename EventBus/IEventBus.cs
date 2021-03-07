using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        void Trigger<TEventData>(TEventData eventData, Type eventHandlerType) where TEventData : IEventData;

        Task TriggerAsync<TEventData>(TEventData eventData) where TEventData : IEventData;

        Task TriggerAsycn<TEventData>(TEventData eventData, Type eventHandlerType) where TEventData : IEventData;
    }
}

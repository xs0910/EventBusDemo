﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace EventBus
{
    public class EventBus : IEventBus
    {
        public IWindsorContainer IocContainer { get; private set; }
        public static EventBus Default { get; private set; }

        /// <summary>
        /// 定义线程安全集合(Key:EventData,Value:List<Type>)
        /// </summary>
        private readonly ConcurrentDictionary<Type, List<Type>> _eventAndHandlerMapping;

        public EventBus()
        {
            IocContainer = new WindsorContainer();
            _eventAndHandlerMapping = new ConcurrentDictionary<Type, List<Type>>();
            //MapEventToHandler();
        }

        static EventBus()
        {
            Default = new EventBus();
        }

        /// <summary>
        /// 通过反射，将事件源与事件处理绑定
        /// </summary>
        private void MapEventToHandler()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            if (assembly == null)
            {
                return;
            }
            foreach (var type in assembly.GetTypes())
            {
                // 判断当前类型是否实现了IEventHandler接口
                if (typeof(IEventHandler).IsAssignableFrom(type))
                {
                    // 获取该类实现的泛型接口
                    Type handlerInterface = type.GetInterface("IEventHandler`1");

                    if (handlerInterface != null)
                    {
                        // 获取泛型接口指定的参数类型
                        Type eventDataType = handlerInterface.GetGenericArguments()[0];

                        if (_eventAndHandlerMapping.ContainsKey(eventDataType))
                        {
                            List<Type> handlerTypes = _eventAndHandlerMapping[eventDataType];
                            handlerTypes.Add(type);
                            _eventAndHandlerMapping[eventDataType] = handlerTypes;
                        }
                        else
                        {
                            var handlerTypes = new List<Type>();
                            handlerTypes.Add(type);
                            _eventAndHandlerMapping[eventDataType] = handlerTypes;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 手动绑定事件源与事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventHandler"></param>
        public void Register<TEventData>(IEventHandler eventHandler)
        {
            Register(typeof(TEventData), eventHandler.GetType());
        }

        public void Register<TEventData>(Action<TEventData> action) where TEventData : IEventData
        {
            var actionHandler = new ActionEventHandler<TEventData>(action);
            Register<TEventData>(actionHandler);
        }

        public void Register(Type eventType, Type eventHandler)
        {
            if (_eventAndHandlerMapping.Keys.Contains(eventType))
            {
                var handlerTypes = _eventAndHandlerMapping[eventType];
                if (!handlerTypes.Contains(eventHandler))
                {
                    handlerTypes.Add(eventHandler);
                    _eventAndHandlerMapping[eventType] = handlerTypes;
                }
            }
            else
            {
                _eventAndHandlerMapping.GetOrAdd(eventType, (type) => new List<Type>()).Add(eventHandler);
            }
        }

        /// <summary>
        /// 提供入口支持注册其他程序集中实现的IEventHandler
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterAllEventHandlerFromAssembly(Assembly assembly)
        {
            IocContainer.Register(Classes.FromAssembly(assembly)
                .BasedOn(typeof(IEventHandler<>))
                .WithService.AllInterfaces()
                .LifestyleSingleton());

            var handlers = IocContainer.Kernel.GetHandlers(typeof(IEventHandler));
            foreach (var handler in handlers)
            {
                if (!typeof(IEventHandler).IsAssignableFrom(handler.ComponentModel.Implementation))
                {
                    return;
                }

                var interfaces = handler.ComponentModel.Implementation.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).IsAssignableFrom(@interface))
                    {
                        continue;
                    }
                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        //var handlerType = typeof(IEventHandler<>).MakeGenericType(genericArgs[0]);
                        //var eventHandler = IocContainer.Resolve(handlerType) as IEventHandler;
                        Register(genericArgs[0], handler.ComponentModel.Implementation);
                    }
                }
            }
        }

        /// <summary>
        /// 手动解除事件源与事件处理的绑定
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventHandler"></param>
        public void UnRegister<TEventData>(Type eventHandler)
        {
            //List<Type> handlerTypes = _eventAndHandlerMapping[typeof(TEventData)];
            _eventAndHandlerMapping.GetOrAdd(typeof(TEventData), (type) => new List<Type>()).RemoveAll(t => t == eventHandler);
        }

        /// <summary>
        /// 根据事件源绑定触发的事件处理
        /// </summary>
        /// <typeparam name="TEventData"></typeparam>
        /// <param name="eventData"></param>
        public void Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            List<Type> handlers = _eventAndHandlerMapping[typeof(TEventData)];

            if (handlers != null && handlers.Count > 0)
            {
                foreach (var handler in handlers)
                {
                    //var eventHandler = handler as IEventHandler<TEventData>;
                    //eventHandler.HandleEvent(eventData);

                    var handlerType = handler.GetInterface("IEventHandler`1");
                    var eventHandler = IocContainer.Resolve(handlerType) as IEventHandler<TEventData>;
                    eventHandler?.HandleEvent(eventData);

                    //MethodInfo methodInfo = handler.GetMethod("HandleEvent");
                    //if (methodInfo != null)
                    //{
                    //    object obj = Activator.CreateInstance(handler);
                    //    methodInfo.Invoke(obj, new object[] { eventData });
                    //}
                }
            }
        }
    }
}

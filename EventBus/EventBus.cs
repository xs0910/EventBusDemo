using System;
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
            IocContainer.Register(Component.For<IEventHandler<TEventData>>().UsingFactoryMethod(() => actionHandler));
            Register<TEventData>(actionHandler);
        }

        public void Register(Type eventType, Type eventHandler)
        {
            // 问题：当同一个泛型接口注册了多个实现到IOC容器，则在依赖解析时总是解析到第一个实现
            var handlerInterface = eventHandler.GetInterface("IEventHandler`1");
            if (!IocContainer.Kernel.HasComponent(handlerInterface))
            {
                IocContainer.Register(Component.For(handlerInterface, eventHandler));
            }

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
            // 1.将IEventHandler注册到IOC容器
            IocContainer.Register(Classes.FromAssembly(assembly)
                .BasedOn(typeof(IEventHandler<>))
                .WithService.Base());

            // 2.从IOC容器中获取注册的所有IEventHandler
            var handlers = IocContainer.Kernel.GetAssignableHandlers(typeof(IEventHandler));
            foreach (var handler in handlers)
            {
                // 3.循环遍历所有的IEventHandler<T>
                var interfaces = handler.ComponentModel.Implementation.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).IsAssignableFrom(@interface))
                    {
                        continue;
                    }
                    // 获取泛型参数类型
                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        //var handlerType = typeof(IEventHandler<>).MakeGenericType(genericArgs[0]);
                        //var eventHandler = IocContainer.Resolve(handlerType) as IEventHandler;
                        // 注册到事件源与事件处理的映射字典中
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
                foreach (var handlerType in handlers)
                {
                    // 从IOC容器中获取所有的实例
                    var handlerInterface = handlerType.GetInterface("IEventHandler`1");
                    var eventHandlers = IocContainer.ResolveAll(handlerInterface);

                    // 循环遍历，仅当解析的实例类型与映射字典中事件处理类型一致时，才触发事件
                    foreach (var eventHandler in eventHandlers)
                    {
                        if (eventHandler.GetType() == handlerType)
                        {
                            var handler = eventHandler as IEventHandler<TEventData>;
                            handler.HandleEvent(eventData);
                        }
                    }
                }
            }
        }
    }
}

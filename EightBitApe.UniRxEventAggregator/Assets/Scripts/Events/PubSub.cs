using System;
using UniRx;

namespace UniRxEventAggregator.Events
{
    public static class PubSub
    {
        private static RxEventAggregator eventAggregator = new RxEventAggregator();

        /// <summary>Expose events directly as type T</summary>
        /// <typeparam name="TEvent">The type of event being fired</typeparam>
        /// <returns>An observable stream of the event type that will never error or complete</returns>
        public static IObservable<TEvent> GetEvent<TEvent>()
        {
            return eventAggregator.GetEvent<TEvent>();
        }

        /// <summary>Exposes a combination of all events that inherit the specified type TEvent</summary>
        /// <typeparam name="TEvent">The base type to look for in event publications</typeparam>
        /// <returns>An observable stream of events inheriting the type that will never error or complete</returns>
        public static IObservable<TEvent> GetEventsOfType<TEvent>()
        {
            return eventAggregator.GetEventsOfType<TEvent>();
        }

        /// <summary>Registers a converter that lasts until the disposable is disposed</summary>
        /// <remarks>Warning: Mapping the same event to itself will cause an infinite recursive subscription</remarks>
        public static IDisposable Convert<TEventIn, TEventOut>(Func<TEventIn, TEventOut> eventConverter)
        {
            return eventAggregator.Convert<TEventIn, TEventOut>(eventConverter);
        }

        /// <summary>Registers a converter that lasts until the disposable is disposed</summary>
        /// <remarks>Warning: Mapping the same event to itself will cause an infinite recursive subscription</remarks>
        public static IDisposable Convert<TEventIn, TEventOut>(Func<TEventIn, TEventOut> eventConverter, Func<TEventIn, bool> predicate)
        {
            return eventAggregator.Convert<TEventIn, TEventOut>(eventConverter, predicate);
        }

        /// <summary>Registers an additional observable source against this event type that will be merged with any 
        /// other sources and all manually published events of this type. 
        /// 
        /// On dispose this additional observable is removed from the streams of subscribers</summary>
        /// <remarks>Warning: If passing an observable of TEvent that relies on a subscription of TEvent you will cause an infinite recursive subscription</remarks>
        public static IDisposable Register<TEvent>(IObservable<TEvent> observable)
        {
            return eventAggregator.Register<TEvent>(observable);
        }

        /// <summary>
        /// Publish an event of type TEvent
        /// </summary>
        /// <typeparam name="TEvent">The type of event being fired</typeparam>
        /// <param name="sampleEvent">The event</param>
        public static void Publish<TEvent>(TEvent @event)
        {
            eventAggregator.Publish(@event);
        }
    }
}

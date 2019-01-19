using UniRxEventAggregator.Extensions;
using Realtime.Messaging.Internal;
using System;
using System.Linq;
using UniRx;

namespace UniRxEventAggregator.Events
{
    /// <remarks>This works better than a Subject<IObservable<T>> approach for warm observables (in converters)
    /// but technically leaves an ever expanding subject dictionary (though these are small footprints).
    /// We could attempt to somehow identify when subscriber counts hit zero and all 'otherSources' are finished
    /// but this could become complicated</remarks>
    public class RxEventAggregator
    {
        // .NET version of ConcurrentDictionary unaccessible in unity so use this from github
        private readonly ConcurrentDictionary<Type, object> subjects
            = new ConcurrentDictionary<Type, object>();

        /// <summary>Retrieve the wrapped internal event</summary>
        private PubSubEvent<TEvent> GetEventInternal<TEvent>()
        {
            return (PubSubEvent<TEvent>)subjects.GetOrAdd(typeof(TEvent), t => new PubSubEvent<TEvent>());
        }

        /// <summary>Expose events directly as type TEvent</summary>
        /// <typeparam name="TEvent">The type of event being fired</typeparam>
        /// <returns>An observable stream of the event type that will never error or complete</returns>
        public IObservable<TEvent> GetEvent<TEvent>()
        {
            return this.GetEventInternal<TEvent>().GetEvent().Select(e =>
            {
                // UnityEngine.Debug.Log(string.Format("Event Emitted: {0} ({1})", typeof(TEvent).Name, DateTime.UtcNow.ToString("HH:mm:ss.fff")));
                return e;
            });
        }

        /// <summary>Exposes a combination of all events that inherit the specified type TEvent</summary>
        /// <typeparam name="TEvent">The base type to look for in event publications</typeparam>
        /// <returns>An observable stream of events inheriting the type that will never error or complete</returns>
        public IObservable<TEvent> GetEventsOfType<TEvent>()
        {
            return TypeExtensions.SubClassesOf<TEvent>().Distinct().Select(t =>
            {
                var method = this.GetType().GetMethod("GetEvent");
                var genericMethod = method.MakeGenericMethod(t);

                var observable = genericMethod.Invoke(this, null);

                var castMethod = typeof(Observable).GetMethods()
                               .Where(m => m.Name == "Cast")
                               .Single(m => m.GetParameters().Length == 1)
                               .MakeGenericMethod(t, typeof(TEvent));

                return castMethod.Invoke(null, new[] { observable }) as IObservable<TEvent>;
            })
            .Where(e =>
            {
                return e != null;
            })
            .Merge();
        }

        /// <summary>Registers a converter that lasts until the disposable is disposed</summary>
        /// <remarks>Warning: Mapping the same event to itself will cause an infinite recursive subscription</remarks>
        public IDisposable Convert<TEventIn, TEventOut>(Func<TEventIn, TEventOut> eventConverter)
        {
            return this.GetEventInternal<TEventOut>().AddOther(this.GetEvent<TEventIn>().Select(eventConverter));
        }

        /// <summary>Registers a converter that lasts until the disposable is disposed</summary>
        /// <remarks>Warning: Mapping the same event to itself will cause an infinite recursive subscription</remarks>
        public IDisposable Convert<TEventIn, TEventOut>(Func<TEventIn, TEventOut> eventConverter, Func<TEventIn, bool> predicate)
        {
            return this.GetEventInternal<TEventOut>().AddOther(this.GetEvent<TEventIn>().Where(predicate).Select(eventConverter));
        }

        /// <summary>Registers an additional observable source against this event type that will be merged with any 
        /// other sources and all manually published events of this type. 
        /// 
        /// On dispose this additional observable is removed from the streams of subscribers</summary>
        /// <remarks>Warning: If passing an observable of TEvent that relies on a subscription of TEvent you will cause an infinite recursive subscription</remarks>
        public IDisposable Register<TEvent>(IObservable<TEvent> observable)
        {
            // If this is cold I want the cold events go to to new subscribers
            return this.GetEventInternal<TEvent>().AddOther(observable);
        }

        /// <summary>
        /// Publish an event of type TEvent
        /// </summary>
        /// <typeparam name="TEvent">The type of event being fired</typeparam>
        /// <param name="sampleEvent">The event</param>
        public void Publish<TEvent>(TEvent @event)
        {
            object subject;
            if (this.subjects.TryGetValue(typeof(TEvent), out subject))
            {
                ((PubSubEvent<TEvent>)subject).Publish(@event);
            }
        }
    }
}

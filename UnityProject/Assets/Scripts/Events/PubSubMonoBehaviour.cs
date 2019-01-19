using System;
using UniRx;

namespace UniRxEventAggregator.Events
{
    /// <summary>
    /// A behaviour that interacts with the PubSub event aggregator
    /// </summary>
    public class PubSubMonoBehaviour : RxMonoBehaviour
    {
        /// <summary>
        /// Register an observable against a type within the event aggregator for the lifetime of this mono behaviour.
        /// 
        /// Note: The stream errors will be ignored and complete will not pass through
        /// </summary>
        /// <typeparam name="TEvent">The type of event</typeparam>
        /// <param name="stream">The observable stream to register</param>
        /// <returns>A disposable to unregister the stream.  This is called for you in OnDestroy.</returns>
        public IDisposable Register<TEvent>(IObservable<TEvent> stream)
        {
            var registration = PubSub.Register(stream);

            this.disposables.Add(registration);
            return registration;
        }

        /// <summary>
        /// Register an converter against an event type to operate as an additional event mapped from the source for the lifetime of this mono behaviour.
        /// </summary>
        /// <typeparam name="TEventIn">The source event type</typeparam>
        /// <typeparam name="TEventOut">The destination event type</typeparam>
        /// <param name="converter">The conversion function</param>
        /// <returns>A disposable to unregister the conversion.  This is called for you in OnDestroy.</returns>
        public IDisposable RegisterConversion<TEventIn, TEventOut>(Func<TEventIn, TEventOut> converter)
        {
            var registration = PubSub.Convert(converter);

            this.disposables.Add(registration);
            return registration;
        }

        /// <summary>
        /// Register an converter against an event type to operate as an additional event mapped from the source for the lifetime of this mono behaviour.
        /// </summary>
        /// <typeparam name="TEventIn">The source event type</typeparam>
        /// <typeparam name="TEventOut">The destination event type</typeparam>
        /// <param name="converter">The conversion function</param>
        /// <param name="predicate">The predicate to apply to the source before conversion</param>
        /// <returns>A disposable to unregister the conversion.  This is called for you in OnDestroy.</returns>
        public IDisposable RegisterConversion<TEventIn, TEventOut>(Func<TEventIn, TEventOut> converter, Func<TEventIn, bool> predicate)
        {
            var registration = PubSub.Convert(converter, predicate);

            this.disposables.Add(registration);
            return registration;
        }

        /// <summary>
        /// Subscribe to events of type TEvent from the event aggregator for the lifetime of this mono behaviour.
        /// 
        /// Note: The stream will only ever fire onNext, errors and completes are supressed
        /// </summary>
        /// <param name="handler">A handler for when the events occur</param>
        /// <returns>A disposable to unsubscribe from the stream.  This is called for you in OnDestroy.</returns>
        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
        {
            return this.Subscribe(PubSub.GetEvent<TEvent>(), handler);
        }

        // <summary>
        /// Subscribe to events of type TEvent (subclasses/implementors) from the event aggregator for the lifetime of this mono behaviour.
        /// 
        /// Note: The stream will only ever fire onNext, errors and completes are supressed
        /// </summary>
        /// <param name="handler">A handler for when the events occur</param>
        /// <returns>A disposable to unsubscribe from the stream.  This is called for you in OnDestroy.</returns>
        public IDisposable SubscribeToEventsOfType<TEvent>(Action<TEvent> handler)
        {
            return this.Subscribe(PubSub.GetEventsOfType<TEvent>(), handler);
        }

        /// <summary>
        /// A helper to subscribe to an arbitrary stream for the lifetime of this mono behaviour.
        /// </summary>
        /// <typeparam name="TEvent">The type of event</typeparam>
        /// <param name="handstreamler">The stream to subscribe to</param>
        /// <param name="handler">A handler for when the events occur</param>
        /// <returns>A disposable to unsubscribe from the stream.  This is called for you in OnDestroy.</returns>
        public IDisposable Subscribe<TEvent>(IObservable<TEvent> stream, Action<TEvent> handler)
        {
            var subscription = stream.Subscribe(handler);

            this.disposables.Add(subscription);
            return subscription;
        }
    }
}
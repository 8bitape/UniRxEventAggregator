using System;
using UniRx;
using UnityEngine;

namespace UniRxEventAggregator.Events
{
    /// <summary>
    /// A behaviour that interacts with the PubSub event aggregator
    /// </summary>
    public static class PubSubMonoBehaviourGameObjectExtensions
    {
        /// <summary>
        /// Subscribe to events of type TEvent from the event aggregator for the lifetime of this mono behaviour.
        /// 
        /// Note: The stream will only ever fire onNext, errors and completes are supressed
        /// </summary>
        /// <param name="self">The PubSubMonoBehaviour</param>
        /// <param name="gameObject">A GameObject to filter the subscription with</param>
        /// <param name="handler">A handler for when the events occur</param>
        /// <returns>A disposable to unsubscribe from the stream.  This is called for you in OnDestroy.</returns>
        public static IDisposable Subscribe<TEvent>(this PubSubMonoBehaviour self, GameObject gameObject, Action<TEvent> handler)
            where TEvent : class, IGameObjectEvent
        {
            return self.Subscribe(PubSub.GetEvent<TEvent>().Where(e => e.GameObject == gameObject), handler);
        }

        /// <summary>
        /// Subscribe to events of type TEvent (subclasses/implementors) from the event aggregator for the lifetime of this mono behaviour.
        /// 
        /// Note: The stream will only ever fire onNext, errors and completes are supressed
        /// </summary>
        /// <param name="self">The PubSubMonoBehaviour</param>
        /// <param name="gameObject">A GameObject to filter the subscription with</param>
        /// <param name="handler">A handler for when the events occur</param>
        /// <returns>A disposable to unsubscribe from the stream.  This is called for you in OnDestroy.</returns>
        public static IDisposable SubscribeToEventsOfType<TEvent>(this PubSubMonoBehaviour self, GameObject gameObject, Action<TEvent> handler)
            where TEvent : class, IGameObjectEvent
        {
            return self.Subscribe(PubSub.GetEventsOfType<TEvent>().Where(e => e.GameObject == gameObject), handler);
        }
    }
}
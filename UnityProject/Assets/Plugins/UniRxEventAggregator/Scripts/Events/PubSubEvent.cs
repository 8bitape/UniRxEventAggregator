using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace UniRxEventAggregator.Events
{
    /// <remarks>
    /// This class exists to allow for 'hot' manual publishes through the PubSub event aggregator
    /// but also to allow for cold or warm observables to be registered against the event type and passed
    /// to all subscribers (now and in the future)
    /// </remarks>
    internal class PubSubEvent<T>
    {
        // For manual publishes
        private Subject<EventAndSource<T>> publisher = new Subject<EventAndSource<T>>();

        // For separate streams (some of which may be cold or warm).
        // Start with never to avoid completions ending the overall stream when this is merged
        private List<IObservable<T>> others = new List<IObservable<T>> { Observable.Never<T>()  };

        /// <summary>
        /// Publishes a manually fired event of type T
        /// </summary>
        /// <param name="event"></param>
        public void Publish(T @event)
        {
            this.publisher.OnNext(new EventAndSource<T>(@event));
        }

        /// <remarks>
        /// This needs to cover:
        /// * Existing subscribers who need to receive any cold/warm events from the 'other' and any future (hot) events
        /// * Subscribers who subscribe after this registration who need to receive cold/warm followed by hot
        /// * Any disposal of this 'other' needs to stop both current and future subscribers from receiving
        /// </remarks>
        public IDisposable AddOther(IObservable<T> other)
        {
            var killSwitch = false;

            var killableOther = other.Select(e =>
            {
                if(killSwitch)
                {
                    throw new Exception("Stream is no longer available");
                }

                return e;
            }).Catch<T, Exception>(e =>
            {
                // We want to mask errors anyway as an error from 'other' shouldn't kill the whole stream
                // but on the plus side this also removes it from merges of subscribers who are still around after this 'other' is disposed
                //
                // This does mean real errors are also hidden and errors for flow should be done via T itself
                //Debug.Log(string.Format("PubSubEvent Exception ({0}) {1}", typeof(T).Name, e.Message));
                return Observable.Empty<T>();
            });

            // This will push all cold events to existing subscribers
            // it will also push out all future events so hold the source against it for filtering out later
            var subscription = killableOther.Subscribe(oe =>
            {
                publisher.OnNext(new EventAndSource<T>(oe, killableOther));
            });

            // Protect for future subscribers who should receive the cold items from a merge followed by the later hot items
            others.Add(killableOther);

            return Disposable.Create(() => {
                // Stop pulsing 'manually' to subscribers who pre-existed this stream being added
                subscription.Dispose();

                // No longer pulse to those who subscribed after add - the observable
                // is still part of their merge.  This will 'die' when the next event on the observable is fired
                killSwitch = true;

                // Don't include this in future merges for new subscribers going forward
                others.Remove(killableOther);
            });
        }

        // This merge will mean warm observables are subscribed to each time
        // allowing late subscribers to get 'warm' replays etc
        public IObservable<T> GetEvent()
        {
            var otherSources = others.ToArray();
            
            // Avoid late subsscribers will not get the 'hot' aspects of 'other' from both the publisher and the merge
            return Observable.Merge(publisher.Where(we =>
            {
                // This avoids subscribers who subscribed after the 'other' was registered from receiving both a 
                // manually fired event and the event from the merge.  Note: We still need the merge as that is what
                // deals with 'cold' events
                var shouldFire = we.Source == null || !otherSources.Contains(we.Source);
                return shouldFire;
            }).Select(we => we.Event), otherSources);
        }
    }
}

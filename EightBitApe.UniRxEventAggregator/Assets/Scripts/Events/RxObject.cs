using UniRxEventAggregator.Events;
using System;
using UniRx;

namespace UniRxEventAggregator.Events
{
    public class RxObject : IDisposable
    {
        private CompositeDisposable disposables = new CompositeDisposable();

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

        #region IDisposable Support
        private bool disposedValue = false;

        /// <summary>
        /// When this behaviour is disposed of ensure that we dispose of all registered disposables
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.disposables.Dispose();
                    this.disposables = new CompositeDisposable();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}

using System;
using UniRx;
using UnityEngine;

namespace UniRxEventAggregator.Events
{
    /// <summary>
    /// A behaviour with helpers for dealing with Rx streams
    /// </summary>
    public class RxMonoBehaviour : MonoBehaviour, IDisposable
    {
        protected CompositeDisposable disposables = new CompositeDisposable();

        /// <summary>
        /// When this behaviour is destroys any subscriptions registered to the disposables container
        /// will be unsubscribed
        /// </summary>
        public virtual void OnDestroy()
        {
            try
            {
                this.Dispose();
            }
            catch
            {
                // Do nothing as we are just trying to dispose
            }
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
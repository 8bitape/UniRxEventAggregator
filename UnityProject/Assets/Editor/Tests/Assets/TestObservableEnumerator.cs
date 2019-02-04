using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using UniRx;

namespace UniRxEventAggregator.Tests.Assets
{
    public class TestObservableEnumerator
    {
        public IEnumerator CreateColdObservableCoroutineForSize(int size)
        {
            var items = Enumerable.Range(1, size);
            var observable = items.Select(i => i == size ? true : false).ToObservable();
            return CreateCoroutineEnumeratorFromObservableUsingToYield(observable);
        }

        /// <summary>
        /// This is an example of a coroutine returning a yield instruction
        /// </summary>
        public IEnumerator CreateCoroutineEnumeratorFromObservableUsingToYield<T>(IObservable<T> observable)
        {
            while (true)
            {
                // ToYieldInstruction subscribes meaning a cold observable
                // will potentially completely finish before MoveNext is ever called
                yield return observable.ToYieldInstruction();

                Assert.True(false);
            }
        }
    }
}

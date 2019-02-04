using NUnit.Framework;
using UniRxEventAggregator.Tests.Assets;
using UniRx;
using System.Collections;

namespace UniRxEventAggregator.Tests
{

    public class ObservableTests
    {
        /// <remarks>
        /// It would appear that the way coroutines are delayed is by yielding
        /// an enumerator see: https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html
        /// </remarks>
        [Test]
        public void ToYieldInstruction_ColdObservable_CompletesImmediately()
        {
            var size = 5;
            var observableEnumerator = new TestObservableEnumerator();
            var coroutineEnumerator = observableEnumerator.CreateColdObservableCoroutineForSize(size);
            coroutineEnumerator.MoveNext();
            var enumerator = coroutineEnumerator.Current as IEnumerator;

            Assert.False(enumerator.MoveNext());
            Assert.True((bool)enumerator.Current);
        }

        /// <remarks>
        /// It would appear that the way coroutines are delayed is by yielding
        /// an enumerator see: https://docs.unity3d.com/ScriptReference/CustomYieldInstruction.html
        /// </remarks>
        [Test]
        public void ToYieldInstruction_HotObservable_YieldsCurrentUntilObservableCompletes()
        {
            var subject = new Subject<bool>();
            var observableEnumerator = new TestObservableEnumerator();
            var coroutineEnumerator = observableEnumerator.CreateCoroutineEnumeratorFromObservableUsingToYield(subject);
            coroutineEnumerator.MoveNext();
            var enumerator = coroutineEnumerator.Current as IEnumerator;

            Assert.True(enumerator.MoveNext());
            Assert.False((bool)enumerator.Current);

            subject.OnNext(false);
            Assert.True(enumerator.MoveNext());
            Assert.False((bool)enumerator.Current);

            subject.OnNext(true);
            Assert.True(enumerator.MoveNext());
            Assert.True((bool)enumerator.Current);

            subject.OnCompleted();
            Assert.False(enumerator.MoveNext());
        }
    }
}
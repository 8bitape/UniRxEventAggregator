using NUnit.Framework;
using UniRxEventAggregator.Events;
using UniRx;

namespace UniRxEventAggregator.Tests
{
    public class EventAggregatorTests
    {
        public interface ITestEvent
        {

        }

        public interface ITestEvent2
        {

        }

        public class TestEvent : ITestEvent
        {

        }

        public class TestEvent2 : ITestEvent2
        {

        }

        [Test]
        public void Publish_SimpleEvent_IsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            pubSub.Publish(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void Publish_SimpleEventOfType_IsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            subscriptions.Add(pubSub.GetEventsOfType<ITestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            pubSub.Publish(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_HotObservablePublishedAfter_IsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new Subject<TestEvent>();

            subscriptions.Add(pubSub.Register<TestEvent>(subject));

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            subject.OnNext(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_ColdObservableSubscribedBefore_IsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            var observable = new[] { new TestEvent() }.ToObservable();
            subscriptions.Add(pubSub.Register<TestEvent>(observable));

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_ColdObservableSubscribedAfter_IsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var observable = new[] { new TestEvent() }.ToObservable();

            subscriptions.Add(pubSub.Register<TestEvent>(observable));

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_WarmObservableSubscribedBeforeFiring_IsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            var subject = new Subject<TestEvent>();
            subscriptions.Add(pubSub.Register<TestEvent>(subject.Replay(1).RefCount()));

            subject.OnNext(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_WarmObservableSubscribedAfterFiring_IsReceivedImmediately()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new Subject<TestEvent>();
            subscriptions.Add(pubSub.Register<TestEvent>(subject.Replay(1).RefCount()));

            subject.OnNext(new TestEvent());

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_WarmObservableSubscribedAfterFiring_SubsequentEventsReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new Subject<TestEvent>();
            subscriptions.Add(pubSub.Register<TestEvent>(subject.Replay(1).RefCount()));

            subject.OnNext(new TestEvent());

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            subject.OnNext(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(2, calledCount);
        }

        [Test]
        public void RegisterObservable_WarmObservableSubscribedAfterFiringAndDisposed_SubsequentEventsNotReceived()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new Subject<TestEvent>();
            var registration = pubSub.Register<TestEvent>(subject.Replay(1).RefCount());

            subject.OnNext(new TestEvent());

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            registration.Dispose();

            subject.OnNext(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_WarmObservableSubscribeAfterFirstFiring_NoDuplicatesReceivedAfterward()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new Subject<TestEvent>();
            var registration = pubSub.Register<TestEvent>(subject.Replay(1).RefCount());

            subject.OnNext(new TestEvent());

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            subject.OnNext(new TestEvent());

            registration.Dispose();
            subscriptions.Dispose();

            Assert.AreEqual(2, calledCount);
        }

        [Test]
        public void RegisterObservable_WarmObservableWithRegisteredConversionSubscribedAfterFiring_IsReceivedImmediately()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new Subject<TestEvent>();
            subscriptions.Add(pubSub.Register<TestEvent>(subject.Replay(1).RefCount()));

            subject.OnNext(new TestEvent());

            // Note, if you share from this point on you're creating a new 'root' for new subscriptions
            // which will not include the replay
            var newStream = pubSub.GetEvent<TestEvent>().Select(t => new TestEvent2());

            pubSub.Register<TestEvent2>(newStream);

            subscriptions.Add(pubSub.GetEvent<TestEvent2>().Subscribe(e =>
            {
                calledCount++;
            }));

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }

        [Test]
        public void RegisterObservable_SubscribeBeforeUnregistering_NoEventsAfterUnregister()
        {
            var subscriptions = new CompositeDisposable();
            var pubSub = new RxEventAggregator();
            var calledCount = 0;

            var subject = new BehaviorSubject<TestEvent>(new TestEvent());
            var registration = pubSub.Register<TestEvent>(subject);
            subscriptions.Add(registration);

            subject.OnNext(new TestEvent());

            subscriptions.Add(pubSub.GetEvent<TestEvent>().Subscribe(e =>
            {
                calledCount++;
            }));

            registration.Dispose();

            subject.OnNext(new TestEvent());

            subscriptions.Dispose();

            Assert.AreEqual(1, calledCount);
        }
    }
}
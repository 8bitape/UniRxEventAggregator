using System;
using UniRx;

namespace UniRxEventAggregator.Events
{
    public class EventAndSource<T>
    {
        public T Event { get; set; }
        public IObservable<T> Source { get; set; }

        public EventAndSource(T @event)
            : this(@event, null)
        {
        }

        public EventAndSource(T @event, IObservable<T> source) {
            this.Event = @event;
            this.Source = source;
        }
    }
}

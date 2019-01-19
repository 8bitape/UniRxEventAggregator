using System;
using UniRx;

namespace UniRxEventAggregator.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<object> AsSignal<T>(this IObservable<T> stream)
        {
            return stream.Select(e => e as object);
        }

        public static IObservable<T> WhereNot<T>(this IObservable<T> stream, Func<T, bool> predicate)
        {
            return stream.Where(item => !predicate(item));
        }
    }
}

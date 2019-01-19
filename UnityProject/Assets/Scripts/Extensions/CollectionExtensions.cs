using System.Collections.Generic;
using System.Linq;

namespace UniRxEventAggregator.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(IEnumerable<T> items) 
        {
            return items == null || !items.Any();
        }
    }
}

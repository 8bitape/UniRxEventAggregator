using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRxEventAggregator.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> SubClassesOf<TBaseType>()
        {
            var baseType = typeof(TBaseType);
            var assembly = baseType.Assembly;

            return assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t));
        }
    }
}

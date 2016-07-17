using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Helpers
{
    public static class ObjectExtensions
    {
        public static TResult IfNotNull<TSource, TResult>(this TSource instance, Func<TSource, TResult> getter, TResult defaultValue = default(TResult))
            where TSource : class
        {
            if (instance != null)
                return getter(instance);
            return defaultValue;
        }
    }
}

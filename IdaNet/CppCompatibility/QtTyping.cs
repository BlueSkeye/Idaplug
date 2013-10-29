using System;
using System.Collections.Generic;
using System.Text;

namespace IdaNet.CppCompatibility
{
    internal class QtTyping
    {
        internal static bool is_pod_type<T>()
        {
            // TODO : Rather complicated. Shouldn't it always return true ?
            return check_type_trait(ida_type_traits<T>.is_pod_type);
        }

        internal static unsafe void Free(void* theObject)
        {
            // Must invoke the free function.
            throw new NotImplementedException();
        }

        private static bool check_type_trait(ida_false_type candidate)
        {
            return false;
        }

        private static bool check_type_trait(ida_true_type candidate )
        {
            return true;
        }

        internal struct ida_true_type
        {
        };

        internal struct ida_false_type
        {
        };

        internal class ida_type_traits
        {
            internal static ida_false_type is_pod_type;
        }

        internal class ida_type_traits<T>
        {
            internal static ida_true_type is_pod_type;
        }
    }
}

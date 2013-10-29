using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#if __EA64__
using AddressDifference = System.Int64;
using EffectiveAddress = System.UInt64;
using MemoryChunkSize = System.UInt64;
using NodeIndex = System.UInt64;
using SegmentSelector = System.UInt64;
using SignedSize = System.Int64;
#else
using AddressDifference = System.Int32;
using EffectiveAddress = System.UInt32;
using MemoryChunkSize = System.UInt32;
using NodeIndex = System.UInt32;
using SegmentSelector = System.UInt32;
using SignedSize = System.Int32;
#endif

namespace IdaNet.CppCompatibility
{
    internal class Vector<T>
        where T : class, IMarshalable
    {
        // Native layout
        // 0x00 - 0x00 : T *array;
        private unsafe void** array;
        // 0x04 - 0x08 : size_t n;
        private int n;
        // 0x08 - 0x0C : size_t alloc;
        private int alloc;

        internal static int NatvieSize
        {
            get
            {
#if __EA64__
                return 0x10;
#else
                return 0x0C;
#endif
            }
        }

        //qvector<T> &assign(const qvector<T> &x)
        //{
        //if ( x.n > 0 )
        //{
        //    array = (T*)qalloc_or_throw(x.alloc * sizeof(T));
        //    alloc = x.alloc;
        //    while ( n < x.n )
        //    {
        //    new (array+n) T(x.array[n]);
        //    ++n;
        //    }
        //}
        //return *this;
        //}
        
        //// move data down in memory
        //void shift_down(T *dst, T *src, size_t cnt)
        //{
        //if ( may_move_bytes<T>() )
        //{
        //    memmove(dst, src, cnt*sizeof(T));
        //}
        //else
        //{
        //    ssize_t s = cnt;
        //    while( --s >= 0 )
        //    {
        //    new(dst) T(*src);
        //    src->~T();
        //    ++src;
        //    ++dst;
        //    }
        //}
        //}
        
        //// move data up in memory
        //void shift_up(T *dst, T *src, size_t cnt)
        //{
        //if ( may_move_bytes<T>() )
        //{
        //    memmove(dst, src, cnt*sizeof(T));
        //}
        //else
        //{
        //    ssize_t s = cnt;
        //    dst += s;
        //    src += s;
        //    while( --s >= 0 )
        //    {
        //    --src;
        //    --dst;
        //    new(dst) T(*src);
        //    src->~T();
        //    }
        //}
        //}
        
        //public:
        //typedef T value_type;
        //qvector(void) : array(NULL), n(0), alloc(0) {}
        //qvector(const qvector<T> &x) : array(NULL), n(0), alloc(0) { assign(x); }
        //~qvector(void) { clear(); }
        
        //DEFINE_MEMORY_ALLOCATION_FUNCS()
        //void push_back(const T &x)
        //{
        //reserve(n+1);
        //new (array+n) T(x); // create a new element in the qvector
        //++n;
        //}
        
        //T &push_back(void)
        //{
        //reserve(n+1);
        //T *ptr = array + n;
        //new (ptr) T;
        //++n;
        //return *ptr;
        //}
        
        //void pop_back(void)
        //{
        //if ( n > 0 )
        //{
        //#ifdef UNDER_CE         // clarm.exe is buggy
        //    --n;
        //    if ( !is_pod_type<T>() )
        //    array[n].~T();
        //#else
        //    array[--n].~T();
        //#endif
        //}
        //}
        
        internal int size()
        {
            return n;
        }
        
        internal bool empty()
        {
            return 0 == n;
        }
        
        internal T this[int idx]
        {
            get { unsafe { return FromNative(array[idx]); } }
        }

        private static unsafe T FromNative(void* candidate)
        {
            ICustomMarshaler marshaler = GetMarshaler(typeof(T));

            return (T)marshaler.MarshalNativeToManaged(new IntPtr(candidate));
        }

        private static ICustomMarshaler GetMarshaler(Type candidate)
        {
            ICustomMarshaler result;

            if (!_knownMarshalers.TryGetValue(candidate, out result))
            {
                throw new NotImplementedException();
            }
            return result;
        }

        private static Dictionary<Type, ICustomMarshaler> _knownMarshalers =
            new Dictionary<Type, ICustomMarshaler>();
        
        //    T &operator[](size_t idx)       { return array[idx]; }
        
        //const T &at(size_t idx) const { return array[idx]; }
        
        //    T &at(size_t idx)       { return array[idx]; }
        
        //const T &front(void) const { return array[0]; }
        
        //    T &front(void)       { return array[0]; }
        
        internal unsafe T back()
        {
            unsafe { return FromNative(array[n - 1]); }
        }
        
        //    T &back(void)       { return array[n-1]; }
        
        internal void qclear() // destruct elements but do not free memory
        {
            if (QtTyping.is_pod_type<T>()) { n = 0; }
            else { while (n-- > 0) { unsafe { CppTyping.CallDestructor(((void**)array)[n]); } } }
            return;
        }
        
        internal void clear()
        {
            unsafe
            {
                if (null == array) { return; }
                qclear();
                QtTyping.Free(array);
                array = null;
                alloc = 0;
                return;
            }
        }
        
        //qvector<T> &operator=(const qvector<T> &x)
        //{
        //size_t mn = qmin(n, x.n);
        //for ( size_t i=0; i < mn; i++ )
        //    array[i] = x.array[i];
        //if ( n > x.n )
        //{
        //    if ( is_pod_type<T>() )
        //    {
        //    n = x.n;
        //    }
        //    else
        //    {
        //    while ( n > x.n )
        //    {
        //        array[n-1].~T();
        //        --n;
        //    }
        //    }
        //}
        //else
        //{
        //    reserve(x.n);
        //    while ( n < x.n )
        //    {
        //    new(array+n) T(x.array[n]);
        //    ++n;
        //    }
        //}
        //return *this;
        //}
        
        //void resize(size_t s, const T &x)
        //{
        //if ( s < n )
        //{
        //    if ( !is_pod_type<T>() )
        //    for ( ssize_t i=ssize_t(n); --i >= ssize_t(s); )
        //        array[i].~T();
        //    n = s;
        //}
        //else
        //{
        //    reserve(s);
        //    while ( n < s )
        //    {
        //    new(array+n) T(x);
        //    ++n;
        //    }
        //}
        //}
        
        //void resize(size_t s) { resize(s, T()); }
        
        //void grow(const T &x=T())
        //{
        //reserve(n+1);
        //new(array+n) T(x);
        //++n;
        //}
        
        //size_t capacity(void) const { return alloc; }
        
        //void reserve(size_t cnt)
        //{
        //if ( cnt > alloc )
        //{
        //    size_t m0 = alloc * 2;
        //    size_t m = qmax(m0, cnt);
        //    size_t b = m * sizeof(T);
        //    if ( b < m )
        //    b = 0xDEADBEEF; // integer overflow, ask too much and it will throw
        //    if ( may_move_bytes<T>() )
        //    {
        //    array = (T*)qrealloc_or_throw(array, b);
        //    }
        //    else
        //    {
        //    T *new_array = (T*)qalloc_or_throw(b);
        //    shift_down(new_array, array, n);
        //    qfree(array);
        //    array = new_array;
        //    }
        //    alloc = m;
        //}
        //}
        
        //void truncate(void)
        //{
        //if ( alloc > n )
        //{
        //    array = (T*)qrealloc(array, n*sizeof(T)); // should not fail
        //    alloc = n;
        //}
        //}
        
        //void swap(qvector<T> &r)
        //{
        //T *array2     = array;
        //size_t n2     = n;
        //size_t alloc2 = alloc;
        //array = r.array;
        //n     = r.n;
        //alloc = r.alloc;
        //r.array = array2;
        //r.n     = n2;
        //r.alloc = alloc2;
        //}

        //// method to extract data from the vector and to empty it
        //// the caller must free the result of this function
        //T *extract(void)
        //{
        //truncate();
        //alloc = 0;
        //n = 0;
        //T *res = array;
        //array = NULL;
        //return res;
        //}
        
        //// method to populate qvector with a pointer to dynamic memory
        //// qvector must be empty before calling this method!
        //void inject(T *s, size_t len)
        //{
        //array = s;
        //alloc = len;
        //n = len;
        //}
        //bool operator == (const qvector<T> &r) const
        //{
        //if ( n != r.n )
        //    return false;
        //for ( size_t i=0; i < n; i++ )
        //    if ( array[i] != r[i] )
        //    return false;
        //return true;
        //}
        
        //bool operator != (const qvector<T> &r) const { return !(*this == r); }

        //typedef T *iterator;
        //typedef const T *const_iterator;

        //iterator begin(void) { return array; }
        
        //iterator end(void) { return array + n; }
        
        //const_iterator begin(void) const { return array; }
        
        //const_iterator end(void) const { return array + n; }

        //iterator insert(iterator it, const T &x)
        //{
        //size_t idx = it - array;
        //reserve(n+1);
        //T *p = array + idx;
        //size_t rest = end() - p;
        //shift_up(p+1, p, rest);
        //new(p) T(x);
        //n++;
        //return iterator(p);
        //}
        
        //template <class it2> iterator insert(iterator it, it2 first, it2 last)
        //{
        //size_t cnt = last - first;
        //if ( cnt == 0 )
        //    return it;
        //size_t idx = it - array;
        //reserve(n+cnt);
        //T *p = array + idx;
        //size_t rest = end() - p;
        //shift_up(p+cnt, p, rest);
        //while ( first != last )
        //{
        //    new(p) T(*first);
        //    ++p;
        //    ++first;
        //}
        //n += cnt;
        //return iterator(array+idx);
        //}

        //iterator erase(iterator it)
        //{
        //it->~T();
        //size_t rest = end() - it - 1;
        //shift_down(it, it+1, rest);
        //n--;
        //return it;
        //}
        
        //iterator erase(iterator first, iterator last)
        //{
        //for ( T *p=first; p != last; ++p )
        //    p->~T();
        //size_t rest = end() - last;
        //shift_down(first, last, rest);
        //n -= last - first;
        //return first;
        //}
        
        //// non-standard extensions:
        //iterator find(const T &x)
        //{
        //iterator p;
        //for ( p=begin(); p != end(); ++p )
        //    if ( x == *p )
        //    break;
        //return p;
        //}
        
        //const_iterator find(const T &x) const
        //{
        //const_iterator p;
        //for ( p=begin(); p != end(); ++p )
        //    if ( x == *p )
        //    break;
        //return p;
        //}
        
        //bool has(const T &x) const { return find(x) != end(); }
        
        //bool add_unique(const T &x)
        //{
        //if ( has(x) )
        //    return false;
        //push_back(x);
        //return true;
        //}
        
        //bool del(const T &x)
        //{
        //iterator p = find(x);
        //if ( p == end() )
        //    return false;
        //erase(p);
        //return true;
        //}

        internal class Marshaler : ICustomMarshaler
        {
            private Marshaler()
            {
                return;
            }

            public static ICustomMarshaler GetInstance()
            {
                return _singleton;
            }

            public void CleanUpManagedData(object ManagedObj)
            {
                throw new NotImplementedException();
            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                throw new NotImplementedException();
            }

            public int GetNativeDataSize()
            {
                throw new NotImplementedException();
            }

            public IntPtr MarshalManagedToNative(object ManagedObj)
            {
                throw new NotImplementedException();
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                throw new NotImplementedException();
            }

            private static readonly Marshaler _singleton = new Marshaler();
        }
    }
}

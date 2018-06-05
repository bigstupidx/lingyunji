#if !USE_HOT
namespace xys.hot
{
    class Any<T>
    {
        public Any()
        {

        }

        public Any(T v)
        {
            value = v;
        }

        public T value;
    }
}
#endif
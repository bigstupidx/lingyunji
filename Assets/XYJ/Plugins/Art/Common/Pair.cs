﻿
namespace XTools
{
    public class Pair<T1, T2>
	{
        public Pair()
        {

        }

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public T1 first;
        public T2 second;

        public override string ToString()
        {
            return string.Format("f:{0} s:{1}", (first == null ? "(null)" : first.ToString()), second.ToString());
        }
    }

    public class Pair3<T1, T2, T3>
    {
        public Pair3()
        {

        }

        public Pair3(T1 first, T2 second, T3 third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public T1 first;
        public T2 second;
        public T3 third;

        public override string ToString()
        {
            return string.Format("f:{0} s:{1} t:{2}", (first == null ? "(null)" : first.ToString()), second.ToString(), third.ToString());
        }
    }
}

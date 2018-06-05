using wProtobuf;
using wProtobuf.RPC;
using System.Collections;

namespace xys.RPC
{
    // 协程返回
    class Yield : IYieldResult
    {
        public Yield()
        {
            isDone = false;
            code = Error.Unknown;
        }

        public bool isDone { get; private set; } // 是否执行完成

        public Error code { get; private set; }

        ICoroutine coroutine;
        IEnumerator ator;

        public void OnEnd(Error code)
        {
            this.code = code;
            isDone = true;

            if (coroutine == null)
            {
                throw new System.Exception("Yield coroutine = null!");
            }
            else
            {
                coroutine.StartCoroutine(ator);
            }
        }

        public void OnYield(ICoroutine coroutine, IEnumerator ator)
        {
            this.coroutine = coroutine;
            this.ator = ator;
        }
    }

    class Yield<T> : IYieldResult<T>
    {
        public bool isDone { get; private set; } // 是否执行完成

        public Error code { get; private set; }
        public T result { get; private set; }

        ICoroutine coroutine;
        IEnumerator ator;

        public Yield() { code = Error.Unknown; isDone = false; }
        public Yield(Error code, T result)
        {
            this.code = code;
            this.result = result;
            isDone = true;
        }

        public void OnEnd(Error code, T result)
        {
            this.code = code;
            this.result = result;
            isDone = true;

            if (coroutine == null)
            {
                throw new System.Exception("Yield coroutine = null!");
            }
            else
            {
                coroutine.StartCoroutine(ator);
            }
        }

        public void OnYield(ICoroutine coroutine, IEnumerator ator)
        {
            this.coroutine = coroutine;
            this.ator = ator;
        }
    }

    public class YieldFactory : IYieldFactory
    {
        public IYieldResult Create() { return new Yield(); }
        public IYieldResult<T> Create<T>() { return new Yield<T>(); }
    }
}

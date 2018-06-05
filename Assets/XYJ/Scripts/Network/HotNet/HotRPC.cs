#if !USE_HOT
namespace xys.hot.RPC
{
    public interface IMessage
    {
        int CalculateSize();
        void MergeFrom(wProtobuf.IReadStream input);
        void WriteTo(wProtobuf.IWriteStream output);
    }

    public interface IYieldResult
    {
        wProtobuf.RPC.IYieldResult yield { get; }
        wProtobuf.RPC.Error code { get; }
        void OnEnd(wProtobuf.RPC.Error error);
    }

    public interface IYieldResult<T>
    {
        T result { get; }
        wProtobuf.RPC.IYieldResult yield { get; }
        wProtobuf.RPC.Error code { get; }
        void OnEnd(wProtobuf.RPC.Error error, T value);
    }

    public class Yield : IYieldResult
    {
        public Yield(wProtobuf.RPC.IYieldResult yield)
        {
            this.yield = yield;
        }

        public wProtobuf.RPC.IYieldResult yield { get; private set; }
        public wProtobuf.RPC.Error code { get; private set; }

        public void OnEnd(wProtobuf.RPC.Error error)
        {
            this.code = error;
            yield.OnEnd(error);
        }
    }

    public class YieldResult<T> : IYieldResult<T>
    {
        public YieldResult(wProtobuf.RPC.IYieldResult yield)
        {
            this.yield = yield;
        }

        public wProtobuf.RPC.IYieldResult yield { get; private set; }
        public wProtobuf.RPC.Error code { get { return yield.code; } }

        public T result { get; private set; }

        public void OnEnd(wProtobuf.RPC.Error error, T value)
        {
            this.result = value;
            yield.OnEnd(error);
        }
    }

    public static class YieldFactory
    {
        public static IYieldResult Create()
        {
            return new Yield(wProtobuf.RPC.YieldFactory.Create());
        }

        public static YieldResult<T> Create<T>()
        {
            return new YieldResult<T>(wProtobuf.RPC.YieldFactory.Create());
        }
    }

    public interface ILocalCall
    {
        void Call<Param, Result>(string key, Param param, System.Action<wProtobuf.RPC.Error, Result> onEnd) where Result : new();
        void Call(string key, System.Action<wProtobuf.RPC.Error> fun);
        void Call<Result>(string key, System.Action<wProtobuf.RPC.Error, Result> fun) where Result : new();
        void Call<Param>(string key, Param param, System.Action<wProtobuf.RPC.Error> fun);
    }

    public class OutValue<T>
    {
        public T value;
    }

    public interface IRemoteCall
    {
        void Reg(string key, System.Action fun);
        void Reg<Param>(string key, System.Action<Param> fun) where Param : new();
        void Reg<Result>(string key, System.Func<Result> fun) where Result : new();
        void Reg<Param, Result>(string key, System.Func<Param, Result> fun) where Param : new() where Result : new();
        void RegAsync(string key, System.Func<System.Collections.IEnumerator> fun);
        void RegAsync<Param>(string key, System.Func<Param, System.Collections.IEnumerator> fun) where Param : new();
        void RegAsync<Result>(string key, System.Func<OutValue<Result>, System.Collections.IEnumerator> fun) where Result : new();
        void RegAsync<Param, Result>(string key, System.Func<Param, OutValue<Result>, System.Collections.IEnumerator> fun) where Param : new() where Result : new();
    }
}
#endif
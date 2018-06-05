#if !USE_HOT
namespace xys.hot.RPC
{
    using System.Collections;

    class Remote : IRemoteCall
    {
        public Remote(wProtobuf.RPC.IRemoteCall parent)
        {
            this.parent = parent;
        }

        wProtobuf.RPC.IRemoteCall parent { get; set; }

        public void Reg(string key, System.Action fun)
        {
            parent.Reg(key, fun);
        }

        class RegParamType<T> where T : new()
        {
            public RegParamType(System.Action<T> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Action<T> onEnd;

            public void OnEnd(wProtobuf.RealBytes bytes)
            {
                T obj = RPCHelp.MergeFrom<T>(bytes);
                if (onEnd != null)
                    onEnd(obj);
            }
        }

        public void Reg<Param>(string key, System.Action<Param> fun) where Param : new()
        {
            RegParamType<Param> value = new RegParamType<Param>(fun);
            parent.Reg<wProtobuf.RealBytes>(key, value.OnEnd);
        }

        class RegResultType<T> where T : new()
        {
            public RegResultType(System.Func<T> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Func<T> onEnd;

            public wProtobuf.RealBytes OnEnd()
            {
                if (onEnd == null)
                    return null;

                T result = onEnd();
                if (result == null)
                    return null;

                return new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(result) };
            }
        }

        public void Reg<Result>(string key, System.Func<Result> fun) where Result : new()
        {
            RegResultType<Result> result = new RegResultType<Result>(fun);
            parent.Reg(key, result.OnEnd);
        }

        class RegParamResultType<Param, Result> where Param : new() where Result : new()
        {
            public RegParamResultType(System.Func<Param, Result> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Func<Param, Result> onEnd;

            public wProtobuf.RealBytes OnEnd(wProtobuf.RealBytes param)
            {
                if (onEnd == null)
                    return null;

                Param p = RPCHelp.MergeFrom<Param>(param);
                Result result = onEnd(p);
                if (result == null)
                    return null;

                return new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(result) };
            }
        }

        public void Reg<Param, Result>(string key, System.Func<Param, Result> fun) where Param : new() where Result : new()
        {
            RegParamResultType<Param, Result> rprt = new RegParamResultType<Param, Result>(fun);
            parent.Reg<wProtobuf.RealBytes, wProtobuf.RealBytes>(key, rprt.OnEnd);
        }

        // IEnumerator key()
        public void RegAsync(string key, System.Func<IEnumerator> fun)
        {
            parent.RegAsync(key, fun);
        }

        class RegParamTypeAtor<Param> where Param : new()
        {
            public RegParamTypeAtor(System.Func<Param, IEnumerator> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Func<Param, IEnumerator> onEnd;

            public IEnumerator OnEnd(wProtobuf.RealBytes param)
            {
                if (onEnd == null)
                    yield break;

                yield return onEnd(RPCHelp.MergeFrom<Param>(param));
            }
        }

        // IEnumerator key(Param input)
        public void RegAsync<Param>(string key, System.Func<Param, IEnumerator> fun) where Param : new()
        {
            var ator = new RegParamTypeAtor<Param>(fun);
            parent.RegAsync<wProtobuf.RealBytes>(key, ator.OnEnd);
        }

        class RegResultAtor<Result> where Result : new()
        {
            public RegResultAtor(System.Func<OutValue<Result>, IEnumerator> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Func<OutValue<Result>, IEnumerator> onEnd;

            public IEnumerator OnEnd(wProtobuf.RPC.OutValue<wProtobuf.RealBytes> param)
            {
                if (onEnd == null)
                    yield break;

                OutValue<Result> result = new OutValue<Result>();
                yield return onEnd(result);
                if (result.value == null)
                    yield break;

                param.value = new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(result.value) };
            }
        }

        // IEnumerator key(Result outValue)
        public void RegAsync<Result>(string key, System.Func<OutValue<Result>, IEnumerator> fun) where Result : new()
        {
            RegResultAtor<Result> rrar = new RegResultAtor<Result>(fun);
            parent.RegAsync<wProtobuf.RealBytes>(key, rrar.OnEnd);
        }

        class RegResultAtor<Param, Result> where Param : new() where Result : new()
        {
            public RegResultAtor(System.Func<Param, OutValue<Result>, IEnumerator> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Func<Param, OutValue<Result>, IEnumerator> onEnd;

            public IEnumerator OnEnd(wProtobuf.RealBytes param, wProtobuf.RPC.OutValue<wProtobuf.RealBytes> outValue)
            {
                if (onEnd == null)
                    yield break;

                OutValue<Result> result = new OutValue<Result>();
                yield return onEnd(RPCHelp.MergeFrom<Param>(param), result);
                if (result.value == null)
                    yield break;

                outValue.value = new wProtobuf.RealBytes() { bytes = RPCHelp.WriteTo(result.value) };
            }
        }

        // IEnumerator key(Param input, Result outValue)
        public void RegAsync<Param, Result>(string key, System.Func<Param, OutValue<Result>, IEnumerator> fun) where Param : new() where Result : new()
        {
            RegResultAtor<Param, Result> rrpr = new RegResultAtor<Param, Result>(fun);
            parent.RegAsync<wProtobuf.RealBytes, wProtobuf.RealBytes>(key, rrpr.OnEnd);
        }
    }

}
#endif
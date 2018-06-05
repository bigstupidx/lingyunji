#if MEMORY_CHECK
using System;
using System.Collections.Generic;
using Network;

public class NetChannel : Channel
{
    Dictionary<DebugProtocol, Action<SocketClient, BitStream>> mDic = new Dictionary<DebugProtocol, Action<SocketClient, BitStream>>();

    public void Reg(DebugProtocol dp, Action<SocketClient, BitStream> fun)
    {
        lock(mDic)
        {
            mDic[dp] = fun;
        }
    }

    public void UnReg(DebugProtocol dp)
    {
        lock(mDic)
        {
            mDic.Remove(dp);
        }
    }

    List<SocketClient> mSockets = new List<SocketClient>();

    protected override bool isReceiveSocket(SocketClient socket)
    {
		lock(mSockets)
		{
        	mSockets.Add(socket);
        	return true;
		}
    }

    public override void Release()
    {
        base.Release();
		lock(mSockets)
		{
        	for (int i = 0; i < mSockets.Count; ++i)
            	mSockets[i].Close();
        	mSockets.Clear();
		}
    }

    public override void OnRecMessage(SocketClient socket, BitStream stream)
    {
        DebugProtocol dp = (DebugProtocol)stream.ReadInt16();
        Action<SocketClient, BitStream> fun = null;
        lock (mDic)
        {
            mDic.TryGetValue(dp, out fun);
        }

        if (fun != null)
        {
            try
            {
                fun(socket, stream);
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}

#endif
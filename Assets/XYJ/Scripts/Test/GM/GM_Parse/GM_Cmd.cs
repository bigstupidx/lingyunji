/*----------------------------------------------------------------
// 创建者：
// 创建日期:
// 模块描述：
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using NetProto;
using xys;

public partial class GM_Cmd
{
    class CmdInfo
    {
        public string cmd;          //命令id
        public string cmdExplain;   //命令说明
        public string paraExplain;  //参数说明
        public bool sendToServer;
        public bool addToHelp;
        public Action<CmdParse> handleFun;    //命令处理
    }

    public static GM_Cmd instance = new GM_Cmd();

    Dictionary<string, CmdInfo> m_map = new Dictionary<string, CmdInfo>();

    //服务器发送回来的信息
    public List<string> m_gmCmdResultText = new List<string>();

    public GM_Cmd()
    {
        App.my.handler.Reg<GMMessageRespone>(Protoid.A2C_GM_CMD_Respone, GmCmdRet);
        RegisterAllCmd();
    }

    //注册一个gm命令
    public void AddCmd(Action<CmdParse> handleFun, string _cmd, string cmdExplain, string paraExplain,bool sendToServer,bool addToHelp)
    {
        string cmd = _cmd.ToLower();

        CmdInfo old_info;
        if (!m_map.TryGetValue(cmd, out old_info))
        {
            CmdInfo info = new CmdInfo();
            info.cmd = cmd;
            info.cmdExplain = cmdExplain;
            info.paraExplain = paraExplain;
            info.handleFun = handleFun;
            info.sendToServer = sendToServer;
            info.addToHelp = addToHelp;
            m_map.Add(cmd, info);
        }
        else
        {
            //UnityEngine.Debug.LogWarning("Repeate Regist GM Command " + cmd);
            Debug.Log("Repeate Regist GM Command " + cmd);
        }
    }



    void GmCmdRet(Network.IPacket packet, GMMessageRespone info)
    {
        xys.gm.GM_Log.AddLog(info.text);
    }


    //获得帮助
    public string GetHelp()
    {
        string text = "";
        foreach (CmdInfo p in m_map.Values)
        {
            if (p.addToHelp)
                text += string.Format("{0}   {1}   {2} \r\n", p.cmd, p.cmdExplain, p.paraExplain);
        }
        return text;
    }


    //解释命令
    public void ParseCmd(string inputText)
    {
        CmdParse cmdParse = new CmdParse(inputText);
        CmdInfo info;

        //本地cmd
        if (m_map.TryGetValue(cmdParse.GetStr().ToLower(), out info) && !info.sendToServer)
        {
            if (info.handleFun != null)
                info.handleFun(cmdParse);
        }
        //客户端没有注册的直接发给服务器
        else
        {
            GMMessage msg = new GMMessage();
            msg.cmd = inputText;
            App.my.socket.SendGame(Protoid.C2A_GM_CMD, msg);
        }
    }


    //
    public class CmdParse
    {
        string[] m_paraText;
        int m_paraIndex;

        public CmdParse(string cmd)
        {
            m_paraText = cmd.Split(' ');
        }

        public string GetStr()
        {
            return m_paraText[m_paraIndex++];
        }

        public int GetInt(int def = 0)
        {
            string v = m_paraText[m_paraIndex++];
            int result = 0;
            if (int.TryParse(v, out result))
                return result;
            else
                XYJLogger.LogError("int.TryParse Error! str : " + v);

            return def;
        }

        public float GetFloat(float def = 0)
        {
            string v = m_paraText[m_paraIndex++];

            if (string.IsNullOrEmpty(v))
                return def;
            float result = 0.0f;
            if (float.TryParse(v, out result))
                return result;
            else
                XYJLogger.LogError("float.TryParse Error! str : " + v);

            return def;
        }
    }

}

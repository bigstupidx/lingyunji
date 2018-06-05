///*----------------------------------------------------------------
//// 创建者：zfm
//// 创建日期:2015.10.17
//// 模块描述：游戏时间管理
////----------------------------------------------------------------*/
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using xys.UI;


////一天中的某个时刻,几时几分
//public class DayTime
//{
//    public int hh = 0;
//    public int mm = 0;
//    public int ss = 0;
//    public int ToInt()
//    {
//        return hh * 60 * 60 + mm * 60 + ss;
//    }
//    //偏移时刻
//    public DayTime OffsetTime(int addSce)
//    {
//        int counter = ToInt() + addSce;
//        DayTime d = new DayTime();
//        d.ss = counter % 60;
//        d.mm = (counter / 60) % 60;
//        d.hh = counter / 3600;
//        return d;
//    }

//    public override string ToString()
//    {
//        return string.Format("{0}:{1}", hh.ToString().PadLeft(2,'0'), mm.ToString().PadLeft(2, '0'));
//    }
//};

//public enum enDayTimeSegment
//{
//    enDayTimeSegmentMin,
//    enDayTimeSegmentNotStart,//没到这个时间段
//    enDayTimeSegmentRuning,//时间段内
//    enDayTimeSegmentOver,//过了这个时间了
//    enDayTimeSegmentMax,
//};

////时间段，用于判断在不在今天某个时间段内
//public class DayTimeSegment
//{
//    public DayTime start;
//    public DayTime end;

//    public override string ToString()
//    {
//        return string.Format("{0}~{1}", start.ToString(), end.ToString());
//    }

//    ////是否在时间段内,左开右闭
//    //public bool IsMatchNow()const;
//    //public bool IsMatch(const DayTime& d)const;

//    ////获取时刻相对于时间段的状态(没开始、时间段内已经结束)
//    //public enDayTimeSegment GetStateNow()const;
//    //public enDayTimeSegment GetState(const DayTime& d)const;
//};

////多个时间段，用于判断在不在今天某个时间段内
//public class DayTimeSegments
//{
//    public List<DayTimeSegment> timeSegs;

//    //是否在时间段内,如果不在时间段内返回-1，否则返回对应时间段的索引
//    //public int IsMatchNow()const;
//    //public int IsMatch(const DayTime& d)const;
//};


//public class MainTime
//{
//    //游戏暂停
//    static int s_pause = 0; 
//    //真实时间(不受timescale影响)
//    static public float realTimePass
//    {
//        get{return Time.realtimeSinceStartup;}
//    }

//    //游戏时间(受timescale和pause影响)
//    static public float timePass { get; private set; }


//    //帧间隔(受timescale和pause影响)
//    static public float deltaTime{ get; private set; }

//    //帧间隔(不受timescale和pause影响)
//    static public float realDeltaTime { get; private set; }

//    static private float lastRealTimeRecord;

//    //服务端通信时间延迟
//    static float s_servTimeDelay;
//    //服务端当前的时间戳
//    static double s_servTime;
//    //最后记录服务端时间的时间
//    static float s_lastRecordServTime;
//    static float s_serverOffset=0;
//    //服务端当前时间(返回0表示还没有对时成功,可以监听 EventDefine.MainEvents.FinishSysServerTime 消息来获取对时成功 )
//    static public double curServTime
//    {
//        get
//        {
//            if (s_servTime == 0)
//                return 0;
//            else
//                return s_servTime + (Time.realtimeSinceStartup - s_lastRecordServTime)+ s_serverOffset;
//        }
//    }
    
//    //服务器当前时间
//    static public System.DateTime curServDateTime
//    {
//        get
//        {
//			Debuger.Log ("todo ...");
//			return new  System.DateTime();
//        //    return Battle.TimeHelp.ConvertIntDateTime(curServTime);
//        }
//    }
//    //服务器时间，从2000年起的秒数
//    static public int curServTick{
//		get{ Debuger.Log ("todo cur Tick"); return 1;}
//		//get{ return ToTick(curServDateTime);}
//    }

//    static int s_midnightTimeId = 0;

//    //设置服务器时间
//    static public void SetServTime(double servTime, float servDelay)
//    {
//        s_lastRecordServTime = Time.realtimeSinceStartup;
//        s_servTimeDelay = servDelay;
//        s_servTime = servTime + servDelay;
////        Utils.EventDispatcher.Instance.TriggerEvent(EventDefine.MainEvents.FinishSysServerTime);

//        //打印时间
//       // System.DateTime now = Battle.TimeHelp.ConvertIntDateTime(curServTime);
//        //Debuger.LogError(string.Format("服务器时间={0} 网络延时={1}", now.ToString("yyyy-MM-dd hh:mm:ss"), s_servTimeDelay));

//        if(s_midnightTimeId != 0)
//        {
//            TimerManage.DelTimer(s_midnightTimeId);
//            s_midnightTimeId = 0;
//        }
//       // float midnightDis = (23 - now.Hour) * 3600 + (59 - now.Minute) * 60 + 60 - now.Second;
//       // s_midnightTimeId = Utils.TimerManage.AddTimer(TempFun, midnightDis, 86400);
//    }

//    static void TempFun()
//    {
//     //   Utils.EventDispatcher.Instance.TriggerEvent(EventDefine.MainEvents.MidnightTime);
//    }

//    //是否暂停玩家输入
//    static public bool IsPauseInput()
//    {
//        return s_pause>0;
//    }

//    //暂停玩家输入,需要确保任何时候都要配对调用，包含突然切换场景的情况
//    static public void PauseInput()
//    {
//        if (s_pause == 0)
//        {
////            Utils.EventDispatcher.Instance.TriggerEvent<bool>(EventDefine.SceneEvents.PauseMeInput, true);
//        }
//        s_pause++;
//    }

//    //恢复玩家输入，需要确保任何时候都要配对调用，包含突然切换场景的情况
//    static public void ResumeInput()
//    {
//        s_pause--;
//        if (s_pause == 0)
//        {
//        //    Utils.EventDispatcher.Instance.TriggerEvent<bool>(EventDefine.SceneEvents.PauseMeInput, false);  
//        }
//    }


//    static public void Update()
//    {
//#if COM_DEBUG
//   //     NetCore.SocketClient.s_delaytime = Random.Range(BroadCastHelp.s_testMinDelayTime, BroadCastHelp.s_testMaxDelayTime);
//     ////   if (GM_UI.instance != null)
//        {
//    //        GM_UI.instance.UpdateTick();
//        }

//#endif
//        if (IsPauseInput())
//        {
//            deltaTime = 0;
//            return;
//        }
//        else
//        {
//            deltaTime = Time.deltaTime;
//            timePass += deltaTime;
//        }

//        realDeltaTime = Time.realtimeSinceStartup - lastRealTimeRecord;
//        lastRealTimeRecord = Time.realtimeSinceStartup;
//    }
   
//    //注册消息
//    static public void RegisterEvent()
//    {
//   //     Utils.EventDispatcher.Instance.AddEventListener<LevelPrototype>(EventDefine.SceneEvents.BeginChangeScene, OnBeginChangeScene);
//  //      Utils.EventDispatcher.Instance.AddEventListener(EventDefine.MainEvents.LoginScene, OnLogin);

//    }

//    static void OnLogin()
//    {
//        s_servTimeDelay = 0;
//        s_servTime = 0;
//        s_lastRecordServTime = 0;
//    }

//    //切换场景开始
//  //  static void OnBeginChangeScene(LevelPrototype prototype)
//  //  {

// //   }

//    //设置服务器时间偏移，用于活动时间相关测试
//    public static void SetServerOffset(int offset){ s_serverOffset = offset; }

//    //从1970年起的秒，服务端发下来时间是int的话用这个函数转日期
// //   public static System.DateTime ToTime(int tick){return Battle.TimeHelp.Tick2LocalTime(tick);}
//  //  public static int ToTick(System.DateTime t) { return Battle.TimeHelp.LocalTime2Tick(t); }

//    //是不是同一天
//    public static bool IsSameDay(System.DateTime t, System.DateTime t2)
//    {
//        return t.Year == t2.Year && t.Month == t2.Month && t.Day == t2.Day;
//    }
//    public static bool IsSameDay(int tick,int tick2)
//    {
//		Debuger.Log ("todo ... check day");
//		return false;
//  //      return IsSameDay(ToTime(tick), ToTime(tick2));
//    }

//    //是不是今天
//    public static bool IsToday(System.DateTime t) { return IsSameDay(curServDateTime, t); }
////    public static bool IsToday(int tick) { return IsSameDay(curServDateTime, ToTime( tick)); }

//    //时间转时刻
//    public static DayTime NowDayTime() { return ToDayTime(curServDateTime); }
//    public static DayTime ToDayTime(System.DateTime t)
//    {
//        DayTime d = new DayTime();
//        d.hh = t.Hour;
//        d.mm = t.Minute;
//        d.ss = t.Second;
//        return d;
//    }
//    public static bool TryToDayTime(string str,out DayTime d)//"12:00"
//    {
//        d = new DayTime();
//        List<int> ints= new List<int>();
//        StrUtil.Parse(str, ref ints, ':');
//        if (ints.Count == 0)//至少要有小时
//            return false;
//        d.hh = ints[0];
//        d.mm = ints.Count > 1 ? ints[1] : 0;
//        d.mm = ints.Count > 2 ? ints[2] : 0;
//        return true;
//    }

//    //字符串转时间段,"12:00,13:00"或"12:00~13:00"
//    public static bool TryToDayTimeSegment(string str, out DayTimeSegment  d)
//    {
//        d = new DayTimeSegment();
//        char split = str.IndexOf("~") != -1? '~' : ',';
//        var ss = str.Split(split);
//        if (ss.Length != 2)//必须是两个值	
//            return false;
//        return TryToDayTime(ss[0],out d.start) && TryToDayTime(ss[1], out  d.end);
//    }

//    //字符串转多个时间段
//    public static bool TryToDayTimeSegments(string str, out DayTimeSegments  d)
//    {
//        d = new DayTimeSegments();
//        var ss = str.Split(',');
//        if (ss.Length==0)
//            return false;
//        foreach (var s1 in ss)
//	    {
//            DayTimeSegment dd;
//            if (!TryToDayTimeSegment(s1,out  dd))
//                return false;
//            d.timeSegs.Add(dd);
//        }
//        return true;
//    }
//}

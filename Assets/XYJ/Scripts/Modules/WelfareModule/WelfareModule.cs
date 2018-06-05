using System;
using System.Collections;
using System.Collections.Generic;
using NetProto;
using UnityEngine;
using wProtobuf;

namespace xys
{
    using NetProto;
    using System.Reflection;

    public partial class WelfareModule : HotModule
    {
        public object welfareData { get { return refType.GetField("welfareMgr"); } }

        public HotModule module { get; private set; }

        public WelfareModule() : base("xys.hot.HotWelfareModule")
        {

        }
    }

}

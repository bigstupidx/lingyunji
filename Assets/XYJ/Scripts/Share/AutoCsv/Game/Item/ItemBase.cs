
// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ItemBase
    {
        // id
        public int id { get; set; }
        // 名称
        public string name { get; set; }
        // 类型
        public ItemType type { get; set; }
        // 子类型
        public int sonType { get; set; }
        // 品质
        public ItemQuality quality { get; set; }
        // 使用等级
        public int useLevel { get; set; }
        // 职业
        public JobMask job { get; set; }
        // 图标
        public string icon { get; set; }
        // 叠加数量
        public int stackNum { get; set; }
        // 是否绑定
        public bool isBind { get; set; }
        // 描述
        public string  desc { get; set; }

    }
}


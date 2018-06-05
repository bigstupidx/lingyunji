using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    // 注意，对话框类型，一旦调用隐藏接口，则立即会被对象池回收，
    // 要注意，外部尽量不要保存对话框对象
    public enum DialogType
    {
        Null = 0, // 无效的面板
        OneBtn, // 一个按钮
        TwoBtn, // 两个按钮

        Total, // 总个数
    }
}
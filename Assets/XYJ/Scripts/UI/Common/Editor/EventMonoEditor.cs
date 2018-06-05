using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

namespace xys.UI
{
    [CustomEditor(typeof(EventMono), true)]
    public class EventMonoEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EventMono mono = target as EventMono;
            EventID eid = Str2Enum.To(mono.eventName, EventID.Null);
            eid = (EventID)EditorGUILayout.EnumPopup("事件名", eid);
            if (eid.ToString() != mono.eventName)
            {
                UITools.RegisterUndo(mono, "EventMono");
                mono.eventName = eid.ToString();
            }

            mono.isAgain = EditorGUILayout.Toggle("重复触发", mono.isAgain);
            if (mono.isAgain)
            {
                mono.interval = EditorGUILayout.FloatField("触发间隔", mono.interval);
            }

            if (eid != EventID.Null)
            {
                var ept = (EventMono.ParamType)EditorGUILayout.EnumPopup("参数类型", mono.type);
                if (ept != mono.type)
                {
                    UITools.RegisterUndo(mono, "EventMono");
                    mono.type = ept;
                }
            }

            GUI.changed = false;
            switch (mono.type)
            {
            case EventMono.ParamType.Null:
                break;
            case EventMono.ParamType.Str:
                {
                    var v = EditorGUILayout.TextField("参数", mono.strParam);
                    if (v != mono.strParam)
                    {
                        UITools.RegisterUndo(mono, "EventMono");
                        mono.strParam = v;
                    }
                }
                break;
            case EventMono.ParamType.Int:
                {
                    var v = EditorGUILayout.IntField("参数", mono.intParam);
                    if (v != mono.intParam)
                    {
                        UITools.RegisterUndo(mono, "EventMono");
                        mono.intParam = v;
                    }
                }
                break;
            }
        }
    }
}

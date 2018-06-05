using UnityEngine;
using UnityEditor;

namespace UI
{
    [CustomEditor(typeof(UIPlayTween))]
    public class UIPlayTweenEditor : Editor
    {
        enum ResetOnPlay
        {
            ContinueFromCurrent,
            RestartTween,
            RestartIfNotPlaying,
        }

        enum SelectedObject
        {
            KeepCurrent,
            SetToNothing,
        }

        public override void OnInspectorGUI()
        {
            UIEditorTools.SetLabelWidth(120f);
            UI.UIPlayTween tw = target as UI.UIPlayTween;
            GUILayout.Space(6f);

            GUI.changed = false;
            GameObject tt = (GameObject)EditorGUILayout.ObjectField("Tween Target", tw.tweenTarget, typeof(GameObject), true);

            bool inc = EditorGUILayout.Toggle("Include Children", tw.includeChildren);
            int group = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(160f));

            AnimationOrTween.Direction dir = (AnimationOrTween.Direction)EditorGUILayout.EnumPopup("Play direction", tw.playDirection);
            AnimationOrTween.EnableCondition enab = (AnimationOrTween.EnableCondition)EditorGUILayout.EnumPopup("If target is disabled", tw.ifDisabledOnPlay);
            ResetOnPlay rs = tw.resetOnPlay ? ResetOnPlay.RestartTween : (tw.resetIfDisabled ? ResetOnPlay.RestartIfNotPlaying : ResetOnPlay.ContinueFromCurrent);
            ResetOnPlay reset = (ResetOnPlay)EditorGUILayout.EnumPopup("On activation", rs);
            AnimationOrTween.DisableCondition dis = (AnimationOrTween.DisableCondition)EditorGUILayout.EnumPopup("When finished", tw.disableWhenFinished);

            if (GUI.changed)
            {
                UIEditorTools.RegisterUndo("Tween Change", tw);
                tw.tweenTarget = tt;
                tw.tweenGroup = group;
                tw.includeChildren = inc;
                tw.playDirection = dir;
                tw.ifDisabledOnPlay = enab;
                tw.resetOnPlay = (reset == ResetOnPlay.RestartTween);
                tw.resetIfDisabled = (reset == ResetOnPlay.RestartIfNotPlaying);
                tw.disableWhenFinished = dis;
                EditorUtility.SetDirty(tw);
            }

            UIEditorTools.SetLabelWidth(80f);

            EditorGUILayout.PropertyField(onFinishedProperty);
        }

        SerializedProperty onFinishedProperty;

        void OnEnable()
        {
            onFinishedProperty = serializedObject.FindProperty("onFinished");
        }
    }
}
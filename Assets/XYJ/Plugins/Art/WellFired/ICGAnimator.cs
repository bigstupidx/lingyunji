using UnityEngine;
using System.Collections;

/// <summary>
/// 过场插件动态获取animator
/// </summary>
public interface ICGAnimator 
{
    Animator GetAnimator();

    void AniCrossFade(string stateName, float transitionDuration, int layer, float normalizedTime);

    void AniPlay(string stateName, int layer, float normalizedTime);
}

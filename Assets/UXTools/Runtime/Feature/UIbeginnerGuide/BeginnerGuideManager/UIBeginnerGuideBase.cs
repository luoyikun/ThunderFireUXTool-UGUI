using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GuideFinishType
{
    Strong, //强引导:黑幕镂空,必须点击镂空区域结束;强调，必须点强调目标，才能下一步，但是不影响其他非目标按钮操作
    Middle, //中引导：黑幕镂空,点击任意区域结束；强调，点任何地方下一步
    Weak    //弱引导：无黑幕，设定时间过后自动结束
}

//引导模板
public enum GuideTemplate
{
    t0, //手势模板，GuideTemplate_Gesture
    t1, //手柄模板
    // t2,
    // t3,
    // t4
}

/// <summary>
/// 所有引导模板上挂的UIBeginnerGuide的基类
/// 新建的引导模板Prefab上都应该挂一个派生出的UIBeginnerGuide类型
/// </summary>
public class UIBeginnerGuideBase : MonoBehaviour
{
    protected UIBeginnerGuideData guideData;
    protected string guideId;
    protected GuideFinishType guideFinishType;
    protected float guideFinishDuration;
    protected GameObject guidePrefab;
    public virtual void Init(UIBeginnerGuideData data)
    {
        guideData = data;

        guideId = guideData.guideID;
        guideFinishType = guideData.guideFinishType;
        guideFinishDuration = guideData.guideFinishDuration;
        guidePrefab = guideData.guideTemplatePrefab;
    }

    public virtual void Show()
    {

    }

    public virtual void Finish()
    {
    }
}

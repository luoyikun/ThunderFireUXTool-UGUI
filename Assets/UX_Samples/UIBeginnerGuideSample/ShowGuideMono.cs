using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 新手引导测试脚本
/// </summary>
public class ShowGuideMono : MonoBehaviour 
{
    void Start(){
        //StartCoroutine("ok");
        //具体某个UI上的引导数据加入到引导Mgr
        var guideDataList = transform.GetComponentInChildren<UIBeginnerGuideDataList>();
        UIBeginnerGuideManager.Instance.AddGuideList(guideDataList);
        //开始播放引导，会从第一个开始播放
        UIBeginnerGuideManager.Instance.ShowGuideList();
    }
    
    IEnumerator ok(){
        yield return new WaitForSeconds(0.5f);
        //UIBeginnerGuideManager.Instance.SetGuideID("MiddleGuide");
        UIBeginnerGuideManager.Instance.ShowGuideList();
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 新手引导管理器
/// </summary>
public class UIBeginnerGuideManager : MonoBehaviour
{
    private static UIBeginnerGuideManager instance;
    public static UIBeginnerGuideManager Instance
    {
        get
        {
            return instance;
        }
    }
    public bool isPreviewing;

    private List<UIBeginnerGuideDataList> guideDataList = new List<UIBeginnerGuideDataList>();//N组引导
    private UIBeginnerGuideDataList curGuideList;// 一组引导，包含一组UIBeginnerGuideData的引导列表，列表中的某个引导完成后，会自动开启列表中的下一个引导，直到整个引导列表完成
    private UIBeginnerGuideData curGuideData; //一组引导中的一个，记录UIBeginnerGuide中的各种数据。包括引导ID、引导类型、引导时长、引导模板，以及UIBeginnerGuide中包含的GuideWidgetData
    private UIBeginnerGuide curGuide;//一个引导的界面模板，界面上包含多种不同的GuideWidget
    private string targetID;

    // private bool guideShowing = false;
    // private bool GuideShowing { get { return guideShowing; } }
    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    // 描述：设置下一个引导列表从名字为id的引导项开始引导
    // 所属类：UIBeginnerGuideManager
    // 参数：
    //     id 下次播放引导时播放的id
    // 返回值:无
    public void SetGuideID(string id)
    {
        if (isPreviewing)
        {
            return;
        }
        targetID = id;
    }
    // 描述：向引导列表队列中添加一个引导
    // 所属类：UIBeginnerGuideManager
    // 参数：
    //     datalist 要添加的引导对象
    // 返回值:无
    public void AddGuideList(UIBeginnerGuideDataList datalist)
    {
        if (isPreviewing)
        {
            return;
        }
        int listIndex = guideDataList.IndexOf(datalist);
        if (listIndex == -1)
        {
            guideDataList.Add(datalist);
        }
        // if (GuideShowing)
        //     return;

        //ShowGuideList(datalist);
    }

    //清除N组引导
    public void ClearGuideList()
    {
        guideDataList.Clear();
    }
    public void RemoveGuideList(UIBeginnerGuideDataList dataList)
    {
        guideDataList.Remove(dataList);
    }

    // 描述：播放第一个引导列表
    // 所属类：UIBeginnerGuideManager
    // 参数：无
    // 返回值: 无
    public void ShowGuideList()
    {
        if (isPreviewing)
        {
            return;
        }
        if (guideDataList.Count != 0)
        {
            ShowGuideList(guideDataList[0]);
        }
    }

    // 描述：播放指定的引导列表
    // 所属类：UIBeginnerGuideManager
    // 参数：
    //     datalist 要播放的引导列表对象，是挂载在UI面板上
    // 返回值: 无
    public void ShowGuideList(UIBeginnerGuideDataList datalist)
    {
        if (isPreviewing)
        {
            return;
        }
        int listIndex = guideDataList.IndexOf(datalist);
        if (listIndex == -1)
        {
            return;
        }

        curGuideList = datalist;
        if (curGuideList.guideDataList.Count != 0)
        {
            if (string.IsNullOrEmpty(targetID))
            {
                ShowGuide(curGuideList.guideDataList.First());
            }
            else
            {
                var targetGuideList = curGuideList.guideDataList.Where(data => data.guideID == targetID).ToList();
                if (targetGuideList.Count != 0)
                {
                    targetID = null;
                    var targetGuide = targetGuideList.First();
                    if (targetGuide == null)
                    {
                        StartNextGuide();
                    }
                    ShowGuide(targetGuide);
                }
            }
        }
        else
        {
            StartNextGuide();
        }
    }

    // 描述：播放指定的引导列表中的指定ID
    // 所属类：UIBeginnerGuideManager
    // 参数：
    //     datalist 要播放的引导列表对象
    //     id 下次播放引导时播放的id
    // 返回值: 无
    public void ShowGuideList(UIBeginnerGuideDataList datalist, string guideID)
    {
        if (isPreviewing)
        {
            return;
        }
        targetID = guideID;
        int listIndex = guideDataList.IndexOf(datalist);
        if (listIndex == -1)
        {
            return;
        }
        curGuideList = datalist;
        if (curGuideList.guideDataList.Count != 0)
        {
            if (string.IsNullOrEmpty(targetID))
            {
                ShowGuide(curGuideList.guideDataList.First());
            }
            else
            {
                var targetGuideList = curGuideList.guideDataList.Where(data => data.guideID == targetID).ToList();
                if (targetGuideList.Count != 0)
                {
                    targetID = null;
                    var targetGuide = targetGuideList.First();
                    if (targetGuide == null)
                    {
                        StartNextGuide();
                    }
                    ShowGuide(targetGuide);
                }
            }
        }
        else
        {
            StartNextGuide();
        }
    }
    private void ShowGuide(UIBeginnerGuideData data)
    {
        curGuideData = data;

        var guideGo = Instantiate(curGuideData.guideTemplatePrefab, curGuideList.transform);
        Debug.Log($"实例化引导面板{curGuideData.guideTemplatePrefab.name}");
        curGuide = guideGo.GetComponent<UIBeginnerGuide>();
        curGuide.Init(curGuideData);
        curGuide.Show();
        if (curGuideData.guideFinishType == GuideFinishType.Weak)
        {
            //弱引导注册定时取消
            StartCoroutine(RegisterAutoFinish(curGuideData.guideFinishDuration, curGuideData.guideID));
        }
    }
    // 描述：结束某个ID的引导（若当前引导不是该ID，则该函数无效）
    // 所属类：UIBeginnerGuideManager
    // 参数：
    //     guideID 要结束的引导ID
    // 返回值: 无
    public void FinishGuide(string guideId)
    {
        if (curGuideData.guideID == guideId)
        {
            curGuide.Finish();
            //一个引导完成了，删除gameObject，再下一步创建先遮罩
            //这里会造成频繁的实例化，销毁消耗
            //因为原版是可以选择模板form，直接用手势模板就行了，一个游戏不需要切换不同模板
            //复用guideForm
            DestroyImmediate(curGuide.gameObject);
            StartNextGuide();
        }
    }

    // 描述：结束当前引导
    // 所属类：UIBeginnerGuideManager
    // 参数：无
    // 返回值: 无
    public void FinishGuide()
    {
        curGuide.Finish();
        DestroyImmediate(curGuide.gameObject);
        StartNextGuide();
    }
    private IEnumerator RegisterAutoFinish(float duration, string ID)
    {
        yield return new WaitForSeconds(duration);
        if (curGuideData != null && ID == curGuideData.guideID)
            FinishGuide(curGuideData.guideID);
    }
    private void StartNextGuide()
    {
       
        int index = curGuideList.guideDataList.IndexOf(curGuideData);
        Debug.Log($"开启下个引导，当前id:{curGuideData.guideID},idx:{index}");
        if (index < curGuideList.guideDataList.Count - 1)
        {
            //一个List没完成,只切换data
            index++;
            ShowGuide(curGuideList.guideDataList[index]);
        }
        else
        {
            int listIndex = guideDataList.IndexOf(curGuideList);
            if (listIndex < guideDataList.Count - 1)
            {
                //还有剩余的guideDataList没完成,切换到下一个List
                listIndex++;
                ShowGuideList(guideDataList[listIndex]);
            }
            else
            {
                //也没有其他的guideDataList了,结束引导,等待新的guidedatalist
                //guideShowing = false;
                curGuide = null;
                curGuideData = null;
            }
        }
    }
}

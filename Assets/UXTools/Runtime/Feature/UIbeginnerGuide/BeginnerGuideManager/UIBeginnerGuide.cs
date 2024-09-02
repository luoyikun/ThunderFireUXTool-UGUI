using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThunderFireUnityEx;

/// <summary>
/// 引导面板，包含高亮，强调，手势，文本等组件设置。例如GuideTemplate_Gesture 预制体
/// </summary>
public class UIBeginnerGuide : UIBeginnerGuideBase
{
    public GuideHighLight highLightWidget;
    public GuideText textWidget;
    public GuideGesture gestureWidget;
    public UIBeginnerGuideGamePad GamePadWidget;
    public GuideArrowLine arrowLineWidget;
    public GuideTargetStroke targetStrokeWidget;

    [HideInInspector]
    public List<Transform> customObjects;

    private GuideHighLightData guideHighLightData;
    private GuideTextData guideTextData;
    private GuideGestureData guideGestureData;
    private GuideGamePadData gamePadData;
    private GuideArrowLineData guideArrowLineData;
    private GuideTargetStrokeData targetStrokeData;
    private List<GuideSelfDefinedData> selfDefinedDataList;
    bool m_isInit = false;
#if UNITY_EDITOR
    public void EditorInit()
    {
        if (gestureWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideGesturePanelData))
            {
                guideGestureData.EditorInit();
            }
        }
    }
#endif

    public void Awake()
    {
        InitDataComp();
    }

    //初始化数据控件，防止每次都需要找
    public void InitDataComp()
    {
        if (m_isInit == false)
        {
            guideTextData = GetComponentInChildren<GuideTextData>(true);
            guideGestureData = GetComponentInChildren<GuideGestureData>(true);

            gamePadData = GetComponentInChildren<GuideGamePadData>(true);
            targetStrokeData = GetComponentInChildren<GuideTargetStrokeData>(true);
            guideArrowLineData = GetComponentInChildren<GuideArrowLineData>(true);
            guideHighLightData = GetComponentInChildren<GuideHighLightData>(true);

            m_isInit = true;
        }
    }
    public override void Init(UIBeginnerGuideData data)
    {
        base.Init(data);
        InitDataComp();
        Debug.Log($"初始化引导数据：{data.guideID}");
        //文本
        if (textWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideTextPanelData))
            {
                textWidget.gameObject.SetActive(true);
                Debug.Log($"引导文本{guideData.guideTextPanelData}");
                //guideTextData = GetComponentInChildren<GuideTextData>(true);
                guideTextData.Load(guideData.guideTextPanelData);
                textWidget.Init(guideTextData);
            }
            else
            {
                textWidget.gameObject.SetActive(false);
            }
        }


        //手势
        if (gestureWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideGesturePanelData))
            {
                gestureWidget.gameObject.SetActive(true);
                Debug.Log($"引导手势{guideData.guideGesturePanelData}");
                //guideGestureData = GetComponentInChildren<GuideGestureData>(true);
                guideGestureData.Load(guideData.guideGesturePanelData);
                guideGestureData.SetCustomGesturePrefab(guideData.GestureObject);
                guideGestureData.SetTarget(guideData.selectedObject);
                gestureWidget.SetCustomGesturePrefab(guideData.GestureObject);
                gestureWidget.Init(guideGestureData);
            }
            else
            {
                gestureWidget.gameObject.SetActive(false);
                //gestureWidget.Init(null);
            }
        }

        //游戏面板
        if (GamePadWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.gamePadPanelData))
            {
                GamePadWidget.gameObject.SetActive(true);
                Debug.Log($"引导游戏面板{guideData.gamePadPanelData}");
                //gamePadData = GetComponentInChildren<GuideGamePadData>(true);
                gamePadData.Load(guideData.gamePadPanelData);
                GamePadWidget.Init(gamePadData);
            }
            else {
                GamePadWidget.gameObject.SetActive(false);
            }
        }

        //强调框
        if (targetStrokeWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.targetStrokeData))
            {
                //Debug.Log($"引导强调框{guideData.targetStrokeData}");
                //targetStrokeData = GetComponentInChildren<GuideTargetStrokeData>(true);
                targetStrokeWidget.gameObject.SetActive(true);
                targetStrokeWidget.SetGuideData(guideData);
                targetStrokeData.Load(guideData.targetStrokeData);
                if (targetStrokeData.targetType == TargetType.Target && guideData.strokeTarget)
                {
                    targetStrokeData.SetTarget(guideData.strokeTarget);
                    targetStrokeWidget.SetTarget(guideData.strokeTarget);
                }
                targetStrokeWidget.Init(targetStrokeData);
            }
            else
            {
                targetStrokeWidget.gameObject.SetActive(false);
            }
        }

        if (arrowLineWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideArrowLineData))
            {
                arrowLineWidget.gameObject.SetActive(true);
                //guideArrowLineData = GetComponentInChildren<GuideArrowLineData>(true);
                guideArrowLineData.Load(guideData.guideArrowLineData);
                arrowLineWidget.Init(guideArrowLineData);
            }
            else
            {
                arrowLineWidget.gameObject.SetActive(false);
            }
        }

        //高亮
        if (highLightWidget != null)
        {
            if (!string.IsNullOrEmpty(guideData.guideHighLightData))
            {
                Debug.Log($"引导高亮{guideData.guideHighLightData}");
                highLightWidget.gameObject.SetActive(true);
                guideHighLightData.Load(guideData.guideHighLightData);
                highLightWidget.Init(guideHighLightData);


                highLightWidget.SetType(data.guideFinishType);
                highLightWidget.SetID(data.guideID);
                //应该先设置数据，再设置ui目标。如果是强调型，这里不应该执行
                highLightWidget.SetTarget(guideData.highLightTarget);
                if (guideData.highLightTarget)
                {
                    guideHighLightData.SetTarget(guideData.highLightTarget);
                }

            }
            else
            {
                highLightWidget.gameObject.SetActive(false);
            }
        }


        if (guideData.GuideSelfDefinedData != null && guideData.GuideSelfDefinedData.Count != 0)
        {
            foreach (string item in guideData.GuideSelfDefinedData)
            {
                GuideSelfDefinedData guideSelfDefinedData = new GuideSelfDefinedData();
                guideSelfDefinedData.Load(item);
                GameObject go = GameObject.Find(transform.name + guideSelfDefinedData.parentPath);
                // 如果父节点的Active为false，就会变成null
                if (go != null)
                {
                    GameObject gameObject = new GameObject(guideSelfDefinedData.name);
                    gameObject.transform.SetParent(go.transform);
                    gameObject.AddComponent<RectTransform>();
                    gameObject.AddComponent<Text>();
                    gameObject.GetComponent<Text>().text = guideSelfDefinedData.text;
                    gameObject.AddComponent<GuideSelfDefinedData>();
                    guideSelfDefinedData = gameObject.GetComponent<GuideSelfDefinedData>();
                    guideSelfDefinedData.Load(item);
                    gameObject.AddComponent<GuideSelfDefined>();
                    gameObject.GetComponent<GuideSelfDefined>().Init(guideSelfDefinedData);
                }
            }
        }

        InitCustomObject();
    }

    private void InitCustomObject()
    {
        if (!string.IsNullOrEmpty(guideData.CustomTransformDatas))
        {
            Dictionary<string, string> transformDatas = JsonUtilityEx.FromJson<string, string>(guideData.CustomTransformDatas);

            foreach (var kvp in transformDatas)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    GuideTransformData data = trans.GetOrAddComponent<GuideTransformData>();
                    data.Load(kvp.Value);
                    data.ApplyTransformData(trans);
                    Object.DestroyImmediate(data);
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomTextDatas))
        {
            Dictionary<string, string> textDatas = JsonUtilityEx.FromJson<string, string>(guideData.CustomTextDatas);

            foreach (var kvp in textDatas)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Text text = trans.GetComponent<Text>();
                    if (text != null)
                    {
                        JsonUtility.FromJsonOverwrite(kvp.Value, text);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomTextFont))
        {
            Dictionary<string, string> TextFont = JsonUtilityEx.FromJson<string, string>(guideData.CustomTextFont);
            foreach (var kvp in TextFont)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Text text = trans.GetComponent<Text>();
                    if (text != null)
                    {
                        text.font = ResourceManager.Load<Font>(TextFont[kvp.Key]);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomImageDatas))
        {
            Dictionary<string, string> imageDatas = JsonUtilityEx.FromJson<string, string>(guideData.CustomImageDatas);

            foreach (var kvp in imageDatas)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Image image = trans.GetComponent<Image>();
                    if (image != null)
                    {
                        JsonUtility.FromJsonOverwrite(kvp.Value, image);
                    }
                }
            }
        }

        if (!string.IsNullOrEmpty(guideData.CustomImagesprite))
        {
            Dictionary<string, string> imageSprite = JsonUtilityEx.FromJson<string, string>(guideData.CustomImagesprite);
            foreach (var kvp in imageSprite)
            {
                Transform trans = transform.Find(kvp.Key);
                if (trans != null)
                {
                    Image image = trans.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = ResourceManager.Load<Sprite>(imageSprite[kvp.Key]);
                    }
                }
            }
        }
    }

    public override void Show()
    {
        base.Show();
        gestureWidget?.Show();
        GamePadWidget?.Show();
    }

    public override void Finish()
    {
        base.Finish();
        gestureWidget?.Stop();
        highLightWidget?.Stop();
        textWidget?.Stop();
        arrowLineWidget?.Stop();
        targetStrokeWidget?.Stop();
        GamePadWidget?.Stop();
    }
#if UNITY_EDITOR
    #region Test

    public void HighLightAreaPreview(RectTransform target)
    {

    }
    #endregion
#endif
}

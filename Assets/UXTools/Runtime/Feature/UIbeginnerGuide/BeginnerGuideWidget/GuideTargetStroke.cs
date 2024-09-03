using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 强调框
/// </summary>
public class GuideTargetStroke : GuideWidgetBase
{
    private GameObject target;
    public GameObject square;
    public GameObject circle;
    public Button m_btnTarget = null; //目标按钮
    UIBeginnerGuideData m_guideData;
    public override void Init(GuideWidgetData data)
    {
        GuideTargetStrokeData targetStrokeData = data as GuideTargetStrokeData;
        if (targetStrokeData != null)
        {
            targetStrokeData.ApplyTransformData(transform);
            if (targetStrokeData.targetType == TargetType.Target && target != null)
            {
                transform.position = target.transform.position;
                transform.eulerAngles = target.transform.eulerAngles;
                transform.localScale = target.transform.localScale;
                transform.GetComponent<RectTransform>().sizeDelta = target.GetComponent<RectTransform>().sizeDelta;
            }
            square.SetActive(targetStrokeData.strokeType == StrokeType.Square);
            circle.SetActive(targetStrokeData.strokeType == StrokeType.Circle);
            square.GetComponent<Animator>().enabled = targetStrokeData.playAnimator;
            circle.GetComponent<Animator>().enabled = targetStrokeData.playAnimator;
        }
    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();

        return list;
    }


    public void SetTarget(GameObject go)
    {
        BtnTargetRemoveListener();
        target = go;
        BtnTargetAddListener(go);
    }
    public override void Show()
    {
    }
    public override void Stop()
    {
    }

    void BtnTargetRemoveListener()
    {
        if (m_btnTarget != null)
        {
            m_btnTarget.onClick.RemoveListener(finish);
            Debug.Log($"引导移除目标按钮回调-》{m_btnTarget.name}");
            m_btnTarget = null;
        }
    }

    void BtnTargetAddListener(GameObject go)
    {
        if (m_guideData.guideFinishType == GuideFinishType.Strong)
        {
            m_btnTarget = go.GetComponent<Button>();
            if (m_btnTarget != null)
            {
                m_btnTarget.onClick.AddListener(finish);
                Debug.Log($"强引导绑定目标按钮回调-》{m_btnTarget.name}");
            }
            else
            {
                Debug.LogError($"强引导需要目标具有button-》target{go.name}");
            }


        }
    }

    public void SetGuideData(UIBeginnerGuideData guideData)
    {
        m_guideData = guideData;
    }

    /// <summary>
    /// 开启下一步
    /// </summary>
    public void finish()
    {
        //需要先移除target回调再开启下一步
        BtnTargetRemoveListener();
        UIBeginnerGuideManager.Instance.FinishGuide(m_guideData.guideID);

    }
}

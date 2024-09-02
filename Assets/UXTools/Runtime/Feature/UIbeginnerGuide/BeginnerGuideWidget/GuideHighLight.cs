using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public enum HighLightType
{
    Circle,
    Square
}
//高亮遮罩脚本
public class GuideHighLight : GuideWidgetBase, ICanvasRaycastFilter, IPointerClickHandler
{
    private GuideFinishType guideFinishType;
    private string guideID;
    private bool isCircle = true; //rect at first
    private bool isVague = false;
    private bool UseCustomTarget;
    private RectTransform target;

    private Vector3 center;     // 镂空区域的中心
    private float width;        // 镂空区域的宽
    private float height;       // 镂空区域的高
    private Canvas canvas;

    public GameObject childObject;
    public Material rectMaterial;
    public Material circleMaterial;

    private Vector3[] targetCorners = new Vector3[4];//存储要镂空组件的四个角的数组
    public Button m_btnTarget = null; //目标按钮
    public Vector2 WorldToScreenPoint(Canvas canvas, Vector3 world)
    {
        //把世界坐标转化为屏幕坐标
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, world);

        // 屏幕坐标转换为局部坐标
        //out的是vector2类型，事先声明
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(),
                                            screenPoint, canvas.worldCamera, out localPoint);
        return localPoint;
    }

    public override void Init(GuideWidgetData data)
    {
        GuideHighLightData guideHighLightData = data as GuideHighLightData;
        if (guideHighLightData != null)
        {
            guideHighLightData.ApplyTransformData(transform);
            if (guideHighLightData.highLightType == HighLightType.Circle)
            {
                isCircle = true;
            }
            else isCircle = false;

            isVague = guideHighLightData.isVague;
            UseCustomTarget = guideHighLightData.UseCustomTarget;
            //GetComponent<RectGuide>().target = guideHighLightData.target;
            childObject.transform.localPosition = guideHighLightData.childPos;
            childObject.transform.eulerAngles = guideHighLightData.childRot;
            childObject.transform.localScale = guideHighLightData.childScale;
            childObject.transform.GetComponent<RectTransform>().sizeDelta = guideHighLightData.childSize;
        }

    }

    public override List<int> GetControlledInstanceIds()
    {
        List<int> list = new List<int>();

        return list;
    }

    public void SetType(GuideFinishType type)
    {
        this.guideFinishType = type;
        Debug.Log($"设置引导类型{type.ToString()}");
    }

    private void SetRectHighLightArea()
    {
        InitTarget();
        //设置材质的中心点
        rectMaterial.SetVector("_Center", center);
        //设置材质的宽高
        rectMaterial.SetFloat("_SliderX", width);
        rectMaterial.SetFloat("_SliderY", height);
    }
    private void SetCircleHighLightArea()
    {
        InitTarget();
        circleMaterial.SetVector("_Center", center);
        circleMaterial.SetFloat("_SliderX", width);
        circleMaterial.SetFloat("_SliderY", height);

    }

    /// <summary>
    /// 点击穿透区域,穿透下去，如何驱动引导下一步执行
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="eventCamera"></param>
    /// <returns>返回false为穿透，true为屏蔽下层点击</returns>
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        
        if (target == null) { return true; }//点击不了
        if (guideFinishType == GuideFinishType.Strong)
        {
            //强引导穿透到挖孔区
            if (RectTransformUtility.RectangleContainsScreenPoint(target, sp))
            {
                return false;
            }
        }
        return true;


    }
    //移除上一个按钮的响应
    void BtnTargetRemoveListener()
    {
        if (m_btnTarget != null)
        {
            m_btnTarget.onClick.RemoveListener(() => finish());
            Debug.Log($"引导移除目标按钮回调-》{m_btnTarget.name}");
            m_btnTarget = null;
        }
    }

    void BtnTargetAddListener(GameObject go)
    {
        if (guideFinishType == GuideFinishType.Strong)
        {
            m_btnTarget = go.GetComponent<Button>();
            if (m_btnTarget != null)
            {
                m_btnTarget.onClick.AddListener(() => finish());
                Debug.Log($"强引导绑定目标按钮回调-》{m_btnTarget.name}");
            }
            else
            {
                Debug.LogError($"强引导需要目标具有button-》target{go.name}");
            }


        }
    }
    /// <summary>
    /// 开启下一步
    /// </summary>
    public void finish()
    {
        //需要先移除target回调再开启下一步
        BtnTargetRemoveListener();
        UIBeginnerGuideManager.Instance.FinishGuide(guideID);
        
    }
    public void SetTarget(GameObject go)
    {

        BtnTargetRemoveListener();

        if (go == null)
        {
            target = childObject.GetComponent<RectTransform>();
            Debug.Log($"设置目标1：{target.name}");
        }
        else
        {
            if (UseCustomTarget)
            {
                childObject.transform.position = go.transform.position;
                childObject.transform.eulerAngles = go.transform.eulerAngles;
                childObject.GetComponent<RectTransform>().sizeDelta = new Vector2(go.GetComponent<RectTransform>().rect.width, go.GetComponent<RectTransform>().rect.height);
                childObject.transform.localScale = go.transform.localScale;
                target = go.GetComponent<RectTransform>();
                Debug.Log($"设置目标2：{target.name}");
                BtnTargetAddListener(go);
            }
            else
            {
                target = childObject.GetComponent<RectTransform>();
                Debug.Log($"设置目标3：{target.name}");
            }
        }
        //Debug.Log(target.position);
        InitTarget();
        if (isCircle)
        {
            transform.GetComponent<Image>().material = circleMaterial;
            SetCircleHighLightArea();
        }
        else
        {
            transform.GetComponent<Image>().material = rectMaterial;
            SetRectHighLightArea();
        }

    }
    public void SetID(string id)
    {
        guideID = id;
    }
    //设置目标的4个顶点围起来的范围
    private void InitTarget()
    {
        canvas = transform.GetComponentInParent<Canvas>();
        if (target == null) return;
        // 获取中心点
        // GetWorldCorners:在世界空间中得到计算的矩形的角。参数角的数组
        target.GetWorldCorners(targetCorners);

        // 讲四个角的世界坐标转为局部坐标坐标
        for (int i = 0; i < targetCorners.Length; i++)
        {
            targetCorners[i] = WorldToScreenPoint(canvas, targetCorners[i]);
        }

        //计算中心点// 计算宽高
        center.x = targetCorners[0].x + (targetCorners[3].x - targetCorners[0].x) / 2;
        center.y = targetCorners[0].y + (targetCorners[1].y - targetCorners[0].y) / 2;
        width = (targetCorners[3].x - targetCorners[0].x) / 2;
        height = (targetCorners[1].y - targetCorners[0].y) / 2;
    }
    public override void Show()
    {
        if (guideFinishType == GuideFinishType.Strong)
        {

        }
        else if (guideFinishType == GuideFinishType.Middle)
        {

        }
    }

    public override void Stop()
    {
    }
    public void OnPointerClick(PointerEventData data)
    {
        Debug.Log("OnPointerClick");
        if (guideFinishType == GuideFinishType.Middle || guideFinishType == GuideFinishType.Weak)
        {
            if (guideFinishType == GuideFinishType.Weak)
            {
                //如果是弱引导，还需要关闭自动关闭的协程
                UIBeginnerGuideManager.Instance.StopCorWeekFinish();
            }
            //点击任何地方都可以开启下一步
            UIBeginnerGuideManager.Instance.FinishGuide(guideID);

           
        }
        // else if (guideFinishType == GuideFinishType.Strong)
        // {
        //     // Vector2 sp = Input.mousePosition;
        //     // Debug.Log(sp);
        //     // //Debug.Log(target.)
        //     // if (RectTransformUtility.RectangleContainsScreenPoint(target, sp))
        //     // {
        //     //     UIBeginnerGuideManager.Instance.FinishGuide(guideID);
        //     // }
        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit))
        //     {
        //         Debug.Log(hit.collider.gameObject.name);
        //         // obj = hit.collider.gameObject;
        //         // //通过名字
        //         // if (obj.name.Equals("BeiJiChuan"))
        //         // {
        //         //     Debug.Log("点中" + obj.name);
        //         // }
        //         // //通过标签
        //         // if (obj.tag == "ClicObj")
        //         // {
        //         //     Debug.Log("点中" + obj.name);
        //         // }
        //     }
        // }

    }
}
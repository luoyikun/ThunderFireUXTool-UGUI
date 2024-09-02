
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//高亮数据脚本
[System.Serializable]
public class GuideHighLightData : GuideWidgetData
{
    public HighLightType highLightType;
    public bool isVague;
    public bool UseCustomTarget;
    public Vector3 childPos;
    public Vector3 childScale;
    public Vector3 childRot;
    public Vector2 childSize;
    public RectTransform target;

    public override string Serialize()
    {
        if (Open)
        {
            childPos = transform.GetChild(0).localPosition;
            childRot = transform.GetChild(0).eulerAngles;
            childScale = transform.GetChild(0).localScale;
            childSize = transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
            UpdateTransformData();
            string data = JsonUtility.ToJson(this);
            Debug.Log($"高亮数据保存{data}");
            return data;
        }
        Debug.Log($"高亮数据保存空");
        return "";
        //throw new System.NotImplementedException();
    }
    public void SetTarget(GameObject go)
    {
        target = go.GetComponent<RectTransform>();
    }
}

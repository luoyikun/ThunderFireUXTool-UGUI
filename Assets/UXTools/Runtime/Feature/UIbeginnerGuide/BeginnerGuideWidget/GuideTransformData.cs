
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuideTransformData : GuideWidgetData
{
    public override string Serialize()
    {
        if (Open)
        {
            UpdateTransformData();
            string data = JsonUtility.ToJson(this);
            return data;
        }
        return "";
    }
}

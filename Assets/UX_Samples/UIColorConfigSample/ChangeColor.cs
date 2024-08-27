using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    public UXImage img1;
    public UXImage img2;
    // Start is called before the first frame update
    void Start()
    {
        UIColorUtils.LoadGamePlayerConfig();
        string[] s1 = Enum.GetNames(typeof(UIColorGenDef.UIColorConfigDef));
        UIColorGenDef.UIColorConfigDef[] v1 = (UIColorGenDef.UIColorConfigDef[])Enum.GetValues(typeof(UIColorGenDef.UIColorConfigDef));
        for(int i = 0; i< s1.Length; i++)
        {
            if(s1[i] == "Def_COLOR1"){
                img1.color = UIColorUtils.GetDefColor(v1[i]);
                break;
            }
        }
        img2.m_ColorType = UXImage.ColorType.Gradient_Color;
        img2.Direction = UXImage.GradientDirection.Horizontal;
        string[] s2 = Enum.GetNames(typeof(UIGradientGenDef.UIGradientConfigDef));
        UIGradientGenDef.UIGradientConfigDef[] v2 = (UIGradientGenDef.UIGradientConfigDef[])Enum.GetValues(typeof(UIGradientGenDef.UIGradientConfigDef));
        for(int i = 0; i< s2.Length; i++)
        {
            if(s2[i] == "Def_渐变1"){
                img2.gradient = UIColorUtils.GetDefGradient(v2[i]);
                break;
            }
        }
        
        
    }


    // Update is called once per frame
    void Update()
    {

    }
}

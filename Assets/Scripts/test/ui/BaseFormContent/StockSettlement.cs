using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockSettlement : MonoBehaviour
{
    /// <summary>
    /// 股票总数量
    /// </summary>
    int theSum;
    /// <summary>
    /// 实时剩余数量 根据这个数值来动态调整 其他控件的最大值
    /// </summary>
    int remainSum;
    /// <summary>
    /// 玩家持有的股票数量
    /// </summary>
    public TextMeshProUGUI sumText;
    /// <summary>
    /// 要出售的股票数量
    /// </summary>
    public Slider sliderA;
    /// <summary>
    /// 玩家要出售的股票数量
    /// </summary>
    public TextMeshProUGUI sellText;
    /// <summary>
    /// 要替换的股票数量
    /// </summary>
    public Slider sliderB;
    /// <summary>
    /// 玩家要替换成新股的股票数量
    /// </summary>
    public TextMeshProUGUI transText;
    /// <summary>
    /// 要保留的股票数量
    /// </summary>
    public Slider sliderC;
    /// <summary>
    /// 玩家要保留的股票数量
    /// </summary>
    public TextMeshProUGUI keepText;
    private Company theNewCompany;

    int SliderAVal;
    int SliderBVal;
    int SliderCVal;

    int SliderAmaxVal;
    int SliderBmaxVal;
    int SliderCmaxVal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SliderAChange()
    {
        if (sliderA != null)
        {
            int sumtext = (int)(theSum * sliderA.value);//取个整数
            SliderAVal = sumtext;
            remainSum = theSum - SliderAVal - SliderBVal - SliderCVal;
            if (remainSum >= 0)
            {
                sellText.text = sumtext.ToString();

                sumText.text = remainSum.ToString();
                UpdateVal();
            }
        }
    }
    public void SliderBChange()
    {
        if (sliderB != null)
        {
            int sliderMaxVal = theSum;
            //这里需要增加新公司股票余量的判断
            if (theNewCompany != null)
            {
                if (theNewCompany.RemainStock * 2 < remainSum)
                {
                    sliderMaxVal = theNewCompany.RemainStock * 2;
                }
            }
            int sumtext = (int)(sliderMaxVal * sliderB.value);//取个整数
            //交换新股需要至少两张换一张
            sumtext = sumtext - sumtext % 2;
            if (sumtext < 2)
            {
                sumtext = 0;
            }
            SliderBVal = sumtext;
            remainSum = theSum - SliderAVal - SliderBVal - SliderCVal;
            if (remainSum >= 0)
            {
                transText.text = sumtext.ToString();
                sumText.text = remainSum.ToString();
                UpdateVal();
            }

        }
    }
    public void SliderCChange()
    {
        if (sliderC != null)
        {
            int sumtext = (int)(theSum * sliderC.value);//取个整数
            SliderCVal = sumtext;
            remainSum = theSum - SliderAVal - SliderBVal - SliderCVal;
            if (remainSum >= 0)
            {
                keepText.text = sumtext.ToString();
                sumText.text = remainSum.ToString();
                UpdateVal();
            }
        }
    }

    internal void InitContent(string val)
    {
        string[] strings = val.Split('|');
        if (strings.Length > 0) 
        { theSum =remainSum=ConvertFuntion.StringToInt(strings[0]); }
        if (strings.Length > 2)
        {
            string[] strss = strings[2].Split(',');
            theNewCompany =new Company(strings[2]);
            theNewCompany.RemainStock = ConvertFuntion.StringToInt(strss[4]); ;
            sumText.text = remainSum.ToString();
        }
    }

    void UpdateVal()
    {
        BaseForm baseForm=GetComponentInParent<BaseForm>();
        if (baseForm != null)
        {
            baseForm.reVal = sellText.text + "," + transText.text + "," + keepText.text;
        }
    }
}

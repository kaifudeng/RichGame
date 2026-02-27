using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StockSum : MonoBehaviour
{
    public TextMeshProUGUI sum;
    public TextMeshProUGUI message;
    public Slider Slider;

    public int SlidermaxVal;
    /// <summary>
    /// 玩家存款的上限
    /// </summary>
    public int CashLimit;

    internal void InitContent(int maxVal,int limit)
    {
        SlidermaxVal = maxVal;
        CashLimit = limit;
    }
    public void SliderChange()
    {
        if (Slider != null)
        {
            int sumtext = (int)(SlidermaxVal * Slider.value);

            sum.text = sumtext.ToString();

            BaseForm baseForm = gameObject.GetComponentInParent<BaseForm>();
            if (baseForm != null) 
            {
                baseForm.reVal = sum.text;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

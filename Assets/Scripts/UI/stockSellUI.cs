using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class stockSellUI : MonoBehaviour
{
    public bool type;//0为出售 1为置换
    public StockSetForm parentForm;

    public string theCompanyName;
    /// <summary>
    /// 股价
    /// </summary>
    public int stockPrice;

    public CompaniesController companiesController;

    public TextMeshProUGUI message;

    public Company stockCompany;
    public Player player;
    public TextMeshProUGUI sum;
    public TextMeshProUGUI title;
    public Slider Slider;
    /// <summary>
    /// 滑动条拉满时代表的数值
    /// </summary>
    public int SlidermaxVal;

    public void SliderChange()
    {
        if (Slider != null)
        {
            int sumtext = (int)(SlidermaxVal * Slider.value);
            sum.text = sumtext.ToString();
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
    public void onSubmitButtonClick()
    {
        parentForm.ReturnValue(Convert.ToInt16(sum.text));
    }
    public void onExitButtonClick()
    {

    }
}

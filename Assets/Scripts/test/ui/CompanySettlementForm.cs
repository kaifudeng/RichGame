using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CompanySettlementForm : MonoBehaviour
{
    /// <summary>
    /// 标题
    /// </summary>
    public TextMeshProUGUI title;
    public Company theCompany;
    CompaniesController companiesController;
    string reqId;
    GameStage gameStage;
    public Player thePlayer;
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
    public void SliderAChange()
    {
        if (sliderA != null)
        {
            int sumtext = (int)(theSum * sliderA.value);//取个整数
            SliderAVal = sumtext;
            remainSum = theSum - SliderAVal- SliderBVal- SliderCVal;
            if (remainSum >= 0)
            {
                sellText.text = sumtext.ToString();

                sumText.text = remainSum.ToString();
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
            sumtext = sumtext-sumtext % 2;
            if (sumtext < 2)
            {
                sumtext = 0;
            }
            SliderBVal =sumtext;
            remainSum = theSum - SliderAVal - SliderBVal - SliderCVal;
            if (remainSum >= 0)
            {
                transText.text = sumtext.ToString();
                sumText.text = remainSum.ToString();
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

    internal void InitForm(string requestId, GameStage gameStage, Player player, string comps)
    {
        reqId = requestId;
        this.gameStage = gameStage;
        thePlayer = player;
        
        try
        {
            companiesController = GameObject.Find("CompaniesController(Clone)").GetComponent<CompaniesController>();
            string[] strs = comps.Split(',');
            if (strs.Length > 0)
            {
                theCompany = companiesController.FindCompanyWithName(strs[0]);
                if (theCompany != null)
                {
                    theSum=remainSum = thePlayer.GetStockSum(theCompany);
                    sumText.text = remainSum.ToString();
                    title.text = "当前持有(" + theCompany.CompanyName + ")的股票为：";
                }
                if (strs.Length > 1)
                    theNewCompany = companiesController.FindCompanyWithName(strs[1]);
            }
            //SliderAmaxVal = SliderBmaxVal = SliderCmaxVal = remainSum;
        } catch (Exception ex)
        {
            Debug.LogError("初始化窗体时出错：" + ex.Message);
        }
    }

    public void onSubmitButtonClick()
    {
        try
        {
            if (remainSum > 0)//还有没处理的股票 统一按照保留处理
            {
                keepText.text = remainSum + ConvertFuntion.StringToInt(keepText.text).ToString();
            }
            CommunicationTool.GetCommunicationTool().CmdSubmitResponse(reqId, sellText.text + "," + transText.text + "," + keepText.text);
            Destroy(gameObject);
        }
        catch(Exception ex)
        {
            Debug.LogError("提交处理方案时出现错误" + ex);
        }
    }

    public void onExitButtonClick()
    {
        keepText.text = remainSum + ConvertFuntion.StringToInt(keepText.text).ToString();
        CommunicationTool.GetCommunicationTool().CmdSubmitResponse(reqId,  "0," + "0," + keepText);
        //player.OverThisRound();
        Destroy(gameObject);
    }
}

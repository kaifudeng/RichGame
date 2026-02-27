using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class stockSumUI : NetworkBehaviour
{
    //[SyncVar(hook =nameof(TheCompanyNameChange))]
    public string theCompanyName;
    /// <summary>
    /// 股价
    /// </summary>
    [SyncVar] 
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
    public int SlidermaxVal = 3;
    internal GameStage gameStage;
    public string requestId;


    // Start is called before the first frame update
    void Start()
    {
        //companiesController = GameObject.Find("CompaniesController(Clone)").GetComponent<CompaniesController>();
    }
    /// <summary>
    /// 加载公司相关信息
    /// </summary>
    /// <param name="newVal"></param>
    public void InitCompanyInfo(string ComName)
    {
        if (companiesController == null)
        {
            companiesController = GameObject.Find("CompaniesController(Clone)").GetComponent<CompaniesController>();
        }
        theCompanyName = ComName;
        title.text = "买入" + theCompanyName + "的股票(最大为3)";
        foreach (Company company in companiesController.companies)
        {
            
            if (company.CompanyName == theCompanyName)
            {
                stockCompany = company;
                stockPrice = companiesController.GetPrice(stockCompany);
                if (stockCompany.RemainStock >= 3)
                {
                    title.text = theCompanyName + "的股票当前价格为" + stockPrice.ToString() + ",选择买入的数量(最大为3)";
                }
                else
                {
                    if (stockCompany.RemainStock == 0)
                    {
                        title.text = theCompanyName + "的股票已售空";
                        SlidermaxVal = 0;
                    }
                    else
                    {
                        title.text = theCompanyName + "的股票当前价格为" + stockPrice.ToString() + ",选择买入的数量(最大为" + stockCompany.RemainStock + ")";
                    }
                }

                break;
            }
        }
    }

    public void SliderChange()
    {
        if (Slider != null)
        {
            int sumtext = (int)(SlidermaxVal * Slider.value);
            sum.text = sumtext.ToString();
        }
    }

    public void onSubmitButtonClick()
    {
        if (stockCompany == null)
        {
            foreach (Company company in companiesController.companies)
            {
                if (company.CompanyName == theCompanyName)
                {
                    stockCompany = company;
                }
            }
        }
        int stockSum = Convert.ToInt32(sum.text);
        if (stockSum > 0)
        {
            // playerController.stocks.Add((stockCompany, stockSum));

            if (player.cash >= stockPrice * stockSum)//先判断玩家的钱是否足够
            {
                CommunicationTool.GetCommunicationTool().CmdSubmitResponse(requestId,stockSum.ToString());
                
                //1、玩家持有股票增加 更新玩家总资产数据
                //player.PlayerStockAdd(stockCompany.CompanyName, stockSum);
                //2、公司剩余股票减少
                //companiesController.CompanyStockAdd(stockCompany, -stockSum);

                //player.OverThisRound();
                Destroy(gameObject);
                //player.SyncStockInfo();

                //下面是同步玩家持有股票信息的方法 改到player里去实现 再在这里调用
                //List<string> lists = new List<string>(player.StocksToStrings());
                //if (lists.Count > 0)
                //{
                //    bool getIt = false;
                //    for (int i = 0; i < lists.Count; i++)
                //    {
                //        string[] strs = lists[i].Split(',');
                //        if (strs[0] == stockCompany.CompanyName)
                //        {
                //            getIt = true;
                //            lists[i] = strs[0] + "," + (ConvertFuntion.StringToInt(strs[1]) + stockSum);
                //        }
                //    }
                //    if (!getIt)
                //    {
                //        lists.Add(stockCompany.CompanyName + "," + stockSum);
                //    }
                //}
                //else
                //{
                //    lists.Add(stockCompany.CompanyName + "," + stockSum);
                //}
                //player.UpadateStocksStr(lists.ToArray());
            }
            else
            {
                message.text = "玩家金钱不足！";
                return;
            }


        }
        else
        {
            CommunicationTool.GetCommunicationTool().CmdSubmitResponse(requestId, stockSum.ToString());
            //player.OverThisRound();
            Destroy(gameObject);
        }
    }

    public void onExitButtonClick()
    {
        CommunicationTool.GetCommunicationTool().CmdSubmitResponse(requestId, "0");
        //player.OverThisRound();
        Destroy(gameObject);
    }

    //[Command(requiresAuthority = false)]
    //public void CmdGetPrice(string comName)
    //{
    //    Company company = companiesController.FindCompanyWithName(comName);
    //    stockPrice= companiesController.GetPrice(company);
    //}

    internal void InitForm(string requestId, GameStage gameStage, Player player)
    {
        this.requestId = requestId;
        this.gameStage = gameStage;
        this.player = player;
    }
}

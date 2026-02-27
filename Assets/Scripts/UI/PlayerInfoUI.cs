using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{

// 玩家名称：
//玩家当前现金：
//玩家总资产：
    public string playerName;
    public string playerCash;
    /// <summary>
    /// 总资产
    /// </summary>
    public string TotalAssets;

    public string stocks;
    //玩家信息菜单的展开状态
    bool openState=false;

    

    public TextMeshProUGUI baseInfo;
    public TextMeshProUGUI stockInfo;

    public Button button;

    public void OnPlayerNameChange(string val)
    {
        playerName = val;
        UpdateBaseInfo();
    }

    public void OnPlayerCashChange(int val)
    {
        playerCash = val.ToString();
        UpdateBaseInfo();
    }

    public void OnTotalAssetsChange(int val)
    {
        TotalAssets = val.ToString();
        UpdateBaseInfo();
    }

    public void UpdateBaseInfo()
    {
        baseInfo.text = "玩家名称：" + playerName+ "\n玩家当前现金："+ playerCash+ "\n玩家总资产："+ TotalAssets; 
    }

    public void OnPlayerStocksChange(string[] strings)
    {
        stocks = "持有股票:\n";
        foreach (string s in strings) 
        {
            if (!string.IsNullOrEmpty(s))
            {
                string[] theStr = s.Split(',');
                stocks += theStr[0] + " " + theStr[1] + "\n";
            }
        }
        stockInfo.text= stocks;
    }

    public void openPlayerInfo()
    {
        
        TextMeshProUGUI textMesh=button.transform.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null&& !openState) 
        {
            //gameObject.transform.Translate(new Vector3(-240,0,0));
            gameObject.transform.position = new Vector3(243,0,0);
            textMesh.text = "收起";
            openState = true;
        }
        else
        {
            //gameObject.transform.Translate(new Vector3(240, 0, 0));
            gameObject.transform.position = new Vector3(-243, 0, 0);
            textMesh.text = "展开";
            openState = false;
        }
    }
}

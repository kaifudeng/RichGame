using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Player player;
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
    bool openState = false;

    public GameObject infoPanel;

    public TextMeshProUGUI baseInfo;
    public TextMeshProUGUI stockInfo;
    /// <summary>
    /// 展开侧边信息栏的按钮
    /// </summary>
    public Button button;

    public MyButton c1;
    public MyButton c2; public MyButton c3;
    public MyButton c4;
    public MyButton c5; public MyButton c6;

    public List<MyButton> buttons;
    private void Awake()
    {
        buttons = new List<MyButton>
        {
            c1,
            c2,
            c3,
            c4,
            c5,
            c6
        };
        foreach (MyButton button in buttons)
        {
            BindButtonEvents(button);
            //cardButtons.Add(button);
        }
    }

    // 绑定按钮事件
    void BindButtonEvents(MyButton button)
    {
        // 移除所有已有的监听，避免重复绑定
        button.onButtonDown.RemoveAllListeners();
        button.onButtonEnter.RemoveAllListeners();
        button.onButtonExit.RemoveAllListeners();

        // 添加新的监听
        button.onButtonDown.AddListener(() => CardOnclick(button));
        //button.onButtonEnter.AddListener(() => OnCardButtonEnter(button));
        //button.onButtonExit.AddListener(() => OnCardButtonExit(button));

        // 或者使用带参数的方法
        // button.onButtonDown.AddListener(OnCardButtonClicked);
    }
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

    [Client]
    public void OnCardsChange(string[] cards)
    {
        int index = 0;
        foreach (MyButton button in buttons)
        {
            TextMeshProUGUI btntext = button.GetComponentInChildren<TextMeshProUGUI>();
            if (btntext != null && index < cards.Length)
            {
                GameObject btn = button.gameObject;
                btn.SetActive(true);
                btntext.text = cards[index];
                index++;
            }
            else
            {
                GameObject btn = button.gameObject;
                btn.SetActive(false);
            }
        }
    }
    [Client]
    public void OnStateChange(bool state)
    {
        //if (state == true)
        //{
        //    foreach (MyButton button in buttons)
        //    {
        //        button.enabled = true;
        //    }
        //}
        //else
        //{
        //    foreach (MyButton button in buttons)
        //    {
        //        button.enabled = false;
        //    }
        //}
    }

    public void UpdateBaseInfo()
    {
        baseInfo.text = "玩家名称：" + playerName + "\n玩家当前现金：" + playerCash + "\n玩家总资产：" + TotalAssets;
    }
    [Client]
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
        stockInfo.text = stocks;
    }

    public void openPlayerInfo()
    {

        TextMeshProUGUI textMesh = button.transform.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null && !openState)
        {
            infoPanel.transform.Translate(new Vector3(-250, 0, 0));
            textMesh.text = "收起";
            openState = true;
        }
        else
        {
            infoPanel.transform.Translate(new Vector3(250, 0, 0));
            textMesh.text = "展开";
            openState = false;
        }
    }

    public void CardOnclick(MyButton button)
    {
        if (player.handstate == true)
        {

                    GameObject btn = button.gameObject;
                    TextMeshProUGUI btntext = button.GetComponentInChildren<TextMeshProUGUI>();
                   // player.AttackOneCard(btntext.text);

                    player.ReturnTheCard(btntext.text);
                    //playerController.CmdSendPLayerMessage(playerController.PlayerName + "attack card " + btntext.text + "\n");
                    //playerController.GetPlayerTotalAssets();
                    //buttons.Remove(button);
                    btn.SetActive(false);
                   
                
            
            //RectTransform ui = GameInfoUI.GetPlayersInfoPanel();
            //GameInfoUI gameInfoUI=ui.GetComponentInChildren<GameInfoUI>();
            //if (gameInfoUI != null) 
            //{
            //    AddRealTimeInfo=gameInfoUI.AddRealTimeInfo;
            //    AddRealTimeInfo?.Invoke(Time.time.ToString()+" "+ );
            //}
        }
    }
}

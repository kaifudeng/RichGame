using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BaseForm : MonoBehaviour
{
    public TextMeshProUGUI Message;
    public Slider Slider;
    public TextMeshProUGUI title;
    string requestId;
    GameStage gameStage;
    Player thePlayer;
    public List<GameObject> thePrefabs;

    [SerializeField] private float waitTime;

    private float elapsedTime;

    GameObject Content;

    public GameObject ContentParent;

    public string reVal;

    public string MaxVal;

    void Start()
    {
        waitTime = 30f;
        StartCoroutine(TimerWithElapsedTime());
    }

    IEnumerator TimerWithElapsedTime()
    {
        elapsedTime = 0f;

        while (elapsedTime < waitTime)
        {
            // 获取已等待时长
            elapsedTime += Time.deltaTime;

            // 在这里使用elapsedTime
            //Debug.Log($"已等待: {elapsedTime:F1}秒 / {waitTime}秒");

            // 可以控制更新频率
            yield return null; // 每帧更新

            // 或者每0.1秒更新一次
            // yield return new WaitForSeconds(0.1f);
            // elapsedTime += 0.1f;
        }

        Debug.Log($"等待完成！总耗时: {elapsedTime:F2}秒");
        OnWaitComplete();
    }

    void OnWaitComplete()
    {
        // 执行你的代码
        Debug.Log("执行定时任务");
        OnFalseButtonClick();
    }

    private void Update()
    {
        float progress = Mathf.Clamp01(elapsedTime / waitTime);
        Slider.value = progress;
    }
   public void OnTrueButtonClick()
    {
        if (!string.IsNullOrEmpty(MaxVal))
        {
            if (ReValCheck())
            {
                CommunicationTool.GetCommunicationTool().CmdSubmitResponse(requestId, reVal);
                Destroy(gameObject);
            }
            else
            {
                elapsedTime = 0f;
            }
        }
        else
        {
            CommunicationTool.GetCommunicationTool().CmdSubmitResponse(requestId, reVal);
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 对返回值进行检查
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private bool ReValCheck()
    {
        if (ConvertFuntion.StringToInt(reVal) > ConvertFuntion.StringToInt(MaxVal))
        {
            Message.text = "!玩家金币不足";
            return false;
        }
        else {  return true; }
    }

    public void OnFalseButtonClick()
    {
        if (!string.IsNullOrEmpty(requestId))
        {
            CommunicationTool.GetCommunicationTool().CmdSubmitResponse(requestId, "");
        }
        Destroy(gameObject);
    }

    public void InitForm(string requestId,RequestType requestType, Player player,string val, string maxVal)
    {
        this.requestId = requestId;
        thePlayer = player;
        InitContent(requestType, val);
        MaxVal=maxVal;
    }

    public void InitContent(RequestType requestType, string val)
    {
        try
        {
            switch (requestType)
            {
                case RequestType.ChoiceACompany:
                    title.text = "选择要创建的公司：";
                    foreach (GameObject obj in thePrefabs)
                    {
                        if (obj.name == "CompaniesInfo")
                        {
                            Content = Instantiate(obj);
                            Content.transform.SetParent(ContentParent.transform, false);
                            CompaniesInfo companies = Content.GetComponent<CompaniesInfo>();
                            companies.InitContent(val);
                            break;
                        }
                    }
                    break;
                case RequestType.BuyStock:
                    string[] strs = val.Split(',');
                    if (strs.Length > 0)
                    {
                        title.text = "买入【" + strs[0] + "】的股票(最大为" + strs[1] + ")：";
                        foreach (GameObject obj in thePrefabs)
                        {
                            if (obj.name == "StockSlider")
                            {
                                Content = Instantiate(obj);
                                Content.transform.SetParent(ContentParent.transform, false);
                                StockSum stockSum = Content.GetComponent<StockSum>();

                                stockSum.InitContent(ConvertFuntion.StringToInt(strs[1]), thePlayer.cash);
                                break;
                            }
                        }
                    }
                    break;
                case RequestType.MergerCompanies:
                    title.text = "选择要保留的公司：";
                    foreach (GameObject obj in thePrefabs)
                    {
                        if (obj.name == "CompaniesInfo")
                        {
                            Content = Instantiate(obj);
                            Content.transform.SetParent(ContentParent.transform, false);
                            CompaniesInfo companies = Content.GetComponent<CompaniesInfo>();
                            companies.InitContent(val);
                            break;
                        }
                    }
                    break;
                case RequestType.CompaniesOut:
                    strs = val.Split('|');
                    if (strs.Length > 1)
                    {
                        string[] strings = strs[1].Split(',');
                        title.text = "选择【" + strings[1] + "】股票的处理方式：";
                        foreach (GameObject obj in thePrefabs)
                        {
                            if (obj.name == "StockSettlement")
                            {
                                Content = Instantiate(obj);
                                Content.transform.SetParent(ContentParent.transform, false);
                                StockSettlement stockSettlement = Content.GetComponent<StockSettlement>();
                                stockSettlement.InitContent(val);
                                break;
                            }
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("尝试注入窗口内容时出错："+ex);
        }
    }

    internal void InitContent(DataTable dataTable)
    {
        title.text = "玩家排名";
        theTrueButton.gameObject.SetActive(false);

        theFalseButtonText.text = "关闭";
        foreach (GameObject obj in thePrefabs)
        {
            if (obj.name == "PlayersDataTable")
            {
                Content = Instantiate(obj);
                Content.transform.SetParent(ContentParent.transform, false);

                PlayersDatatable tableContent = Content.GetComponent<PlayersDatatable>();
                tableContent.InitDataTable(dataTable);
                break;
            }
        }

    }

    public Button theTrueButton;
    public Button theFalseButton;
    public TextMeshProUGUI theFalseButtonText;
}

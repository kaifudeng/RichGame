using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
//using System.Diagnostics;

public class GameManager : NetworkBehaviour
{

    
    public GameObject gameGroundPrefab;
    //private CancellationTokenSource _cts;

    private GameObject CardsManagerObj;
    private CardsController theCardsController;
    private GameObject CompaniesManagerObj;
    private CompaniesController theCompaniesController;
    private GameObject PlayersManagerObj;
    private PlayersManager thePlayersManager;
    private GameProcess theLogsManager;
    private GameObject theLogsManagerObj;

    public GameObject baseFormObj;
    void Start()
    {
        // _cts = new CancellationTokenSource(30000);
        // StartGameFlow(_cts.Token).Forget();
        //StartGameFlow();
    }

    //void OnDestroy()
    //{
    //    _cts?.Cancel();
    //    _cts?.Dispose();
    //}

    public void StartGameFlow()
    {
        try
        {
            MainGameFlow();
        }
        catch (OperationCanceledException)
        {
            Debug.Log("游戏流程被取消");
        }
        catch (Exception e)
        {
            Debug.LogError($"游戏流程异常: {e}");
        }
    }

    private void MainGameFlow()
    {
        // 1. 游戏初始化
        InitializeGame();

        // 2. 显示开始界面，等待玩家点击开始
        //await WaitForGameStart(token);

        // 3. 主游戏循环

        GameLoop();

        //方便测试 只执行一次
        //return;

    }

    private async void GameLoop()
    {
        bool gameOver = false;
        while (!gameOver)
        {

            foreach (Player player in thePlayersManager.GetPlayers())
            {
                //开始一个玩家的回合
                try
                {
                    theLogsManager.AddNewLog("--现在是【" + player.PlayerName + "】的回合--");
                    GameGround gameGround = CreateGround(player);

                    await gameGround.ExecuteGround();

                    OverGround(gameGround);
                    theLogsManager.AddNewLog("--【" + player.PlayerName + "】的回合结束--");
                    //这个玩家的回合结束 进入下一个玩家的回合

                    if (theCardsController.lightingCards.Count() == 100)
                    {
                        theLogsManager.AddNewLog("-----------游戏结束-----------");
                        gameOver =true;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return;
                }
            }
            //gameOver=true;
        }
        //游戏结算
        GameSettlement();
    }
    /// <summary>
    /// 结束一个玩家回合
    /// </summary>
    /// <param name="gameGround"></param>
    [Server]
    private void OverGround(GameGround gameGround)
    {
        try
        {
            gameGround.DestroyObject();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        //GameSettlement();
    }

    private void GameSettlement()
    {
        //公司给大股东分红
        foreach(Company company in theCompaniesController.companies)
        {
            if (company.IsLife)
            {
                theCompaniesController.RankingPlayerWithCompany(company, thePlayersManager);

                if (company.PlayerList.Count > 0)
                {
                    int Dividend = theCompaniesController.GetCompanyDividendFirst(company);

                    thePlayersManager.PlayerCashAdd(Dividend, company.PlayerList[0]);
                    UnityEngine.Debug.Log("【" + company.PlayerList[0].PlayerName + "】获得公司【" + company.CompanyName + "】的分红：（" + Dividend + "）元\n");
                }
                if (company.PlayerList.Count > 1)
                {
                    int Dividend = theCompaniesController.GetCompanyDividendSecond(company);

                    thePlayersManager.PlayerCashAdd(Dividend, company.PlayerList[1]);

                    UnityEngine.Debug.Log("【" + company.PlayerList[0].PlayerName + "】获得公司【" + company.CompanyName + "】的分红：（" + Dividend + "）元\n");
                }
                if (company.PlayerList.Count > 2)
                {
                    int Dividend = theCompaniesController.GetCompanyDividendThird(company);

                    thePlayersManager.PlayerCashAdd(Dividend, company.PlayerList[2]);

                    UnityEngine.Debug.Log("【" + company.PlayerList[0].PlayerName + "】获得公司【" + company.CompanyName + "】的分红：（" + Dividend + "）元\n");
                }
            }
        }
        //对玩家进行排序
        List<Player> list = thePlayersManager.GetPlayers().OrderByDescending(Player => Player.TotalAssets).ToList();

        string[] xml = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            string str = (i + 1).ToString() + "," + list[i].PlayerName + "," + list[i].TotalAssets.ToString();
            xml[i] = str;
        }
        ShowRanking(xml);
    }
    [ClientRpc]
    void ShowRanking(string[] xml)
    {
        
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add("排名");
        dataTable.Columns.Add("玩家名称");
        dataTable.Columns.Add("总资产");
        for(int i=0;i < xml.Length; i++)
        {
            DataRow row=dataTable.NewRow();
            string[] strings=xml[i].Split(',');
            row["排名"]=strings[0];
            row["玩家名称"] = strings[1];
            row["总资产"] = strings[2];
            dataTable.Rows.Add(row);
        }
        //GameObject canvas = GameObject.FindGameObjectWithTag("canvas");
        
        GameObject baseForm = Instantiate(baseFormObj);
        baseForm.transform.SetAsFirstSibling();

        BaseForm form = baseForm.GetComponentInChildren<BaseForm>();
        
        form.InitContent( dataTable);

    }
    [Server]
    private void InitializeGame()
    {
        //初始化牌堆 分发初始手牌 排列玩家顺位 等操作
        try
        {
            //1、获取各方面的管理器
            InitManagers();
            


            //2、生成牌堆和初始化棋盘
            theLogsManager.AddNewLog("正在生成牌堆，初始化棋盘……");
            theCardsController.InitCardsSystem();

            //3、加载公司信息
            theLogsManager.AddNewLog("正在加载公司信息……");
            theCompaniesController.InitCompaniesSystem();

            //4、初始化玩家相关数据 玩家顺位 初始金币 初始手牌等
            theLogsManager.AddNewLog("正在加载玩家信息……");
            thePlayersManager.InitPlayers();

            thePlayersManager.InitPlayerHandCard(theCardsController, 6);

            thePlayersManager.RankingPlayers(theCardsController);

            thePlayersManager.InitPlayersInfo();

            thePlayersManager.SyncTotalAssets(theCompaniesController);

        }
        catch (Exception e)
        {
            Debug.LogError($"游戏初始化异常: {e}");
        }
    }

    [Server]
    private void InitManagers()
    {
        CardsManagerObj = GameObject.Find("cardsController(Clone)");
        CompaniesManagerObj = GameObject.Find("CompaniesController(Clone)");
        PlayersManagerObj = GameObject.Find("PlayersManager(Clone)");
        theLogsManagerObj = GameObject.Find("gameProcessManager(Clone)");
        if (CardsManagerObj != null)
        {
            theCardsController = CardsManagerObj.GetComponent<CardsController>();
            if (theCardsController == null)
            {
                throw new ArgumentException("找不到卡片管理器组件");
            }
        }
        else
        {
            throw new ArgumentException("找不到卡片管理器");
        }

        if (CompaniesManagerObj != null)
        {
            theCompaniesController = CompaniesManagerObj.GetComponent<CompaniesController>();
            if (theCompaniesController == null)
            {
                throw new ArgumentException("找不到公司管理器组件");
            }
        }
        else
        {
            throw new ArgumentException("找不到公司管理器");
        }

        if (PlayersManagerObj != null)
        {
            thePlayersManager = PlayersManagerObj.GetComponent<PlayersManager>();
            if (thePlayersManager == null)
            {
                throw new ArgumentException("找不到玩家管理器组件");
            }
        }
        else
        {
            throw new ArgumentException("找不到玩家管理器");
        }
        if (theLogsManagerObj != null)
        {
            theLogsManager = theLogsManagerObj.GetComponent<GameProcess>();
            if (theLogsManagerObj == null)
            {
                throw new ArgumentException("找不到日志管理器组件");
            }
        }
        else
        {
            throw new ArgumentException("找不到日志管理器");
        }
    }

   

    /// <summary>
    /// 新建一个玩家回合
    /// </summary>
    /// <param name="thePlayer"></param>
    /// <returns></returns>
    [Server]
    private GameGround CreateGround(Player thePlayer)
    {
        GameObject groundObj = Instantiate(gameGroundPrefab);
        NetworkServer.Spawn(groundObj);
        GameGround gameGround = groundObj.GetComponent<GameGround>();
        gameGround.InitGround(thePlayer, theCardsController, theCompaniesController, thePlayersManager, theLogsManager);
        return gameGround;
    }
}

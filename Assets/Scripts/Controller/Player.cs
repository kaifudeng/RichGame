using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telepathy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    /// <summary>
    /// 出牌时间进度条
    /// </summary>
    public GameObject RoundSlider;
    CardPlayTimer cardPlayTimer;

    public GameObject canvas;
    public GameObject BaseFormPrefab;
    string PlayACardRequestId;
    /// <summary>
    /// 当前所处的游戏环节
    /// </summary>
    GameStage stage;
    /// <summary>
    /// 游戏流程管理器
    /// </summary>
    //public GameObject GameProcessObject;
    //GameProcess gameProcess;
    ///// <summary>
    ///// 卡片管理器
    ///// </summary>
    //public GameObject CardsControllerObject;
    //CardsController cardsController;
    ///// <summary>
    ///// 公司管理器
    ///// </summary>
    //public GameObject CompaniesControllerObject;
    //CompaniesController companiesController;
    /// <summary>
    /// 公司选择界面
    /// </summary>
    public GameObject CreateCompanyWindowPrefab;
    CreateCompWindow createCompWindow;
    ///// <summary>
    ///// 破产公司股票处理界面
    ///// </summary>
    //public GameObject CompanySettlementFormPrefab;
    //CompanySettlementForm companySettlement;

    //public GameObject StockBuyFormPrefab;
    //stockSumUI stockSumUI;

    /// <summary>
    /// 玩家的表现层 包括个人信息的展示和手牌区
    /// </summary>
    public GameObject playerUIPrefab;
    PlayerUI playerUI;

    [SyncVar(hook = nameof(PlayerNameChange))]
    public string PlayerName;
    [SyncVar(hook = nameof(PlayerCardsChange))]
    public string[] cards;

    [SyncVar(hook = nameof(Cashchange))]
    /// <summary>
    /// 现金
    /// </summary>
    public int cash=6000;
    /// <summary>
    /// 总资产
    /// </summary>
    [SyncVar(hook = nameof(TotalAssetschange))]
    public int TotalAssets;

    /// <summary>
    /// 持有股票 格式（公司，数量） 这个数据保留两种格式，方便同步的同时也方便处理业务逻辑
    /// </summary>
    public List<Stock> stocks;
    [SyncVar(hook = nameof(stocksChange))]
    public string[] stocks_str;

    [SyncVar(hook = nameof(PlayerStateChange))]
    public bool handstate;//状态 是否可以操作手牌

   // public static List<Player> Players = new List<Player>();

    public int rank;
    

    #region 同步时使用到的事件

    public event Action<string[]> OnPlayerCardsChange;
    public event Action<string[]> OnPlayerstockschange;
    public event Action<string> OnPlayerNameChange;
    public event Action<int> OnPlayerTotalAssetschange;
    public event Action<int> OnPlayerCashchange;
    public event Action<bool> OnPlayerStateChange;
    private void PlayerStateChange(bool oldState, bool newState)
    {
        OnPlayerStateChange?.Invoke(newState);
    }
    private void stocksChange(string[] oldval, string[] newval)
    {
        OnPlayerstockschange?.Invoke(newval);
    }
    private void Cashchange(int oldval, int newval)
    {
        OnPlayerCashchange?.Invoke(newval);
    }
    private void TotalAssetschange(int oldval, int newval)
    {
        OnPlayerTotalAssetschange?.Invoke(newval);
    }
    public void PlayerNameChange(string oldName, string newName)
    {
        OnPlayerNameChange?.Invoke(newName);
    }
    public void PlayerCardsChange(string[] oldCards, string[] newCards)
    {
        OnPlayerCardsChange?.Invoke(newCards);
    }
    #endregion
    public override void OnStartServer()
    {
        try
        {
            //GameProcessObject = GameObject.Find("gameProcessManager(Clone)");
            //CardsControllerObject = GameObject.Find("cardsController(Clone)");
            //CompaniesControllerObject = GameObject.Find("CompaniesController(Clone)");

            //cardsController = CardsControllerObject.GetComponent<CardsController>();
            ////gameProcess = GameProcessObject.GetComponent<GameProcess>();
            //companiesController = CompaniesControllerObject.GetComponent<CompaniesController>();


            //stocks = new List<Stock>();
            //getCardFromDeck(6);
            //gameProcess.StartFirstPlayerRound();
        }
        catch (Exception e) 
        {
            Debug.Log($"测试{e}");
        }
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        try
        {
            ShowPlayerInfo();

            GameObject CardPlayTimerObj = Instantiate(RoundSlider);

            cardPlayTimer = CardPlayTimerObj.GetComponent<CardPlayTimer>();

        }
        catch(Exception e)
        {
            Debug.Log($"测试{e}");
        }

    }
    [Server]
    /// <summary>
    /// 手牌增加
    /// </summary>
    /// <param name="card"></param>
    public void AddCard(string card)
    {
        List<string> cardList = cards.ToList();
        cardList.Add(card);
        cards = cardList.ToArray();
    }
    [Server]
    /// <summary>
    /// 从牌堆获取一张牌 添加到手牌
    /// </summary>
    public void getOneCardFromdeck(CardsController cardsController)
    {
        List<string> cardList = cards.ToList();
        string newcard = cardsController.GetCard();
        if (newcard != null)
        {
            AddCard(newcard);
        }
    }

    [Client]
    public void ShowPlayerInfo()
    {
        //canvas = GameObject.FindGameObjectWithTag("canvas");
        GameObject PlayerCanvas= Instantiate(canvas);
        //GameObject playerUIObject = Instantiate(playerUIPrefab, PlayerCanvas.transform);

        //playerUI = playerUIObject.GetComponent<PlayerUI>();
        playerUI= PlayerCanvas.GetComponentInChildren<PlayerUI>();
        playerUI.player = this;

        OnPlayerNameChange = playerUI.OnPlayerNameChange;
        OnPlayerNameChange?.Invoke(PlayerName);

        OnPlayerCashchange = playerUI.OnPlayerCashChange;
        OnPlayerCashchange?.Invoke(cash);

        OnPlayerTotalAssetschange = playerUI.OnTotalAssetsChange;
        OnPlayerTotalAssetschange?.Invoke(TotalAssets);

        OnPlayerstockschange = playerUI.OnPlayerStocksChange;
        OnPlayerstockschange?.Invoke(stocks_str);

        OnPlayerCardsChange = playerUI.OnCardsChange;
        OnPlayerCardsChange?.Invoke(cards);

        //state=true;
        OnPlayerStateChange = playerUI.OnStateChange;
        OnPlayerStateChange?.Invoke(handstate);

    }
    
    /// <summary>
    /// 玩家获取原始股票
    /// </summary>
    [Server]
    public void PlayerGetFirstStock(Company company)
    {
        bool isHave = false;
        Stock stock = new Stock(company, 1);
        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i].Company.CompanyName == stock.Company.CompanyName)
            {
                isHave = true;
                stocks[i].Sum += 1;
            }
        }
        if (!isHave)
        {
            stocks.Add(stock);
        }
        SyncStockInfo();
        //SyncTotalAssets();
    }
    /// <summary>
    /// 同步股票信息到stocks_str 通过string[]同步到玩家UI面板
    /// </summary>
    [Server]
    public void SyncStockInfo()
    {
        List<string> strings = new List<string>();

        foreach (var stock in stocks)
        {
            if (stock != null)
            {
                string str = stock.Company.CompanyName + "," + stock.Sum;
                strings.Add(str);
            }
        }
        stocks_str = strings.ToArray();
    }
    
    /// <summary>
    /// 开始出牌
    /// </summary>
    [TargetRpc]
    public void TargetRpcStartPlayACard(string theRequestId)
    {
        cardPlayTimer.StartPlayTimer();
        handstate =true;
        //stage=gameStage;
        PlayACardRequestId = theRequestId;
    }
    
    /// <summary>
    /// 结束出牌
    /// </summary>
    [TargetRpc]
    public void OverPlayACard()
    {
        cardPlayTimer.StopPlayTimer();
        handstate = false;
    }
    /// <summary>
    /// 将ui层选择的卡牌返回到服务端
    /// </summary>
    /// <param name="card"></param>
    [Client]
    public void ReturnTheCard(string card)
    {
        CommunicationTool.GetCommunicationTool().CmdSubmitResponse(PlayACardRequestId, card);
    }
    /// <summary>
    /// 手牌减少
    /// </summary>
    /// <param name="card">指定的卡牌</param>
    public void ReduceACard(string card)
    {
        foreach (string cardStr in cards)
        {
            if (cardStr == card)
            {
                cards = cards.Where(val => val != cardStr).ToArray();
            }
        }
    }
    /// <summary>
    /// 根据公司获取玩家持有股票数量
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    public int GetStockSum(Company company)
    {
        int count = 0;
        foreach(string stock in stocks_str)
        {
            string[] strings=stock.Split(',');
            if (strings[0] == company.CompanyName)
            {
                count= count+ConvertFuntion.StringToInt(strings[1]);
            }
        }
        return count;
    }



    #region 与服务端通信
    /// <summary>
    /// 收到服务端请求
    /// </summary>
    [TargetRpc]
    public void TargetServerRequestForm(string theRequestId,RequestType requestType,string val,string MaxVal)
    {
       
       InitForm(theRequestId, requestType,val, MaxVal);
    }
    /// <summary>
    /// 加载通用窗体
    /// </summary>
    /// <param name="theRequestId"></param>
    /// <param name="gameStage"></param>
    /// <param name="val"></param>
    private void InitForm(string theRequestId,RequestType requestType, string val, string maxVal)
    {

        GameObject baseForm = Instantiate(BaseFormPrefab);
        //baseForm.transform.SetAsFirstSibling();
        //NetworkServer.Spawn(theWin);

        BaseForm form = baseForm.GetComponentInChildren<BaseForm>();
        form.InitForm(theRequestId, requestType, this,val, maxVal);

    }
    #endregion

    #region 已弃用
    ///// <summary>
    ///// 玩家出售股票
    ///// </summary>
    ///// <param name="company"></param>
    ///// <param name="sum"></param>
    //internal void SellStock(Company company, int sum, CompaniesController companiesController)
    //{
    //    if (sum > 0)
    //    {
    //        for (int i = 0; i < stocks.Count; i++)
    //        {
    //            if (stocks[i].Company.CompanyName == company.CompanyName)
    //            {
    //                stocks[i].Sum -= sum;
    //                cash += sum * companiesController.GetPrice(company);
    //                if (stocks[i].Sum == 0)
    //                {
    //                    stocks.Remove(stocks[i]);
    //                }

    //            }
    //        }
    //    }
    //    SyncStockInfo();
    //}
    ///// <summary>
    ///// 初始化玩家对象 给玩家分配初始金币等操作在这里执行
    ///// </summary>
    ///// <param name="roomPlayer"></param>
    //public void InitPlayer(GameObject roomPlayer)
    //{
    //    //cash = 6000;
    //    rank = roomPlayer.GetComponent<RoomPlayer>().index;
    //    PlayerName = roomPlayer.GetComponent<RoomPlayer>().playerName;
    //    Players.Add(this);
    //}
    ///// <summary>
    ///// 增加或者减少股票
    ///// </summary>
    ///// <param name="company"></param>
    ///// <param name="sum"></param>
    ///// <returns></returns>
    //public void PlayerStockAdd(string companyName, int sum, CompaniesController companiesController)
    //{
    //    try
    //    {
    //        bool isHave = false;
    //        Company company = companiesController.FindCompanyWithName(companyName);
    //        Stock stock = new Stock(company, sum);
    //        for (int i = 0; i < stocks.Count; i++)
    //        {
    //            if (stocks[i].Company.CompanyName == stock.Company.CompanyName)
    //            {
    //                isHave = true;
    //                stocks[i].Sum += sum;
    //            }
    //        }
    //        if (!isHave)
    //        {
    //            stocks.Add(stock);
    //        }

    //        int stockPrice = 0;
    //        foreach (Company theComp in companiesController.companies)
    //        {
    //            if (company == theComp)
    //            {

    //                stockPrice = companiesController.GetPrice(theComp);
    //            }
    //        }
    //        PlayerCashAdd(stockPrice * -sum);
    //        SyncStockInfo();
    //        //SyncTotalAssets();
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError("玩家增加股票时出错" + ex);
    //    }
    //}
    /// <summary>
    /// 给每个新加入的玩家发放手牌
    /// </summary>
    /// <param name = "sum" ></ param >
    //[Server]
    //public void getCardFromDeck(int sum)
    //{
    //    foreach (var controller in Players)
    //    {
    //        if (controller.cards.Length > 0)
    //        {
    //            continue;
    //        }
    //        if (sum <= cardsController.deck.Length)
    //        {
    //            controller.cards = new string[sum];
    //            for (int i = 0; i < sum; i++)
    //            {
    //                int index = UnityEngine.Random.Range(0, deck.Length - 1);
    //                controller.cards[i] = deck[index];
    //                deck = deck.Where(val => val != deck[index]).ToArray();
    //            }

    //            controller.cards = cardsController.GetCards(sum);
    //        }
    //        else
    //        {
    //            Debug.Log("剩余牌数不足");
    //        }
    //    }

    //}

    //[Client]
    ///// <summary>
    ///// 开始一个回合
    ///// </summary>
    //public void StartRound()
    //{
    //    //测试用代码 正式版本需要注释
    //    //cards = new string[] { "A1", "A2", "A3", "A4", "A5", "A6" };
    //    handstate = true;
    //    //gameProcess.nowPlayer = this;
    //    CmdSendPLayerMessage(PlayerName,"开始一个回合");
    //}


    ///// <summary>
    ///// 玩家发送消息
    ///// </summary>
    ///// <param name="message"></param>
    //[Command]
    //public void CmdSendPLayerMessage(string playerName, string message)
    //{
    //    gameProcess.AddNewInfo(playerName + ":" + message);
    //}

    ///// <summary>
    ///// 从手牌中打出一张牌
    ///// </summary>
    ///// <param name="theCard"></param>
    //[Command]
    //public void AttackOneCard(string theCard)
    //{
    //    gameProcess.AddNewInfo(PlayerName + "打出手牌" + theCard);

    //    //CmdSendPLayerMessage("打出手牌: "+theCard);//不能在command中再使用command方法 会报错

    //    foreach (string cardStr in cards)
    //    {
    //        if (cardStr == theCard)
    //        {
    //            cards = cards.Where(val => val != cardStr).ToArray();

    //            //TargetRpcGetCompaniesSelectForm();

    //            getOneCardFromdeck();
    //            //handstate = false;

    //            //if (!cardsController.AddNewCardToLightCards(theCard))
    //            //{
    //            //    gameProcess.AddNewInfo(PlayerName + ":结束回合");
    //            //    gameProcess.StartNextRound(this);
    //            //}
    //            //break;
    //        }
    //    }
    //}

    //[TargetRpc]
    //public void TargetRpcGetCompaniesSelectForm(string requestId, GameStage gameStage)
    //{

    //    GameObject theWin = Instantiate(CreateCompanyWindowPrefab);
    //    createCompWindow = theWin.GetComponentInChildren<CreateCompWindow>();
    //    createCompWindow.InitForm(requestId, gameStage, this, 0, "");

    //}

    //[TargetRpc]
    //public void TargetRpcGetCompaniesSelectForm(string requestId, GameStage gameStage, string comps)
    //{

    //    GameObject theWin = Instantiate(CreateCompanyWindowPrefab);
    //    createCompWindow = theWin.GetComponentInChildren<CreateCompWindow>();
    //    createCompWindow.InitForm(requestId, gameStage, this, 1, comps);

    //}

    //[TargetRpc]
    //public void TargetRpcCompanySettlementForm(string requestId, GameStage gameStage, string comps)
    //{

    //    GameObject theWin = Instantiate(CompanySettlementFormPrefab);
    //    companySettlement = theWin.GetComponentInChildren<CompanySettlementForm>();
    //    companySettlement.InitForm(requestId, gameStage, this, comps);

    //}
    //[TargetRpc]
    //public void TargetRpcGetStockSelectForm(string company)
    //{
    //    GameObject theWin = Instantiate(StockSetForm);
    //    StockSetWindow = theWin.GetComponentInChildren<StockSetForm>();
    //    StockSetWindow.thePlayer = this;

    //    StockSetWindow.comp=companiesController.FindCompany(company);
    //}

    //[TargetRpc]
    //public void TargetRpcGetStockBuyForm(string requestId,GameStage gameStage,string compName)
    //{
    //    GameObject theWin = Instantiate(StockBuyFormPrefab);
    //    //NetworkServer.Spawn(theWin);

    //    stockSumUI = theWin.GetComponentInChildren<stockSumUI>();
    //    stockSumUI.InitForm(requestId,gameStage,this);

    //    stockSumUI.InitCompanyInfo(compName);
    //}

    /// <summary>
    /// 创建公司的命令
    /// </summary>
    /// <param name="theCompany"></param>
    //[Command]
    //public void CmdCreateCompany(string theCompany)
    //{
    //    companiesController.CreateNewCompany(theCompany);
    //    PlayerGetFirstStock(companiesController.FindCompany(theCompany));
    //}

    //[Client]
    //public void SyncTotalAssets()
    //{
    //    int total = cash;
    //    int stockPrice = 0;
    //    foreach (var stock in stocks)
    //    {
    //        if (stock != null)
    //        {
    //            foreach (Company company in companiesController.companies)
    //            {
    //                if (company.ToString() == stock.Company.ToString())
    //                {
    //                    stockPrice += companiesController.GetPrice(company) * stock.Sum;
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    TotalAssets = total + stockPrice;
    //}


    #endregion
}

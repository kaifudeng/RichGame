//using Mirror;
//using Mirror.BouncyCastle.Utilities;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Unity.VisualScripting;
//using UnityEngine;

//public class PlayerController : NetworkBehaviour
//{
//    [SyncVar(hook =nameof(Cashchange))]
//    /// <summary>
//    /// 现金
//    /// </summary>
//    public int cash;
//    [SyncVar(hook = nameof(TotalAssetschange))]
//    /// <summary>
//    /// 总资产
//    /// </summary>
//    public int TotalAssets;
//    /// <summary>
//    /// 持有股票 格式（公司，数量）
//    /// </summary>
//    public List<(CompanyController, int)> stocks;
//    [SyncVar(hook =nameof(stocksChange))]
//    public string[] stocks_str;

//    public GameObject playerInfoPanelPrefab;

//    private void stocksChange(string[] oldval, string[] newval)
//    {
//        stocks = new List<(CompanyController, int)>();
        
//            stocks.Clear();
//            foreach (var kvp in newval)
//            {
//                string[] strings = kvp.Split(',');
//                CompanyController comp = CompaniesManager.GetCompaniesManager().FindCompanyController(strings[0]);
//                stocks.Add((comp, ConvertFuntion.StringToInt(strings[1])));
//            }
        
//        OnPlayerstockschange?.Invoke(newval);
//    }
//    private void Cashchange(int oldval, int newval)
//    {
//        OnPlayerCashchange?.Invoke(newval);
//    }

//    private void TotalAssetschange(int oldval, int newval)
//    {
//        OnPlayerTotalAssetschange?.Invoke(newval);
//    }

//    [SyncVar(hook = nameof(PlayerStateChange))]
//    public bool handstate;//状态 是否可以操作

//    public int rank;//玩家顺位

//    public static GameProcessManager gameProcessManager;

//    private void PlayerStateChange(bool oldState,bool newState)
//    {
//        OnPlayerStateChange?.Invoke(newState);
//    }

//    [SyncVar(hook = nameof(PlayerCardsChange))]
//    public string[] cards;

//    public event Action<string[]> OnPlayerCardsChange;
//    public event Action<string[]> OnPlayerstockschange;
//    public event Action<string> OnPlayerNameChange;
//    public event Action<int> OnPlayerScoreChange;
//    public event Action<int> OnPlayerTotalAssetschange;
//    public event Action<int> OnPlayerCashchange;
//    public event Action<bool> OnPlayerStateChange;
//   // public event Action<bool> OnCompanyWindowState;

//    [SyncVar(hook =nameof(PlayerNameChange))]
//    public string PlayerName;
//    [SyncVar(hook = nameof(PlayerScoreChange))]
//    public int PlayerScore;

//    //public GameObject PlayerUIPrefab;
//    // GameObject playerui;
//    public GameObject cardPrefab;
//     GameObject cardui;
//    public GameObject windowPrefab;
//    public GameObject stockSumPrefab;
//    GameObject windowUI;
//    GameObject stockSumUIobj;
//    //PlayerUIController PlayerUIController;
//    CardUI cardUI;
//    WindowUI WindowUI;
//     GameObject ShaderChangeObj;

//    public GameObject companiesChosePrefab;


//     GameObject CompanyWindowObj;
//    ///// <summary>
//    ///// 是否要显示公司信息相关窗口
//    ///// </summary>
//    //[SyncVar(hook = nameof(CompanyWindowStateChange))]
//    //public bool CompanyWindowState= false;

//    //private void CompanyWindowStateChange(bool oldState, bool newState)
//    //{
//    //    OnCompanyWindowState?.Invoke(newState);
//    //}

//    public static List<PlayerController> PlayerControllers = new List<PlayerController>();

//    public static string[] deck= InitDeck();
//    //    = {"A1","A2","A3","A4","A5", "A6", "A7", "A8", "A9", "A10",
//    //"B1","B2","B3","B4","B5", "B6", "B7", "B8", "B9", "B10" ,
//    //"C1","C2","C3","C4","C5", "C6", "C7", "C8", "C9", "C10" ,};

//    //public static List<Company> companies= InitCompanyList();

//    /// <summary>
//    /// 初始化牌堆
//    /// </summary>
//     static string[] InitDeck()
//    {
//        deck = null;
//        deck = new string[100];
//        int len = 0;
//        for (int i = 1; i < 11; i++)
//        {
//            for (int j = 1; j < 11; j++)
//            {
//                string val = (char)(i + 64) + j.ToString();
//                deck[len] = val;
//                len++;
//            }
//        }
//        return deck;
//    }

//    //static List<Company> InitCompanyList()
//    //{
//    //    companies = new List<Company>();
//    //    //Company companyA = new("A公司", 4,'A');
//    //    //companies.Add(companyA);
//    //    //Company companyB = new("B公司", 5,'A');
//    //    //companies.Add(companyB);
//    //    //Company companyC = new("C公司", 6, 'B');
//    //    //companies.Add(companyC);
//    //    //Company companyD = new("D公司", 7, 'B');
//    //    //companies.Add(companyD);
//    //    //Company companyE = new("E公司", 8, 'C');
//    //    //companies.Add(companyE);
//    //    //Company companyF = new("F公司", 9, 'C');
//    //    //companies.Add(companyF);
//    //    //Company companyG = new("G公司", 3, 'C');
//    //    //companies.Add(companyG);
//    //    return companies;
//    //}
//    public  string[] GetCards(int sum)
//    {
//        string[] backCards = new string[sum];
//        for(int i = 0; i < sum; i++)
//        {
//                backCards[i] = GetCard();
//        }
//        return backCards;
//    }

//    /// <summary>
//    /// 从牌堆获取一张卡  牌堆减少
//    /// </summary>
//    /// <returns></returns>
//    public string GetCard()
//    {
//        string backCard= deck[UnityEngine.Random.Range(0, deck.Length)];
//        deck = deck.Where(val => val != backCard).ToArray();
//        return backCard;
//    }
//    /// <summary>
//    /// 手牌增加
//    /// </summary>
//    /// <param name="card"></param>
//    public void AddCard(string card)
//    {
//        List<string> cardList = cards.ToList();
//        cardList.Add(card);
//        cards = cardList.ToArray();
//    }
//    //[ServerCallback]
//    //public static void ResetPlayerName()
//    //{
//    //    int i = 0;
//    //    foreach (var controller in PlayerControllers) 
//    //    {
//    //        controller.PlayerName = "Player" + UnityEngine.Random.Range(100, 300);
//    //        controller.rank = i;
//    //        i++;
//    //    }
        
//    //}

//    [Server]
//    public static void StartFirstPlayerRound()
//    {
//        if (PlayerControllers.Count > 0)
//            PlayerControllers[0].StartRound();
//    }
//    /// <summary>
//    /// 给每个新加入的玩家发放手牌
//    /// </summary>
//    /// <param name="sum"></param>
//     [ServerCallback]
//    public void getCardFromDeck(int sum)
//    {
//        foreach (var controller in PlayerControllers)
//        {
//            if(controller.cards.Length > 0)
//            {
//                continue;
//            }
//            if (sum <= deck.Length)
//            {
//                //controller.cards=new string[sum];
//                //for (int i = 0; i < sum; i++)
//                //{
//                //    int index = UnityEngine.Random.Range(0, deck.Length-1);
//                //    controller.cards[i] = deck[index];
//                //    deck = deck.Where(val => val != deck[index]).ToArray();
//                //}

//                controller.cards=GetCards(sum);
//            }
//            else
//            {
//                Debug.Log("剩余牌数不足");
//            }
//        }
        
//    }

//    public void PlayerNameChange(string oldName,string newName)
//    {
//        OnPlayerNameChange?.Invoke(newName);
//    }

//    public void PlayerCardsChange(string[] oldCards, string[] newCards)
//    {
//        OnPlayerCardsChange?.Invoke(newCards);
//    }

//    public void PlayerScoreChange(int oldVal,int newVal)
//    {
//        OnPlayerScoreChange?.Invoke(newVal);
//    }
//    public override void OnStartClient()
//    {
//        base.OnStartClient();

//        //playerui= Instantiate(PlayerUIPrefab, CanvasUI.GetRect());
//        //PlayerUIController = playerui.GetComponent<PlayerUIController>();

//        //OnPlayerNameChange = PlayerUIController.onPlayerNameChange;
//        //OnPlayerScoreChange = PlayerUIController.onPlayerScoreChange;
//        //OnPlayerCardsChange = PlayerUIController.onPlayerCardsChange;

//        //OnPlayerNameChange?.Invoke(PlayerName);
//        //OnPlayerScoreChange?.Invoke(PlayerScore);
//        //OnPlayerCardsChange?.Invoke(cards);

        
//    }

//    public override void OnStartServer()
//    {
//        base.OnStartServer();

//        //PlayerControllers.Add(this);

//        //InvokeRepeating(nameof(getPlayerScore), 1, 1);

//        getCardFromDeck(6);
//        StartFirstPlayerRound();
//    }

//    public override void OnStopServer()
//    {
//        base.OnStopServer();
//        CancelInvoke();
//        PlayerControllers.Remove(this);
//    }
//    public override void OnStopClient()
//    {
//        base.OnStopClient();

//        CancelInvoke();

//        PlayerControllers.Remove(this);

//        OnPlayerNameChange = null;
//        OnPlayerScoreChange = null;
//        //Destroy(playerui);

//    }
//    public override void OnStopLocalPlayer()
//    {
//        base.OnStopLocalPlayer();

        
//    }
//    public override void OnStartLocalPlayer()
//    {
//        base.OnStartLocalPlayer();
        
//        ShowCard();
//        ShowPlayerInfo();
//    }
//    [Client]
//    public void ShowPlayerInfo()
//    {
//        GameObject playerInfo = Instantiate(playerInfoPanelPrefab, CanvasUI.GetRect());
//        PlayerInfoUI playerInfoUI=playerInfo.GetComponent<PlayerInfoUI>();

//        OnPlayerNameChange=playerInfoUI.OnPlayerNameChange;
//        OnPlayerNameChange?.Invoke(PlayerName);

//        OnPlayerCashchange=playerInfoUI.OnPlayerCashChange;
//        OnPlayerCashchange?.Invoke(cash);

//        OnPlayerTotalAssetschange = playerInfoUI.OnTotalAssetsChange;
//        OnPlayerTotalAssetschange?.Invoke(TotalAssets);

//        OnPlayerstockschange=playerInfoUI.OnPlayerStocksChange;
//        OnPlayerstockschange?.Invoke(stocks_str);

//    }

//    /// <summary>
//    /// 将玩家手牌展示到ui界面
//    /// </summary>
//    [Client]
//    public void ShowCard()
//    {
//        cardui = Instantiate(cardPrefab, HandUI.GetRect());
//        cardUI = cardui.GetComponent<CardUI>();
//        cardUI.playerController = this;

       

//        OnPlayerCardsChange = cardUI.OnCardsChange;
//        OnPlayerCardsChange?.Invoke(cards);

//        //state=true;
//        OnPlayerStateChange= cardUI.OnStateChange;
//        OnPlayerStateChange?.Invoke(handstate);

//        //OnCompanyWindowState = WindowUI.ShowWindow;
//        //OnCompanyWindowState?.Invoke(CompanyWindowState);
//    }

//    /// <summary>
//    /// 玩家发送消息
//    /// </summary>
//    /// <param name="message"></param>
//    [Command]
//    public void CmdSendPLayerMessage(string message)
//    {
//        //RectTransform ui = GameInfoUI.GetPlayersInfoPanel();
//        //GameInfoUI gameInfoUI = ui.GetComponentInChildren<GameInfoUI>();
//        //if (gameInfoUI != null)
//        //{
//        //    gameInfoUI.realTimeInfo += message;
//        //}
//    }
    
//    /// <summary>
//    /// 从牌堆获取一张牌 添加到手牌
//    /// </summary>
//    public void getOneCardFromdeck()
//    {
//        List<string> cardList=cards.ToList();
//        string newcard = GetCard();
//        AddCard(newcard);
//    }

//    [TargetRpc]
//    public void TargetRpcSelectCompanyRound()
//    {
//        CompaniesManager manager= CompaniesManager.GetCompaniesManager();
//        List<CompanyController> cs = manager.getWaitAliveCompany();
//        if (cs.Count>0)
//        {
//            windowUI = Instantiate(windowPrefab, CanvasUI.GetRect());
//            WindowUI WindowUIComp = windowUI.GetComponent<WindowUI>();
//            WindowUIComp.playerController = this;
//            windowUI.SetActive(true);
//        }
//    }

//    [Command]
//    public void OpenBuyStockWindow(string companyName)
//    {
//        TargetRpcSelectStockRound(companyName);
//    }

//    /// <summary>
//    /// 弹出选择股票数量的窗体
//    /// </summary>
//    /// <param name="companyName"></param>
//    [TargetRpc]
//    public void TargetRpcSelectStockRound(string companyName)
//    {
//        Debug.Log("测试" + companyName);
//        stockSumUIobj = Instantiate(stockSumPrefab, CanvasUI.GetRect());
//        stockSumUI stockSumUI = stockSumUIobj.GetComponent<stockSumUI>();
//        stockSumUI.playerController = this;
//        stockSumUI.theCompanyName=companyName;
//        stockSumUI.InitCompanyInfo();
//    }
//    /// <summary>
//    /// 公司并购的环节
//    /// </summary>
//    /// <param name="companyNameA"></param>
//    /// <param name="companyNameB"></param>
//    [TargetRpc]
//    public void TargetRpcCompanyBattle(string companyNameA,string companyNameB,string card)
//    {
//        CompaniesManager companiesManager= CompaniesManager.GetCompaniesManager();
//        CompanyController companyA = companiesManager.FindCompanyController(companyNameA);
//        CompanyController companyB = companiesManager.FindCompanyController(companyNameB);

//        int reVal = companiesManager.CompaniesBattle(companyA, companyB);
//        if (reVal == 1) 
//        {
//            CmdSendPLayerMessage(companyNameA + " 吞并 " + companyNameB+"\n");
//            companiesManager.CompanyGetCard(companyNameA,card);
//            TargetRpcSelectStockRound(companyNameA);
//        }
//        if (reVal == 2)
//        {
//            CmdSendPLayerMessage(companyNameB + " 吞并 " + companyNameA + "\n");
//            companiesManager.CompanyGetCard(companyNameB, card);
//            TargetRpcSelectStockRound(companyNameB);
//        }
//        if(reVal == 3)//A公司等于B公司 需要由当前玩家选择一个保留
//        {
//            TargetRpcSelectWinCompany(companyNameA, companyNameB,card);
//        }
//        if (reVal == 4)//两个公司均为安全公司
//        {
//            List<string> cards = new List<string>
//            {
//                card + ",10"
//            };
//            Chessboard stock = Chessboard.GetChessboard().GetComponent<Chessboard>();
//            stock.cardInfo = cards;
//            stock.RemoveCardOnLightCards(card);
//        }
//    }
//    /// <summary>
//    /// 选择并购中要保留的公司
//    /// </summary>
//    [TargetRpc]
//    public void TargetRpcSelectWinCompany(string comAName,string comBName,string cardText)
//    {
//        GameObject obj= Instantiate(companiesChosePrefab, CanvasUI.GetRect());
//        CompaniesChoseUI choseUI = obj.GetComponent<CompaniesChoseUI>();
//        choseUI.ButtonAText = comAName;
//        choseUI.ButtonBText = comBName;

//        choseUI.card = cardText;
//    }
//    /// <summary>
//    /// 从手牌中打出一张牌
//    /// </summary>
//    /// <param name="theCard"></param>
//    [Command]
//    public void AttackOneCard(string theCard)
//    {
//        Chessboard stock = Chessboard.GetChessboard().GetComponent<Chessboard>();
//        stock.AddNewCardToLightCards(theCard);
//        List<string> cardList= new()
//        {
//            theCard + ",2"
//        };
//        stock.cardInfo = cardList;
//        var val = stock.CheckCards(theCard);
//        if (val.Item1 == 1)
//        {
//            handstate = false;
//            TargetRpcSelectCompanyRound();
//        }
//        if (val.Item1 == 2)
//        {
//            TargetRpcSelectStockRound(val.Item2.CompanyName);
//        }
//        if (val.Item1 == 3)
//        {
//            TargetRpcCompanyBattle(val.Item2.CompanyName,val.Item3.CompanyName, theCard);
//        }
//        foreach (string cardStr in cards)
//        {
//            if (cardStr == theCard)
//            {
//                cards = cards.Where(val => val != cardStr).ToArray();
//                //Stock.SetACard(card, 1);
//                //if (cardStr.Length > 1)
//                //{
//                //    foreach (Transform trans in Stock.GetStock().transform)
//                //    {
//                //        if (trans.name == cardStr)
//                //        {
//                //            CardController card = trans.GetComponent<CardController>();
//                //            //card.isLight = true;
//                //            //card.material = material;
//                //            //refreshPlane();
//                //            card.useMaterialIndex = 1;

//                //            break;
//                //        }
//                //    }
//                //}
//                getOneCardFromdeck();
//                OverThisRound();
//                //state = false;
//                break;
//            }
//        }
//    }
//    /// <summary>
//    /// 结束当前回合
//    /// </summary>
//    public void OverThisRound()
//    {

//        //UpdatePlayerTotalAssets();
//        UpdateCompaniesBoard();


//        //do something
//        if (gameProcessManager == null)
//        {
//            gameProcessManager = GameProcessManager.GetGameProcessManager();
//        }
//        handstate = false;
//        gameProcessManager.StartNextRound(this);
//    }
//    /// <summary>
//    /// 开始一个回合
//    /// </summary>
//    public void StartRound()
//    {
//        //测试用代码 正式版本需要注释
//        //cards = new string[] { "A1", "A2", "A3", "A4", "A5", "A6" };


//        if (gameProcessManager == null)
//        {
//            gameProcessManager = GameProcessManager.GetGameProcessManager();
//        }
//        handstate = true;
//        gameProcessManager.nowPlayer=this;
//    }
//    [Command]
//    /// <summary>
//    /// 创建公司
//    /// </summary>
//    public void CmdCreateCompany(string companyName)
//    {
//        Chessboard stock = Chessboard.GetChessboard().GetComponent<Chessboard>();
//        stock.EnableNewCompany(companyName);
//        UpadateCompanyState(companyName);


//    }
//    [ClientRpc]
//    public void UpadateCompanyState(string companyName)
//    {
//        CompaniesManager companies = CompaniesManager.GetCompaniesManager();
//        CompanyController company = companies.FindCompanyController(companyName);
//        company.UpdateCompanyAliveState(true);
//    }
//    [Command]
//    public void UpdateCompaniesBoard()
//    {
//        CompaniesBoardUI companiesBoardUI = CompaniesBoardUI.GetCompaniesBoardUI();
//        companiesBoardUI.info =UnityEngine.Random.Range(0,1000);
//    }
//    [Command]
//    /// <summary>
//    /// 更新股票数组
//    /// </summary>
//    public void UpadateStocksStr(string[] newstr)
//    {
//        stocks_str=newstr;
//    }
//    [Command]
///// <summary>
///// 增加或减少某个公司的剩余股票
///// </summary>
///// <param name="company"></param>
///// <param name="s">可以是正负数</param>
//    public void CompanyStockChange(string companyName,int s)
//    {
//        CompaniesManager companiesManager = CompaniesManager.GetCompaniesManager();
//        for (int i = 0;i < companiesManager.companies.Count; i++)
//        {
//            CompanyController companyController= companiesManager.companies[i].GetComponent<CompanyController>();
//            if (companyController.CompanyName == companyName)
//            {
//                companyController.RemainStock += s;
//                Debug.Log(companyName+"股票剩余"+ companyController.RemainStock);
//                break;
//            }
//        }
//    }
//    [Command]
//    /// <summary>
//    /// 对玩家手头现金的增减 sum可以是正负数
//    /// </summary>
//    /// <param name="company"></param>
//    /// <param name="sum"></param>
//    public void PlayerCashChange(int sum)
//    {
//        cash += sum;
//        //if (cash > 0)
//        //{
//        //    return true;
//        //}
//        //else
//        //{
//        //    cash -= sum;
//        //    return false;
//        //}
//    }
//    [Command]
//    /// <summary>
//    /// 更新玩家实时总资产 每个玩家回合结束 更新一次
//    /// </summary>
//    public void GetPlayerTotalAssets()
//    {
//        TotalAssets = 0;
//        TotalAssets += cash;
//        int stockPrice = 0;
//        if (stocks != null && stocks.Count > 0)
//        {
//            foreach (var item in stocks)
//            {
//                int thePrice = CompaniesManager.GetCompaniesManager().GetPrice(item.Item1);
//                stockPrice += thePrice * item.Item2;
//            }
//        }
//        TotalAssets += stockPrice;
//    }

//    //[Server]
//    //public static void CompanyCardsAdd(Company company, string[] strings)
//    //{
//    //    foreach(Company comp in companies)
//    //    {
//    //        if (comp == company)
//    //        {
//    //            comp.HasCardCount++;
//    //            comp.HasCards = strings;
//    //            break;
//    //        }
//    //    }
//    //}

//    //public static int GetPrice(Company company)
//    //{
//    //    foreach (Company comp in companies)
//    //    {
//    //        if (comp == company)
//    //        {
//    //            return comp.GetPrice();
//    //        }
//    //    }
//    //    return 0;
//    //}

//    //public static void CompanyCardsChange(Company company, int count)
//    //{
//    //    foreach (Company comp in companies)
//    //    {
//    //        if (comp == company)
//    //        {
//    //            comp.HasCardCount=count;
//    //            break;
//    //        }
//    //    }
//    //}

//    /// <summary>
//    /// 获取未上市的公司list
//    /// </summary>
//    /// <returns></returns>
//    //[Command]
//    //public List<Company> getWaitAliveCompany()
//    //{
//    //    List<Company> theCompanies = new List<Company>();
//    //    foreach(Company company in companies)
//    //    {
//    //        if (!company.IsLife)
//    //        {
//    //            theCompanies.Add(company);
//    //        }
//    //    }
//    //    return theCompanies;
//    //}
    
//    public string[] StocksToStrings()
//    {
//        if (stocks != null)
//        {
//            string[] strings = new string[stocks.Count()];
//            int i = 0;
//            foreach (var item in stocks)
//            {
//                strings[i] = item.Item1.CompanyName + "," + item.Item2.ToString();
//                i++;
//            }
//            return strings;
//        }
//        else
//        {
//            return null;
//        }
//    }
//}

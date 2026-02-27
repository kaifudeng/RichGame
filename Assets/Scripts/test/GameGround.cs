using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameGround : MonoBehaviour
{
    //public enum RequestType
    //{
    //    Question,
    //    Message
    //}
    public Player nowPlayer;
    public GameObject gameStagePrefab;
    Player player;
    int point;
    CardsController CardsManager;
    CompaniesController CompaniesManager;
    PlayersManager PlayersManager;
    private GameProcess TheLogsManager;

    CommunicationTool CommunicationTool=CommunicationTool.GetCommunicationTool();

    //bool overGround;
    //GameStage Stage;
    //internal async Task OverGround()
    //{
    //    await UniTask.Delay(3000);
    //}

    void Start()
    {
        //CommunicationTool = CommunicationTool.GetCommunicationTool();
        //try
        //{
        //    GameObject gameObject = GameObject.Instantiate(gameStagePrefab);
        //    GameStage Stage = gameObject.GetComponent<GameStage>();
        //}
        //catch (Exception e) 
        //{
        //    Debug.LogError(e.Message);
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }

    GameStage CreateGameStage(Player player, RequestType type)
    {
        GameObject gameStageObject = GameObject.Instantiate(gameStagePrefab);
        gameStageObject.transform.parent = gameStageObject.transform;
        NetworkServer.Spawn(gameStageObject);

        GameStage Stage = gameStageObject.GetComponent<GameStage>();
        Stage.id=player.netId+"_"+type.ToString();
        Stage.InitNowPlayer(player, type);
        return Stage;
    }

    public async UniTask ExecuteGround()
    {
        try
        {
            //开始第一阶段 出牌
            TheLogsManager.AddNewLog("--【"+nowPlayer.PlayerName+"】出牌阶段--");
            string theCard = await FirstStage();
            if (!string.IsNullOrEmpty(theCard))
            {
                TheLogsManager.AddNewLog("--【" + nowPlayer.PlayerName + "】打出了|" + theCard + "|--");
                //根据打出的牌判断是否需要进入第二阶段
                List<CardController> cards = new List<CardController>();
                Company theCompany = null;
                List<Company> theCompanies = null;
                int val = JudgmentEnterSecondStage(theCard, out cards, out theCompany, out theCompanies);

                switch (val)
                {
                    //零家公司 先创建 再购买
                    case 0:
                        TheLogsManager.AddNewLog("--【" + nowPlayer.PlayerName + "】正在创建公司--");
                        theCompany = await ChoiceACompanyStage(cards);
                        break;
                    //一家公司 购买股票
                    case 1:
                        //await BuyStockStage(theCompany);
                        break;
                    //两家公司 考虑并购
                    case 2:
                        TheLogsManager.AddNewLog("--【" + nowPlayer.PlayerName + "】正在处理公司并购事件--");
                        theCompany = await CompaniesMergerStage(theCompanies, theCard);
                        if (theCompany != null)//发生并购
                        {
                            TheLogsManager.AddNewLog("--【" + theCompany.CompanyName + "】正在收购其他公司--");
                            await CompaniesOutStage(theCompany, theCompanies);
                        }
                        else//两家安全公司 无事发生
                        {

                        }
                        break;
                    case -1: break;
                    //两家以上的公司 考虑并购
                    default:
                        TheLogsManager.AddNewLog("--【" + nowPlayer.PlayerName + "】正在处理公司并购事件--");
                        theCompany = await CompaniesMergerStage(theCompanies, theCard);
                        if (theCompany != null)//发生并购
                        {
                            TheLogsManager.AddNewLog("--【" + theCompany.CompanyName + "】正在收购其他公司--");
                            await CompaniesOutStage(theCompany, theCompanies);
                        }
                        else//两家安全公司 无事发生
                        {

                        }
                        break;
                }

                if (theCompany != null)
                {
                    //更新公司相关的信息
                    CompaniesManager.UpadateCompanyHasCard(theCompany, cards);
                    CompaniesManager.UpdateCompanyStockPrice();

                    //更新卡片棋盘相关的信息
                    CardsManager.AddCardToCompany(theCompany);

                    await BuyStockStage(theCompany);
                }
                else
                {
                    //TheLogsManager.AddNewLog("--【" + nowPlayer.PlayerName + "】似乎还在积蓄力量--");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            //return false;
        }
    }
    /// <summary>
    /// 第一阶段的操作
    /// </summary>
    /// <returns></returns>
    public async UniTask<string> FirstStage()
    {
        try
        {
            //第一阶段 出牌
            GameStage PlayACardStage = CreateGameStage(nowPlayer, RequestType.PlayACard);

            //PlayersManager.SetPlayerHandStateTrue(nowPlayer);

            string reVal = await CommunicationTool.WaitForPlayerInput(PlayACardStage);

            //补充手牌等固定的操作
            if (!string.IsNullOrEmpty(reVal))
            {
                try
                {
                    //1、玩家手牌减少
                    nowPlayer.ReduceACard(reVal);
                    //2、补充一张手牌
                    nowPlayer.getOneCardFromdeck(CardsManager);
                    //3、将玩家出牌阶段结束 避免连续出牌的情况
                    nowPlayer.OverPlayACard();
                    //4、将新卡片点亮到棋盘
                    CardsManager.AddNewCardToLightCards(reVal);
                    //5、结束出牌阶段
                    OverStage(PlayACardStage);
                }
                catch (Exception ex)
                {
                    Debug.LogError("处理玩家手牌时出现错误：" + ex.Message);
                    OverStage(PlayACardStage);
                }
                return reVal;
            }
            else
            {
                Debug.LogError("未收到玩家出牌阶段的相关回复");
                OverStage(PlayACardStage);
                return null;
            }


        }
        catch (Exception e)
        {
            Debug.LogError("出牌阶段出现错误：" + e.Message);
            return null;
        }
    }
    /// <summary>
    /// 判断要进入哪一个阶段
    /// </summary>
    /// <returns>返回值为0时，附近没有公司 需要进入创建公司的环节；返回值为1时，有一家公司，需要进入购买股票的环节；返回值为2时，需要进入公司并购的环节</returns>
    public int JudgmentEnterSecondStage(string card, out List<CardController> cards, out Company theComp,out List<Company> companies)
    {
        cards = new List<CardController>();
        theComp = null;
        companies = new List<Company>();
        try
        {
            //检查新卡是否触发其他事件
            cards = CardsManager.CheckCards();

            if (cards != null)//出现了两个以上的相邻卡片的情况
            {
                //进行进一步的逻辑判断
                int val = CardsManager.FindCompaniesWithNewCard(cards, out theComp,out companies);
                return val;

            }
            else//没有发生任何事件 结束当前玩家回合
            {
                return -1;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("判断新增卡牌相关逻辑时出错：" + ex.Message);
            return -1;
        }
    }
    /// <summary>
    /// 创建公司的环节
    /// </summary>
    /// <returns></returns>
    public async UniTask<Company> ChoiceACompanyStage(List<CardController> cards)
    {
        try
        {

            GameStage ChoiceACompanyStage = CreateGameStage(nowPlayer, RequestType.ChoiceACompany);
            if (!CompaniesManager.IsAllCompaniesLife())
            {
                List<Company> companiesList=CompaniesManager.GetAllCompaniesByState(false);

                string compstr= CompaniesManager.CompaniesListToString(companiesList);

                ChoiceACompanyStage.sendValue=compstr;

                //发送一个选择窗体 请求玩家选择要创建的公司
                string compName = await CommunicationTool.WaitForPlayerInput(ChoiceACompanyStage);
                if (string.IsNullOrEmpty(compName))
                {
                    //return true;
                    Debug.LogError("未收到用户回复的公司信息");
                    OverStage(ChoiceACompanyStage);
                    return null;
                }
                else
                {
                    //创建新公司
                    Company newCompany = CompaniesManager.CreateNewCompany(compName);
                    TheLogsManager.AddNewLog("--【" + newCompany.CompanyName + "】上市了--");
                    //更新公司相关的信息
                    CompaniesManager.UpadateCompanyHasCard(newCompany, cards);
                    CompaniesManager.UpdateCompanyStockPrice();

                    //更新卡片棋盘相关的信息
                    CardsManager.AddCardToCompany(newCompany);

                    //玩家获取新公司的原始股票
                    nowPlayer.PlayerGetFirstStock(newCompany);
                    CompaniesManager.CompanyRemindStockAdd(newCompany, -1);

                    //将玩家加入到公司的股东列表
                    //CompaniesManager.AddPlayerToCompanyList(nowPlayer, newCompany);
                    CompaniesManager.RankingPlayerWithCompany(newCompany,PlayersManager);

                    //更新玩家相关信息
                    PlayersManager.SyncTotalAssets(CompaniesManager);

                    //结束创建阶段
                    OverStage(ChoiceACompanyStage);

                    return newCompany;
                }
            }
            else
            {
                Debug.Log("所有公司都以上市，没有可创建的公司了");
                OverStage(ChoiceACompanyStage);
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("创建公司时出现错误：" + ex);
            return null;
        }
    }
    /// <summary>
    /// 购买股票的环节
    /// </summary>
    /// <returns></returns>
    public async UniTask BuyStockStage(Company company)
    {
        try
        {
            if (company == null) return;
            GameStage BuyStockStage = CreateGameStage(nowPlayer, RequestType.BuyStock);
            if (company!= null)
            {
                //获取该公司剩余的股票
                int remainStockSum = CompaniesManager.GetRemainStockSum(company);

                remainStockSum = remainStockSum > 3 ? 3 : remainStockSum;

                BuyStockStage.sendValue = company.CompanyName +","+ remainStockSum.ToString();

                //获取当前玩家的剩余金钱 预测玩家输入的最大值
                int MaxSum= nowPlayer.cash/ company.price;
                if (MaxSum > 0)
                {
                    BuyStockStage.MaxValue = MaxSum.ToString();

                    int sum = ConvertFuntion.StringToInt(await CommunicationTool.WaitForPlayerInput(BuyStockStage));

                    if (sum > 0)
                    {
                        //1、玩家持有股票增加 更新玩家总资产数据
                        PlayersManager.PlayerStockAdd(nowPlayer, sum, company);
                        PlayersManager.SyncTotalAssets(CompaniesManager);
                        //2、公司剩余股票减少
                        CompaniesManager.CompanyRemindStockAdd(company, -sum);
                        //3、公司持股人排序变化
                        CompaniesManager.RankingPlayerWithCompany(company, PlayersManager);
                    }
                    else
                    {
                        //Debug.LogError("玩家没有购买股票");
                    }
                }
                else
                {
                    //Debug.LogError("玩家没有钱购买股票");
                }
            }
            else
            {
                Debug.LogError("该公司剩余股票为零");
            }

            OverStage(BuyStockStage);
        }
        catch (Exception ex)
        {
            Debug.LogError("股票购买环节出现错误：" + ex.Message);
        }

    }
    /// <summary>
    /// 公司并购的环节
    /// </summary>
    /// <returns></returns>
    public async UniTask<Company> CompaniesMergerStage(List<Company> companies,string card)
    {
        //分两种情况，第一种，并购的公司中最大的公司有至少两个 且均不为安全公司 规模一致 需要玩家进行选择
        //第二种情况 并购的公司中不存在任何两个规模相同的 不需要玩家参与并购处理 
        //第三种情况 并购的公司中存在两家或以上的安全公司 不需要玩家参与并购处理
        //
        try
        {
            Company ReComp=null;
            List<Company> safeList = CompaniesManager.FindtheSafeCompany(companies);
            if (safeList.Count >= 2)
            {
                //第三种情况 废弃卡牌
                CardsManager.SetCardBlack(CardsManager.GetCardController(card));
                return null;
            }
            else
            {

            }
            List<Company> MaxList = CompaniesManager.FindtheMaxCompany(companies);
            if (MaxList != null && MaxList.Count > 0)
            {
                if (MaxList.Count == 1) //第二种情况
                {
                    for(int i = 0; i < companies.Count; i++)
                    {
                        if (companies[i].Equals(MaxList[0]))
                        {
                            companies.Remove(companies[i]);
                            break;
                        }
                    }
                 
                    CompaniesManager.CompanyEatCompanies(MaxList[0], companies);

                    //找到破产公司的持股玩家
                    List<Player> players = PlayersManager.GetPlayersWithCompanies(companies);

                    ReComp = MaxList[0];

                }
                
                else
                {
                    //第一种情况 需要玩家进行选择
                    GameStage companiesMergerStage = CreateGameStage(nowPlayer, RequestType.MergerCompanies);
                    try
                    {
                        
                        string compstr = CompaniesManager.CompaniesListToString(MaxList);

                        companiesMergerStage.sendValue=compstr;

                        string compName = await CommunicationTool.WaitForPlayerInput(companiesMergerStage);

                        if (compName != null)
                        {
                            Company winComp = CompaniesManager.FindCompanyWithName(compName);

                            CompaniesManager.CompanyEatCompanies(MaxList[0], companies);

                            ReComp = winComp;
                            for (int i = 0; i < companies.Count; i++)
                            {
                                if (companies[i].Equals(ReComp))
                                {
                                    companies.Remove(companies[i]);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("公司并购时客户返回选择客户时出现空值错误");
                            return null;
                        }
                        OverStage(companiesMergerStage);
                    }
                    catch (Exception ex) 
                    {
                        Debug.LogError("公司收购过程出现错误：" + ex);
                        OverStage(companiesMergerStage);
                    }
                    
                }
            }
            else
            {
                Debug.LogError("并购返回的公司为空");
                return null;
            }
            return ReComp;

        }
        catch (Exception ex) 
        {
            Debug.LogError("公司并购时出现错误："+ex);
            return null;
        }
    }
    /// <summary>
    /// 公司破产结算的环节
    /// </summary>
    /// <param name="theWiner"></param>
    /// <param name="outCompanies"></param>
    /// <returns></returns>
    public async UniTask<SettlementWay> CompaniesOutStage(Company theWiner,List<Company> outCompanies)
    {
        
        try
        {
            List<Player> outPlayers = new List<Player>();//受害者列表

            outPlayers = PlayersManager.GetPlayersWithCompanies(outCompanies);

            foreach (Company company in outCompanies)
            {
                foreach (Player player in outPlayers)
                {
                    GameStage companiesOutStage = CreateGameStage(player, RequestType.CompaniesOut);

                    int stockSum=player.GetStockSum(company);

                    if (stockSum <= 0) { continue; }

                    string val= stockSum.ToString();

                    val += "|" + CompaniesManager.CompanyToString(company)+ CompaniesManager.CompanyToString(theWiner);

                    companiesOutStage.sendValue=val;

                    SettlementWay settlement = new SettlementWay(await CommunicationTool.WaitForPlayerInput(companiesOutStage));

                    if (settlement != null)
                    {
                        if (settlement.sellCount > 0)
                        {
                            try
                            {
                                PlayersManager.SellStock(company, settlement.sellCount,CompaniesManager,player);

                                CompaniesManager.CompanyRemindStockAdd(company, settlement.sellCount);
                            }
                            catch (Exception ex) 
                            {
                                Debug.LogError("出售股票时出现错误："+ex);
                            }
                        }
                        if(settlement.translateCount > 0)
                        {
                            try
                            {
                                CompaniesManager.CompanyRemindStockAdd(company, settlement.translateCount);

                                CompaniesManager.CompanyRemindStockAdd(theWiner, -settlement.translateCount / 2);

                                //公司持股人排序变化
                                CompaniesManager.RankingPlayerWithCompany(company, PlayersManager);

                                PlayersManager.PlayerStockAdd(player, settlement.translateCount / 2, theWiner);

                                PlayersManager.PlayerStockAdd(player, -settlement.translateCount, company);

                                PlayersManager.SyncTotalAssets(CompaniesManager);
                            }catch(Exception ex)
                            {
                                Debug.LogError("替换股票时出现错误：" + ex);
                            }
                        }
                    }

                    CompaniesManager.CompanyOut(company);
                    TheLogsManager.AddNewLog("--【" + company.CompanyName + "】已退市--");
                    OverStage(companiesOutStage);
                }
            }
            //return ReComp;
            return null;
        }
        catch (Exception e)
        {

            Debug.LogError("公司破产结算出现错误" + e);
            return null;
        }
    }
    internal void InitGround(Player thePlayer, CardsController theCardsController, CompaniesController companiesController, PlayersManager playersManager, GameProcess gameProcess)
    {
        nowPlayer = thePlayer;
        CardsManager = theCardsController;
        CompaniesManager = companiesController;
        PlayersManager = playersManager;
        TheLogsManager= gameProcess;
    }
    /// <summary>
    /// 结束当前阶段
    /// </summary>
    public void OverStage(GameStage stage)
    {
        try
        {
            stage.DestroyObject();
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    internal void DestroyObject()
    {
            NetworkServer.Destroy(gameObject);
    }
}

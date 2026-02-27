using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using Telepathy;
using Unity.VisualScripting;
/// <summary>
/// 管理所有的卡片 所有关于卡片的逻辑在此处执行 默认全部在服务器上跑
/// </summary>
public class CardsController : NetworkBehaviour
{
    /// <summary>
    /// 表现层的游戏对象
    /// </summary>
    public GameObject chessBoardUIObject;
    ChessBoardUI chessBoardUI;
    /// <summary>
    /// 用于向表现层传递数据
    /// </summary>
    [SyncVar(hook = nameof(CardInfoChange))]
    public string[] cardInfo;

    public Action<string[]> OncardInfoChange;
    [SyncVar]
    public string[] deck;
    /// <summary>
    /// 已经处于激活状态的卡片
    /// </summary>
    public List<CardController> lightingCards;

    /// <summary>
    /// 场上所有卡片集合 100个
    /// </summary>
    public List<CardController> allCards;
    /// <summary>
    /// 新增的那张卡片
    /// </summary>
    public CardController theNewCard;
    /// <summary>
    /// 公司管理器
    /// </summary>
    public CompaniesController companiesController;
    /// <summary>
    /// 游戏流程管理器
    /// </summary>
    public GameProcess gameProcess;

    public void CardInfoChange(string[] cardInfo, string[] newCardInfo)
    {
        OncardInfoChange?.Invoke(newCardInfo);
    }

    public void InitCardsSystem()
    {
        InitDeck();
        InitChessBoard();

    }
    /// <summary>
    /// 初始化公共棋盘
    /// </summary>
    void InitChessBoard()
    {
        chessBoardUIObject = Instantiate(chessBoardUIObject);
        NetworkServer.Spawn(chessBoardUIObject);

        chessBoardUI = chessBoardUIObject.GetComponent<ChessBoardUI>();
        OncardInfoChange = chessBoardUI.RpcOnChessBoardInfoChange;
        //OncardInfoChange?.Invoke(cardInfo);

        allCards = chessBoardUI.allCards;
    }
    /// <summary>
    /// 初始化公共牌堆
    /// </summary>
    string[] InitDeck()
    {
        deck = null;
        deck = new string[100];
        int len = 0;
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                string val = (char)(i + 64) + j.ToString();
                deck[len] = val;
                len++;
            }
        }
        return deck;
    }

    /// <summary>
    /// 从牌堆获取任意数量的卡片
    /// </summary>
    /// <param name="sum"></param>
    /// <returns></returns>
    public string[] GetCards(int sum)
    {
        string[] backCards = new string[sum];
        for (int i = 0; i < sum; i++)
        {
            backCards[i] = GetCard();
        }
        return backCards;
    }
    /// <summary>
    /// 从牌堆获取一张卡  牌堆减少
    /// </summary>
    /// <returns></returns>
    public string GetCard()
    {
        //测试用
        //string backCard = deck[deck.Length - 1];
        try
        {
            if (deck.Length == 0)
            {
                Debug.Log("牌堆为空");
                return null;
            }
            else
            {
                string backCard = deck[UnityEngine.Random.Range(0, deck.Length)];
                deck = deck.Where(val => val != backCard).ToArray();
                return backCard;
            }
            
        }
        catch (Exception e) 
        {
            Debug.LogError(e);
            return null;

        }
    }
    [Server]
    /// <summary>
    /// 新增一张卡片到点亮的卡片列表中
    /// </summary>
    /// <param name="newcard"></param>
    public void AddNewCardToLightCards(string newcard)
    {
        foreach (CardController card in allCards)
        {
            if (card.ToString() == newcard)
            {
                lightingCards.Add(card);
                //Debug.Log("LightCards add"+card.ToString());
                theNewCard = card;
                string[] theCardInfo = null;
                theCardInfo = new string[1]
               { theNewCard.ToString() + ",2"};

                UpadateCardInfo(theCardInfo);
                //card.state = true;
                //if (CheckCards())
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}
            }
        }
        //return false;
    }
    [Server]
    /// <summary>
    /// 返回当前卡片的所有相邻已点亮卡片
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    public List<CardController> FindAllNextCard(CardController theCard)
    {
        List<CardController> waitColorCards = new()
        { theCard};

        for (int i = 0; i < waitColorCards.Count; i++)
        {
            foreach (var cardController in lightingCards)
            {
                if ((waitColorCards[i].x == cardController.x && Math.Abs(waitColorCards[i].y - cardController.y) == 1) ||
                    (waitColorCards[i].y == cardController.y && Math.Abs(waitColorCards[i].x - cardController.x) == 1))
                {
                    if (!waitColorCards.Contains(cardController)&& cardController.useMaterialIndex!=10)
                    {
                        waitColorCards.Add(cardController);
                    }
                }
            }
        }
        return waitColorCards;
    }

    /// <summary>
    /// 确认落点附近有多少家公司
    /// </summary>
    /// <param name="nextCards"></param>
    public int FindCompaniesWithNewCard(List<CardController> nextCards, out Company comp, out List<Company> companies)
    {
        List<Company> theComps = new List<Company>();
        comp = null;
        foreach (CardController theCard in nextCards)
        {
            if (theCard.Company != null)
            {
                if (comp == null)
                {
                    comp = theCard.Company;
                    if (!theComps.Contains(comp))
                        theComps.Add(comp);
                }
                else
                {
                    // theComps.Add(comp);
                    if (comp == theCard.Company)
                    {
                        continue;
                    }
                    else
                    {
                        if (!theComps.Contains(theCard.Company))
                            theComps.Add(theCard.Company);

                    }
                }
            }
        }
        if (theComps.Count >= 2)//第三种情况!!! 周围存在两家或两家以上公司 需要进行并购流程
        {
            companies = theComps;
            return theComps.Count;

        }
        if (comp == null)//周边无任何一家公司 进入创建公司的流程
        {
            companies = null;
            return 0;

        }
        else//第二种情况 周边有一家公司
        {
            companies = null;
            return 1;

        }
    }
    /// <summary>
    /// 检查新卡片是否触发事件 触发事件为true 否则为false 第一种情况 周边无任何一家公司 亦无任何相邻卡片 
    /// </summary>
    /// <returns></returns>
    public List<CardController> CheckCards()
    {
        List<CardController> nextCards = FindAllNextCard(theNewCard);
        if (nextCards.Count == 1)//第一种情况 不触发任何事件
        {
            return null;
        }
        else if (nextCards.Count > 1)//周边存在卡片
        {
            return nextCards;
        }
        else return null;

    }
    /// <summary>
    /// 公司持有卡片发生变化之后回调这个方法 更新卡片
    /// </summary>
    //[Server]
    //public void UpdateCompanyCard(Company theComp)
    //{
    //    companiesController.UpadateCompanyHasCard(theComp, FindAllNextCard(theNewCard));
    //    AddCardToCompany(theComp);

    //}
    /// 更新棋盘上公司卡片的颜色
    /// </summary>
    /// <param name="company"></param>
    [Server]
    public void AddCardToCompany(Company company)
    {
        List<string> strings = new List<string>();
        foreach (CardController cardController in company.GetCards())
        {
            strings.Add(cardController.ToString() + "," + company.useMaterialIndex);
            for (int i = 0; i < lightingCards.Count; i++)
            {
                if (lightingCards[i].ToString() == cardController.ToString())
                {
                    lightingCards[i].Company = company;
                }
            }
        }
        UpadateCardInfo(strings.ToArray());
    }

    /// <summary>
    /// 将一张卡片设为弃用状态
    /// </summary>
    /// <param name="card"></param>
    [Server]
    public void SetCardBlack(CardController card)
    {
        //card.state=false;
        List<string> strings = new List<string>
        {
            card.ToString() + ",10"
        };
        UpadateCardInfo(strings.ToArray());

    }
    public void UpadateCardInfo(string[] strings)
    {
        cardInfo = strings;
    }

    /// <summary>
    /// 找到一副手牌中 最小的一张
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public CardController FindTheLessCard(string[] cards)
    {
        string reVal = "J10";
        int min = 'J' + 10;
        foreach (string theCard in cards)
        {
            int A = theCard[0];
            int B = (int)char.GetNumericValue(theCard[1]);
            if (A + B <= min)
            {
                min = A + B;
                reVal = theCard;
            }
        }
        return GetCardController(reVal);
    }
    /// <summary>
    /// 获取对象形式的Card
    /// </summary>
    /// <param name="cardStr"></param>
    /// <returns></returns>
    public CardController GetCardController(string cardStr)
    {
        foreach (CardController card in allCards)
        {
            if (!string.IsNullOrEmpty(cardStr))
            {
                char a = cardStr[0];
                int b = Convert.ToInt32(cardStr.Replace(cardStr[0], '0'));
                if (a == card.x && b == card.y)
                {
                    return card;
                }
            }
        }
        return null;
    }
    /// <summary>
    /// A卡和B卡进行比较 若A大于B 返回true 
    /// </summary>
    /// <param name="ACard"></param>
    /// <param name="BCard"></param>
    /// <returns></returns>
    public bool CardCompare(CardController ACard, CardController BCard)
    {
        if (ACard.x + ACard.y > BCard.x + BCard.y)
        {
            return true;
        }
        else if (ACard.x + ACard.y < BCard.x + BCard.y)
        {
            return false;
        }
        else if (ACard.x + ACard.y == BCard.x + BCard.y)
        {
            if (ACard.x > BCard.x)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else { return false; }
    }
}

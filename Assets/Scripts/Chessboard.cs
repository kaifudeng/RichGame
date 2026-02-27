//using Mirror;
//using Mirror.BouncyCastle.Utilities;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class Chessboard : NetworkBehaviour
//{

//    [SyncVar(hook = nameof(UpdateChessboard))]
//    public List<string> cardInfo;

//    CompaniesManager companiesManager;
//    public static List<string> cardstring;

//    public static Chessboard theChessboard;
//    public static GameObject GetChessboard() => theChessboard.gameObject;

//    /// <summary>
//    /// 场上所有卡片集合 100个
//    /// </summary>
//    public List<CardController> allCards;
//    /// <summary>
//    /// 场上已激活卡片集合 包括已创建公司和未创建
//    /// </summary>
//    public List<CardController> LightCards;
//    /// <summary>
//    /// 待启用区块
//    /// </summary>
//    public static CardController WaitForUseCard;
//    public GameObject cardPrefab;
//    public Material lightCardMaterial01;
//    private void Awake()
//    {
//        theChessboard = this;
//        companiesManager = CompaniesManager.GetCompaniesManager();
//        //syncMode = SyncMode.Observers;
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        InitPlane();
//        cardstring = new List<string>();
//        LightCards = new List<CardController>();
//    }

//    // Update is called once per frame
//    void Update()
//    {

//    }

//    /// <summary>
//    /// 初始化棋盘
//    /// </summary>
//    public void InitPlane()
//    {
//        allCards = new List<CardController>();
//        for (int i = 1; i < 11; i++)
//        {
//            for (int j = 1; j < 11; j++)
//            {
//                char letter = (char)(j + 64);

//                UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(i, 0, (int)(letter - 64));
//                GameObject obj = Instantiate(cardPrefab, vector3, cardPrefab.transform.rotation);
//                CardController cardComponent = obj.GetComponent<CardController>();
//                cardComponent.x = letter;
//                cardComponent.y = i;
//                cardComponent.UpdateMaterial(0);
//                obj.transform.parent = gameObject.transform;
//                obj.name = letter.ToString() + i.ToString();
//                allCards.Add(cardComponent);
//            }
//        }
//        //foreach (CardController card in cards)
//        //{



//        //}

//    }

//    ///// <summary>
//    ///// 刷新棋盘
//    ///// </summary>
//    //public static void refreshPlane()
//    //{
//    //    foreach (Transform trans in GetStock().transform)
//    //    {
//    //        CardController card = trans.GetComponent<CardController>();
//    //        ShaderChange.ChangeMaterial(trans.gameObject, card.material);
//    //    }
//    //}

//    ///// <summary>
//    ///// 初始化卡片列表
//    ///// </summary>
//    //public void InitCardList()
//    //{

//    //}


//    /// <summary>
//    /// 根据最新信息变化，同步棋盘上卡片颜色
//    /// </summary>
//    /// <param name="old"></param>
//    /// <param name="newval"></param>
//    /// <returns></returns>
//    public static bool UpdateChessboard(List<string> old, List<string> newval)
//    {
//        foreach (string c in newval)
//        {
//            string[] strs = c.Split(',');
//            if (strs.Length > 0)
//            {
//                foreach (Transform trans in GetChessboard().transform)
//                {
//                    CardController card = trans.GetComponent<CardController>();
//                    if (card.name == strs[0])
//                    {
                        
//                        card.UpdateMaterial(Convert.ToInt16(strs[1]));
//                        //LightCards.Add(card);
//                        //cardstring.Add(strs[0]);
//                        //WaitForUseCard = card;
//                        //if (CheckCards(strs[0]))
//                        //{
//                        //    GameInfoUI.ShowCompanyInfo();
//                        //    return true;
//                        //}
//                        //else
//                        //{
//                        //    return false;
//                        //}
//                    }
//                }
//            }
//        }
//        return false;

//    }

//    public void cardsChange(string[] olds, string[] news)
//    {


//    }

//    /// <summary>
//    /// 检查新卡片是否触发事件 第一种情况 周边无任何一家公司 第二种情况 周边有一家公司 第三种情况!!! 周围存在两家公司 需要进行并购流程 返回0则表示什么也不发生
//    /// </summary>
//    /// <returns></returns>
//    public (int, CompanyController, CompanyController) CheckCards(string newCard)
//    {
//        CardController newCardObj = null;
//        foreach (CardController theCard in allCards)
//        {
//            if (theCard.ToString() == newCard.ToString())
//            {
//                newCardObj = theCard;
//            }

//        }
//        List<CardController> nextCards = FindAllNextCard(newCardObj);
//        if (nextCards.Count > 1)//周边存在卡片 分三种情况处理
//        {
//            CompanyController comp = null;
//            foreach (CardController theCard in nextCards)
//            {
//                if (theCard.Company != null)
//                {
//                    if (comp == null)
//                    {
//                        comp = theCard.Company;
//                    }
//                    else
//                    {
//                        if (comp == theCard.Company)
//                        {
//                            continue;
//                        }
//                        else
//                        {
//                            //第三种情况!!! 周围存在两家公司 需要进行并购流程
//                            return (3, comp, theCard.Company);
//                        }
//                    }
//                }
//            }
//            if (comp == null)//第一种情况 周边无任何一家公司 
//            {
//                return (1, null, null);
//            }
//            else//第二种情况 周边有一家公司
//            {
//                List<string> theCards = new();
//                List<string> strings = new();
//                int count = 0;
//                foreach (CardController theCard in nextCards)
//                {
//                    theCard.Company = comp;
//                    theCard.RefeshMaterial();
//                    count++;
//                    strings.Add(theCard.ToString() + "," + theCard.getMaterial());
//                    theCards.Add(theCard.ToString());
//                }
//                cardInfo = strings;
//                companiesManager.CompanyCardsChange(comp, theCards);
//                return (2, comp, null);
//            }
//        }
//        else
//        {
//            return (0, null, null);
//        }
//    }

//    ///// <summary>
//    ///// 更换卡片的颜色
//    ///// </summary>
//    ///// <param name="cardstr"></param>
//    //public void changeCardColor(string[] cardstr)
//    //{
//    //    foreach (CardController card in allCards)
//    //    {
//    //        if (card.x == cardstr[0] && card.y == (int)char.GetNumericValue(cardstr[1]))
//    //        {
//    //            card.UpdateMaterial();
//    //        }
//    //    }
//    //}
//    [Server]
//    public void EnableNewCompany(string companyName)
//    {
//        CompanyController theCom = null;
//        companiesManager = CompaniesManager.GetCompaniesManager();
//        foreach (GameObject company in companiesManager.companies)
//        {
//            CompanyController companyController = company.GetComponent<CompanyController>();
//            if (companyController.CompanyName == companyName)
//            {
//                theCom = companyController;
//            }
//        }
//        List<string> strings = new List<string>();
//        List<string> Companycards = new List<string>();
//        //foreach (CardController card in LightCards)
//        //{
//        //    if (card.ToString() == WaitForUseCard.ToString())
//        //    {
//        //        card.Company = theCom;
//        //        card.useMaterialIndex = theCom.useMaterialIndex;
//        //        strings.Add(card.ToString() +","+card.useMaterialIndex);
//        //    }
//        //    else if ((card.x == WaitForUseCard.x && Math.Abs(card.y - WaitForUseCard.y) == 1) ||
//        //        (card.y == WaitForUseCard.y && Math.Abs(card.x - WaitForUseCard.x) == 1))
//        //    {
//        //        card.Company = theCom;
//        //        card.useMaterialIndex = theCom.useMaterialIndex;
//        //        strings.Add(card.ToString() + "," + card.useMaterialIndex);

//        //    }
//        //}
//        List<CardController> waitColorCards = FindAllNextCard(WaitForUseCard);
//        //waitColorCards.Add(WaitForUseCard);
//        int count = 0;
//        foreach (CardController card in waitColorCards)
//        {
//            card.Company = theCom;
//            card.UpdateMaterial(theCom.useMaterialIndex);
//            count++;
//            strings.Add(card.ToString() + "," + card.getMaterial());
//            Companycards.Add(card.ToString());  
//        }
//        companiesManager.CompanyCardsChange(theCom, Companycards);
//        cardInfo = strings;
//        //Debug.Log("创建公司变色：" + cardInfo);
//    }

//    [ClientRpc]
//    /// <summary>
//    /// 用这个方法给LightCards添加新增的卡片
//    /// </summary>
//    /// <param name="newcard"></param>
//    public void AddNewCardToLightCards(string newcard)
//    {
//        foreach (CardController card in allCards)
//        {
//            if (card.ToString() == newcard)
//            {
//                LightCards.Add(card);
//                //Debug.Log("LightCards add"+card.ToString());
//                WaitForUseCard = card;
//                card.state = true;
//            }
//        }
//    }

//    [ClientRpc]
//    /// <summary>
//    /// 用这个方法给LightCards删除卡片
//    /// </summary>
//    /// <param name="newcard"></param>
//    public void RemoveCardOnLightCards(string theCard)
//    {
//        foreach (CardController card in LightCards)
//        {
//            if (card.ToString() == theCard)
//            {
//                LightCards.Remove(card);
//                break;

//            }
//        }
//    }
//    /// <summary>
//    /// 用这个方法给LightCards更换所属公司
//    /// </summary>
//    [ClientRpc]
//    public void ChangeCardCompany(string theCard,string companyName)
//    {
//        CompanyController companyController= companiesManager.FindCompanyController(companyName);
//        for (int i = 0; i < LightCards.Count; i++)
//        {
//            if(LightCards[i].ToString() == theCard)
//            {
//                LightCards[i].Company=companyController;
//            }
//        }
//    }

//    /// <summary>
//    /// 返回当前卡片的所有相邻已点亮卡片
//    /// </summary>
//    /// <param name="card"></param>
//    /// <returns></returns>
//    public List<CardController> FindAllNextCard(CardController theCard)
//    {
//        List<CardController> waitColorCards = new()
//        { theCard};

//        for (int i = 0; i < waitColorCards.Count; i++)
//        {
//            foreach (var cardController in LightCards)
//            {
//                if ((waitColorCards[i].x == cardController.x && Math.Abs(waitColorCards[i].y - cardController.y) == 1) ||
//                    (waitColorCards[i].y == cardController.y && Math.Abs(waitColorCards[i].x - cardController.x) == 1))
//                {
//                    if (!waitColorCards.Contains(cardController))
//                    {
//                        waitColorCards.Add(cardController);
//                    }
//                }
//            }
//        }
//        return waitColorCards;
//    }

//    //public void ColorNext(CardController theCard) 
//    //{
//    //    List<string> strings=new();
//    //    List<CardController> cards = FindAllNextCard(theCard);
//    //    foreach (var card in cards)
//    //    {
//    //        if (card != null && card.useMaterialIndex != theCard.useMaterialIndex)
//    //        {
//    //            card.useMaterialIndex = theCard.useMaterialIndex;
//    //            card.Company = theCard.Company;
//    //            strings.Add(card.ToString() + "," + card.useMaterialIndex);
//    //            cardInfo = strings;
//    //            ColorNext(card);
//    //        }
//    //    }
//    //}
//}

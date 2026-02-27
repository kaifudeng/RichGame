//using Mirror;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CompaniesManager : MonoBehaviour
//{

//    static CompaniesManager instance;
//    public static CompaniesManager GetCompaniesManager() => instance;

//    public GameObject companyPrefab;

//    public List<GameObject> companies;

//    public List<string> InitCompaniesStrs;
//    // Start is called before the first frame update
//    void Start()
//    {
//        instance = this;
//        //NetworkServer.Spawn(gameObject);
//        //InitCompanies(InitCompaniesStrs);
//    }

    

//    // Update is called once per frame
//    void Update()
//    {
//        if (companies.Count == 0)
//        {
//            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Company");
//            foreach (GameObject gameObject in gameObjects)
//            {
//                //CompanyController companyController=gameObject.GetComponent<CompanyController>();
//                companies.Add(gameObject);
//                gameObject.transform.parent = transform;
//            }
//        }
//    }
//    [Server]
//    /// <summary>
//    /// 初始化公司管理器
//    /// </summary>
//    /// <param name="InitCompaniesStrs"></param>
//    public void InitCompanies()
//    {
//        if (InitCompaniesStrs != null)
//        {
//            foreach (string Init in InitCompaniesStrs)
//            {
//                string[] strs = Init.Split(',');
                
//                GameObject compA = Instantiate(companyPrefab);
//                NetworkServer.Spawn(compA);
//                CompanyController controllerA = compA.GetComponent<CompanyController>();
//                controllerA.InitCompany(strs[0], ConvertFuntion.StringToInt(strs[1]), ConvertFuntion.StringToChar(strs[2]));
//                //companies.Add(compA);
//                //compA.transform.parent = transform;

//                //Debug.Log(controllerA.netIdentity.assetId);
//            }
//        }
//    }

//    public int GetPrice(CompanyController company)
//    {
//        foreach (GameObject comp in companies)
//        {
//            CompanyController companyController = comp.GetComponent<CompanyController>();
//            if (companyController == company)
//            {
//                return companyController.GetPrice();
//            }
//        }
//        return -1;
//    }

//    public void CompanyCardsChange(CompanyController company, List<string> cards)
//    {
//        foreach (GameObject comp in companies)
//        {
//            CompanyController companyController = comp.GetComponent<CompanyController>();
//            if (companyController == company)
//            {
//                //companyController.HasCardCount = count;
//                companyController.RefreshHasCards(cards.ToArray());
                
//                break;
//            }

//        }
//    }

//    /// <summary>
//    /// 获取未上市的公司list
//    /// </summary>
//    /// <returns></returns>
//    public List<CompanyController> getWaitAliveCompany()
//    {
//        List<CompanyController> theCompanies = new List<CompanyController>();
//        foreach (GameObject obj in companies)
//        {
//            CompanyController company = obj.GetComponent<CompanyController>();
//            if (!company.IsLife)
//            {
//                theCompanies.Add(company);
//            }
//        }
//        return theCompanies;
//    }

//    /// <summary>
//    /// 获取公司
//    /// </summary>
//    /// <param name="name"></param>
//    /// <returns></returns>
//    public CompanyController FindCompanyController(string name)
//    {
//        foreach (GameObject comp in companies)
//        {
//            CompanyController compController = comp.GetComponent<CompanyController>();
//            if (compController.CompanyName == name)
//            {
//                return compController;
//            }
//        }
//        return null;
//    }

//    /// <summary>
//    /// 获取公司股价
//    /// </summary>
//    /// <returns></returns>
//    public int getCompanyStockPrice(string companyName)
//    {
//        CompanyController companyController = FindCompanyController(companyName);
//        return companyController.GetPrice();
//    }


//    /// <summary>
//    /// 公司并购 分为四种情况 1.A公司大于B公司；2.B公司大于A公司；3.A公司等于B公司； 4.公司A和公司B均为安全公司
//    /// </summary>
//    /// <param name="companyA"></param>
//    /// <param name="companyB"></param>
//    /// <returns></returns>
//    public int CompaniesBattle(CompanyController companyA, CompanyController companyB)
//    {
//        if(companyA.HasCards.Length>=15&& companyA.HasCards.Length >= 15)
//        {
//            return 4;
//        }
//        if (companyA.HasCards.Length == companyB.HasCards.Length)
//        {
//            //两家公司尺寸相同的情况 
//            return 3;
//        }
//        CompanyController Big;
//        CompanyController Small;
//        int returnVal = 1;
//        if (companyA.HasCards.Length > companyB.HasCards.Length)
//        {
//            Big = companyA;
//            Small = companyB;
//            returnVal = 1;
//        }
//        else
//        {
//            Big = companyB;
//            Small = companyA;
//            returnVal = 2;
//        }
//            Chessboard stock = Chessboard.GetChessboard().GetComponent<Chessboard>();
//            foreach (string cardStr in Small.HasCards)
//            {
//                Big.HasCardAdd(cardStr);
//                stock.ChangeCardCompany(cardStr, Big.CompanyName);
//            }
//            Small.IsLife = false;
//            Big.CompanyRefreshChessBoard();
//        return returnVal;
//    }

//    /// <summary>
//    /// 将新的卡片纳入某个公司
//    /// </summary>
//    /// <param name="comName"></param>
//    /// <param name="card"></param>
//    public void CompanyGetCard(string comName,string card)
//    {
//        CompanyController company= FindCompanyController(comName);
//        company.HasCardAdd(card);
//        company.CompanyRefreshChessBoard();
//    }
//    /// <summary>
//    /// 公司破产分钱
//    /// </summary>
//    public void CompanyDead(string companyName)
//    {

//    }

//    /// <summary>
//    /// 公司资源迁移
//    /// </summary>
//    /// <param name="companyA"></param>
//    /// <param name="companyB"></param>
//    public void CompanySmallToBig(CompanyController Big, CompanyController Small)
//    {
//        Chessboard stock = Chessboard.GetChessboard().GetComponent<Chessboard>();
//        foreach (string cardStr in Small.HasCards)
//        {
//            Big.HasCardAdd(cardStr);
//            stock.ChangeCardCompany(cardStr, Big.CompanyName);
//        }
//        Small.IsLife = false;
//        Big.CompanyRefreshChessBoard();
//    }
//}

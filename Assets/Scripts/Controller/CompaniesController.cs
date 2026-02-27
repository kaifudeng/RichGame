using Mirror;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Text;
using System.Xml;
using System.ComponentModel;
using Unity.VisualScripting;
using static Mirror.BouncyCastle.Math.EC.ECCurve;
using System.Linq;

public class CompaniesController : NetworkBehaviour
{
    public GameObject CreateCompanyWindowPrefab;

    /// <summary>
    /// 公司股价分红等信息的计算规则 
    /// </summary>
    List<computeRule> computeRules;
    /// <summary>
    /// 公司股价分红等信息的计算规则 字符串格式
    /// </summary>
    [SyncVar(hook =nameof(computeRulesChange))]
    string[] computeRulesStrs;

    /// <summary>
    /// 公司信息的xml文件路径
    /// </summary>
    string xmlConfigPath;
    /// <summary>
    /// 所有公司的集合
    /// </summary>
    public List<Company> companies;
    /// <summary>
    /// 所有公司的集合 字符串格式
    /// </summary>
    [SyncVar(hook =nameof(CompaniesChange))]
    public string[] companiesStrs;
    [Client]
    public void CompaniesChange(string[] old, string[] newval)
    {

        if (companies == null || companies.Count == 0)
        {
            companies = new List<Company>();
            foreach (string str2 in newval)
            {
                string[] strs2 = str2.Split(',');
                bool isSafe = (ConvertFuntion.StringToInt(strs2[2]) == 1) ? true : false;
                int remainStock = Convert.ToInt32(strs2[3]);
                int thePrice = Convert.ToInt32(strs2[4]);
                bool IsLife = (ConvertFuntion.StringToInt(strs2[5]) == 1) ? true : false;
                int useMaterialindex = Convert.ToInt32(strs2[6]);
                int hasCardCount = Convert.ToInt32(strs2[7]);
                {
                    Company company = new Company(str2, isSafe, remainStock, thePrice, IsLife, useMaterialindex, hasCardCount);
                    companies.Add(company);

                }
            }
        }
        else
        {
            foreach (string str2 in newval)
            {
                string[] strs2 = str2.Split(',');
                bool isSafe = (ConvertFuntion.StringToInt(strs2[2]) == 1) ? true : false;
                int remainStock = Convert.ToInt32(strs2[3]);
                int thePrice = Convert.ToInt32(strs2[4]);
                bool IsLife = (ConvertFuntion.StringToInt(strs2[5]) == 1) ? true : false;
                int useMaterialindex = Convert.ToInt32(strs2[6]);
                int hasCardCount = Convert.ToInt32(strs2[7]);
                for (int i = 0; i < companies.Count; i++) //当companies里已经有数据时 只能通过这种方式更新 整个重构会丢失HasCard数组的数据
                {
                    if (str2.Contains(companies[i].CompanyName))
                    {
                        string[] strings = str2.Split(',');
                        companies[i].CompanyType = strings[0][0];
                        companies[i].CompanyName = strings[1];
                        companies[i].IsSafe = isSafe;
                        companies[i].RemainStock = remainStock;
                        companies[i].price = thePrice;
                        companies[i].IsLife = IsLife;
                        companies[i].HasCardCount = hasCardCount;
                        companies[i].useMaterialIndex = useMaterialindex;
                    }
                }
            }
        
        }
    }

    [Client]
    public void computeRulesChange(string[] old, string[] newval)
    {
        computeRules = new List<computeRule>();
        if (computeRules == null || computeRules.Count == 0)
        {
            foreach (string str in newval)
            {
                computeRule rule = new computeRule(str);
                computeRules.Add(rule);
            }
        }
        else
        {

        }
    }

    public GameObject companiesInfoUI;

    public CompaniesUI companiesUI;

    /// <summary>
    /// 需要同步到UI界面的已上市公司列表
    /// </summary>
    [SyncVar(hook = nameof(AliveCompaniesChange))]
    public string[] aliveCompanies;

    public event Action<string[]> OnAliveCompaniesChange;

    CardsController cardsController;
    GameProcess gameProcessManager;
    private void AliveCompaniesChange(string[] oldVal, string[] newVal)
    {
        OnAliveCompaniesChange?.Invoke(newVal);
    }

    private void Awake()
    {
        xmlConfigPath = Application.streamingAssetsPath ;
    }
    /// <summary>
    /// 初始化公司管理器
    /// </summary>
    internal void InitCompaniesSystem()
    {
        try
        {
            //SaveCompaniesRules();
            GetCompaniesRules();
            GetCompaniesInfo();

            UpdateCompanyStockPrice();
            InitCompaniesInfoUI();

            SyncAllCompanies();

            companiesUI = companiesInfoUI.GetComponent<CompaniesUI>();
            OnAliveCompaniesChange = companiesUI.OnCompaniesChange;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    /// <summary>
    /// 获取剩余股票
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    internal int GetRemainStockSum(Company company)
    {
        foreach (Company comp in companies)
        {
            if (comp.CompanyName == company.CompanyName)
            {
                return comp.RemainStock;
            }
        }
        return 0;
    }
    /// <summary>
    /// 是否所有公司都已上市
    /// </summary>
    /// <returns></returns>
    public bool IsAllCompaniesLife()
    {
        if (companies.Count == aliveCompanies.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    /// <summary>
    /// 指定公司的股票增加或减少
    /// </summary>
    /// <param name="company"></param>
    /// <param name="sum"></param>
    public void CompanyRemindStockAdd(Company company, int sum)
    {
        for (int i = 0; i < companies.Count; i++) 
        {
            if (company== companies[i]) 
            {
                companies[i].RemainStock += sum;
                SyncAllCompanies();
                break;
            }
        }
    }

    /// <summary>
    /// 将Companies中的数据同步到字符串数组中 以将数据同步到所有客户端
    /// </summary>
    public void SyncAllCompanies()
    {
        int i = 0;
        string[] newStrs = new string[companies.Count];
        foreach (Company company in companies) 
        {
            newStrs[i]=company.ToLongString();
            i++;
        }
        companiesStrs = newStrs;
    }
    /// <summary>
    /// 将CompuRule中的数据同步到字符串数组中 以将数据同步到所有客户端
    /// </summary>
    public void SyncComputeRules()
    {
        int i = 0;
        string[] newStrs = new string[computeRules.Count];
        foreach (computeRule computeRule in computeRules)
        {
            newStrs[i] = computeRule.ToString();
            i++;
        }
        computeRulesStrs = newStrs;
    }
    /// <summary>
    /// 查找公司
    /// </summary>
    /// <param name="companyName"></param>
    /// <returns></returns>
    public Company FindCompany(string theCompany)
    {
        string[] strs=theCompany.Split(',');
        string compName=strs[1];

        foreach(Company company in companies)
        {
            if(company.CompanyName == compName)
                return company;
        }
        return null;
    }
    /// <summary>
    /// 根据公司名称查找公司
    /// </summary>
    /// <param name="theCompanyName"></param>
    /// <returns></returns>
    public Company FindCompanyWithName(string theCompanyName)
    {
        
        foreach (Company company in companies)
        {
            if (company.CompanyName == theCompanyName)
                return company;
        }
        return null;
    }
    /// <summary>
    /// 将公司持有卡牌列表进行更新 
    /// </summary>
    /// <param name="company"></param>
    /// <param name="cards"></param>
    [Server]
    public void UpadateCompanyHasCard(Company company,List<CardController> cards)
    {
        
        for (int i = 0; i < companies.Count; i++)
        {
            if (companies[i].ToString() == company.ToString())
            {
                companies[i].ChangeCard(cards);
                if (cards.Count == 0)
                {
                    companies[i].IsLife=false;
                }
                else
                {
                    companies[i].IsLife = true;
                }
                SyncAllCompanies();
                break;
            }
        }
        UpdateCompaniesUI();
    }
    /// <summary>
    /// 获取指定公司的股票价格
    /// </summary>
    /// <param name="theCompany"></param>
    /// <returns></returns>
    public int GetPrice(Company theCompany)
    {
        int price = 0;
        foreach(Company company in companies)
        {
            if (company.ToString() == theCompany.ToString())
            {
                foreach(computeRule rule in computeRules)
                {
                    if (rule.companyLevel == company.CompanyType.ToString())
                    {
                        price = rule.getStockPrice(company.GetHasCardsCount());
                    }
                }
            }
         }
        return price;
    }
    /// <summary>
    /// 更新所有公司的股票价格 同步到各个客户端
    /// </summary>
    [Server]
    public void UpdateCompanyStockPrice()
    {
        try
        {
            for (int i = 0; i < companies.Count; i++)
            {
                foreach (computeRule rule in computeRules)
                {
                    if (rule.companyLevel == companies[i].CompanyType.ToString())
                    {
                        companies[i].price = rule.getStockPrice(companies[i].GetHasCardsCount());
                    }
                }
            }
            SyncAllCompanies();
        }
        catch (Exception ex) 
        {
            Debug.LogError("更新股票价格时出错："+ex);
        }
    }
    /// <summary>
    /// 加载公司信息的UI界面
    /// </summary>
    [Server]
    public void InitCompaniesInfoUI()
    {
        Debug.Log("加载公司信息的UI界面");
        companiesInfoUI = Instantiate(companiesInfoUI);
        NetworkServer.Spawn(companiesInfoUI);
        Debug.Log("加载完成！");
    }
    /// <summary>
    /// 创建新公司
    /// </summary>
    [Server]
    public Company CreateNewCompany(string theCompany)
    {
        
        for (int i = 0; i < companies.Count; i++) 
        {
            if (companies[i].ToString().Contains(theCompany))
            {
                companies[i].IsLife=true;
                //cardsController.AfterCreateCompany(companies[i]);
                SyncAllCompanies();
                UpdateCompaniesUI();
                return companies[i];
                
            }
        }
        return null;
        
        
    }
    /// <summary>
    /// 更新当前上市公司的UI界面
    /// </summary>
    [Command(requiresAuthority = false)]
    public void UpdateCompaniesUI()
    {
        aliveCompanies = GetAliveCompanies();
    }
    [Server]
    /// <summary>
    /// 获取已上市公司的String列表
    /// </summary>
    /// <returns></returns>
    public string[] GetAliveCompanies()
    {
        List<string> comps = new List<string>();
        foreach (Company company in companies)
        {
            if (company.IsLife != false)
            {
                company.price=GetPrice(company);

                comps.Add(company.ToLongString());
            }
        }
        return comps.ToArray();
    }

    ///// <summary>
    ///// 公司并购 需要区分四种情况 1.正常并购 A吞并B并结算；2.A=B 需要由玩家确定保留哪家公司；3.AB均为安全公司，不作任何操作；
    ///// </summary>
    //[Server]
    //public Company CompaniesBattle(List<Company> companiesList)
    //{
    //    bool theBigOneisSame = false;
    //    Company theBigOne = null;
    //    foreach (Company company in companiesList) 
    //    {
    //        if (theBigOne == null)
    //        {
    //            theBigOne = company;
    //        }
    //        else
    //        {
    //            if (theBigOne.IsSafe && company.IsSafe)//并购双方均为安全公司 此并购无效
    //            {
    //                return null;
    //            }
    //            if (company.GetHasCardsCount() == theBigOne.GetHasCardsCount())
    //            {
    //                theBigOneisSame = true;
    //            }
    //            else if (company.GetHasCardsCount() > theBigOne.GetHasCardsCount())
    //            {
    //                theBigOne = company;
    //                theBigOneisSame=false;
    //            }
    //        }
    //    }
    //    if (!theBigOneisSame) //第一种情况 正常并购
    //    {
    //        List<CardController> newCards= new List<CardController>();
    //        foreach (Company company in companiesList) 
    //        {
    //            //CardsMove(theBigOne, company);UpadateCompanyHasCard
    //            newCards.AddRange(company.GetCards());

    //            UpadateCompanyHasCard(company, new List<CardController>());
    //            //找到倒闭公司的所有股东 
    //            List<Player> players=FindCustomerWithCompany(company,playersManager);
    //            foreach (Player player in players)
    //            {
    //                //string companyName=company.CompanyName;
    //                //player.TargetRpcGetStockSelectForm(company.ToString());
    //            }
    //        }
    //        UpadateCompanyHasCard(theBigOne, newCards);
    //        cardsController.AddCardToCompany(theBigOne);
    //        return theBigOne;
    //    }
    //    else //第二种情况 两家实力相同
    //    {
    //        return null;
    //    }
    //}
    /// <summary>
    /// 寻找当前公司的所有股东
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    //[Server]
    //List<Player> FindCustomerWithCompany(Company company,PlayersManager playersManager)
    //{
    //    List<Player> players = new List<Player>();
    //    foreach (Player player in playersManager.GetPlayers())
    //    {
    //        if (player.stocks != null && player.stocks.Count > 0) 
    //        {
    //            foreach (Stock stock in player.stocks) 
    //            {
    //                if(stock.Company.CompanyName == company.CompanyName)
    //                {
    //                    players.Add(player);
    //                }
    //            }
    //        }
    //    }
    //    return players;
    //}
    /// <summary>
    /// 找到规模最大的那几个公司 
    /// </summary>
    /// <param name="companyList"></param>
    /// <returns></returns>
    public List<Company> FindtheMaxCompany(List<Company> companyList) 
    {
        List<Company> newList= new List<Company>();
        int Maxcount = companyList[0].HasCardCount;//先找出最大的那个
        foreach (Company company in companyList)
        {
            if (company.HasCardCount >= Maxcount)
            {
                Maxcount = company.HasCardCount;
            }
        }
        foreach (Company company in companyList) 
        {
            if (company.HasCardCount == Maxcount)
            {
                newList.Add(company);
            }
        }
        return newList;
    }

    /// <summary>
    /// 找到所有安全公司 
    /// </summary>
    /// <param name="companyList"></param>
    /// <returns></returns>
    public List<Company> FindtheSafeCompany(List<Company> companyList)
    {
        List<Company> newList = new List<Company>();
        
        foreach (Company company in companyList)
        {
            if (company.IsSafe)
            {
                newList.Add(company);
            }
        }
        return newList;
    }
    /// <summary>
    /// 公司并购一系列其他公司的方法
    /// </summary>
    /// <param name="company"></param>
    /// <param name="companies"></param>
    public void CompanyEatCompanies(Company company, List<Company> outCompanies)
    {
        List<CardController> newCards = new List<CardController>();
        foreach (Company theComp in outCompanies)
        {
            newCards.AddRange(theComp.GetCards());
            CompanyOut(theComp);
        }
        for (int i = 0; i < companies.Count; i++)
        {
            if (companies[i].CompanyName == company.CompanyName)
            {
                companies[i].AddCards(newCards);
                break;
            }
            else
            {
                continue;
            }
        }
    }
    /// <summary>
    /// 公司倒闭
    /// </summary>
    public void CompanyOut(Company company)
    {
        for (int i = 0; i < companies.Count; i++)
        {
            if (company.CompanyName == companies[i].CompanyName)
            {
                companies[i].IsLife = false;

                companies[i].ChangeCard(new List<CardController>());
                break;
            }
        }
    }

    /// <summary>
    /// 根据公司状态获取公司列表
    /// </summary>
    /// <param name="state">上市状态</param>
    /// <returns></returns>
    public List<Company> GetAllCompaniesByState(bool state)
    {
        List<Company> list = new List<Company>();

        foreach (Company company in companies)
        {
            if (company.IsLife == state)
            {
                list.Add(company);
            }
        }
        return list;
    }
    /// <summary>
    /// 将List对象转为string
    /// </summary>
    /// <param name="companiesList"></param>
    /// <returns></returns>
    public string CompaniesListToString(List<Company> companiesList) 
    {
        
        string companiesStrs = "";
        foreach (Company company in companiesList)
        {
            companiesStrs += company.CompanyType + "," + company.CompanyName + "," + company.price
                + "," + company.HasCardCount + ","+company.RemainStock+"|";
        }
        return companiesStrs;
    }
    /// <summary>
    /// 将company对象转为string
    /// </summary>
    /// <param name="companiesList"></param>
    /// <returns></returns>
    public string CompanyToString(Company company)
    {
        return company.CompanyType + "," + company.CompanyName + "," + company.price
                + "," + company.HasCardCount + "," + company.RemainStock + "|";
    }
    /// <summary>
    /// 获取某公司第一股东的分红金额
    /// </summary>
    /// <returns></returns>
    public int GetCompanyDividendFirst(Company company)
    {
        foreach(Company theComp in companies)
        {
            if (theComp.Equals(company))
            {
                foreach(computeRule computeRule in computeRules)
                {
                    if(computeRule.companyLevel== company.CompanyType.ToString())
                    {
                        return computeRule.getDividendFirst(company.HasCardCount);
                    }
                }
            }
        }
        return 0;
    }
    /// <summary>
    /// 获取某公司第二股东的分红金额
    /// </summary>
    /// <returns></returns>
    public int GetCompanyDividendSecond(Company company)
    {
        foreach (Company theComp in companies)
        {
            if (theComp.Equals(company))
            {
                foreach (computeRule computeRule in computeRules)
                {
                    if (computeRule.companyLevel == company.CompanyType.ToString())
                    {
                        return computeRule.getDividendSecond(company.HasCardCount);
                    }
                }
            }
        }
        return 0;
    }
    /// <summary>
    /// 获取某公司第三股东的分红金额
    /// </summary>
    /// <returns></returns>
    public int GetCompanyDividendThird(Company company)
    {
        foreach (Company theComp in companies)
        {
            if (theComp.Equals(company))
            {
                foreach (computeRule computeRule in computeRules)
                {
                    if (computeRule.companyLevel == company.CompanyType.ToString())
                    {
                        return computeRule.getDividendThird(company.HasCardCount);
                    }
                }
            }
        }
        return 0;
    }
    /// <summary>
    /// 对该公司的股东按持股数量进行排序
    /// </summary>
    public void RankingPlayerWithCompany(Company company,PlayersManager playersManager)
    {
        for (int i = 0; i < companies.Count; i++) 
        {
            if (companies[i].Equals(company))
            {
                companies[i].PrintPlayerList();

                companies[i].PlayerList = playersManager.GetPlayers()
                    .Where(Stock => Stock.GetStockSum(company)>0)
                    .OrderByDescending(Stock => Stock.GetStockSum(company)).ToList();

                //Linq排序测试
                companies[i].PrintPlayerList();
                    break;
            }
        }
        
    }

    public void AddPlayerToCompanyList(Player player,Company company)
    {
        for(int i = 0;i < companies.Count; i++)
        {
            if (companies[i].Equals(company))
            {
                companies[i].PlayerListAdd(player);break;
            }
        }
    }



    ///// <summary>
    ///// 持有卡片进行移动
    ///// </summary>
    //public void CardsMove(Company parCom,Company sonCom)
    //{
    //    for (int i = 0; i < companies.Count; i++) 
    //    {
    //        if (companies[i].CompanyName == sonCom.CompanyName)
    //        {
    //            parCom.AddCards(sonCom.GetCards());
    //            companies[i].ChangeCard(null);
    //        }
    //    }
    //    for (int i = 0;i < companies.Count; i++)
    //    {
    //        if (companies[i].CompanyName == parCom.CompanyName)
    //        {
    //            companies[i] = parCom;
    //        }
    //    }
    //    //SyncAllCompanies();
    //}

    #region 加载配置的方法

    /// <summary>
    /// 读取公司的基础数据
    /// </summary>
    //[Server]
    //private void GetCompaniesInfo()
    //{
    //    try
    //    {
    //        companies = new List<Company>();
    //        string StrPath = xmlConfigPath + "\\companies.xml";
    //        string Astr = File.ReadAllText(StrPath, Encoding.GetEncoding("gb2312"));
    //        DataTable Atbl = XmlToDataTable(Astr);
    //        foreach (DataRow row in Atbl.Rows)
    //        {
    //            //CompanyType + "," + CompanyName + "," + IsSafe + "," + RemainStock + "," + price + "," + IsLife + "," + useMaterialIndex;
    //            string str = row["CompanyType"] + "," + row["CompanyName"].ToString() + "," + row["useMaterialIndex"].ToString();
    //            Company company = new Company(str);
    //            companies.Add(company);
    //        }
    //        aliveCompanies = GetAliveCompanies();
    //        SyncAllCompanies();
    //    }catch(Exception e)
    //    {
    //        Debug.LogError($"加载公司信息时出错{e}");
    //    }
    //}
    [Server]
    private void GetCompaniesInfo()
    {
        try
        {
            companies = new List<Company>();

            Debug.Log("正在获取公司信息………………");
            string fileName = "companies";

            XMLManagerForAndroid xMLManagerForAndroid = new XMLManagerForAndroid();
            xMLManagerForAndroid.fileName = fileName;
            xMLManagerForAndroid.pathType = XMLManagerForAndroid.PathType.PersistentDataPath;
            DataTable Atbl = xMLManagerForAndroid.ReadXmlToDataTable();

            foreach (DataRow row in Atbl.Rows)
            {
                // CompanyType + "," + CompanyName + "," + IsSafe + "," + RemainStock + "," + price + "," + IsLife + "," + useMaterialIndex;
                string str = row["CompanyType"] + "," + row["CompanyName"].ToString() + "," + row["useMaterialIndex"].ToString();
                Company company = new Company(str);
                companies.Add(company);
            }

            Debug.Log("获取公司信息成功！");
            aliveCompanies = GetAliveCompanies();
            SyncAllCompanies();
        }
        catch (Exception e)
        {
            Debug.LogError($"加载公司信息时出错: {e.Message}\n堆栈: {e.StackTrace}");
        }
    }

    // 新增方法：安全获取GB2312编码
    private Encoding GetGB2312Encoding()
    {
        try
        {
            // 方法1：尝试直接获取GB2312
            return Encoding.GetEncoding("gb2312");
        }
        catch
        {
            try
            {
                // 方法2：使用代码页936
                return Encoding.GetEncoding(936);
            }
            catch
            {
                // 方法3：如果都没有，使用UTF-8（大多数情况下能兼容中文）
                Debug.LogWarning("GB2312编码不可用，使用UTF-8替代");
                return Encoding.UTF8;
            }
        }
    }

    [Server]
    /// <summary>
    /// 将公司信息保存到XML中
    /// </summary>
    private void SaveCompaniesInfo()
    {
        DataTable Atb = new DataTable("companies");
        Atb.Columns.Add("CompanyName");
        Atb.Columns.Add("CompanyType");
        Atb.Columns.Add("HasCards");
        Atb.Columns.Add("IsSafe");
        Atb.Columns.Add("RemainStock");
        Atb.Columns.Add("price");
        Atb.Columns.Add("IsLife");
        Atb.Columns.Add("useMaterialIndex");
        DataRow Adr = Atb.NewRow();
        Adr["CompanyName"] = "苹果";
        Adr["CompanyType"] = "A";
        Adr["HasCards"] = "";
        Adr["IsSafe"] = "0";
        Adr["RemainStock"] = "30";
        Adr["price"] = "800";
        Adr["IsLife"] = "1";
        Adr["useMaterialIndex"] = "4";
        Atb.Rows.Add(Adr);

        DataRow Adr2 = Atb.NewRow();
        Adr2["CompanyName"] = "华为";
        Adr2["CompanyType"] = "B";
        Adr2["HasCards"] = "";
        Adr2["IsSafe"] = "0";
        Adr2["RemainStock"] = "30";
        Adr2["price"] = "800";
        Adr2["IsLife"] = "1";
        Adr2["useMaterialIndex"] = "5";
        Atb.Rows.Add(Adr2);
        string Astr = DataTable2Xml(Atb);
        string StrPath = xmlConfigPath;
        File.WriteAllText(StrPath, Astr, Encoding.GetEncoding("gb2312"));//不存在该XML文件时会自动生成一个文件
    }

    //[Server]
    ///// <summary>
    ///// 读取公司相关计算规则
    ///// </summary>
    //private void GetCompaniesRules()
    //{
    //    try
    //    {
    //        computeRules = new List<computeRule>();
    //        string StrPath = xmlConfigPath + "\\computeRules.xml";
    //        string Astr = File.ReadAllText(StrPath, Encoding.GetEncoding("gb2312"));
    //        DataTable Atbl = XmlToDataTable(Astr);
    //        foreach (DataRow row in Atbl.Rows)
    //        {
    //            computeRule rule = new computeRule(row["公司等级"].ToString(), Convert.ToInt32(row["初始股价"]), Convert.ToInt32(row["每多少个地皮升级"]),
    //                Convert.ToInt32(row["股价每级增长"]), Convert.ToInt32(row["分红_第一股东"]), Convert.ToInt32(row["分红_第二股东"]), Convert.ToInt32(row["分红_第三股东"])
    //                , Convert.ToInt32(row["分红每级增长"]));
    //            computeRules.Add(rule);
    //        }
    //        SyncComputeRules();
    //    }catch(Exception e)
    //    {
    //        Debug.LogError($"加载公司规则信息时出错：{e}");
    //    }
    //}
    [Server]
    private void GetCompaniesRules()
    {
        try
        {
            Debug.Log("正在获取规则信息………………");
            computeRules = new List<computeRule>();
            string fileName = "computeRules";

            XMLManagerForAndroid xMLManagerForAndroid=new XMLManagerForAndroid();
            xMLManagerForAndroid.fileName = fileName;
            xMLManagerForAndroid.pathType = XMLManagerForAndroid.PathType.PersistentDataPath;
            DataTable Atbl = xMLManagerForAndroid.ReadXmlToDataTable();

            foreach (DataRow row in Atbl.Rows)
            {
                // 添加数据验证，避免空值或格式错误
                computeRule rule = new computeRule(
                    SafeGetString(row, "公司等级", ""),
                    SafeGetInt(row, "初始股价", 0),
                    SafeGetInt(row, "每多少个地皮升级", 1),
                    SafeGetInt(row, "股价每级增长", 0),
                    SafeGetInt(row, "分红_第一股东", 0),
                    SafeGetInt(row, "分红_第二股东", 0),
                    SafeGetInt(row, "分红_第三股东", 0),
                    SafeGetInt(row, "分红每级增长", 0)
                );
                computeRules.Add(rule);
            }
            Debug.Log("获取规则信息成功！");
            SyncComputeRules();
        }
        catch (Exception e)
        {
            Debug.LogError($"加载公司规则信息时出错：{e.Message}\n堆栈: {e.StackTrace}");
        }
    }

    // 修改5：添加安全获取字符串的方法
    private string SafeGetString(DataRow row, string columnName, string defaultValue = "")
    {
        if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
        {
            return row[columnName].ToString();
        }
        Debug.LogWarning($"列 '{columnName}' 不存在或为空，使用默认值 '{defaultValue}'");
        return defaultValue;
    }

    // 修改6：添加安全获取整数的方法
    private int SafeGetInt(DataRow row, string columnName, int defaultValue = 0)
    {
        if (row.Table.Columns.Contains(columnName) && row[columnName] != DBNull.Value)
        {
            string value = row[columnName].ToString().Trim();
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            Debug.LogWarning($"列 '{columnName}' 的值 '{value}' 无法转换为整数，使用默认值 {defaultValue}");
        }
        else
        {
            Debug.LogWarning($"列 '{columnName}' 不存在或为空，使用默认值 {defaultValue}");
        }
        return defaultValue;
    }

   

    [Server]
    /// <summary>
    /// 保存计算规则 测试用 后续改成可以在游戏内更改？
    /// </summary>
    private void SaveCompaniesRules()
    {
        DataTable dataTable = new DataTable("computeRules");
        dataTable.Columns.Add("公司等级");
        dataTable.Columns.Add("初始股价");
        dataTable.Columns.Add("股价每级增长");
        dataTable.Columns.Add("分红_第一股东");
        dataTable.Columns.Add("分红_第二股东");
        dataTable.Columns.Add("分红_第三股东");
        dataTable.Columns.Add("分红每级增长");
        dataTable.Columns.Add("每多少个地皮升级");

        DataRow row = dataTable.NewRow();
        row["公司等级"] = "A";
        row["初始股价"] = "800";
        row["股价每级增长"] = "400";
        row["分红_第一股东"] = "2000";
        row["分红_第二股东"] = "1000";
        row["分红_第三股东"] = "500";
        row["分红每级增长"] = "500";
        row["每多少个地皮升级"] = "3";
        DataRow row1 = dataTable.NewRow();
        row1["公司等级"] = "B";
        row1["初始股价"] = "600";
        row1["股价每级增长"] = "300";
        row1["分红_第一股东"] = "1500";
        row1["分红_第二股东"] = "800";
        row1["分红_第三股东"] = "400";
        row1["分红每级增长"] = "400";
        row1["每多少个地皮升级"] = "3";
        DataRow row2 = dataTable.NewRow();
        row2["公司等级"] = "C";
        row2["初始股价"] = "400";
        row2["股价每级增长"] = "200";
        row2["分红_第一股东"] = "1000";
        row2["分红_第二股东"] = "500";
        row2["分红_第三股东"] = "300";
        row2["分红每级增长"] = "300";
        row2["每多少个地皮升级"] = "3";

        dataTable.Rows.Add(row);
        dataTable.Rows.Add(row1);
        dataTable.Rows.Add(row2);
        string Astr = DataTable2Xml(dataTable);
        string StrPath = xmlConfigPath+ "//computeRules.xml";
        File.WriteAllText(StrPath, Astr, Encoding.GetEncoding("gb2312"));//不存在该XML文件时会自动生成一个文件
    }
    #endregion
    #region xml与DataTable相互转化的方法
    /// <summary>
    /// 将XML生成DataTable
    /// </summary>
    /// <param name="xmlStr">XML字符串</param>
    /// <returns></returns>
    public static DataTable XmlToDataTable(string xmlStr)
    {
        if (!string.IsNullOrEmpty(xmlStr))
        {
            StringReader StrStream = null;
            XmlTextReader Xmlrdr = null;
            try
            {
                DataSet ds = new DataSet();
                //读取字符串中的信息
                StrStream = new StringReader(xmlStr);
                //获取StrStream中的数据
                Xmlrdr = new XmlTextReader(StrStream);
                //ds获取Xmlrdr中的数据               
                ds.ReadXml(Xmlrdr);
                return ds.Tables[0];
            }
            catch
            {
                return null;
            }
            finally
            {
                //释放资源
                if (Xmlrdr != null)
                {
                    Xmlrdr.Close();
                    StrStream.Close();
                    StrStream.Dispose();
                }
            }
        }
        return null;
    }


    /// <summary>
    /// 将datatable转为xml
    /// </summary>
    /// <param name="vTable">要生成XML的DataTable</param>
    /// <returns></returns>
    public static string DataTable2Xml(DataTable vTable)
    {
        try
        {
            if (null == vTable) return string.Empty;
            StringWriter writer = new StringWriter();
            vTable.WriteXml(writer);
            string xmlstr = writer.ToString();
            writer.Close();
            return xmlstr;
        }
        catch
        {
            return string.Empty;
        }
    }
   
    #endregion
}

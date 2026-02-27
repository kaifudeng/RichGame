//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class WindowUI : NetworkBehaviour
//{
//    /// <summary>
//    /// 选中的公司
//    /// </summary>
//    public GameObject selectCompany;
//    public Button enterBtn;
//    public GameObject companysInfo;
//    public GameObject companyPrefab;
//    public List<Company> CompanyList;
//    public List<Material> materialList;
//    CompaniesManager companiesManager;

//    public PlayerController playerController;
//    // Start is called before the first frame update
//    void Start()
//    {
//        companiesManager=CompaniesManager.GetCompaniesManager();
//        enterBtn.enabled = false;
//        ShowCompany();
//        //foreach (var comp in CompanyList)
//        //{
//        //    GameObject company = Instantiate(companyPrefab);
//        //    CompanyUI companyComponent = company.GetComponent<CompanyUI>();
//        //    companyComponent.companyController = comp;
//        //    companyComponent.companyName = comp.CompanyName;
//        //    companyComponent.otherInfo = comp.Price.ToString()+comp.HasCardCount;
//        //    company.transform.SetParent(companysInfo.transform, false);
//        //}
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    public void OnExitButtonClick()
//    {
//        Destroy(gameObject);
//    }
//    [Client]
//    public void OnSelectCompanyChange(GameObject oldObj,GameObject newObj)
//    {
//        if (newObj == null)
//        {
//            enterBtn.enabled = false;
//        }
//        else
//        {
//            enterBtn.enabled = true;
//        }
//    }

//    public void OnSubmitButtonClick()
//    {
        

//        CompanyController selectCom = companiesManager.companies[0].GetComponent<CompanyController>();
//        CompanyUI companyUI=selectCompany.GetComponent<CompanyUI>();
//        foreach (GameObject obj in companiesManager.companies) 
//        {
//            CompanyController comp= obj.GetComponent<CompanyController>();
//            if (comp.CompanyName == companyUI.companyName)
//            {
//                selectCom=comp;
//            }
//        }

//        playerController.CmdCreateCompany(selectCom.CompanyName);
//        playerController.CmdSendPLayerMessage(playerController.PlayerName + "创建公司：" + companyUI.companyName + "\n");
//        //selectCom.IsLife = true;
//        //playerController.PlayerStockChange(selectCom, 1);
//        playerController.CompanyStockChange(selectCom.CompanyName, -1);

//        //int i = 0;
//        //string[] newstr = new string[playerController.stocks.Count];
//        //for (i = 0; i < playerController.stocks.Count; i++)
//        //{
//        //    newstr[i] = playerController.stocks[i].Item1.CompanyName + "," + playerController.stocks[i].Item2;
//        //}

//        //playerController.UpadateStocksStr(newstr);
//        List<string> lists = new List<string>();
//        if (playerController.StocksToStrings()!=null)
//        { 
//        lists = new List<string>(playerController.StocksToStrings());
        
        
//            bool getIt = false;
//            for (int i = 0; i < lists.Count; i++)
//            {
//                string[] strs = lists[i].Split(',');
//                if (strs[0] == selectCom.CompanyName)
//                {
//                    getIt = true;
//                    lists[0] = strs[0] + "," + (ConvertFuntion.StringToInt(strs[1]) + 1);
//                }
//            }
//            if (!getIt)
//            {
//                lists.Add(selectCom.CompanyName + "," + 1);
//            }
//        }
//        else
//        {
//            lists.Add(selectCom.CompanyName + "," + 1);
//        }
//        playerController.UpadateStocksStr(lists.ToArray());
//        Destroy(gameObject);

//        playerController.OpenBuyStockWindow(selectCom.CompanyName);
//    }

//    //public void ShowWindow(bool state)
//    //{
//    //    if (state)
//    //    {
//    //        gameObject.SetActive(true);
//    //        ShowCompany();
//    //    }
//    //    else
//    //    {
//    //        gameObject.SetActive(false);
//    //    }
//    //}
//    [Client]
//    public void ShowCompany()
//    {
//        foreach (Transform child in companysInfo.transform)
//        {
//            Destroy(child.gameObject);
//        }

//        if (playerController != null) 
//        {
//            foreach (GameObject obj in companiesManager.companies)
//            {
//                CompanyController comp= obj.GetComponent<CompanyController>();
//                if (!comp.IsLife)
//                {
//                    GameObject company = Instantiate(companyPrefab);
//                    CompanyUI companyComponent = company.GetComponent<CompanyUI>();
//                    //companyComponent.companyController = comp;
                    
//                    //companyComponent.companyName = comp.CompanyName;
//                    //companyComponent.otherInfo = comp.Price.ToString() + comp.HasCardCount;
//                    //companyComponent.InitCompanyInfo(comp);
//                    company.transform.SetParent(companysInfo.transform, false);
//                }
//            }
//        }
//    }
//}

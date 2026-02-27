//using Mirror;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

//public class CompaniesBoardUI : NetworkBehaviour
//{
//    static CompaniesBoardUI instance;
//    public static CompaniesBoardUI GetCompaniesBoardUI() { return instance; }

//    public TextMeshProUGUI textMeshProUGUI;
//    [SyncVar(hook = nameof(InfoChange))]
//    public int info;

//    private void InfoChange(int old, int newVal)
//    {
//        UpdateCompaniesInfo();
//    }

//    // Start is called before the first frame update
//    void Start()
//    {
//        instance = this;
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//    //[ClientRpc]
//    /// <summary>
//    /// 更新公司实时状态
//    /// </summary>
//    public void UpdateCompaniesInfo()
//    {
//        textMeshProUGUI.text = "已上市公司股价一览：\n";
//        CompaniesManager companiesManager=CompaniesManager.GetCompaniesManager();
//        int count = 1;
//        List<CompanyController> coms = new List<CompanyController>();
//        foreach (GameObject gameObject in companiesManager.companies) 
//        {
//            CompanyController companyController = gameObject.GetComponent<CompanyController>();
//            if (companyController != null && companyController.IsLife) 
//            {
//                coms.Add(companyController);
//            }
//        }
//        coms.Sort();

//        foreach (CompanyController companyController in coms) 
//        {
//            textMeshProUGUI.text += count.ToString() + ":" + companyController.CompanyName + " 实时股价：" + companyController.Price + "元 "
//                +"持有地皮数量："+ companyController.HasCards.Length.ToString()+"\n";
//            count++;
//        }
//    }
//}

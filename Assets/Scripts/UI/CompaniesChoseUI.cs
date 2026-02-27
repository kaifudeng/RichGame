//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

//public class CompaniesChoseUI : NetworkBehaviour
//{
//    [SyncVar(hook =nameof(OnButtonATextChange))]
//    public string ButtonAText;
//    [SyncVar(hook = nameof(OnButtonBTextChange))]
//    public string ButtonBText;

//    public string card;
//    public string selectVal;
//    public TextMeshProUGUI TextMeshProUGUIBtnA;
//    public TextMeshProUGUI TextMeshProUGUIBtnB;
//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }

//    void OnButtonATextChange(string oldVal,string newVal)
//    {
//        TextMeshProUGUIBtnA.text= newVal;
//    }
//    void OnButtonBTextChange(string oldVal, string newVal)
//    {
//        TextMeshProUGUIBtnB.text = newVal;
//    }

//    public void ReturnPlayerChose(TextMeshProUGUI text)
//    {
//        selectVal = text.text;

//        CompaniesManager companiesManager=CompaniesManager.GetCompaniesManager();
//        if (text == TextMeshProUGUIBtnA)
//        {
//            CompanyController comA = companiesManager.FindCompanyController(selectVal);
//            CompanyController comB = companiesManager.FindCompanyController(TextMeshProUGUIBtnB.text);
//            companiesManager.CompanySmallToBig(comA, comB);
//            companiesManager.CompanyGetCard(comA.CompanyName, card);
//        }
//        else
//        {
//            CompanyController comA = companiesManager.FindCompanyController(selectVal);
//            CompanyController comB = companiesManager.FindCompanyController(TextMeshProUGUIBtnB.text);
//            companiesManager.CompanySmallToBig(comB, comA);
//            companiesManager.CompanyGetCard(comB.CompanyName, card);
//        }
//        Destroy(gameObject);
//    }
//}

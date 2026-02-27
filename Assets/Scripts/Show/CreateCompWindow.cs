using Mirror;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateCompWindow : MonoBehaviour
{
    /// <summary>
    /// 选中的公司
    /// </summary>
    public GameObject selectCompany;
    public Button enterBtn;
    public GameObject companysInfo;
    public GameObject companyPrefab;
    public List<Company> CompanyList;
    public List<Material> materialList;
    CompaniesController companiesController;
    string reqId;
    GameStage gameStage;
    public Player thePlayer;

    public TextMeshProUGUI titleText;

    int startType;
    string comps;
    // Start is called before the first frame update
    void Start()
    {
        
        companiesController = GameObject.Find("CompaniesController(Clone)").GetComponent<CompaniesController>();
        enterBtn.enabled = false;
        if (startType == 0)//创建公司使用这个路线
        {
            ShowCompany(companiesController.companies);
        }
        else//并购使用这个路线
        {
            ShowCompany(GetCompany(comps));
        }
    }

    /// <summary>
    /// 将序列化的companies解压出来
    /// </summary>
    /// <param name="comps"></param>
    private List<Company> GetCompany(string comps)
    {
        List<Company> list = new List<Company>();

        string[] strings=comps.TrimEnd(',').Split(',');

        foreach(Company company in companiesController.companies)
        {
            for(int i = 0; i < strings.Length; i++)
            {
                if(strings[i] == company.CompanyName)
                {
                    list.Add(company);
                }
            }
        }
        return list;
    }

    [Client]
    public void OnExitButtonClick()
    {
        //thePlayer.OverThisRound();
        if (startType == 0)
        {
            CommunicationTool.GetCommunicationTool().CmdSubmitResponse(reqId, "");
            Destroy(gameObject);
        }
        else
        {

        }
    }
    [Client]
    public void OnSubmitButtonClick()
    {
        CompanyUI compUI=selectCompany.GetComponent<CompanyUI>();
        //thePlayer.CmdCreateCompany(compUI.company.ToString());
        CommunicationTool.GetCommunicationTool().CmdSubmitResponse(reqId, compUI.company.ToString());
        Destroy(gameObject);
    }

    [Client]
    public void ShowCompany(List<Company> companies)
    {
        foreach (Transform child in companysInfo.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Company comp in companies)
        {
            if (startType == 1|| !comp.IsLife && startType == 0)
            {
                GameObject company = Instantiate(companyPrefab);
                CompanyUI companyComponent = company.GetComponent<CompanyUI>();
                companyComponent.InitCompanyInfo(comp);
                company.transform.SetParent(companysInfo.transform, false);
            }
        }

    }

    internal void InitForm(string requestId, GameStage gameStage,Player player,int startType, string comps)
    {
        reqId = requestId;
        this.gameStage = gameStage;
        thePlayer = player;
        this.startType = startType;
        this.comps = comps;
        if(startType == 1)
        {
            titleText.text = "请选择保留哪一家公司：";
        }
        else
        {
            titleText.text = "请选择要创建的公司：";
        }
    }
}

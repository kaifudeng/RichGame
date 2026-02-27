using Mirror;
using Mirror.BouncyCastle.Asn1.X9;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CompanyUI : MonoBehaviour,IPointerClickHandler
{

    public string companyName;

    public string otherInfo;

    public GameObject theShiled;

    public Company company;

    public TextMeshProUGUI companyNameUI;
    public TextMeshProUGUI otherInfoUI;

    public bool isChose=false;

    public CompaniesInfo info;

    void Start()
    {
        info = GetComponentInParent<CompaniesInfo>();
    }
    //[Client]
    //private void OnCompanyNameChange(string oldName, string newname)
    //{
        
    //}
    //[Client]
    //private void OnotherInfoChange(string oldInfo, string newInfo)
    //{
        
    //}

    public void InitCompanyInfo(Company theCompany)
    {
        company = theCompany;
        //CompanyType + "," + CompanyName + "," + IsSafe + "," + RemainStock+","+ price+","+ IsLife+","+ useMaterialIndex;
        companyNameUI.text = company.CompanyName;
        companyName= company.CompanyName;

        otherInfo = "公司类型："+company.CompanyType+"\n股票价格："+company.price.ToString()+"\n公司规模："+company.HasCardCount+"\n";
        if (company.IsSafe) 
        {
            theShiled.SetActive(true);
        }
        else
        {
            theShiled.SetActive(false);
        }
        otherInfoUI.text = otherInfo;
    }

    private void OnChoseStateChange(bool newInfo)
    {
        if (newInfo)
        {
            UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(255, 255, 255, 123);
            if (info != null)
            {
                info.SetVal( company.ToString());
                //WindowUI.selectCompany = gameObject;
                //WindowUI.enterBtn.enabled = true;
            }
        }
        else
        {
            UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
            image.color = new Color(255, 255, 255, 0);
            if (info != null)
            {
                info.SetVal(company.ToString());
                //WindowUI.selectCompany = null;
                //WindowUI.enterBtn.enabled = false;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isChose)
        {
            ExitChose();
            isChose = true;
            OnChoseStateChange(true);
        }
        else
        {
            isChose = false;
            OnChoseStateChange(false);
        }
    }

    public void IsPointerOverGameObject(PointerEventData eventData)
    {
        if (!isChose)
        {
            
        }
    }

    /// <summary>
    /// 取消上一张公司卡片的选中状态
    /// </summary>
    public void ExitChose()
    {
        GameObject[] coms = GameObject.FindGameObjectsWithTag("CompanyCard");
        foreach (GameObject game in coms)
        {
            CompanyUI companyUI = game.GetComponent<CompanyUI>();
            if(companyUI.isChose == true)
            {
                companyUI.isChose = false;
                companyUI.OnChoseStateChange(false);
            }
        }
    }

    //public void changeChoseState(bool state)
    //{
    //    if (state)
    //    {
    //        isChose = true;
    //        UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
    //        image.color = new Color(255, 255, 255, 123);

    //        GameObject[] coms= GameObject.FindGameObjectsWithTag("CompanyCard");
    //        foreach(GameObject game in coms)
    //        {
    //            CompanyUI companyUI = game.GetComponent<CompanyUI>();
    //            companyUI.changeChoseState(false);
    //        }
    //    }
    //    else
    //    {
    //        isChose = false;
    //        UnityEngine.UI.Image image = GetComponent<UnityEngine.UI.Image>();
    //        image.color = new Color(255, 255, 255, 0);
    //    }
    //}
}

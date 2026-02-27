using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompaniesUI : NetworkBehaviour
{
    public List<GameObject> companiesUI;

    /// <summary>
    /// 公司信息面板的预制件
    /// </summary>
    public GameObject companyUIPrefab;

    public GameObject companyGrid;
    
    [ClientRpc]
    /// <summary>
    /// 公司信息发生变化 同步更新UI界面
    /// </summary>
    /// <param name="strings"></param>
    public void OnCompaniesChange(string[] strings)
    {
        companiesUI=new List<GameObject>();
        RemoveAllChildren(companyGrid);
        if(strings != null)
        foreach (var str in strings)
        {
                //Debug.Log("将字符串转化为company对象---");
                string[] strs=str.Split(',');
                bool isSafe=(ConvertFuntion.StringToInt(strs[2])==1)?true:false;
                int remainStock=Convert.ToInt32(strs[3]);
                int thePrice = Convert.ToInt32(strs[4]);
                bool IsLife= (ConvertFuntion.StringToInt(strs[5]) == 1) ? true : false;
                int useMaterialindex= Convert.ToInt32(strs[6]);
                int hasCardCount= Convert.ToInt32(strs[7]);

                //Debug.Log("转化成功！");
                Company company = new Company(str,isSafe,remainStock,thePrice, IsLife, useMaterialindex, hasCardCount);
                GameObject comObj = Instantiate(companyUIPrefab);
                comObj.transform.SetParent(companyGrid.transform);
                companiesUI.Add(comObj);

                CompanyUI companyUI=comObj.GetComponent<CompanyUI>();
                companyUI.InitCompanyInfo(company);
                
        }
    }



    public static void RemoveAllChildren(GameObject parent)
    {
        Transform transform;
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            transform = parent.transform.GetChild(i);
            GameObject.Destroy(transform.gameObject);
        }
    }

}

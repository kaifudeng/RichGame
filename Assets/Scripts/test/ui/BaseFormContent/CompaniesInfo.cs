using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompaniesInfo : MonoBehaviour
{
    public GameObject companyPrefab;
    
    private string comps;
    //public string reVal;

    // public string SelectVal;
    // Start is called before the first frame update
    void Start()
    {
        ShowCompany(GetCompany(comps));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitContent(string compstr)
    {
        comps = compstr;
        //this.companiesController = companiesController;
        
    }

    /// <summary>
    /// 将序列化的companies解压出来
    /// </summary>
    /// <param name="comps"></param>
    private List<Company> GetCompany(string comps)
    {
        List<Company> list = new List<Company>();

        string[] strings = comps.TrimEnd('|').Split('|');

        for (int i = 0; i < strings.Length; i++)
        {
            string[] strs = strings[i].Split(',');
            Company company = new Company(strings[i]);
            company.price =ConvertFuntion.StringToInt(strs[2]);
            company.HasCardCount = ConvertFuntion.StringToInt(strs[3]);
            list.Add(company);
        }

        return list;
    }

    public void ShowCompany(List<Company> companies)
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Company comp in companies)
        {
            GameObject company = Instantiate(companyPrefab);
            CompanyUI companyComponent = company.GetComponent<CompanyUI>();
            companyComponent.InitCompanyInfo(comp);
            company.transform.SetParent(transform, false);
        }

    }

    public void SetVal(string reval)
    {
        BaseForm form = gameObject.GetComponentInParent<BaseForm>();
        if (form != null) 
        {
            string[] strings=reval.Split(',');
            if (strings.Length > 0)
            {
                form.reVal = strings[1];
            }
        }
    }
}

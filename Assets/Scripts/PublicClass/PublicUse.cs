using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RequestType
{
    Question,
    Message_Pass,
    Message_Fail,
    PlayACard,
    ChoiceACompany,
    BuyStock,
    MergerCompanies,
    CompaniesOut,
    OverGame

}

public enum StageType
{
    PlayACard,
    ChoiceACompany,
    BuyStock,
    MergerCompanies,
    CompaniesOut,
    OverGame
}
public class PublicUse : MonoBehaviour
{
    public enum RequestType
    {
        Question,
        Message
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

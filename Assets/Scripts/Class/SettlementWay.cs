using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementWay
{
    //结算方式分三种，可以组合使用
    // Start is called before the first frame update

    public int sellCount;

    public int translateCount;

    public int keepCount;
    
    public SettlementWay(int sellCount,int translateCount,int keepCount)
    {
        this.sellCount = sellCount;
        this.translateCount = translateCount;
        this.keepCount = keepCount;
    }
    public override string ToString()
    {
        return sellCount.ToString()+","+translateCount.ToString()+","+keepCount.ToString();
    }

    public SettlementWay(string str)
    {
        string[] strings = str.Split(',');
        if (strings.Length >= 3)
        {
            this.sellCount = ConvertFuntion.StringToInt(strings[0]);
            this.translateCount = ConvertFuntion.StringToInt(strings[1]);
            this.keepCount = ConvertFuntion.StringToInt(strings[2]);
        }
        else
        {
            this.keepCount = ConvertFuntion.StringToInt(str);
        }
    }
}

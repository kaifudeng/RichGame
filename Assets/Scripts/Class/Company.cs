using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Company
{

    /// <summary>
    /// 公司类型 分为ABC三类 
    /// </summary>
    public char CompanyType;

    public string CompanyName;
    //[SyncVar]
    public int HasCardCount;

    List<CardController> HasCards;

    /// <summary>
    /// 是否为安全公司
    /// </summary>
    public bool IsSafe;

    /// <summary>
    /// 剩余股票
    /// </summary>
    public int RemainStock;

    /// <summary>
    /// 价格
    /// </summary>
    public int price;

    /// <summary>
    /// 是否活着（上市）
    /// </summary>
    public bool IsLife;
    ///// <summary>
    ///// 公司颜色
    ///// </summary>
    //public Material CompanyColor;
    /// <summary>
    /// 使用的颜色
    /// </summary>
    public int useMaterialIndex;
    /// <summary>
    /// 股东战斗力排行
    /// </summary>
    public List<Player> PlayerList;

   // /// <summary>
   // /// 初始化公司对象的时候需要输入的参数
   // /// </summary>
   // /// <param name="name"></param>
   // /// <param name="useMaterial"></param>
   // /// <param name="tp"></param>
   //public void InitCompany(string name,int useMaterial,char tp)
   // {
   //     CompanyType = tp;
   //     CompanyName =name;
   //     //HasCardCount = 0;
   //     HasCards=new List<CardController>();
   //     IsSafe = false;
   //     RemainStock = 30;
   //     IsLife = false;
   //     useMaterialIndex = useMaterial;
   // }

   // /// <summary>
   // /// 获取实时股价
   // /// </summary>
   // /// <returns></returns>
   //public int GetPrice()
   // {
   //     //A类公司 基础股价为800 B类为600 C类为400
   //     //A类公司每+3地皮 股价+400 B类公司+300 C类+200
   //     int thePrice=0;
   //     switch (CompanyType)
   //     {
   //         case 'A': thePrice = 800 + 400 * (HasCards.Length / 3); break;
   //         case 'B': thePrice = 600 + 300 * (HasCards.Length / 3); break;
   //         case 'C': thePrice = 400 + 200 * (HasCards.Length / 3); break;
   //     }
   //     return thePrice;
   // }

    public override string ToString()
    {
        return CompanyType + "," + CompanyName + ","+ useMaterialIndex;
    }

    public string ToLongString()
    {
        return CompanyType + "," + CompanyName + "," + (IsSafe ? 1 : 0) + "," + RemainStock + "," + price + "," + (IsLife ? 1 : 0) + "," + useMaterialIndex+","+ HasCardCount;
    }

    /// <summary>
    /// company的构造函数 仅包含一些基本信息
    /// </summary>
    /// <param name="str"></param>
    public Company(string str)
    {
        string[] strings = str.Split(',');
        CompanyType = strings[0][0];
        CompanyName = strings[1];
        IsSafe = false;
        RemainStock = 30;
        //price = Convert.ToInt32(strings[4]);
        IsLife = false;
        useMaterialIndex = Convert.ToInt32(strings[2]);

        HasCards = new List<CardController>();
    }
    /// <summary>
    /// 包含复杂变量的构造函数 用于在表现层同步逻辑层的数据
    /// </summary>
    /// <param name="name"></param>
    /// <param name="useMaterial"></param>
    /// <param name="tp"></param>
    public Company(string str, bool isSafe, int remainStock,int thePrice,bool islife,int material,int hasCardCount)
    {
        string[] strings = str.Split(',');
        CompanyType = strings[0][0];
        CompanyName = strings[1];
        IsSafe= isSafe;
        RemainStock= remainStock;
        price = thePrice;
        IsLife= islife;
        HasCardCount= hasCardCount;
        useMaterialIndex = material;

        HasCards = new List<CardController>();

    }

    public void ChangeCard(List<CardController> theCard)
    {
        HasCards=theCard;
        HasCardCount=theCard.Count;
        if (HasCardCount > 15) { IsSafe = true; }
        else { IsSafe = false; }
    }

    public List<CardController> GetCards() 
    {
        return HasCards;
    }

    public void AddCards(List<CardController> theCard)
    {
        foreach (CardController card in theCard)
        {
            HasCards.Add(card);
        }
        ChangeCard(HasCards);
    }

    public int GetHasCardsCount()
    {
        return HasCards.Count;
    }

    public override bool Equals(object obj)
    {
        if(CompanyName==(obj as Company).CompanyName) { return true; }
        else return false;
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// 携带更多参数的company
    /// </summary>
    /// <param name="str"></param>
    /// <param name="islong"></param>
    //public Company(string str,bool islong)
    //{
    //    string[] strings = str.Split(',');
    //    CompanyType = strings[0][0];
    //    CompanyName = strings[1];
    //    IsSafe = false;
    //    IsLife = true;
    //    useMaterialIndex = Convert.ToInt32(strings[2]);

    //    HasCards = new List<CardController>();
    //}

    public void PlayerListAdd(Player player)
    {
        if (PlayerList == null)
        {
            PlayerList = new List<Player>();
        }
        bool noin=false;
        foreach (Player player1 in PlayerList) 
        {
            if(player1.PlayerName== player.PlayerName)
            {
                noin=true;
                break;
            }

        }
        if (!noin)
        {
            PlayerList.Add(player);
        }
    }

    public void PrintPlayerList()
    {
        if (PlayerList == null)
        {
            PlayerList = new List<Player>();
        }
        int cout = 0;
        string val = "";
        foreach (Player player in PlayerList)
        {
            cout++;
            val += "\n测试" + cout.ToString() +":"+ player.PlayerName+"\n";
        }
        Debug.Log(val);
    }
}

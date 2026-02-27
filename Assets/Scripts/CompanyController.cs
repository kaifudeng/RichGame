//using Mirror;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.VisualScripting;
//using UnityEngine;

//public class CompanyController : NetworkBehaviour,IComparable<CompanyController>
//{
//    [SyncVar]
//    /// <summary>
//    /// 公司类型 分为ABC三类 
//    /// </summary>
//    public char CompanyType;
//    [SyncVar]
//    public string CompanyName;
//    //[SyncVar]
//    //public int HasCardCount;
//    [SyncVar]
//    public string[] HasCards;
//    [SyncVar]
//    /// <summary>
//    /// 是否为安全公司
//    /// </summary>
//    public bool IsSafe;
//    [SyncVar]
//    /// <summary>
//    /// 剩余股票
//    /// </summary>
//    public int RemainStock;
//    [SyncVar]
//    /// <summary>
//    /// 价格
//    /// </summary>
//    public int price;

//    public int Price
//    {
//        get
//        {
//            return GetPrice();
//        }
//    }
//    [SyncVar(hook = nameof(IsLifeStateChange))]
//    /// <summary>
//    /// 是否活着（上市）
//    /// </summary>
//    public bool IsLife;

//   // public event Action<bool> OnIsLifeStateChange;

//    private void IsLifeStateChange(bool oldState,bool newState)
//    {
//        //OnIsLifeStateChange?.Invoke(newState);
//    }

//    ///// <summary>
//    ///// 公司颜色
//    ///// </summary>
//    //public Material CompanyColor;
//    [SyncVar]
//    /// <summary>
//    /// 使用的颜色
//    /// </summary>
//    public int useMaterialIndex;
//    /// <summary>
//    /// 初始化公司对象的时候需要输入的参数
//    /// </summary>
//    /// <param name="name"></param>
//    /// <param name="useMaterial"></param>
//    /// <param name="tp"></param>
//    public void InitCompany(string name, int useMaterial, char tp)
//    {
//        CompanyType = tp;
//        CompanyName = name;
//       // HasCardCount = 0;
//        HasCards = new string[0];
//        IsSafe = false;
//        RemainStock = 30;
//        IsLife = false;
//        useMaterialIndex = useMaterial;
//    }

//    /// <summary>
//    /// 获取实时股价
//    /// </summary>
//    /// <returns></returns>
//    public int GetPrice()
//    {
//        //A类公司 基础股价为800 B类为600 C类为400
//        //A类公司每+3地皮 股价+400 B类公司+300 C类+200
//        int thePrice = 0;
//        switch (CompanyType)
//        {
//            case 'A': thePrice = 800 + 400 * (HasCards.Length / 3); break;
//            case 'B': thePrice = 600 + 300 * (HasCards.Length / 3); break;
//            case 'C': thePrice = 400 + 200 * (HasCards.Length / 3); break;
//        }
//        return thePrice;
//    }
//    /// <summary>
//    /// 更改公司的状态
//    /// </summary>
//    /// <param name="name">公司名称</param>
//    /// <param name="state">是否上市 true为是 false为否</param>
//    public void UpdateCompanyAliveState(bool state)
//    {
//        Debug.Log(netIdentity.assetId);
//        IsLife = state;
//    }

//    public int CompareTo(CompanyController other)
//    {
//        if (null == other)
//        {
//            return 0;//空值比较大，返回1
//        }
//        return other.Price.CompareTo(this.Price);//降序
//    }
//    [Server]
//    /// <summary>
//    /// 新增card到公司
//    /// </summary>
//    /// <param name="cardxy"></param>
//    public void HasCardAdd(string cardxy)
//    {
//        List<string> strs=HasCards.ToList();
//        strs.Add(cardxy);
//        HasCards=strs.ToArray();
//    }
//    [Server]
//    /// <summary>
//    /// 公司刷新地盘颜色
//    /// </summary>
//    public void CompanyRefreshChessBoard()
//    {
//        List<string> cards=new List<string>();
//        foreach (string str in HasCards) 
//        {
//            cards.Add(str+","+useMaterialIndex);
//        }
//        Chessboard stock = Chessboard.GetChessboard().GetComponent<Chessboard>();
//        stock.cardInfo = cards;
//    }
//    public void RefreshHasCards(string[] strings)
//    {
//        HasCards = strings.ToArray();
//    }
//    [TargetRpc]
//    void TargetRpcTest()
//    {

//    }
//}

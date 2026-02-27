//using Mirror;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Unity.VisualScripting;
//using UnityEngine;

//public class PlayerRound
//{

//    int state = 0;//0为未启用 1为启用 2为结束
//    private Player thePlayer;

//    public PlayerRound(Player player)
//    {
//        state = 1;
//        thePlayer = player;
//        StartRound();
//    }

//    async void StartRound()
//    {
//        int a = await CardTime();
//        switch (a)
//        {
//            case 0: break;
//            case 1: CompanyCreateProcess(); break;
//            case 2: break;
//            case 3: break;
//            default: EndRound(); break;
//        }

//    }
//    void EndRound()
//    {
//        state = 2;
//    }

//    async Task<int> CardTime()
//    {
//        thePlayer.handstate = true;
//        //string theCard = await UniTask.WaitUntil(() => player.Health <= 0);

//        if (1 == 1)
//        {
//            return 0;
//        }
//        else
//        {

//            return 1;

//        }
//    }

//    /// <summary>
//    /// 玩家转入创建公司的流程
//    /// </summary>
//    [Server]
//    public void CompanyCreateProcess()
//    {
//        //AddNewInfo(nowPlayer.PlayerName + ":创建公司");
//        thePlayer.TargetRpcGetCompaniesSelectForm();
//    }

//    /// <summary>
//    /// 玩家转入购买股票的流程
//    /// </summary>
//    [Server]
//    public void BuyStockProcess(Company company)
//    {
//        thePlayer.TargetRpcGetStockBuyForm(company.CompanyName, company.price);
//    }

//    /// <summary>
//    /// 玩家转入公司并购的流程
//    /// </summary>
//    [Server]
//    public void CompaniesBattle(Company company)
//    {
//        //nowPlayer.TargetRpcGetStockBuyForm(company.CompanyName, company.price);
//    }
//}

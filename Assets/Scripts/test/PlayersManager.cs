using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{

    List<Player> players=new List<Player>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 方法1: 获取所有连接的玩家对象
    public List<GameObject> GetAllConnectedPlayers()
    {
        var players = new List<GameObject>();

        // 遍历所有连接
        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn != null && conn.identity != null)
            {
                players.Add(conn.identity.gameObject);
            }
        }

        return players;
    }
    /// <summary>
    /// 获取所有已连接到服务器的玩家对象
    /// </summary>
    public void InitPlayers()
    {
        try
        {
            List<GameObject> gameObjects = GetAllConnectedPlayers();
            foreach (var obj in gameObjects)
            {
                if (obj != null && obj.GetComponent<Player>() != null)
                {
                    players.Add(obj.GetComponent<Player>());
                }
            }
        }
        catch (Exception e) 
        {
            Debug.LogError("加载玩家列表时出错："+e.Message);
        }
    }
    /// <summary>
    /// 给所有玩家发放初始手牌
    /// </summary>
    /// <param name="CardManager">卡牌管理器</param>
    /// <param name="s">手牌数量</param>
    public bool InitPlayerHandCard(CardsController CardManager, int s)
    {
        try
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i] != null)
                {
                    if (s <= CardManager.deck.Length)
                    {
                        players[i].cards = CardManager.GetCards(s);
                    }
                    else
                    {
                        Debug.Log("剩余牌数不足");
                        return false;
                    }
                }
                else
                {
                    Debug.Log("获取玩家失败");
                    return false;
                }
            }
            return true;
        }
        catch (Exception e) 
        {
            Debug.LogError("玩家获取初始手牌时出错：" + e.Message);
            return false;
        }
    }
    /// <summary>
    /// 确认游戏玩家的顺位
    /// </summary>
    /// <param name="CardManager"></param>
    public void RankingPlayers(CardsController CardManager)
    {
        try
        {
            for (int i = 0; i < players.Count; i++)
            {
                //int min = 74+10;
                int minPlayer = players.Count-1;
                CardController ACard = CardManager.FindTheLessCard(players[i].cards);
                for (int j = 0; j < players.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    CardController BCard = CardManager.FindTheLessCard(players[j].cards);
                   if(CardManager.CardCompare(ACard, BCard))
                    {
                        minPlayer --;
                    }
                    else
                    {
                        //minPlayer ++;
                    }
                }
                players[i].rank = minPlayer;
            }

            //按照rank的值进行排序

            players = players.OrderBy(p => p.rank).ToList();
        }
        catch(Exception e) 
        {
            Debug.LogError("生成玩家顺位时出错：" + e.Message);

        }
    }

    /// <summary>
    /// 获取玩家列表
    /// </summary>
    public List<Player> GetPlayers()
    {
        return players;
    }

    internal void InitPlayersInfo()
    {
        foreach (Player p in players) 
        {
            p.cash = 6000;
            p.stocks=new List<Stock>();
            p.stocks_str = new string[] {};
        }
    }

    /// <summary>
    /// 同步玩家的总资产
    /// </summary>
    public void SyncTotalAssets(CompaniesController companiesController)
    {
        foreach (Player p in players)
        {
            int total =p.cash;
            int stockPrice = 0;
            foreach (var stock in p.stocks)
            {
                if (stock != null)
                {
                    foreach (Company company in companiesController.companies)
                    {
                        if (company.ToString() == stock.Company.ToString())
                        {
                            stockPrice += companiesController.GetPrice(company) * stock.Sum;
                            break;
                        }
                    }
                }
            }
            p.TotalAssets = total + stockPrice;
            p.SyncStockInfo();
        }
    }

    public void PlayerStockAdd(Player player,int sum, Company company)
    {
        bool isHave = false;
        Stock stock = new Stock(company, sum);
        for (int i = 0; i < player.stocks.Count; i++)
        {
            if (player.stocks[i].Company.CompanyName == stock.Company.CompanyName)
            {
                isHave = true;
                player.stocks[i].Sum += sum;
            }
        }
        if (!isHave)
        {
            player.stocks.Add(stock);
        }

        int stockPrice = company.price;

        player.cash+=(stockPrice * -sum);
        //SyncStockInfo();
        //SyncTotalAssets();
    }

    public List<Player> GetPlayersWithCompanies(List<Company> companies)
    {
        List<Player> listP = new List<Player>();
        foreach (Player player in players)
        {
            foreach (Company company in companies) 
            {
                foreach(Stock stock in player.stocks)
                {
                    if (stock.Company.CompanyName == company.CompanyName
                        &&stock.Sum>0)
                    {
                        listP.Add(player);
                        break;
                    }
                }
            }
        }
        return listP;
    }

    /// <summary>
    /// 玩家出售股票
    /// </summary>
    /// <param name="company"></param>
    /// <param name="sum"></param>
    internal void SellStock(Company company, int sum, CompaniesController companiesController,Player player)
    {
        if (sum > 0)
        {
            for (int j= 0; j < players.Count; j++)
            {
                if (players[j].name == player.name)
                {
                    for (int i = 0; i < players[j].stocks.Count; i++)
                    {
                        if (players[j].stocks[i].Company.CompanyName == company.CompanyName)
                        {
                            players[j].stocks[i].Sum -= sum;
                            players[j].cash += sum * companiesController.GetPrice(company);
                            if (players[j].stocks[i].Sum == 0)
                            {
                                players[j].stocks.Remove(players[j].stocks[i]);
                            }
                            players[j].SyncStockInfo();
                        }
                    }
                }
            }
            
        }
        
    }

    /// <summary>
    /// 设置玩家状态为可出牌
    /// </summary>
    /// <param name="player"></param>
   public void SetPlayerHandStateTrue(Player player) 
    {
        for (int j = 0; j < players.Count; j++) 
        {
            if(players[j].PlayerName == player.PlayerName)
            {
                players[j].handstate=true;
                break;
            }
        }
    }
    /// <summary>
    /// 设置玩家状态为不可出牌
    /// </summary>
    /// <param name="player"></param>
    void SetPlayerHandStateFalse(Player player)
    {
        for (int j = 0; j < players.Count; j++)
        {
            if (players[j].PlayerName == player.PlayerName)
            {
                players[j].handstate = true;
                break;
            }
        }
    }

    /// <summary>
    /// 玩家金币增加
    /// </summary>
    /// <param name="money"></param>
    public void PlayerCashAdd(int money,Player player)
    {
        for (int j = 0; j < players.Count; j++) 
        {
            if (players[j].PlayerName == player.PlayerName)
            {
                players[j].cash += money;
                break;
            }
        }
    }
    
}

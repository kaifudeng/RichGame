using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardUI : NetworkBehaviour
{
    //public string[] cardInfo;

    public GameObject cardPrefab;

    /// <summary>
    /// 场上所有卡片集合 100个
    /// </summary>
    public List<CardController> allCards;

    private void Awake()
    {
        InitPlane();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        
    }

    private void Start()
    {
        
    }
    [ClientRpc]
    /// <summary>
    /// 根据最新信息变化，同步棋盘上卡片颜色
    /// </summary>
    /// <param name="old"></param>
    /// <param name="newval"></param>
    /// <returns></returns>
    public void RpcOnChessBoardInfoChange(string[] newval)
    {
        foreach (string c in newval)
        {
            string[] strs = c.Split(',');
            if (strs.Length > 0)
            {
                foreach (CardController cardController in allCards) 
                {
                    if(cardController.ToString()== strs[0])
                    {
                        cardController.UpdateMaterial(ConvertFuntion.StringToInt(strs[1]));
                    }
                }
            }
        }
    }

    [Client]
    public void OnChessBoardInfoChange(string[] newval)
    {
        foreach (string c in newval)
        {
            string[] strs = c.Split(',');
            if (strs.Length > 0)
            {
                foreach (CardController cardController in allCards)
                {
                    if (cardController.ToString() == strs[0])
                    {
                        cardController.UpdateMaterial(ConvertFuntion.StringToInt(strs[1]));
                    }
                }
            }
        }
    }

    /// <summary>
    /// 初始化棋盘
    /// </summary>
    public void InitPlane()
    {
        allCards = new List<CardController>();
        for (int i = 1; i < 11; i++)
        {
            for (int j = 1; j < 11; j++)
            {
                char letter = (char)(j + 64);

                UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(i, 0, (int)(letter - 64));
                GameObject obj = Instantiate(cardPrefab, vector3, cardPrefab.transform.rotation);
                MeshRenderer meshRenderer=obj.GetComponent<MeshRenderer>();
                
                CardController cardComponent = obj.GetComponent<CardController>();
                cardComponent.x = letter;
                cardComponent.y = i;
                cardComponent.UpdateMaterial(0);
                obj.transform.parent = gameObject.transform;
                obj.name = letter.ToString() + i.ToString();
                allCards.Add(cardComponent);
            }
        }
        //foreach (CardController card in cards)
        //{



        //}

    }
}

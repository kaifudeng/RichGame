using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProcess : NetworkBehaviour
{
    public Player nowPlayer;
    //PlayerRound nowRound;

    public GameObject gameProcessUIObject;
    GameProcessUI gameProcessUI;

    [SyncVar(hook =nameof(GameProcessChange))]
    public string gameProcessInfo;

    public Action<string> OnGameProcessChange;
    private void GameProcessChange(string oldVal,string newVal)
    {
        OnGameProcessChange?.Invoke(newVal);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitGameProcessInfoUI();
    }

    [Server]
    /// <summary>
    /// 加载游戏流程信息的UI界面
    /// </summary>
    public void InitGameProcessInfoUI()
    {
        gameProcessUIObject = Instantiate(gameProcessUIObject);
        NetworkServer.Spawn(gameProcessUIObject);
        gameProcessUI = gameProcessUIObject.GetComponent<GameProcessUI>();
        OnGameProcessChange = gameProcessUI.OnGameProcessInfoChange;
        OnGameProcessChange?.Invoke(gameProcessInfo);
    }
    [Server]
    public void AddNewLog(string theInfo)
    {
        gameProcessInfo = theInfo + "\n";
    }
}

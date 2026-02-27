using Cysharp.Threading.Tasks;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
public class GameStage : NetworkBehaviour
{
    public string id;
    public int timeOutBy;//超时时长

    public RequestType requestType;

    public Player nowplayer;
    //[SyncVar]
    //string question;
    //public GameObject choseForm;
    //choseForm form;
   //public GameObject canvas;
   // CancellationToken cancellationTokenSource;
    //CancellationToken token;
    // Start is called before the first frame update
    public void Start()
    {
        //question = "猫有几条腿？A两条 B四条";
        //sendMessageToClient(question);

        
    }
    /// <summary>
    /// 等待用户输入
    /// </summary>
    /// <param name="playerId">操作用户id</param>
    /// <returns></returns>
    //public virtual async Task<string> WaitForPlayerInput(uint playerId)
    //{
    //    string requestId = Guid.NewGuid().ToString();
    //    var tcs = new TaskCompletionSource<string>();

    //    pendingRequests[requestId] = tcs;

    //    // 通知客户端
    //    NetworkConnection conn = FindConnectionByNetId(playerId);

    //    //InitRequest(conn, requestId, type, Value);
    //    TargetRequestInput(conn, requestId);

    //    // 核心：异步等待
    //    try
    //    {
    //         return await tcs.Task; // 挂起直到客户端响应
    //        //return await WaitWithTimeout<string>(tcs, 10000); // 挂起直到客户端响应 增加了十秒超时

    //    }
    //    finally
    //    {
    //        Debug.Log(tcs.Task.Result);
    //        pendingRequests.Remove(requestId);
    //        //tcs.SetCanceled();
    //    }
    //}
    ///// <summary>
    ///// 用户通过这个方法返回结果
    ///// </summary>
    ///// <param name="target"></param>
    ///// <param name="requestId"></param>
    //public virtual void TargetRequestInput(NetworkConnection target, string requestId)
    //{
    //   //在这里写target窗体 再通过窗体调用Command
    //}
    ///// <summary>
    ///// 通过客户端调用这个方法在服务端执行，结束前面的等待
    ///// </summary>
    ///// <param name="requestId"></param>
    ///// <param name="choice"></param>
    //[Command(requiresAuthority = false)]
    //public virtual void CmdSubmitResponse(string requestId, string choice)
    //{
    //    // 唤醒正在等待的Task
    //    if (pendingRequests.TryGetValue(requestId, out var tcs))
    //    {
    //        tcs.SetResult(choice); // 关键：这会完成await
    //    }
    //}
    //[TargetRpc]
    //void sendMessageToClient(string message)
    //{

    //    cancellationTokenSource = new CancellationToken();
    //    canvas = GameObject.FindGameObjectWithTag("canvas");
    //    form = Instantiate(choseForm, canvas.transform).GetComponent<choseForm>();
    //    form.question = question;
    //}
    // Update is called once per frame
    void Update()
    {
        
    }

    //public async UniTask createForm()
    //{
    //   // InitForm();
    //    //await form.WaitForClick(cancellationTokenSource);
    //    Debug.Log("输出：用户完成操作");
    //    Destroy(this);
    //}
    //void InitChoseForm(string requestId)
    //{
    //    try
    //    {
    //        cancellationTokenSource = new CancellationToken();
    //        canvas = GameObject.FindGameObjectWithTag("canvas");
    //        form = Instantiate(choseForm, canvas.transform).GetComponent<choseForm>();
    //        form.stage = this;
    //        form.question = question;
    //        form.setQuestion();
    //        form.requestId = requestId;
    //    }
    //    catch(Exception ex)
    //    {
    //        Debug.Log("测试："+ex);
    //    }
    //}
    /// <summary>
    /// 服务端处理请求类型
    /// </summary>
    /// <param name="requestId"></param>
    /// <param name="requestType">请求的类型</param>
    void InitRequest(NetworkConnection connection ,string requestId,RequestType requestType, string Val,string MaxVal)
    {
        //try
        //{
        //    //cancellationTokenSource = new CancellationToken();
        //    //canvas = GameObject.FindGameObjectWithTag("canvas");
        //    //form = Instantiate(choseForm, canvas.transform).GetComponent<choseForm>();
        //    //form.stage = this;
        //    nowplayer=connection.identity.GetComponent<Player>();
        //    switch (requestType)
        //    {
        //        //case RequestType.Question: form.InitDialog(requestId, question,"A","B"); break;
        //        //case RequestType.Message_Pass: form.InitDialog(requestId, "答案正确", "下一关", "退出"); break;
        //        //case RequestType.Message_Fail: form.InitDialog(requestId, "答案错误", "", "退出"); break;

        //        //case RequestType.PlayACard: nowplayer.TargetRpcStartPlayACard(requestId,this); break;

        //        case RequestType.ChoiceACompany: 
        //        case RequestType.BuyStock:
        //        case RequestType.MergerCompanies:
        //        case RequestType.CompaniesOut:
        //            Debug.Log(this.id); 
        //            nowplayer.TargetServerRequestForm(requestId, requestType, this, Val,MaxVal); break;

        //        default:Debug.LogError("未知请求类型"); break;
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.Log("测试：" + ex);
        //}
    }

    // 核心数据结构：存储等待中的请求
    public Dictionary<string, TaskCompletionSource<string>> pendingRequests =
        new Dictionary<string, TaskCompletionSource<string>>();
    internal string sendValue;
    internal string MaxValue;

    // 核心方法1：服务器异步等待客户端输入
    [Server]
    public virtual async Task<string> WaitForPlayerInput(uint playerId,RequestType type,string Value, string MaxVal)
    {
        string requestId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();

        pendingRequests[requestId] = tcs;

        // 通知客户端
        NetworkConnection conn = FindConnectionByNetId(playerId);

        InitRequest(conn,requestId, type, Value,MaxVal);
        //TargetRequestInput(conn, requestId,type);

        // 核心：异步等待
        try
        {
           // return await tcs.Task; // 挂起直到客户端响应
            return await WaitWithTimeout<string>(tcs,10000000); // 挂起直到客户端响应 增加了十秒超时

        }
        catch(Exception ex) 
        {
            Debug.LogError("tcs等待时出错："+ex);
            return null;
        }
        finally
        {
            //Debug.Log(tcs.Task.Result);
            pendingRequests.Remove(requestId);
           // tcs.SetCanceled();
        }
        
    }

    //核心方法2：客户端响应
   [TargetRpc]
    public virtual void TargetRequestInput(NetworkConnection target, string requestId, RequestType type)
    {
        //InitRequest(target, requestId, type);
    }
    /// <summary>
    /// 弹出过关的消息 
    /// </summary>
    public void PassMessage()
    {

    }
    //[Client]
    //private void OnUserClickedButton(string requestId)
    //{
    //    // 用户点击了某个按钮，假设选择1
    //    CmdSubmitResponse(requestId, 1);
    //}

    // 核心方法3：客户端提交响应到服务器
    [Command(requiresAuthority = false)]
    public virtual void CmdSubmitResponse(string requestId, string choice)
    {
        NetworkIdentity identity = GetComponent<NetworkIdentity>();
        // 检查对象是否存在
        if (!NetworkServer.spawned.ContainsKey(identity.netId))
        {
            Debug.LogWarning($"目标对象 {identity.netId} 不存在");
            return; // 提前返回，避免后续错误
        }
        // 唤醒正在等待的Task
        if (pendingRequests.TryGetValue(requestId, out var tcs))
        {
            tcs.SetResult(choice); // 关键：这会完成await
        }
    }

    [Server]
    private NetworkConnection FindPlayerConnection(uint playerId)
    {
        // 简化实现
        return NetworkServer.connections[nowplayer.connectionToClient.connectionId];
    }

    public static NetworkConnection FindConnectionByNetId(uint netId)
    {
        if (!NetworkServer.active)
        {
            Debug.LogWarning("此方法只能在服务器端调用");
            return null;
        }

        foreach (var conn in NetworkServer.connections.Values)
        {
            if (conn == null) continue;

            // 检查连接的 identity 是否匹配
            if (conn.identity != null && conn.identity.netId == netId)
            {
                return conn;
            }
        }

        return null;
    }

    public async Task<string> WaitWithTimeout<T>(TaskCompletionSource<string> tcs, int timeoutMilliseconds)
    {
        using (var cts = new CancellationTokenSource(timeoutMilliseconds))
        {
            // 创建超时任务
            var timeoutTask = Task.Delay(timeoutMilliseconds, cts.Token);

            // 等待第一个完成的任务
            var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

            if (completedTask == timeoutTask)
            {
                // 超时处理
                try
                {
                    //tcs.TrySetCanceled(cts.Token);
                    //throw new TimeoutException($"操作超时，超过 {timeoutMilliseconds}ms");
                    tcs.SetResult("");
                }
                catch(Exception ex)
                {
                    Debug.LogError(ex);
                }
            }


            // 正常返回结果
            cts.Cancel();
            return await tcs.Task;
        }
    }

    internal void InitNowPlayer(Player nowPlayer,RequestType Type)
    {
        nowplayer=nowPlayer;
        requestType = Type;
    }


    public void DestroyObject()
    {
        if (isServer)
        {
            NetworkServer.Destroy(gameObject);
        }
        else if (isClient)
        {
            CmdDestroyObject();
        }
    }
    [Command]
    void CmdDestroyObject()
    {
        NetworkServer.Destroy(gameObject);
    }
}

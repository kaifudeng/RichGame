using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static PublicUse;

public class CommunicationTool : NetworkBehaviour
{
    private static CommunicationTool instance;
    public string id;
    public int timeOutBy;//超时时长
    public StageType stageType;
    public Player nowplayer;
    //[SyncVar]
    //string question;
    public GameObject choseForm;
    choseForm form;
    public GameObject canvas;

    // 核心数据结构：存储等待中的请求
    public Dictionary<string, TaskCompletionSource<string>> pendingRequests =
        new Dictionary<string, TaskCompletionSource<string>>();
    public void Awake()
    {
        timeOutBy = 30000;
        instance =this;
    }
    public static CommunicationTool GetCommunicationTool() => instance;
    [Server]
    public virtual async Task<string> WaitForPlayerInput(GameStage gameStage)
    {
        string requestId = Guid.NewGuid().ToString();
        var tcs = new TaskCompletionSource<string>();

        pendingRequests[requestId] = tcs;

        // 通知客户端
        NetworkConnection conn = FindConnectionByNetId(gameStage.nowplayer.netId);

        InitRequest(conn, requestId, gameStage.requestType, gameStage.sendValue, gameStage.MaxValue);
        //TargetRequestInput(conn, requestId,type);

        // 核心：异步等待
        try
        {
            // return await tcs.Task; // 挂起直到客户端响应
            return await WaitWithTimeout<string>(tcs, timeOutBy); // 挂起直到客户端响应 增加了十秒超时

        }
        catch (Exception ex)
        {
            Debug.LogError("tcs等待时出错：" + ex);
            return null;
        }
        finally
        {
            //Debug.Log(tcs.Task.Result);
            pendingRequests.Remove(requestId);
            // tcs.SetCanceled();
        }

    }
    void InitRequest(NetworkConnection connection, string requestId, RequestType requestType, string Val, string MaxVal)
    {
        try
        {
            //cancellationTokenSource = new CancellationToken();
            //canvas = GameObject.FindGameObjectWithTag("canvas");
            //form = Instantiate(choseForm, canvas.transform).GetComponent<choseForm>();
            //form.stage = this;
            nowplayer = connection.identity.GetComponent<Player>();
            switch (requestType)
            {
                //case RequestType.Question: form.InitDialog(requestId, question,"A","B"); break;
                //case RequestType.Message_Pass: form.InitDialog(requestId, "答案正确", "下一关", "退出"); break;
                //case RequestType.Message_Fail: form.InitDialog(requestId, "答案错误", "", "退出"); break;

                case RequestType.PlayACard: nowplayer.TargetRpcStartPlayACard(requestId); break;

                case RequestType.ChoiceACompany:
                case RequestType.BuyStock:
                case RequestType.MergerCompanies:
                case RequestType.CompaniesOut:
                    Debug.Log(this.id);
                    nowplayer.TargetServerRequestForm(requestId, requestType, Val, MaxVal); break;

                default: Debug.LogError("未知请求类型"); break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("测试：" + ex);
        }
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
                catch (Exception ex)
                {
                    Debug.LogError(ex);
                }
            }


            // 正常返回结果
            cts.Cancel();
            return await tcs.Task;
        }
    }

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

}

using Cysharp.Threading.Tasks;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class choseForm : NetworkBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI ButtonATxt;
    public TextMeshProUGUI ButtonBTxt;
    public Button buttonA;
    public Button buttonB;
    public string requestId;
    public  GameStage stage;
    public string question;
    public UnityEvent OnBClicked;
    public UnityEvent OnAClicked;
    public List<Button> buttons;
    // Start is called before the first frame update
    void Start()
    {
        
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                Debug.Log($"按钮状态: {button.interactable}");
                Debug.Log($"按钮是否启用: {button.enabled}");
                //Debug.Log($"CanvasGroup: {GetComponent<CanvasGroup>()?.interactable}");
                ButtonATxt=button.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        CheckCanvas();

    }

    public void setQuestion()
    {
        text.text = question;
    }

    void CheckCanvas()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("按钮不在 Canvas 下！");
            return;
        }

        Debug.Log($"Canvas渲染模式: {canvas.renderMode}");
        Debug.Log($"Camera: {canvas.worldCamera}");

        // 检查 EventSystem
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("场景中没有 EventSystem！");
            // 自动创建
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void OnButtonAClicked()
    {
        stage.CmdSubmitResponse(requestId, "A");
        //OnAClicked?.Invoke();
        Destroy(gameObject);
    }

    public void OnButtonBClicked()
    {
        stage.CmdSubmitResponse(requestId, "B");
        //OnBClicked?.Invoke();
        Destroy(gameObject);
    }

    //// 等待玩家点击按钮
    //public async UniTask WaitForClick(CancellationToken token)
    //{

    //    // 使用 UniTaskCompletionSource 等待按钮点击
    //    var utcs = new UniTaskCompletionSource<bool>();

    //    // 按钮点击回调
    //    //choseFormComponent.OnButtonAClicked += () => utcs.TrySetResult(true);

    //    UnityAction onClicked = () => utcs.TrySetResult(true);
    //    OnAClicked.AddListener(onClicked);

    //    try
    //    {
    //        await utcs.Task.AttachExternalCancellation(token);
    //    }
    //    finally
    //    {
    //        //startPanel.OnStartClicked -= () => utcs.TrySetResult(true);
    //        //startPanel.Hide();
    //        OnAClicked.RemoveListener(onClicked);
    //    }
    //}

    public void InitDialog(string request,string title,string btnAText,string btnBText)
    {
        requestId = request;
        text.text = title;
        if (!string.IsNullOrEmpty(btnAText))
        {
            ButtonATxt.text = btnAText;
        }
        else
        {
            buttonA.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(btnBText))
        {
            ButtonBTxt.text = btnBText;
        }
        else
        {
            buttonB.gameObject.SetActive(false);
        }
        
    }
}

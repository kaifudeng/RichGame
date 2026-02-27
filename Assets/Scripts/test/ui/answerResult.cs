using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class answerResult : MonoBehaviour
{
    public TextMeshProUGUI title;
    public string requestId;
    public GameStage stage;
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
            }
        }

        CheckCanvas();
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
}

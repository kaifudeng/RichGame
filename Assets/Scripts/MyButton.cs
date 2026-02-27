//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.Events;
//using UnityEngine.UIElements;
//using UnityEngine;
//using Mirror;

//public class MyButton : UnityEngine.UI.Button, IPointerExitHandler, IPointerEnterHandler
//{
//    public UnityEvent onButtonDown;
//    public UnityEvent onButtonUp;
//    public UnityEvent onButtonExit;
//    public UnityEvent onButtonEnter;

//    public GameObject chessBoardUIObject;
//     ChessBoardUI chessBoardUI;
//    // 当按钮被按下时调用
//    public override void OnPointerDown(PointerEventData eventData)
//    {
//        base.OnPointerDown(eventData);
//        if (onButtonDown != null)
//            onButtonDown.Invoke();
//        //gameObject.GetComponentInChildren<TMP_Text>().text = "Pressed";
//    }

//    // 当按钮被抬起时调用
//    public override void OnPointerUp(PointerEventData eventData)
//    {
//        base.OnPointerUp(eventData);
//        if (onButtonUp != null)
//            onButtonUp.Invoke();
//        //gameObject.GetComponentInChildren<TMP_Text>().text = "Released";
//    }

//    // 当鼠标从按钮上离开时调用
//    public override void OnPointerExit(PointerEventData eventData)
//    {
//        base.OnPointerExit(eventData);
//        //if (onButtonExit != null)
//        //    onButtonExit.Invoke();
//        //gameObject.GetComponentInChildren<TMP_Text>().text = "Exit";
//        LeaveThisCard();
//    }

//    // 当鼠标从外面进入到按钮上方时调用
//    public override void OnPointerEnter(PointerEventData eventData)
//    {
//        base.OnPointerEnter(eventData);
//        //if (onButtonEnter != null)
//        //    onButtonEnter.Invoke();
//        //gameObject.GetComponentInChildren<TMP_Text>().text = "Enter";
//        MouseOnThisCard();
//    }
//    [Client]
//    void MouseOnThisCard()
//    {
//        TextMeshProUGUI btntext = GetComponentInChildren<TextMeshProUGUI>();
//        string thecard=btntext.text;
//        chessBoardUIObject=GameObject.FindGameObjectWithTag("chessBoardUI");
//        //Chessboard chessboard=Chessboard.theChessboard;
//        //foreach(CardController cardController in chessboard.allCards)
//        //{
//        //    if (cardController.ToString() == thecard)
//        //    {

//        //        cardController.UpdateMaterial(1);
//        //    }
//        //}
//        chessBoardUI = chessBoardUIObject.GetComponent<ChessBoardUI>();
//        string[] strs = new string[1] { thecard + ",1"};
//        chessBoardUI.OnChessBoardInfoChange(strs);
//    }
//    [Client]
//    void LeaveThisCard()
//    {
//        TextMeshProUGUI btntext = GetComponentInChildren<TextMeshProUGUI>();
//        string thecard = btntext.text;
//        chessBoardUIObject = GameObject.FindGameObjectWithTag("chessBoardUI");
//        //Chessboard chessboard = Chessboard.theChessboard;
//        //foreach (CardController cardController in chessboard.allCards)
//        //{
//        //    if (cardController.ToString() == thecard)
//        //    {

//        //        cardController.UpdateMaterial(0);
//        //    }
//        //}

//        chessBoardUI = chessBoardUIObject.GetComponent<ChessBoardUI>();
//        string[] strs = new string[1] { thecard + ",0" };
//        chessBoardUI.OnChessBoardInfoChange(strs);
//    }
//}

using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine;
using Mirror;
using System.Collections; // 需要添加这个命名空间

public class MyButton : UnityEngine.UI.Button, IPointerExitHandler, IPointerEnterHandler
{
    public bool state;

    public UnityEvent onButtonDown;      // PC单击 / 移动双击
    public UnityEvent onButtonUp;
    public UnityEvent onButtonExit;
    public UnityEvent onButtonEnter;

    public GameObject chessBoardUIObject;
    ChessBoardUI chessBoardUI;

    // 移动端专用：触摸计时
    private float touchStartTime;
    private bool isTouching = false;
    private const float HOVER_THRESHOLD = 0.1f;     // 200ms 触发悬停效果
    private const float DOUBLE_CLICK_THRESHOLD = 0.3f; // 双击时间阈值

    // 是否已经触发了悬停（避免重复触发）
    private bool hoverTriggered = false;

    // 双击检测
    private float lastTapTime = 0f;
    private int tapCount = 0;
    private Coroutine doubleClickCoroutine;

    protected override void Start()
    {
        base.Start();
        chessBoardUIObject = GameObject.FindGameObjectWithTag("chessBoardUI");
        if (chessBoardUIObject != null)
            chessBoardUI = chessBoardUIObject.GetComponent<ChessBoardUI>();
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        // 移动平台：检测触摸悬停
        if (isTouching && !hoverTriggered)
        {
            if (Time.time - touchStartTime >= HOVER_THRESHOLD)
            {
                // 触摸停留超过阈值，触发悬停效果（对应PC的鼠标悬停）
                hoverTriggered = true;
                MouseOnThisCard();
            }
        }
#endif
    }

    // 当按钮被按下时调用
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

#if UNITY_ANDROID || UNITY_IOS
        // 移动平台：记录触摸开始时间
        isTouching = true;
        touchStartTime = Time.time;
        hoverTriggered = false;
#else
        // PC平台：记录按下时间
        touchStartTime = Time.time;
#endif
    }

    // 当按钮被抬起时调用
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

#if UNITY_ANDROID || UNITY_IOS
        // 移动平台：双击检测
        float timeSinceLastTap = Time.time - lastTapTime;

        if (timeSinceLastTap <= DOUBLE_CLICK_THRESHOLD)
        {
            // 这是第二次点击（双击）
            tapCount++;

            if (tapCount == 2)
            {
                // 取消等待中的协程
                if (doubleClickCoroutine != null)
                    StopCoroutine(doubleClickCoroutine);

                // 触发双击事件（对应PC的单击）
                Debug.Log("移动端双击 → 触发单击事件");
                if (onButtonDown != null)
                    onButtonDown.Invoke();

                // 重置双击检测
                tapCount = 0;
                lastTapTime = 0f;
                doubleClickCoroutine = null;
            }
        }
        else
        {
            // 这是第一次点击
            tapCount = 1;
            lastTapTime = Time.time;

            // 启动协程等待第二次点击
            doubleClickCoroutine = StartCoroutine(WaitForDoubleClick());
        }

        // 触发抬起事件
        if (onButtonUp != null)
            onButtonUp.Invoke();

        // 清理触摸状态
        isTouching = false;
        hoverTriggered = false;

#else
        // PC平台：鼠标抬起时触发单击
        float pressDuration = Time.time - touchStartTime;
        
        // 简单的单击触发
        if (onButtonDown != null)
            onButtonDown.Invoke();
            
        if (onButtonUp != null)
            onButtonUp.Invoke();
#endif
    }

    // 等待双击的协程
    private IEnumerator WaitForDoubleClick()
    {
        float startTime = Time.time;

        // 等待双击时间窗口
        while (Time.time - startTime < DOUBLE_CLICK_THRESHOLD)
        {
            // 如果在这期间发生了第二次点击，协程会被取消
            yield return null;
        }

        // 超时，没有第二次点击，这是一个单击（但移动端我们忽略单击，只有双击才触发）
        // 所以这里什么都不做
        Debug.Log("移动端单击被忽略");
        
        tapCount = 0;
        lastTapTime = 0f;
        doubleClickCoroutine = null;
    }

    // 当指针离开按钮时调用
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

#if UNITY_ANDROID || UNITY_IOS
        // 移动平台：如果正在触摸且离开按钮，清理状态
        if (isTouching)
        {
            isTouching = false;
            hoverTriggered = false;
        }
#endif

        // 离开时触发离开效果
        LeaveThisCard();

        if (onButtonExit != null)
            onButtonExit.Invoke();
    }

    // 当指针进入按钮时调用
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

#if UNITY_ANDROID || UNITY_IOS
        // 移动平台：触摸进入时记录，但不立即触发悬停
        // 悬停效果会在Update中根据时间判断
#else
        // PC平台：直接触发进入效果
        MouseOnThisCard();
#endif

        if (onButtonEnter != null)
            onButtonEnter.Invoke();
    }

    [Client]
    void MouseOnThisCard()
    {
        if (chessBoardUIObject == null)
            chessBoardUIObject = GameObject.FindGameObjectWithTag("chessBoardUI");

        if (chessBoardUI == null && chessBoardUIObject != null)
            chessBoardUI = chessBoardUIObject.GetComponent<ChessBoardUI>();

        if (chessBoardUI != null)
        {
            TextMeshProUGUI btntext = GetComponentInChildren<TextMeshProUGUI>();
            if (btntext != null)
            {
                string thecard = btntext.text;
                string[] strs = new string[1] { thecard + ",1" };
                chessBoardUI.OnChessBoardInfoChange(strs);
            }
        }
    }

    [Client]
    void LeaveThisCard()
    {
        if (chessBoardUIObject == null)
            chessBoardUIObject = GameObject.FindGameObjectWithTag("chessBoardUI");

        if (chessBoardUI == null && chessBoardUIObject != null)
            chessBoardUI = chessBoardUIObject.GetComponent<ChessBoardUI>();

        if (chessBoardUI != null)
        {
            TextMeshProUGUI btntext = GetComponentInChildren<TextMeshProUGUI>();
            if (btntext != null)
            {
                string thecard = btntext.text;
                string[] strs = new string[1] { thecard + ",0" };
                chessBoardUI.OnChessBoardInfoChange(strs);
            }
        }
    }

    // 当脚本被禁用或销毁时清理协程
    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (doubleClickCoroutine != null)
        {
            StopCoroutine(doubleClickCoroutine);
            doubleClickCoroutine = null;
        }
    }
}
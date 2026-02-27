using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CardPlayTimer : MonoBehaviour
{
    [Header("UI组件")]
    public Image radialProgress;     // 扇形进度条
    public GameObject progressContainer; // 进度条容器
    public TextMeshProUGUI countdownText;       // 倒计时文本（可选）
    
    [Header("出牌设置")]
     float playTime = 30f;     // 出牌时间（秒）
     float warningTime = 5f;   // 警告时间
    
    [Header("视觉效果")]
    public Color normalColor = new Color(0.2f, 0.8f, 0.2f);  // 绿色
    public Color warningColor = new Color(1f, 0.8f, 0.2f);    // 黄色
    public Color dangerColor = new Color(1f, 0.2f, 0.2f);     // 红色
    
    private float currentTime = 0f;
    private bool isPlaying = false;
    private Coroutine timerCoroutine;

    void Start()
    {
        // 初始隐藏进度条
        //if (progressContainer != null)
        //    progressContainer.SetActive(false);
        
        // 确保进度条设置正确
        SetupRadialProgress();

        StartPlayTimer();
    }

    void SetupRadialProgress()
    {
        if (radialProgress != null)
        {
            // 设置Image为圆形填充
            radialProgress.type = Image.Type.Filled;
            radialProgress.fillMethod = Image.FillMethod.Radial360;
            radialProgress.fillOrigin = (int)Image.Origin360.Top; // 从顶部开始
            radialProgress.fillClockwise = true;
            
            // 初始进度
            radialProgress.fillAmount = 1f;
            radialProgress.color = normalColor;
        }
    }

    // 开始出牌倒计时
    public void StartPlayTimer()
    {
        if (isPlaying) return;
        
        isPlaying = true;
        currentTime = playTime;
        
        // 显示进度条
        if (progressContainer != null)
            progressContainer.SetActive(true);
        
        // 重置进度条
        radialProgress.fillAmount = 1f;
        radialProgress.color = normalColor;
        
        // 开始计时协程
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        timerCoroutine = StartCoroutine(PlayTimerCoroutine());
    }

    // 停止倒计时（出牌完成）
    public void StopPlayTimer()
    {
        if (!isPlaying) return;
        
        isPlaying = false;
        
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        
        // 隐藏进度条
        if (progressContainer != null)
            progressContainer.SetActive(false);
    }

    IEnumerator PlayTimerCoroutine()
    {
        while (currentTime > 0 && isPlaying)
        {
            // 更新剩余时间
            currentTime -= Time.deltaTime;
            
            // 计算进度百分比（0-1）
            float progress = currentTime / playTime;
            
            // 更新UI
            UpdateProgressUI(progress);
            
            yield return null;
        }
        
        // 时间到，自动结束
        if (isPlaying)
        {
            OnTimeOut();
        }
    }

    void UpdateProgressUI(float progress)
    {
        // 更新进度条填充
        radialProgress.fillAmount = progress;
        
        // 更新颜色
        UpdateProgressColor(progress);
        
        // 更新倒计时文本
        if (countdownText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            countdownText.text = seconds.ToString();
            
            // 添加警告效果
            if (currentTime <= warningTime)
            {
                countdownText.color = dangerColor;
                
                // 闪烁效果
                float alpha = Mathf.PingPong(Time.time * 2, 0.5f) + 0.5f;
                Color textColor = countdownText.color;
                textColor.a = alpha;
                countdownText.color = textColor;
            }
            else
            {
                countdownText.color = normalColor;
            }
        }
    }

    void UpdateProgressColor(float progress)
    {
        if (progress > warningTime / playTime)
        {
            // 正常时间：绿色
            radialProgress.color = normalColor;
        }
        else if (progress > 0.2f)
        {
            // 警告时间：绿->黄渐变
            float t = (progress * playTime) / warningTime;
            radialProgress.color = Color.Lerp(warningColor, normalColor, t);
        }
        else
        {
            // 危险时间：红->黄渐变，添加闪烁效果
            float t = progress / 0.2f;
            Color baseColor = Color.Lerp(dangerColor, warningColor, t);
            
            // 闪烁效果
            float pulse = Mathf.Sin(Time.time * 10f) * 0.3f + 0.7f;
            radialProgress.color = new Color(
                baseColor.r * pulse,
                baseColor.g * pulse,
                baseColor.b * pulse
            );
        }
    }

    void OnTimeOut()
    {
        Debug.Log("出牌时间到！");
        
        // 触发超时事件
        // 这里可以播放音效、特效等
        
        // 强制出牌或执行其他逻辑
        // CardManager.Instance.AutoPlayCard();
        
        StopPlayTimer();
    }
}
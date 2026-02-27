using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 使用传统的 ScrollRect 创建表格
/// 不需要 Unity UI Extensions
/// </summary>
public class SimpleTable : MonoBehaviour
{
    [Header("UI 引用")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public GameObject rowPrefab;
    
    [Header("表格设置")]
    public int columnCount = 4;
    public float rowHeight = 40f;
    public Vector2 spacing = new Vector2(5, 5);
    
    [Header("表头")]
    public string[] headers = { "ID", "名称", "等级", "分数" };
    
    private List<GameObject> rows = new List<GameObject>();
    
    void Start()
    {
        InitializeTable();
        PopulateSampleData();
    }
    
    /// <summary>
    /// 初始化表格
    /// </summary>
    void InitializeTable()
    {
        if (content == null && scrollRect != null)
            content = scrollRect.content;
        
        if (content == null)
        {
            Debug.LogError("请设置 Content RectTransform");
            return;
        }
        
        // 清除现有内容
        ClearTable();
        
        // 创建表头
        CreateHeader();
    }
    
    /// <summary>
    /// 创建表头
    /// </summary>
    void CreateHeader()
    {
        if (rowPrefab == null)
        {
            Debug.LogError("请设置行预制体");
            return;
        }
        
        GameObject headerRow = CreateRow("Header");
        
        // 设置表头样式
        Image bg = headerRow.GetComponent<Image>();
        if (bg != null)
            bg.color = new Color(0.2f, 0.4f, 0.6f, 1f);
        
        // 添加表头文本
        for (int i = 0; i < columnCount; i++)
        {
            GameObject cell = CreateCell(headerRow.transform);
            Text text = cell.GetComponentInChildren<Text>();
            if (text != null && i < headers.Length)
            {
                text.text = headers[i];
                text.color = Color.white;
                text.fontStyle = FontStyle.Bold;
            }
        }
    }
    
    /// <summary>
    /// 填充示例数据
    /// </summary>
    void PopulateSampleData()
    {
        // 添加示例数据
        AddRow(new string[] { "001", "张三", "15", "2500" });
        AddRow(new string[] { "002", "李四", "22", "3800" });
        AddRow(new string[] { "003", "王五", "8", "1200" });
        AddRow(new string[] { "004", "赵六", "35", "5200" });
        AddRow(new string[] { "005", "孙七", "19", "3100" });
    }
    
    /// <summary>
    /// 添加数据行
    /// </summary>
    public void AddRow(string[] cellData)
    {
        GameObject row = CreateRow($"Row_{rows.Count}");
        
        // 设置交替行颜色
        Image bg = row.GetComponent<Image>();
        if (bg != null)
        {
            bg.color = (rows.Count % 2 == 0) ? 
                Color.white : 
                new Color(0.95f, 0.95f, 0.95f, 1f);
        }
        
        // 添加单元格数据
        for (int i = 0; i < Mathf.Min(columnCount, cellData.Length); i++)
        {
            GameObject cell = CreateCell(row.transform);
            Text text = cell.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = cellData[i];
            }
        }
        
        rows.Add(row);
        
        // 更新 Content 高度
        UpdateContentHeight();
    }
    
    /// <summary>
    /// 创建行
    /// </summary>
    GameObject CreateRow(string name)
    {
        GameObject row;
        
        if (rowPrefab != null)
        {
            row = Instantiate(rowPrefab, content);
            row.name = name;
        }
        else
        {
            // 动态创建行
            row = new GameObject(name);
            row.transform.SetParent(content);
            
            // 添加 RectTransform
            RectTransform rt = row.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, rowHeight);
            
            // 添加 Image
            Image img = row.AddComponent<Image>();
            img.color = Color.white;
            
            // 添加 Horizontal Layout Group
            HorizontalLayoutGroup hlg = row.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = spacing.x;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            
            // 添加 Layout Element
            LayoutElement layout = row.AddComponent<LayoutElement>();
            layout.minHeight = rowHeight;
            layout.preferredHeight = rowHeight;
        }
        
        return row;
    }
    
    /// <summary>
    /// 创建单元格
    /// </summary>
    GameObject CreateCell(Transform parent)
    {
        GameObject cell = new GameObject("Cell");
        cell.transform.SetParent(parent);
        
        // 添加 RectTransform
        RectTransform rt = cell.AddComponent<RectTransform>();
        
        // 添加 Layout Element
        LayoutElement layout = cell.AddComponent<LayoutElement>();
        layout.flexibleWidth = 1;
        layout.preferredWidth = 100;
        
        // 创建文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(cell.transform);
        
        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 14;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;
        
        // 设置文本位置
        RectTransform textRt = textObj.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(10, 0);
        textRt.offsetMax = new Vector2(-10, 0);
        
        return cell;
    }
    
    /// <summary>
    /// 更新 Content 高度
    /// </summary>
    void UpdateContentHeight()
    {
        if (content == null) return;
        
        float totalHeight = (rows.Count + 1) * rowHeight + // +1 为表头
                           (rows.Count) * spacing.y; // 间距
        
        content.sizeDelta = new Vector2(content.sizeDelta.x, totalHeight);
    }
    
    /// <summary>
    /// 清空表格
    /// </summary>
    public void ClearTable()
    {
        foreach (GameObject row in rows)
        {
            Destroy(row);
        }
        rows.Clear();
        
        // 清空 Content
        if (content != null)
        {
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
    }
    
    /// <summary>
    /// 搜索表格
    /// </summary>
    public void SearchTable(string keyword)
    {
        if (string.IsNullOrEmpty(keyword))
        {
            // 显示所有行
            foreach (Transform child in content)
            {
                if (child.name != "Header")
                    child.gameObject.SetActive(true);
            }
            return;
        }
        
        keyword = keyword.ToLower();
        
        // 跳过表头
        for (int i = 1; i < content.childCount; i++)
        {
            Transform row = content.GetChild(i);
            bool found = false;
            
            // 检查每个单元格
            foreach (Transform cell in row)
            {
                Text text = cell.GetComponentInChildren<Text>();
                if (text != null && text.text.ToLower().Contains(keyword))
                {
                    found = true;
                    break;
                }
            }
            
            row.gameObject.SetActive(found);
        }
    }
    
    /// <summary>
    /// 排序表格
    /// </summary>
    public void SortTable(int columnIndex, bool ascending = true)
    {
        // 这里可以实现排序逻辑
        // 需要保存数据并重新排序后重新生成表格
    }
}
using UnityEngine;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine.Networking;
using System.IO;  // File类在Android上可用，但不能读APK内部

/// <summary>
/// XML文件管理器（跨平台）
/// </summary>
public class XMLManagerForAndroid
{
    [Header("文件设置")]
    [Tooltip("XML文件名（不需要.xml扩展名）")]
    public string fileName = "computeRules";

    [Tooltip("保存路径类型")]
    public PathType pathType = PathType.StreamingAssets;

    private static Encoding gb2312Encoding = null;

    public enum PathType
    {
        StreamingAssets,
        PersistentDataPath,
        Custom
    }

    /// <summary>
    /// 获取完整的文件路径
    /// </summary>
    public string GetFilePath()
    {
        switch (pathType)
        {
            case PathType.StreamingAssets:
                return Application.streamingAssetsPath + "/" + fileName + ".xml";
            case PathType.PersistentDataPath:
                return Application.persistentDataPath + "/" + fileName + ".xml";
            case PathType.Custom:
                return fileName + ".xml";
            default:
                return "";
        }
    }

    /// <summary>
    /// 读取XML文件内容
    /// </summary>
    public string ReadXmlContent()
    {
        string filePath = GetFilePath();
        Debug.Log($"尝试读取XML文件: {filePath}");

        // Android平台处理
#if UNITY_ANDROID && !UNITY_EDITOR
            return ReadOnAndroid(filePath);
#else
        // 其他平台直接用File
        return ReadOnOther(filePath);
#endif
    }

    /// <summary>
    /// Android平台读取
    /// </summary>
    private string ReadOnAndroid(string path)
    {
        try
        {
            // 情况1：StreamingAssets路径（在APK内，不能用File）
            if (path.Contains(Application.streamingAssetsPath))
            {
                Debug.Log("Android: 从StreamingAssets读取（APK内部）");
                return ReadFromApkWithUnityWebRequest(path);
            }
            // 情况2：PersistentDataPath路径（可读写，可以用File）
            else if (path.Contains(Application.persistentDataPath))
            {
                Debug.Log("Android: 从PersistentDataPath读取（外部存储）");
                if (File.Exists(path))
                {
                    return File.ReadAllText(path, GetGB2312Encoding());
                }
                else
                {
                    Debug.LogError($"文件不存在: {path}");
                    return "";
                }
            }
            // 情况3：其他路径
            else
            {
                Debug.Log("Android: 从其他路径读取");
                if (File.Exists(path))
                {
                    return File.ReadAllText(path, GetGB2312Encoding());
                }
                else
                {
                    Debug.LogError($"文件不存在: {path}");
                    return "";
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Android读取失败: {e.Message}");
            return "";
        }
    }

    /// <summary>
    /// 非Android平台读取
    /// </summary>
    private string ReadOnOther(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path, GetGB2312Encoding());
            }
            else
            {
                Debug.LogError($"文件不存在: {path}");
                return "";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"读取失败: {e.Message}");
            return "";
        }
    }

    /// <summary>
    /// 使用UnityWebRequest从APK读取（仅Android StreamingAssets）
    /// </summary>
    private string ReadFromApkWithUnityWebRequest(string path)
    {
        try
        {
            UnityWebRequest request = UnityWebRequest.Get(path);
            request.SendWebRequest();

            // 同步等待
            float timeout = 5f;
            float startTime = Time.realtimeSinceStartup;

            while (!request.isDone)
            {
                if (Time.realtimeSinceStartup - startTime > timeout)
                {
                    Debug.LogError($"读取超时: {path}");
                    request.Dispose();
                    return "";
                }
                System.Threading.Thread.Sleep(10);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                byte[] rawData = request.downloadHandler.data;
                Debug.Log($"成功读取 {rawData.Length} 字节");
                return GetGB2312Encoding().GetString(rawData);
            }
            else
            {
                Debug.LogError($"读取失败: {request.error}");
                return "";
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UnityWebRequest异常: {e.Message}");
            return "";
        }
    }

    /// <summary>
    /// 保存XML内容（只能保存到PersistentDataPath）
    /// </summary>
    public bool SaveXmlContent(string content)
    {
        string filePath = GetFilePath();

        // 只能保存到PersistentDataPath
        if (!filePath.Contains(Application.persistentDataPath))
        {
            Debug.LogError($"只能保存到PersistentDataPath: {filePath}");
            return false;
        }

        try
        {
            // 确保目录存在
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, content, GetGB2312Encoding());
            Debug.Log($"保存成功: {filePath}");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 获取GB2312编码
    /// </summary>
    private Encoding GetGB2312Encoding()
    {
        if (gb2312Encoding != null)
            return gb2312Encoding;

        try
        {
            gb2312Encoding = Encoding.GetEncoding("gb2312");
        }
        catch
        {
            try
            {
                gb2312Encoding = Encoding.GetEncoding(936);
            }
            catch
            {
                Debug.LogWarning("GB2312编码不可用，使用UTF-8");
                gb2312Encoding = Encoding.UTF8;
            }
        }
        return gb2312Encoding;
    }

    #region XML与DataTable转换

    public DataTable XmlToDataTable(string xmlStr)
    {
        if (string.IsNullOrEmpty(xmlStr))
            return null;

        System.IO.StringReader strStream = null;
        XmlTextReader xmlReader = null;

        try
        {
            DataSet ds = new DataSet();
            strStream = new System.IO.StringReader(xmlStr);
            xmlReader = new XmlTextReader(strStream);
            ds.ReadXml(xmlReader);
            return ds.Tables.Count > 0 ? ds.Tables[0] : null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"XML转DataTable失败: {e.Message}");
            return null;
        }
        finally
        {
            xmlReader?.Close();
            strStream?.Close();
            strStream?.Dispose();
        }
    }

    public string DataTableToXml(DataTable table)
    {
        if (table == null)
            return "";

        System.IO.StringWriter writer = null;
        try
        {
            writer = new System.IO.StringWriter();
            table.WriteXml(writer);
            return writer.ToString();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DataTable转XML失败: {e.Message}");
            return "";
        }
        finally
        {
            writer?.Close();
            writer?.Dispose();
        }
    }

    #endregion

    #region 便捷方法

    public DataTable ReadXmlToDataTable()
    {
        string content = ReadXmlContent();
        if (string.IsNullOrEmpty(content))
        {
            Debug.LogError("读取到的XML内容为空");
            return null;
        }
        return XmlToDataTable(content);
    }

    public bool SaveDataTableToXml(DataTable table)
    {
        string xml = DataTableToXml(table);
        if (string.IsNullOrEmpty(xml))
            return false;
        return SaveXmlContent(xml);
    }

    #endregion
}
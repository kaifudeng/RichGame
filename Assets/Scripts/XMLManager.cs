using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;
using System.Data;
/// <summary>
/// XML文件管理器，用于读取、修改和保存XML文件
/// </summary>
public class XMLManager
{
    [Header("文件设置")]
    [Tooltip("XML文件名（不需要.xml扩展名）")]
    public string fileName = "computeRules";

    [Tooltip("保存路径类型")]
    public PathType pathType = PathType.StreamingAssets;

    //[Header("测试数据")]
    //public TestData sampleData;

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
        string path = "";

        switch (pathType)
        {
            case PathType.StreamingAssets:
                path = Path.Combine(Application.streamingAssetsPath, fileName + ".xml");
                break;
            case PathType.PersistentDataPath:
                path = Path.Combine(Application.persistentDataPath, fileName + ".xml");
                break;
            case PathType.Custom:
                path = fileName + ".xml";
                break;
        }

        return path;
    }

    #region xml与DataTable相互转化的方法
    /// <summary>
    /// 将XML生成DataTable
    /// </summary>
    /// <param name="xmlStr">XML字符串</param>
    /// <returns></returns>
    public DataTable XmlToDataTable(string xmlStr)
    {
        if (!string.IsNullOrEmpty(xmlStr))
        {
            StringReader StrStream = null;
            XmlTextReader Xmlrdr = null;
            try
            {
                DataSet ds = new DataSet();
                //读取字符串中的信息
                StrStream = new StringReader(xmlStr);
                //获取StrStream中的数据
                Xmlrdr = new XmlTextReader(StrStream);
                //ds获取Xmlrdr中的数据               
                ds.ReadXml(Xmlrdr);
                return ds.Tables[0];
            }
            catch
            {
                return null;
            }
            finally
            {
                //释放资源
                if (Xmlrdr != null)
                {
                    Xmlrdr.Close();
                    StrStream.Close();
                    StrStream.Dispose();
                }
            }
        }
        return null;
    }


    /// <summary>
    /// 将datatable转为xml
    /// </summary>
    /// <param name="vTable">要生成XML的DataTable</param>
    /// <returns></returns>
    public string DataTable2Xml(DataTable vTable)
    {
        try
        {
            if (null == vTable) return string.Empty;
            StringWriter writer = new StringWriter();
            vTable.WriteXml(writer);
            string xmlstr = writer.ToString();
            writer.Close();
            return xmlstr;
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion
}
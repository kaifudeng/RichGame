
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class DatatableForm : MonoBehaviour
{
    public GameObject cellPrefab;

    public string FileName;

    public DataTable data;
    // Start is called before the first frame update
    void Start()
    {
        refreshDataView();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OpenFile(string filename)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            Debug.Log("当前为移动平台");
            XMLManagerForAndroid xMLManager = new XMLManagerForAndroid();
            xMLManager.fileName=filename;
            xMLManager.pathType = XMLManagerForAndroid.PathType.PersistentDataPath;
            data = xMLManager.ReadXmlToDataTable();
        }
        catch(Exception ex) 
        {
            Debug.LogError(ex);
        }
#else
        try
        {
            Debug.Log("当前为PC平台");
            XMLManager xMLManager = new XMLManager();
            xMLManager.fileName = filename;
            string StrPath = xMLManager.GetFilePath();
            string Astr = File.ReadAllText(StrPath, Encoding.GetEncoding("gb2312"));
            data = xMLManager.XmlToDataTable(Astr);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
        #endif 
   }

    public void UpdateData(int rowIndex, int colIndex, string val)
    {

        data.Rows[rowIndex][colIndex] = val;
        SaveData();
    }

    void SaveData()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            Debug.Log("当前为移动平台");
            XMLManagerForAndroid xMLManager = new XMLManagerForAndroid();
            xMLManager.fileName = FileName;
            xMLManager.pathType = XMLManagerForAndroid.PathType.PersistentDataPath;
            xMLManager.SaveDataTableToXml(data);
        }
        catch(Exception ex) 
        {
            Debug.LogError(ex);
        }
#else
        try
        {
            Debug.Log("当前为PC平台");
            XMLManager xMLManager = new XMLManager();
            xMLManager.fileName = FileName;
            string theXml = xMLManager.DataTable2Xml(data);
            File.WriteAllText(xMLManager.GetFilePath(), theXml, Encoding.GetEncoding("gb2312"));
        }
        catch (Exception ex)
        { Debug.LogError(ex); }
#endif
        refreshDataView();
    }
    private void refreshDataView()
    {
        clearView();
        OpenFile(FileName);
        try
        {
            if (FileName == "computeRules")
            {
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    GameObject theCell = Instantiate(cellPrefab, transform);
                    //TextMeshProUGUI text=theCell.GetComponentInChildren<TextMeshProUGUI>();
                    //text.text = row.ItemArray[j].ToString();

                    TableCell tableCell = theCell.GetComponentInChildren<TableCell>();
                    tableCell.SetIndex(0, 0);
                    tableCell.ButtonText.text = data.Columns[i].ColumnName.ToString();
                }
            }
            if (FileName == "companies")
            {
                string[] strs = { "公司名称", "公司等级", " ", " ", "股票发行上限", "初始股价", "( )", " " };
                for (int i = 0; i < data.Columns.Count; i++)
                {
                    GameObject theCell = Instantiate(cellPrefab, transform);
                    //TextMeshProUGUI text=theCell.GetComponentInChildren<TextMeshProUGUI>();
                    //text.text = row.ItemArray[j].ToString();

                    TableCell tableCell = theCell.GetComponentInChildren<TableCell>();
                    tableCell.SetIndex(0, 0);
                    tableCell.ButtonText.text = strs[i];
                }
            }
            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow row = data.Rows[i];
                for (int j = 0; j < row.ItemArray.Length; j++)
                {
                    GameObject theCell = Instantiate(cellPrefab, transform);
                    //TextMeshProUGUI text=theCell.GetComponentInChildren<TextMeshProUGUI>();
                    //text.text = row.ItemArray[j].ToString();

                    TableCell tableCell = theCell.GetComponentInChildren<TableCell>();
                    tableCell.SetIndex(i, j);
                    tableCell.ButtonText.text = row.ItemArray[j].ToString();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);

        }
    }

    void clearView()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

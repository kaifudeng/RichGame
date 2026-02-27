using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PlayersDatatable : MonoBehaviour
{
    public DataTable data;

    public GameObject cellPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitDataTable(DataTable data)
    {
        this.data = data;
        refreshDataView();
    }

    private void refreshDataView()
    {
        clearView();
        try
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TableCell : MonoBehaviour
{

    public GameObject theButton;

    public GameObject inputObj;

    public TextMeshProUGUI ButtonText;

    public TMP_InputField inputField;

    [SerializeField] private float doubleClickTime = 0.3f; // 双击间隔时间

    private float lastClickTime = 0f;

    public int rowIndex;
    public int colIndex;

    public void SetIndex(int row,int col)
    {
        rowIndex = row;
        colIndex = col;
    }
    protected virtual void OnDoubleClick()
    {
        Debug.Log($"{gameObject.name} 被双击了！第"+rowIndex.ToString()+"行，第"+colIndex.ToString()+"列");

        theButton.SetActive(false);
        inputObj.SetActive(true);

        inputField.text=ButtonText.text;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButtonClick()
    {
        float currentTime = Time.unscaledTime;
        float timeSinceLastClick = currentTime - lastClickTime;

        // 判断是否为双击
        if (timeSinceLastClick <= doubleClickTime)
        {
            OnDoubleClick();
            lastClickTime = 0; // 重置
        }
        else
        {
            lastClickTime = currentTime;
        }
    }

    public void OnEndEdit()
    {
        theButton.SetActive(true);
        inputObj.SetActive(false);
        updateDataTable(inputField.text);
    }

    private void updateDataTable(string newText)
    {
        DatatableForm theform= GetComponentInParent<DatatableForm>();
        theform.UpdateData(rowIndex,colIndex, newText); 
    }
}

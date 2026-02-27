using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class StockSetForm : NetworkBehaviour
{
    public GameStage Stage;
    public string companyName;
    public int stocksum;
    private TaskCompletionSource<int> currentDecision;
    public GameObject theText;
    public GameObject theSellPrefab;
   // public GameObject theReplacePrefab;
    internal Player thePlayer;
    public Company comp;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Stock stock in thePlayer.stocks) 
        {
            if (stock.Company.CompanyName == comp.CompanyName)
            {
                TextMeshProUGUI textMesh = theText.transform.GetComponentInChildren<TextMeshProUGUI>();
                stocksum = stock.Sum;
                textMesh.text = "持有股票数量：" + stock.Sum + "\r\n价值：" + stock.Sum * stock.Company.price + "\r\n请选择股票的处理方式：";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Client]
    public async void SellButtonClick()
    {
        GameObject sellForm = Instantiate(theSellPrefab, CanvasUI.GetRect());
        stockSellUI stockSellUIobj = sellForm.GetComponent<stockSellUI>();
        stockSellUIobj.parentForm= this;
        stockSellUIobj.SlidermaxVal = stocksum;
        currentDecision = new TaskCompletionSource<int>();
        var timeoutTask = Task.Delay(3000);
        var decisionTask = currentDecision.Task;

        var completed = await Task.WhenAny(decisionTask, timeoutTask);
        Console.WriteLine("测试返回值"+completed.ToString());
    }
    [Client]
    public void ReplaceButtonClick()
    {
        GameObject sellForm = Instantiate(theSellPrefab, CanvasUI.GetRect());
    }
    [Client]
    public void CancelButtonClick()
    {
        Destroy(gameObject);
    }

    [Command]
    public void ReturnValue(int sum)
    {
        currentDecision.SetResult(sum);
    }
}

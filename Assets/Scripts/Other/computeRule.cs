using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class computeRule
{
    public string companyLevel;
    /// <summary>
    /// 初始股价
    /// </summary>
    int firstPrice;
    /// <summary>
    /// 等级提升需要的地皮数量
    /// </summary>
    public int LevelUpWithSum;
    /// <summary>
    /// 股价每级增长
    /// </summary>
    int priceUpWithWhichLevelUp;
    /// <summary>
    /// 分红（第一股东）
    /// </summary>
    int dividendFirst;
    /// <summary>
    /// 分红（第二股东）
    /// </summary>
    int dividendSecond;
    /// <summary>
    /// 分红（第三股东）
    /// </summary>
    int dividendthird;

    /// <summary>
    /// 分红每级增长
    /// </summary>
    int dividendUpWithWhichLevelUp;

    //void Start()
    //{
    //    dataTable = new DataTable();
    //    dataTable.Columns.Add("公司等级");
    //    dataTable.Columns.Add("初始股价");
    //    dataTable.Columns.Add("股价每级增长");
    //    dataTable.Columns.Add("分红（第一股东）");
    //    dataTable.Columns.Add("分红（第二股东）");
    //    dataTable.Columns.Add("分红（第三股东）");
    //    dataTable.Columns.Add("分红每级增长");
    //    dataTable.Columns.Add("每多少个地皮+1等级");

    //    DataRow row = dataTable.NewRow();
    //    row[0] = "A类公司";
    //    row[1] = "800";
    //    row[2] = "400";
    //    row[3] = "2000";
    //    row[4] = "1000";
    //    row[5] = "500";
    //    row[6] = "500";
    //    row[7] = "3";
    //    DataRow row1 = dataTable.NewRow();
    //    row1[0] = "B类公司";
    //    row1[1] = "600";
    //    row1[2] = "300";
    //    row1[3] = "1500";
    //    row1[4] = "800";
    //    row1[5] = "400";
    //    row1[6] = "400";
    //    row1[7] = "3";
    //    DataRow row2 = dataTable.NewRow();
    //    row2[0] = "C类公司";
    //    row2[1] = "400";
    //    row2[2] = "200";
    //    row2[3] = "1000";
    //    row2[4] = "500";
    //    row2[5] = "300";
    //    row2[6] = "300";
    //    row2[7] = "3";

    //    dataTable.Rows.Add(row);
    //    dataTable.Rows.Add(row1);
    //    dataTable.Rows.Add(row2);
   // }

    public computeRule(string companyLevel, int firstPrice, int levelUpWithSum, int priceUpWithWhichLevelUp, int dividendFirst, int dividendSecond, int dividendthird, int dividendUpWithWhichLevelUp)
    {
        this.companyLevel = companyLevel;
        this.firstPrice = firstPrice;
        LevelUpWithSum = levelUpWithSum;
        this.priceUpWithWhichLevelUp = priceUpWithWhichLevelUp;
        this.dividendFirst = dividendFirst;
        this.dividendSecond = dividendSecond;
        this.dividendthird = dividendthird;
        this.dividendUpWithWhichLevelUp = dividendUpWithWhichLevelUp;
    }

    public int getStockPrice(int theCardCount)
    {
        int sPrice = (theCardCount / LevelUpWithSum) * priceUpWithWhichLevelUp + firstPrice;
        return sPrice;
    }
    /// <summary>
    /// 获取第一股东应该获得的分红金额
    /// </summary>
    /// <param name="theCardCount"></param>
    /// <returns></returns>
    public int getDividendFirst(int theCardCount)
    {
        return (theCardCount / LevelUpWithSum) * dividendFirst;
    }
    public int getDividendSecond(int theCardCount)
    {
        return (theCardCount / LevelUpWithSum) * dividendSecond;
    }
    public int getDividendThird(int theCardCount)
    {
        return (theCardCount / LevelUpWithSum) * dividendthird;
    }

    public override string ToString()
    {
        return companyLevel + "," + firstPrice + "," + LevelUpWithSum + "," + priceUpWithWhichLevelUp + "," + dividendFirst + "," + dividendSecond + "," + dividendthird + "," + dividendUpWithWhichLevelUp;
    }

    public computeRule(string str)
    {
        string[] strings = str.Split(",");
        this.companyLevel = strings[0];
        this.firstPrice =ConvertFuntion.StringToInt(strings[1]);
        LevelUpWithSum = ConvertFuntion.StringToInt(strings[2]);
        this.priceUpWithWhichLevelUp = ConvertFuntion.StringToInt(strings[3]);
        this.dividendFirst = ConvertFuntion.StringToInt(strings[4]);
        this.dividendSecond = ConvertFuntion.StringToInt(strings[5]);
        this.dividendthird = ConvertFuntion.StringToInt(strings[6]);
        this.dividendUpWithWhichLevelUp = ConvertFuntion.StringToInt(strings[7]);
    }
}

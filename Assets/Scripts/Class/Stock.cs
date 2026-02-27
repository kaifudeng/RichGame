using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stock
{
    public Company Company {  get; set; }

    public int Sum {  get; set; }

    public Stock(Company company, int sum)
    {
        Company = company;
        Sum = sum;
    }
}

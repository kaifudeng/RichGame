using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertFuntion
{

    public static int StringToInt(string str)
    {
        if (str == null || str.Length == 0) return -1;
        else
        {
            int result = int.Parse(str);
            return result;
        }
    }

    public static char StringToChar(string str) 
    {
        if (str == null || str.Length == 0) return '#';
        else
        {
            char result = char.Parse(str);
            return result;
        }
    }
}

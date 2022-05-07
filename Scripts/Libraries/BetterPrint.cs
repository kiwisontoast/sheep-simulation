using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterPrint 
{
    public static void PrintArray(float[] Array)
    {
        string Output = "";
        for(int i = 0; i < Array.GetLength(0); i++)
        {
            Output += "" + i + " : " + Array[i] + "\n";
        }
        Debug.Log(Output);
    }
}

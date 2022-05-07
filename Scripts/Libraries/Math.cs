using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using math = System.Math;

public class Math
{

    static public float Warp(float x) //-1 <-> 1
    {
        return (1 / (1 + Mathf.Pow((float)math.E, -x)))*2 - 1;
    }

    static public float random() //returns float between -1,1
    {
        return Random.Range(-1f, 1f);
    }

    static public float random(float Weight) //returns a number between -1,1 multiplied by weight
    {
        return Random.Range(-1f, 1f) * Weight;
    }
    static public float random(float min, float max)
    {
        return Random.Range(min, max);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepClass
{
    public GameObject sheep;
    public Transform cframe;
    private static GameObject MaleSheepPrefab = Resources.Load("MaleSheep") as GameObject;
    private static GameObject FemaleSheepPrefab = Resources.Load("FemaleSheep") as GameObject;
    public int[] tile;
    public int ID;
    public float offset;

    public SheepClass(GameObject instance, int[] pos, int id)
    {
        sheep = instance;
        cframe = instance.transform;
        cframe.position = CoolFunctions.tileToPos(pos) + new Vector3(0, .75f, 0);
        tile = pos;
        ID = id;
        sheep.name = "Sheep" + ID;
        
    }

}
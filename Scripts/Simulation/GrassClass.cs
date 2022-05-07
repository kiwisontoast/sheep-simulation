using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassClass
{
    public GameObject grass;
    public Transform cframe;
    public int ID;

    public GrassClass(GameObject instance, int[] tile, int id)
    {
        grass = instance;
        cframe = instance.transform;
        cframe.position += CoolFunctions.tileToPos(tile);
        ID = id;
    }
}

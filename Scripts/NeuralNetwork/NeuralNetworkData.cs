using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetworkData 
{
    public int InputCount;
    public int LayerCount;
    public int NodesPerLayer;
    public int OutputCount;

    public NeuralNetworkData(int ic, int lc, int npl, int oc)
    {
        InputCount = ic;
        LayerCount = lc;
        NodesPerLayer = npl;
        OutputCount = oc;
    }
}

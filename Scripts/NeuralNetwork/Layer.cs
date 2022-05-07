using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class Layer
{
    public float[,] Weights; // [,] means its multi-dimensional float array and in this case there are two dimensions like a xy graph
    public float[] Biases; //Array stores biases(Float array) for each node

    public int InputCount; // Number of inputs
    public int Nodes; // Nodes per layer

    public static float GetDifference(Layer p1, Layer p2)
    {
        float Difference = 0;
        int ic = p1.InputCount;
        int n = p1.Nodes;

        float[,] p1Weights = p1.Weights;
        float[,] p2Weights = p2.Weights;;
        float[] p1Biases = p1.Biases;
        float[] p2Biases = p2.Biases;

        float WeightWeight = 2*ic;

        for (int i = 0; i < n; i++)
        {
            Difference += Mathf.Abs(p1Biases[i] + p2Biases[i])/2;
            for (int v = 0; v < ic; v++)
            {
                Difference += Mathf.Abs(p1Weights[i, v] + p2Weights[i, v])/WeightWeight;
            }
        }
        return Difference;
    }
    

    public Layer(int ic, int n) // Constructor to make a new layer. Makes random layer
    {
        InputCount = ic;
        Nodes = n;
        Weights = new float[Nodes, InputCount]; // The number of nodes and InputCount show the number of elements in the two arrays stored in the Weights array
        Biases = new float[Nodes]; //Biases has as many elements as nodes
        for (int i = 0; i < Nodes; i++)
        {
            Biases[i] = Math.random(); //Stores a random float between -1 and 1 for all the elements of the array
            for (int v = 0; v < InputCount; v++)
            {
                Weights[i, v] = Math.random();
                // Sets random float for each of the element of the two arrays
            }
        }
    }

    public Layer(Layer parent, float Difference) //Second Constructor This new Layer constructor offsets from a parent layer class
    {
        Nodes = parent.Nodes;
        InputCount = parent.InputCount;
        Weights = new float[parent.Nodes, parent.InputCount];
        Biases = new float[parent.Nodes];
        for (int i = 0; i < parent.Nodes; i++)
        {
            Biases[i] = parent.Biases[i] + Math.random(Difference);
            for (int v = 0; v < parent.InputCount; v++)
            {
                Weights[i, v] = parent.Weights[i, v] + Math.random(Difference);
            }
        }
    }

    public Layer(Layer p1, Layer p2, float Difference)
    {
        Nodes = p1.Nodes;
        InputCount = p1.InputCount;
        Weights = new float[p1.Nodes, p1.InputCount];
        Biases = new float[p1.Nodes];
        for (int i = 0; i < p1.Nodes; i++)
        {
            Biases[i] = (p1.Biases[i] + p2.Biases[i])/2 + Math.random(Difference);
            for (int v = 0; v < p1.InputCount; v++)
            {
                Weights[i, v] = (p1.Weights[i, v] + p2.Weights[i, v])/2 + Math.random(Difference);
            }
        }
    }

    public float[] Run(float[] Inputs)
    {
        float[] Outputs = new float[Nodes];
        for (int i = 0; i < Nodes; i++) 
        {
            float newValue = 0f;
            for (int v = 0; v < InputCount; v++) 
            {
                newValue += Inputs[v]*Weights[i, v];
            }
            newValue += Biases[i];
            Outputs[i] = Math.Warp(newValue);
        }
        return Outputs;
    }
}

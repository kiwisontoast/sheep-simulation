using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NeuralNetwork
{
    private int protectiveInt = 100;
    private int InputCount;
    private int LayerCount;
    private int NodesPerLayer;
    private int OutputCount;

    public Layer[] Layers; // Stores an array of Layer Classes


    public NeuralNetworkData NetworkData; //new NeuralNetworkData class

    public static int Clamp(int Value, int ClampValue) //This private function is to protect the cpu
    {
        if (Value > ClampValue)
        {
            return ClampValue;
        }
        return Value;
    }

    public static float GetDifference(NeuralNetwork p1, NeuralNetwork p2)
    {
        float Difference = 0;
        for (int i = 0; i < p1.LayerCount+1; i++)
        {
           Difference += Layer.GetDifference(p1.Layers[i], p2.Layers[i])/(p1.NodesPerLayer+2); 
        }
        return Difference;
    }

    public void SetUpVariables()
    {
        InputCount = Clamp(NetworkData.InputCount, protectiveInt); //Uses Clamp function to protect cpu
        LayerCount = Clamp(NetworkData.LayerCount, protectiveInt);
        NodesPerLayer = Clamp(NetworkData.NodesPerLayer, protectiveInt);
        OutputCount = Clamp(NetworkData.OutputCount, protectiveInt);

        Layers = new Layer[LayerCount+1]; //the plus one accounts for the output layer

    }

    public NeuralNetwork(NeuralNetworkData newNetworkData) //Makes a new NeuralNetwork given the NeuralNetworkData
    {
        NetworkData = newNetworkData; //Assigns the class to the empty variable
        SetUpVariables(); //Uses the function above to keep it safe

        Layers[0] = new Layer(InputCount, NodesPerLayer);
        for (int i = 1; i < LayerCount; i++)
        {
           Layers[i] = new Layer(NodesPerLayer, NodesPerLayer); 
        }
        Layers[LayerCount] = new Layer(NodesPerLayer, OutputCount);
    }

    public NeuralNetwork(NeuralNetwork Parent, float Change)
    {
        NetworkData = Parent.NetworkData;
        SetUpVariables();

        for (int i = 0; i < LayerCount+1; i++)
        {
           Layers[i] = new Layer(Parent.Layers[i], Change); 
        }
    }

    public NeuralNetwork(NeuralNetwork p1, NeuralNetwork p2, float Change, float SpeciesThreshold)
    {
        NetworkData = p1.NetworkData;
        SetUpVariables();


        if (NeuralNetwork.GetDifference(p1,p2) <= SpeciesThreshold)
        {
            for (int i = 0; i < LayerCount+1; i++)
            {
                Layers[i] = new Layer(p1.Layers[i], p2.Layers[i], Change); 
            }

        } 
        else 
        {
            NeuralNetwork Parent;
            if (Random.Range(0, 2) == 0) //Its Exculive so to get a 50/50 chance the options have to be 0 and 1
            {
                Parent = p1;
            }
            else
            {
                Parent = p2;
            }

            for (int i = 0; i < LayerCount+1; i++)
            {
                Layers[i] = new Layer(Parent.Layers[i], Change); 
            }
        }
    }

    public float[] Run(float[] Inputs)
    {
        for (int i = 0; i < LayerCount+1; i++)
        {
           Inputs = Layers[i].Run(Inputs);
        }
        return Inputs;

    }
}

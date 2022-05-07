using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleScript : MonoBehaviour
{
    void Start()
    {

        NeuralNetwork Network = new NeuralNetwork(new NeuralNetworkData(5,3,3,10)); //This will create a Neural Network based on the data you gave

        float[] Inputs = new float[] {Random.Range(0,1),Random.Range(0,1), Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1) };//Test Inputs

        float[] Output = Network.Run(Inputs); //Function of the Network class called Run given the Inputs - Main action

        BetterPrint.PrintArray(Output); //Prints the results in a float array

//This will create a neural Network based on the Network you give it and it will offset the value by a Random.Range * the second Argument
        NeuralNetwork newNetwork = new NeuralNetwork(Network, .1f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

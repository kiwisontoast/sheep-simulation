using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep
{
    public NeuralNetwork brain; //inputs are the 24 tile sense, outputs are going
    public int[] currentTile;   // forwards backwards and rotating 90 degrees for first two, and last decides the task 
    public int hunger = 100;
    public int thirst = 100;
    public bool IsMale;
    public int age = 0;
    public static int[] tasks = new int[4] { 1,2,3,4 };
    public Nullable<int> currentTask;
    public int maxAge = 120;
    public int urgeToReproduce = 0;
    public bool reproducing = false;
    public Sheep Mate;
    public int[] targetTile;
    public int yOrientation = 0;
    public bool justBorn = true;
    public int ID;
    public TileClass GetTile
    {
        get
        {
            return SimulationManager.Map[currentTile[0], currentTile[1]];
        }
    }

    public Sheep(int[] tile, bool ismale, int id)
    {
        brain = new NeuralNetwork(SimulationManager.initialNetwork, .01f);
        currentTile = tile;
        SimulationManager.Map[tile[0], tile[1]].EntityType = 2;
        SimulationManager.Map[tile[0], tile[1]].sheepOccupier = this;
        IsMale = ismale;
        ID = id;

    }

    public Sheep(int[] tile, bool ismale, Sheep p1, Sheep p2, int id)
    {
        brain = new NeuralNetwork(p1.brain, p2.brain, .01f, 1);
        SimulationManager.Map[tile[0], tile[1]].EntityType = 2;
        SimulationManager.Map[tile[0], tile[1]].sheepOccupier = this;
        currentTile = tile;
        IsMale = ismale;
        ID = id;
    }

    public Sheep() { }




    public float[] BrainRun(TileClass[,] Map)
    {
        float[] inputs = new float[27];
        int index = 0;
        int xMin;
        int xMax;
        int zMin;
        int zMax;
        switch (yOrientation)
        {
            case 0:
                xMin = -2;
                xMax = 3;
                zMin = -1;
                zMax = 4;
                break;
            case 90:
                xMin = -1;
                xMax = 4;
                zMin = -2;
                zMax = 3;
                break;
            case 180:
                xMin = -2;
                xMax = 3;
                zMin = -3;
                zMax = 2;
                break;
            case 270:
                xMin = -3;
                xMax = 2;
                zMin = -2;
                zMax = 3;
                break;
            default:
                index = 0;
                xMin = 0;
                xMax = 0;
                zMin = 0;
                zMax = 0;
                break;
        }

        for (int x = xMin; x < xMax; x++)
        {
            for (int z = zMin; z < zMax; z++)
            {
                if (x == 0 && z == 0)
                {
                    continue;
                }
                int xVal = currentTile[0] + x;
                int zVal = currentTile[1] + z;
                if (xVal >= 0 && xVal < Map.GetLength(0) && zVal >= 0 && zVal < Map.GetLength(1)) //checks if there actually is any tile at that position
                {
                    inputs[index] = Map[xVal, zVal].EntityType;
                    index++;
                }
                else
                {
                    inputs[index] = -3; //-3 means there isn't any tile
                    index++;
                }
            }
        }


        inputs[index] = this.hunger / 100;
        index++;
        inputs[index] = this.thirst / 100;
        
        return brain.Run(inputs);
        

    }

    public Tuple<int[], int> useOutputs(float[] Outputs, TileClass[,] Map)
    {
        int[] targettile = new int[2];
        int task;
        int rotateAdd;
        int moveAdd;

        if (Mathf.Abs(Outputs[0]) < .25)
            moveAdd = 0;
        else if (Outputs[0] > 0)
            moveAdd = 1;
        else
            moveAdd = -1;

        if (Mathf.Abs(Outputs[1]) < .25)
            rotateAdd = 0;
        else if (Outputs[1] > 0)
            rotateAdd = 90;
        else
            rotateAdd = -90;


        yOrientation = (yOrientation + rotateAdd + 360) % 360;
        switch (yOrientation)
        {
            case 0:
                targettile = new int[2] { currentTile[0], currentTile[1] + moveAdd };
                break;
            case 90:
                targettile = new int[2] { currentTile[0] + moveAdd, currentTile[1] };
                break;
            case 180:
                targettile = new int[2] { currentTile[0], currentTile[1] - moveAdd };
                break;
            case 270:
                targettile = new int[2] { currentTile[0] - moveAdd, currentTile[1] };
                break;
        }

        if(targettile[0] < 0 || targettile[0] > 99|| targettile[1] < 0 || targettile[1] > 99)
        {
            targettile = null;
        }

        if (Outputs[2] > 0)
        {
            if (Outputs[2] > 0.5f)
                task = 1;
            else
                task = 2;
        }
        else
        {
            if (Outputs[2] > -0.5f)
                task = 3;
            else
                task = 4;
        }

        return new Tuple<int[], int>(targettile, task);
    }
}

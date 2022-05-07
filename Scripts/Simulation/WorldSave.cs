using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldSave
{
    public Turn[] Turns = new Turn[500];


    public WorldSave()
    {

    }

}

[System.Serializable]
public class Turn
{
    public SheepSave[] sheep;
    public GrassSave[] grass;
    public int[] sheepRemove;
    public int[] grassRemove;
    public int grassEaten;

    public Turn(List<int> sheepremove, List<int> grassremove)
    {
        grassEaten = SimulationManager.grasseaten;
        sheepRemove = sheepremove.ToArray();
        grassRemove = grassremove.ToArray();
        this.sheep = new SheepSave[SimulationManager.Sheeps.Count];
        this.grass = new GrassSave[SimulationManager.GrassPatches.Count];
        for (int i = 0; i < this.sheep.Length; i++)
        {
            SheepSave newsheepsave = new SheepSave(SimulationManager.Sheeps[i]);
            this.sheep[i] = newsheepsave;
        }
        for (int i = 0; i < this.grass.Length; i++)
        {
            GrassSave newgrasssave = new GrassSave(SimulationManager.GrassPatches[i]);
            this.grass[i] = newgrasssave;
        }
    }

}

[System.Serializable]
public class GrassSave
{
    public int[] Tile;
    public int id;
    public GrassSave() { }

    public GrassSave(Grass grass)
    {
        Tile = grass.tile;
        id = grass.ID;
    }

}

[System.Serializable]
public class SheepSave
{
    public float age;
    public bool ismale;
    public int[] currenttile;
    public Nullable<int> task;
    public int id;
    public int mateID;
    public int grassID;
    public int[] targetTile;
    public int yOrientation;
    public SheepSave() { }

    public SheepSave(Sheep sheep)
    {
        age = sheep.age;
        ismale = sheep.IsMale;
        currenttile = sheep.currentTile;
        task = sheep.currentTask;
        id = sheep.ID;
        targetTile = sheep.targetTile;
        yOrientation = sheep.yOrientation;

        if (sheep.Mate != null)
        {
            mateID = sheep.Mate.ID;
        }
        if (sheep.currentTask == 2)
        {
            grassID = CoolFunctions.arrToTile(sheep.targetTile, SimulationManager.Map).grassOccupier.ID;
        }

    }

}
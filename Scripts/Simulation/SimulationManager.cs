using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System;

public class SimulationManager : MonoBehaviour
{
    //Constants
    public int mapSize;
    public static int MapSize = 100;
    public static NeuralNetwork initialNetwork;
    //Arrays
    public static List<Sheep> Sheeps = new List<Sheep>();
    public static List<Grass> GrassPatches = new List<Grass>();
    public static TileClass[,] Map = new TileClass[100, 100];
    public static WorldSave[] worldSaves = new WorldSave[20];
    public static List<int> removeGrass;
    public static List<int> removeSheeps;
    public static List<Sheep> newSheeps;

    //References
    public static WorldSave CurrentSave;
    public delegate void run();
    public static run RunFrontEnd;
    public static int idnumber = 0;
    static int totalturns = 0;
    public static int totalSaves = 1;
    public static int i = 0;
    public static int o = 0;
    public static int grasseaten = 0;

    void Awake()
    {
        UIHandler.OnClicked += SimulationStart;
        StartCoroutine(Training.Train(new NeuralNetworkData(26, 5, 5, 3),SaveWorld.ReadData()));
        Training.FillMap(Map);
    }

    static IEnumerator getEmptyPosition(TileClass[,] Map)
    {
        int timesdone = 0;
        while (timesdone < 10)
        {
            timesdone++;
            int newX = Random.Range(0, 100);
            int newZ = Random.Range(0, 100);

            if (Map[newX, newZ].EntityType == 0)
            {
                Grass grass = new Grass(new int[2] { newX, newZ }, idnumber);
                idnumber++;
                GrassPatches.Add(grass);
                yield break;

            }
            yield return null;
        }
    }

    public static int[] GetEmptyPosition(TileClass[,] Map)
    {
        int newX = (int)Random.Range(0, 100);
        int newZ = Random.Range(0, 100);

        if (Map[newX, newZ].EntityType != 0)
        {
            return GetEmptyPosition(Map);
        }
        return new int[2] { newX, newZ };
    }

    static void SwitchCase(int chosenTask, Sheep sheep, Dictionary<int, List<TileClass>> lookTile)
    {
        switch (chosenTask)
        {
            case 1:
                if (lookTile.ContainsKey(2))
                {
                    Sheep mate = null;
                    foreach (TileClass tile in lookTile[2])
                    {
                        if (tile.sheepOccupier != null &&
                            sheep.IsMale != tile.sheepOccupier.IsMale &&
                            sheep.age >= 6 && tile.sheepOccupier.age >= 6 &&
                            sheep.IsMale == false)
                        {
                            mate = tile.sheepOccupier;
                            break;
                        }
                    }
                    if (mate != null)
                    {
                        sheep.Mate = mate;
                        sheep.targetTile = mate.currentTile;
                        mate.Mate = sheep;
                        mate.targetTile = sheep.currentTile;
                        sheep.reproducing = true;
                        mate.reproducing = true;
                        sheep.currentTask = chosenTask;
                        mate.currentTask = chosenTask;
                        bool tileForbabyFound = false;

                        if (lookTile.ContainsKey(0))
                        {
                            newSheeps.Add(new Sheep(lookTile[0][0].GridPos, CoolFunctions.halfRandom(), sheep, sheep.Mate, idnumber));
                            idnumber++;
                            tileForbabyFound = true;
                        }
                        if (!tileForbabyFound)
                        {
                            for (int x = -1; x < 2; x++)
                            {
                                for (int z = -1; z < 2; z++)
                                {
                                    int xVal = mate.currentTile[0] + x;
                                    int zVal = mate.currentTile[1] + z;
                                    if (xVal >= 0 && xVal <= 99 && zVal >= 0 && zVal <= 99)
                                    {
                                        TileClass tile = CoolFunctions.arrToTile(new int[2] { xVal, zVal }, Map);
                                        if (tile.EntityType == 0)
                                        {
                                            newSheeps.Add(new Sheep(tile.GridPos, CoolFunctions.halfRandom(), sheep, sheep.Mate, idnumber));
                                            idnumber++;
                                            tileForbabyFound = true;
                                            break;
                                        }
                                    }

                                }
                                if (tileForbabyFound)
                                {
                                    break;
                                }
                            }
                        }

                    }
                    else
                    {
                        sheep.currentTask = 4;
                        sheep.targetTile = sheep.currentTile;
                    }
                }
                else
                {
                    sheep.currentTask = 4;
                    sheep.targetTile = sheep.currentTile;
                }
                break;
            case 2:
                if (lookTile.ContainsKey(1))
                {
                    TileClass tile = lookTile[1][0];
                    sheep.currentTask = 2;
                    Grass grass = tile.grassOccupier;
                    sheep.hunger = NeuralNetwork.Clamp(sheep.hunger + 10,100);
                    removeGrass.Add(grass.ID);
                    GrassPatches.Remove(grass);
                    tile.EntityType = 0;
                    tile.grassOccupier = null;
                    grasseaten++;
                    sheep.targetTile = tile.GridPos;
                }
                else
                {
                    sheep.currentTask = 4;
                    sheep.targetTile = sheep.currentTile;
                }
                break;
            case 3:
                if (lookTile.ContainsKey(-1))
                {
                    TileClass tile = lookTile[-1][0];
                    sheep.currentTask = chosenTask;
                    sheep.thirst = NeuralNetwork.Clamp(sheep.thirst + 10, 100);
                    sheep.targetTile = tile.GridPos;
                    break;
                }
                else
                {
                    sheep.currentTask = 4;
                    sheep.targetTile = sheep.currentTile;
                }
                break;
            case 4:
                sheep.currentTask = 4;
                sheep.targetTile = sheep.currentTile;
                break;
        }
    }

    public IEnumerator GetData()
    {

        for (i = 0; i < totalSaves; i++)
        {

            CurrentSave = new WorldSave();
            for (o = 0; o < 500; o++)
            {
                totalturns++;
                newSheeps = new List<Sheep>();
                removeSheeps = new List<int>();
                removeGrass = new List<int>();

                foreach (Sheep sheep in Sheeps)
                {
                    sheep.currentTask = null;
                    sheep.reproducing = false;
                }

                foreach (Sheep sheep in Sheeps.ToArray())
                {
                    if (Aging(sheep))
                    {

                        Sheeps.Remove(sheep);
                        TileClass tile = sheep.GetTile;
                        tile.EntityType = 0;
                        tile.sheepOccupier = null;
                        removeSheeps.Add(sheep.ID);
                        continue;
                    }
                    if (sheep != null && sheep.currentTask == null)
                    {
                        float[] Outputs = sheep.BrainRun(Map);
                        Tuple<int[], int> outputs = sheep.useOutputs(Outputs, Map);
                        TileClass outputTile = null;
                        if (outputs.Item1 != null)
                        {
                            outputTile = CoolFunctions.arrToTile(outputs.Item1, Map);
                        }
                        int chosenTask = outputs.Item2;
                        Dictionary<int, List<TileClass>> lookTile = new Dictionary<int, List<TileClass>>();
                        if (outputTile != null)
                        {
                            if (outputTile.EntityType == 0 && outputs.Item1 != sheep.currentTile)
                            {
                                sheep.GetTile.EntityType = 0;
                                sheep.GetTile.sheepOccupier = null;
                                outputTile.EntityType = 2;
                                sheep.currentTile = outputs.Item1;
                                outputTile.sheepOccupier = sheep;
                            }
                            for (int x = -1; x < 2; x++)
                            {
                                for (int z = -1; z < 2; z++)
                                {
                                    if (x == 0 && z == 0)
                                    {
                                        continue;
                                    }
                                    int xVal = sheep.currentTile[0] + x;
                                    int zVal = sheep.currentTile[1] + z;

                                    if (xVal >= 0 && xVal < 100 && zVal >= 0 && zVal < 100)
                                    {
                                        TileClass tile = CoolFunctions.arrToTile(new int[2] { xVal, zVal }, Map);
                                        if (lookTile.ContainsKey(tile.EntityType))
                                        {
                                            lookTile[tile.EntityType].Add(tile);
                                        }
                                        else
                                        {
                                            lookTile.Add(tile.EntityType, new List<TileClass>());
                                            lookTile[tile.EntityType].Add(tile);
                                        }
                                    }
                                }
                            }
                            SwitchCase(chosenTask, sheep, lookTile);
                        }
                        //else
                        //{
                        //    for (int x = -1; x < 2; x++)
                        //    {
                        //        for (int z = -1; z < 2; z++)
                        //        {
                        //            if (x == 0 && z == 0)
                        //            {
                        //                continue;
                        //            }
                        //            int xVal = sheep.currentTile[0] + x;
                        //            int zVal = sheep.currentTile[1] + z;

                        //            if (xVal >= 0 && xVal < 100 && zVal >= 0 && zVal < 100)
                        //            {
                        //                TileClass tile = CoolFunctions.arrToTile(new int[2] { xVal, zVal }, Map);
                        //                if (lookTile.ContainsKey(tile.EntityType))
                        //                {
                        //                    lookTile[tile.EntityType].Add(tile);
                        //                }
                        //                else
                        //                {
                        //                    lookTile.Add(tile.EntityType, new List<TileClass>());
                        //                    lookTile[tile.EntityType].Add(tile);
                        //                }
                        //            }
                        //        }
                        //    }
                        //    SwitchCase(chosenTask, sheep, lookTile, newSheeps, removeGrass);
                        //}
                    }
                }
                float randomChance = Random.Range(0, 10);
                if (randomChance <= 1)
                {
                    StartCoroutine(getEmptyPosition(Map));

                }
                CurrentSave.Turns[o] = new Turn(removeSheeps, removeGrass);
                yield return null;
                if (newSheeps.Count > 0)
                {
                    Sheeps.AddRange(newSheeps);
                }
            }
            yield return new WaitForSeconds(.5f);
            worldSaves[i] = CurrentSave;
            GC.Collect();
            if (Sheeps.Count == 0)
            {
                RunFrontEnd();
                yield break;
            }
        }
        RunFrontEnd();
        yield return null;

    }

    public static bool Aging(Sheep specificsheep)
    {
        if (specificsheep.age + 1 >= specificsheep.maxAge || specificsheep.hunger <= 0 || specificsheep.thirst <= 0)
        {
            return true;
        }
        else
        {
            specificsheep.age += 1;
            specificsheep.hunger -= 5;
            specificsheep.thirst -= 2;
            return false;
        }
    }


    void SimulationStart() //this first function will encompass the first world save
    {
        CoolFunctions.ClearMap(Map);
        worldSaves = new WorldSave[20];
        Sheeps.Clear();
        GrassPatches.Clear();
        for (int i = 0; i < 100; i++)
        {
            bool genderIsMale = CoolFunctions.halfRandom();
            int[] specificTileClass = GetEmptyPosition(Map);
            Sheeps.Add(new Sheep(specificTileClass, genderIsMale, idnumber));
            idnumber++;
        }

        for (int i = 0; i < 200; i++)
        {
            int[] emptyTile = GetEmptyPosition(Map);
            new Grass(emptyTile, idnumber);
            idnumber++;
        }
        StartCoroutine(GetData());
    }
}
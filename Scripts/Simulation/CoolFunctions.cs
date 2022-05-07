using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoolFunctions
{
    public static bool Wait(float forUpdate, float sinceLastFrame, float timeInterval)
    {
        if (timeInterval <= sinceLastFrame + forUpdate)
        {
            forUpdate = forUpdate + sinceLastFrame - timeInterval;
            return true;
        }
        else
        {
            forUpdate += sinceLastFrame;
            return false;
        }
    }

    public static bool halfRandom()
    {
        float chance = Random.Range(0, 2);
        if (chance >= 1)
        {
            return true;
        }
        return false;
    }


    public static TileClass arrToTile(int[] arr, TileClass[,] Map)
    {
        return Map[arr[0], arr[1]];
   
    }

    public static Vector3 tileToPos(int[] arr)
    {

        return new Vector3((arr[0] * 5) - 247.5f, 0, (arr[1] * 5) - 247.5f);
    }

    public static int tileEntity(int[] arr, TileClass[,] Map)
    {
        return Map[arr[0], arr[1]].EntityType;
        
    }

    public static void ClearMap(TileClass[,] Map)
    {
        for (int x = 0; x < Map.GetLength(0); x++)
        {
            for (int z = 0; z < Map.GetLength(1); z++)
            {
                TileClass tile = Map[x, z];
                if (tile.EntityType > 0)
                {
                    tile.EntityType = 0;
                }
            }
        }
    }
}
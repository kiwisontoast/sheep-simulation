using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public class Grass
{
    public int[] tile;
    public Sheep gettingeatenby = null;
    public int ID;
    public TileClass GetTile
    {
        get
        {
            return SimulationManager.Map[tile[0], tile[1]];
        }
    }

    public Grass(int[] Tile, int id)
    {
        tile = Tile;
        TileClass reference = CoolFunctions.arrToTile(Tile, SimulationManager.Map);
        SimulationManager.Map[Tile[0], Tile[1]].EntityType = -1;
        SimulationManager.GrassPatches.Add(this);
        reference.grassOccupier = this;
        ID = id;

    }

    public Grass() { }
}

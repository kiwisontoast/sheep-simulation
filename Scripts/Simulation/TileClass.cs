using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClass
{
    public Sheep sheepOccupier = null;
    public int[] GridPos;
    public Grass grassOccupier = null;
    public int EntityType = 0; //-2 for obstacles, -1 for water, 0 for an empty tile, 1 for a grass, 2 for sheep


    public TileClass(int[] gridPos)
    {
        GridPos = gridPos;
        if (gridPos[0] >= 40 && gridPos[1] >= 40 && gridPos[0] <= 59 && gridPos[1] <= 59)
        {
            EntityType = -1;
        }
        else
        {
            Vector3 tilePos = CoolFunctions.tileToPos(GridPos);
            float distance = Vector3.Distance(tilePos, new Vector3(0, 0, 0));
            if (distance > 250)
            {
                EntityType = -2;
            }
        }



    }

}

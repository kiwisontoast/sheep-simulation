using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FrontEnd : MonoBehaviour
{
    public Transform FlockTransform;
    public Transform GrassTransform;
    public static Dictionary<int, SheepClass> sheeps = new Dictionary<int, SheepClass>();
    public static Dictionary<int, GrassClass> grassPatches = new Dictionary<int, GrassClass>();
    public static GameObject MaleSheepPrefab;
    public static GameObject FemaleSheepPrefab;
    public static GameObject GrassPatchPrefab;
    public static int currentSave = 0;
    public static int currentTurn = 0;
    public static int totalTurns = 0;
    WorldSave CurrentSave = null;
    public static bool turnFinished = false;
    Vector3 zeroVector = new Vector3(0, 0, 0);
    public delegate void restart();
    public static restart Restart;
    public static bool paused = false;
    public static int setturn;
    public static int turnsinSave = 500;
    public static int GrassEaten = 0;

    void Awake()
    {
        MaleSheepPrefab = Resources.Load("MaleSheep") as GameObject;
        FemaleSheepPrefab = Resources.Load("FemaleSheep") as GameObject;
        GrassPatchPrefab = Resources.Load("GrassPatch") as GameObject;
        SimulationManager.RunFrontEnd += RunFrontEnd;
    }

    void Aging(SheepSave sheep, SheepClass sheep1)
    {
        float factor = (sheep.age / 40) + 1;
        sheep1.cframe.localScale = new Vector3(factor, factor, factor);
        sheep1.offset = 1.25f * (factor - 1) + .75f;
        sheep1.cframe.position = new Vector3(sheep1.cframe.position.x, sheep1.offset, sheep1.cframe.position.z);

    }

    IEnumerator RunTurn()
    {
        totalTurns++;
        Turn turn = LoadTurn();
        if (turn == null)
        {
            GrassEaten = 0;
            foreach (Transform child in FlockTransform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GrassTransform)
            {
                Destroy(child.gameObject);
            }
            sheeps.Clear();
            grassPatches.Clear();
            currentSave = 0;
            currentTurn = 0;
            Restart();
            yield break;
        }
        turnFinished = false;
        StartCoroutine(TurnManager(turn));
        while (!turnFinished)
        {
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
        StartCoroutine(RunTurn());
    }
    void RunTask(SheepClass sheep, SheepSave save)
    {
        if(save.targetTile!= null && save.currenttile != save.targetTile)
        {
            Vector3 relativePos = sheep.cframe.position - CoolFunctions.tileToPos(save.targetTile) + new Vector3(0, sheep.cframe.position.y, 0);
            if (relativePos != zeroVector)
            {
                sheep.cframe.rotation = Quaternion.LookRotation(relativePos);
            }
        }
        
        
    }

    Turn LoadTurn()
    {
        if (!paused)
        {
            Turn turn = null; //This part loads the turn
            if (CurrentSave == null)
            {
                CurrentSave = SimulationManager.worldSaves[currentSave];
                currentSave++;
                turn = CurrentSave.Turns[currentTurn];
                currentTurn++;
            }
            else if (currentTurn == 500)
            {
                if (currentSave < 20)
                {
                    CurrentSave = SimulationManager.worldSaves[currentSave];
                    currentSave++;
                    currentTurn = 0;
                    if(CurrentSave != null)
                    {
                        turn = CurrentSave.Turns[currentTurn];
                    }                    
                    currentTurn++;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                turn = CurrentSave.Turns[currentTurn];
                currentTurn++;
            }
            return turn;
        }
        else
        {
            currentSave = ((setturn - setturn % turnsinSave) / turnsinSave);
            CurrentSave = SimulationManager.worldSaves[currentSave];
            currentSave++;
            currentTurn = (setturn - 1) % (turnsinSave - 1);
            Turn turn = null;
            if(CurrentSave != null && CurrentSave.Turns != null)
            {
                turn = CurrentSave.Turns[currentTurn];
            }
            currentTurn++;
            paused = false;
            foreach(Transform child in FlockTransform)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in GrassTransform)
            {
                Destroy(child.gameObject);
            }
            sheeps.Clear();
            grassPatches.Clear();
            return turn;

        }

    }

    void RunFrontEnd()
    {
        StartCoroutine(RunTurn());
    }

    IEnumerator MoveToTile(SheepClass sheep, SheepSave save, bool[] bools, int index)
    {
        Vector3 movePos = CoolFunctions.tileToPos(save.currenttile) + new Vector3(0, sheep.cframe.position.y, 0);
        Vector3 relativePos = sheep.cframe.position - movePos;
        if (relativePos != zeroVector)
        {
            sheep.cframe.rotation = Quaternion.LookRotation(relativePos);
        }
        while (sheep.cframe.position != movePos)
        {
            sheep.cframe.position = Vector3.MoveTowards(sheep.cframe.position, movePos, 10 * Time.deltaTime);
            yield return null;
        }
        RunTask(sheep, save);
        bools[index] = true;
    }

    IEnumerator TurnManager(Turn turn)
    {
        bool[] coroutinerun = new bool[turn.sheep.Length];
        GrassEaten = turn.grassEaten;

        for (int i = 0; i < turn.grass.Length; i++)//adding new grass
        {
            GrassSave save = turn.grass[i];
            if (!grassPatches.ContainsKey(save.id))
            {
                GameObject newGrass = Instantiate(GrassPatchPrefab) as GameObject;
                grassPatches.Add(save.id, new GrassClass(newGrass, save.Tile, save.id));
                newGrass.transform.parent = GrassTransform;
            }

        }
        for (int i = 0; i < turn.grassRemove.Length; i++) //removing grass
        {
            int id = turn.grassRemove[i];
            print("removed grass " + id);
            if (grassPatches.ContainsKey(id))
            {
                Destroy(grassPatches[id].grass);
                grassPatches.Remove(id);
            }
            GameObject grassInstance = grassPatches[id].grass;
            Destroy(grassInstance);
            grassPatches.Remove(id);
        }
        for (int i = 0; i < turn.sheepRemove.Length; i++) // removing sheep
        {
            int id = turn.sheepRemove[i];
            if (sheeps.ContainsKey(id))
            {
                Destroy(sheeps[id].sheep);
                sheeps.Remove(id);
            }                
        }

        for (int i = 0; i < turn.sheep.Length; i++)
        {
            SheepSave save = turn.sheep[i];
            if (!sheeps.ContainsKey(save.id))
            {
                GameObject newSheep = save.ismale ? Instantiate(MaleSheepPrefab) as GameObject : Instantiate(FemaleSheepPrefab) as GameObject;
                sheeps.Add(save.id, new SheepClass(newSheep, save.currenttile, save.id));
                newSheep.transform.parent = FlockTransform;
            }
        }

        for (int i = 0; i < turn.sheep.Length; i++)
        {
            coroutinerun[i] = false;
            SheepSave save = turn.sheep[i];
            SheepClass sheep = null;
            if (!sheeps.ContainsKey(save.id))
            {
                GameObject newSheep = save.ismale ? Instantiate(MaleSheepPrefab) as GameObject : Instantiate(FemaleSheepPrefab) as GameObject;
                sheep = new SheepClass(newSheep, save.currenttile, save.id);
                sheeps.Add(save.id, sheep);
                newSheep.transform.parent = FlockTransform;
            }
            else
            {
                sheep = sheeps[save.id];
            }
            Aging(save, sheep);
            StartCoroutine(MoveToTile(sheep, save, coroutinerun, i));
        }
        bool finished = false;
        while (!finished)
        {
            finished = true;
            foreach (bool b in coroutinerun)
            {
                if (!b)
                {
                    finished = false;
                    break;
                }
            }
            yield return new WaitForSeconds(1);
        }
        turnFinished = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class Training
{
    const float MinimumPassingScore = .85f;
    const float Change = .004f;
    const int DelayRate = 400;

    //Reproduce, Eat, Drink, Nothing
    static float[] TaskValues = new float[4] { 0, 0.375f, 0.875f, 1 };
    static int[] TaskEntityTypes = new int[4] { 2, 1, -1, 0 };

    const float MoveForward = 1;
    const float StayPut = 0;
    const float MoveBack = -1;
    public static float TopScore = 0;
    public static bool needsTraining = false;

    public static TileClass[,] Map;

    public static void FillMap(TileClass[,] Map)
    {
        for (int i = 0; i < Map.GetLength(0); i++)
        {
            for (int j = 0; j < Map.GetLength(1); j++)
            {
                Map[i, j] = new TileClass(new int[] { i, j });
                //print(Map[i, j].EntityType);
            }
        }
    }

    static float ScoreTaskSelection(NeuralNetwork Brain)
    {
        //Set up
        Sheep sheep = new Sheep();
        sheep.currentTile = new int[] { 1, 1 };
        sheep.brain = Brain;


        //Checking All types
        float Score = 0;
        for (int Task = 0; Task < 4; Task++)
        {
            Map[1, 2].EntityType = TaskEntityTypes[Task];

            float[] Outputs = sheep.BrainRun(Map);

            Score += Mathf.Abs(Outputs[2] - TaskValues[Task]);
        }


        //Clean Up
        Map[1, 2].EntityType = 0;

        return (4 - Score) / 4;
    }

    static float ScoreSimpleMovement(NeuralNetwork Brain)
    {
        //Set up
        Sheep sheep = new Sheep();
        sheep.currentTile = new int[] { 9, 9 };
        sheep.brain = Brain;

        //Checking All types
        float Score = 0;
        for (int Task = 0; Task < 4; Task++)
        {
            //Placing
            int PlacedEntityX = 9 + Random.Range(-2, 3); //Exclusive
            int PlacedEntityY = 9 + Random.Range(3, 5);
            Map[PlacedEntityX, PlacedEntityY].EntityType = TaskEntityTypes[Task];

            float TargetDirection = MoveForward;

            if (Task == 3)
                TargetDirection = StayPut;

            //Scoring

            float[] Outputs = sheep.BrainRun(Map);

            Score += Mathf.Abs(Outputs[0] - TargetDirection) + Mathf.Abs(Outputs[2] - 1); //1 means no task

            //Clean Up
            Map[PlacedEntityX, PlacedEntityY].EntityType = 0;
        }

        return 1 - Score / 4;
    }




    static float ScoreRotation(NeuralNetwork Brain)
    {
        //Set up
        Sheep sheep = new Sheep();
        sheep.currentTile = new int[] { 9, 9 };
        sheep.brain = Brain;

        //Checking All types
        float Score = 0;
        for (int Direction = -1; Direction < 2; Direction++)
        {
            int Task = 1;//Random.Range(0, 4);

            int PlacedEntityX = 9 + Random.Range(1, 3) * Direction; //3 because it is exclusive
            Map[PlacedEntityX, 9].EntityType = TaskEntityTypes[Task];

            float[] Outputs = sheep.BrainRun(Map);

            //Score += Mathf.Abs(Outputs[0]) + Mathf.Abs(Outputs[1] - Direction);
            Score += Mathf.Abs(Outputs[1] - Direction);

            //Clean Up
            Map[PlacedEntityX, 9].EntityType = 0;
        }

        return 1 - Score / 3;
    }

    static float ScoreSimpleSearch(NeuralNetwork Brain)
    {
        //Set up
        Sheep sheep = new Sheep();
        sheep.currentTile = new int[] { 9, 9 };
        sheep.brain = Brain;

        //Score
        float[] Outputs = sheep.BrainRun(Map);

        float Score = Mathf.Abs(Outputs[0] - 1);

        return 1 - Score;
    }

    public static IEnumerator Train(NeuralNetworkData Data, NeuralNetwork isThere)
    {

        Debug.Log("Started");
        if (isThere != null)
        {
            SimulationManager.initialNetwork = isThere;
            Debug.Log("Procede");
            needsTraining = false;
            yield break;

        }
        needsTraining = true;
        Map = new TileClass[20, 20];
        FillMap(Map);

        NeuralNetwork TrainedNetwork1 = new NeuralNetwork(Data);
        NeuralNetwork TrainedNetwork2 = new NeuralNetwork(Data);

        int Tests = 0;

        float N1Score = 0;
        float N2Score = 0;
        float PassingScore = 0;


        Debug.Log("Started Training");

        //Simple Task Selection Training
        TopScore = 0;
        PassingScore = 1 * MinimumPassingScore;
        while (TopScore < PassingScore)
        {
            N1Score = ScoreTaskSelection(TrainedNetwork1);
            N2Score = ScoreTaskSelection(TrainedNetwork2);

            if (N1Score > N2Score)
            {
                TrainedNetwork2 = new NeuralNetwork(TrainedNetwork1, Change);
                TopScore = N1Score;
            }
            else
            {
                TrainedNetwork1 = new NeuralNetwork(TrainedNetwork2, Change);
                TopScore = N2Score;
            }

            Tests++;
            if (Tests % DelayRate == 0)
            {
                Debug.Log(TopScore + ", Task Selection");
                yield return new WaitForSeconds(.01f);
            }
        }


        //Move To Eat/Drink
        TopScore = 0;
        PassingScore = 2 * MinimumPassingScore;
        while (TopScore < PassingScore)
        {
            N1Score = ScoreTaskSelection(TrainedNetwork1);
            N2Score = ScoreTaskSelection(TrainedNetwork2);

            N1Score += ScoreSimpleMovement(TrainedNetwork1);
            N2Score += ScoreSimpleMovement(TrainedNetwork2);


            if (N1Score > N2Score)
            {
                TrainedNetwork2 = new NeuralNetwork(TrainedNetwork1, Change);
                TopScore = N1Score;
            }
            else
            {
                TrainedNetwork1 = new NeuralNetwork(TrainedNetwork2, Change);
                TopScore = N2Score;
            }

            Tests++;
            if (Tests % DelayRate == 0)
            {
                Debug.Log(TopScore + ", SimpleMovement");
                yield return new WaitForSeconds(.01f);
            }
        }


        //Simple Rotation
        TopScore = 0;
        PassingScore = 3 * MinimumPassingScore;
        while (TopScore < PassingScore)
        {
            N1Score = ScoreRotation(TrainedNetwork1);
            N2Score = ScoreRotation(TrainedNetwork2);

            N1Score += ScoreTaskSelection(TrainedNetwork1);
            N2Score += ScoreTaskSelection(TrainedNetwork2);

            N1Score += ScoreSimpleMovement(TrainedNetwork1);
            N2Score += ScoreSimpleMovement(TrainedNetwork2);


            if (N1Score > N2Score)
            {
                TrainedNetwork2 = new NeuralNetwork(TrainedNetwork1, Change);
                TopScore = N1Score;
            }
            else
            {
                TrainedNetwork1 = new NeuralNetwork(TrainedNetwork2, Change);
                TopScore = N2Score;
            }

            Tests++;
            if (Tests % DelayRate == 0)
            {
                Debug.Log(TopScore + ", Rotation");
                yield return new WaitForSeconds(.01f);
            }
        }

        Debug.Log("Rotation");

        //Simple Search
        TopScore = 0;
        PassingScore = 4 * MinimumPassingScore;
        while (TopScore < PassingScore)
        {
            N1Score = ScoreSimpleSearch(TrainedNetwork1);
            N2Score = ScoreSimpleSearch(TrainedNetwork2);

            N1Score += ScoreRotation(TrainedNetwork1);
            N2Score += ScoreRotation(TrainedNetwork2);

            N1Score += ScoreTaskSelection(TrainedNetwork1);
            N2Score += ScoreTaskSelection(TrainedNetwork2);

            N1Score += ScoreSimpleMovement(TrainedNetwork1);
            N2Score += ScoreSimpleMovement(TrainedNetwork2);


            if (N1Score > N2Score)
            {
                TrainedNetwork2 = new NeuralNetwork(TrainedNetwork1, Change);
                TopScore = N1Score;
            }
            else
            {
                TrainedNetwork1 = new NeuralNetwork(TrainedNetwork2, Change);
                TopScore = N2Score;
            }


            Tests++;
            if (Tests % DelayRate == 0)
            {
                Debug.Log(TopScore);
                yield return new WaitForSeconds(.01f);
            }
        }

        Debug.Log("Procede");

        SimulationManager.initialNetwork = TrainedNetwork1;
        SaveWorld.SaveNeuralNetwork(TrainedNetwork1);

        yield return null;
    }

}

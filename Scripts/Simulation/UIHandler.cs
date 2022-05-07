using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject firstGameObject;
    public Button[] buttons = new Button[5];
    public Button turnbutton;
    GameObject image;
    public delegate void ClickAction();
    public static event ClickAction OnClicked;
    public GameObject obj;
    GameObject numOfsheeps;
    GameObject turnNumber;
    GameObject numOfGrass;
    GameObject grasseaten;
    public GameObject turnmanager;
    public Text wantedTurnNumber;
    public Button GenerateAgain;
    public GameObject loadingScreen;
    public Slider slider;
    public GameObject AIscreen;
    public Slider[] aitests = new Slider[4];
    // Start is called before the first frame update
    void Awake()
    {
        numOfsheeps = GameObject.Find("SheepNumber");
        turnNumber = GameObject.Find("TurnNumber");
        numOfGrass = GameObject.Find("GrassNumber");
        grasseaten = GameObject.Find("grassEaten");
        FrontEnd.Restart += settrue;
        turnbutton.onClick.AddListener(GoToTurn);
        buttons[0].onClick.AddListener(onclick1);
        buttons[1].onClick.AddListener(onclick2);
        buttons[2].onClick.AddListener(onclick3);
        buttons[3].onClick.AddListener(onclick4);
        buttons[4].onClick.AddListener(TrainAgain);
        GenerateAgain.onClick.AddListener(Generate);
    }
    void Start()
    {
        image = transform.GetChild(0).gameObject;
        image.GetComponent<Image>().enabled = true;       
        if (Training.needsTraining)
        {
            AIscreen.SetActive(true);
            StartCoroutine(trainui());
        }
        else
        {
            firstGameObject.gameObject.SetActive(true);
        }

    }
    public void TaskOnClick()
    {
        obj.gameObject.SetActive(true);
    }

    public void CheckStartingSheep()
    {
        OnClicked();
        firstGameObject.gameObject.SetActive(false);
        obj.gameObject.SetActive(false);
        loadingScreen.gameObject.SetActive(true);
        StartCoroutine(loadslider());
    }

    // Update is called once per frame
    void LateUpdate()
    {
        numOfsheeps.GetComponent<TMP_Text>().text = "Number of sheeps: " + FrontEnd.sheeps.Count;
        numOfGrass.GetComponent<TMP_Text>().text = "Number of grass: " + FrontEnd.grassPatches.Count;
        grasseaten.GetComponent<TMP_Text>().text = "Grass Eaten: " + FrontEnd.GrassEaten; 
        int number = (FrontEnd.currentSave - 1) * FrontEnd.turnsinSave + FrontEnd.currentTurn + 1;
        if (number > -1)
        {
            turnNumber.GetComponent<TMP_Text>().text = "Current Turn: " + number;
        }
        else
        {
            turnNumber.GetComponent<TMP_Text>().text = "Current Turn: " + 0;
        }
    }

    void onclick1()
    {
        SimulationManager.totalSaves = 1;
        CheckStartingSheep();
    }
    void onclick2()
    {
        SimulationManager.totalSaves = 2;
        CheckStartingSheep();
    }
    void onclick3()
    {
        SimulationManager.totalSaves = 10;
        CheckStartingSheep();
    }
    void onclick4()
    {
        SimulationManager.totalSaves = 20;
        CheckStartingSheep();
    }

    void settrue()
    {
        turnmanager.gameObject.SetActive(false);
        image.gameObject.SetActive(true);
        firstGameObject.gameObject.SetActive(true);
        obj.gameObject.SetActive(true);
    }

    void GoToTurn()
    {
        FrontEnd.setturn = int.Parse(wantedTurnNumber.text);
        FrontEnd.paused = true;
    }

    void Generate()
    {
        FrontEnd.setturn = SimulationManager.totalSaves * FrontEnd.turnsinSave;
        FrontEnd.paused = true;

        turnmanager.gameObject.SetActive(false);
        image.gameObject.SetActive(true);
        firstGameObject.gameObject.SetActive(true);
        obj.gameObject.SetActive(true);
    }

    void TrainAgain()
    {
        StartCoroutine(Training.Train(new NeuralNetworkData(26, 5, 5, 3), null));
        AIscreen.SetActive(true);
        firstGameObject.SetActive(false);
        obj.SetActive(false);
        StartCoroutine(trainui());
    }

    IEnumerator loadslider()
    {
        bool done = false;
        while (!done)
        {
            float numerator = (SimulationManager.i * 500) + SimulationManager.o + 1;
            float denominator = SimulationManager.totalSaves * 500;
            float fraction = numerator / denominator;

            slider.value = fraction;
            if (slider.value >= 1)
            {
                done = true;
            }
            yield return null;
            obj.SetActive(false);
        }
        loadingScreen.SetActive(false);
        turnmanager.SetActive(true);
        image.gameObject.SetActive(false);
    }

    IEnumerator trainui()
    {
        aitests[0].value = 0;
        aitests[1].value = 0;
        aitests[2].value = 0;
        aitests[3].value = 0;
        float MinimumPassingScore = .85f;
        while (Training.TopScore < MinimumPassingScore)
        {
            float fraction = Training.TopScore / MinimumPassingScore;
            aitests[0].value = fraction;
            yield return null;
        }
        aitests[0].value = 1;
        yield return new WaitForSeconds(.2f);
        while (Training.TopScore < 2 * MinimumPassingScore)
        {
            float fraction = Training.TopScore / (MinimumPassingScore * 2);
            aitests[1].value = fraction;
            yield return null;
        }
        aitests[1].value = 1;
        yield return new WaitForSeconds(.2f);
        while (Training.TopScore < 3 * MinimumPassingScore)
        {
            float fraction = Training.TopScore / (MinimumPassingScore * 3);
            aitests[2].value = fraction;
            yield return null;
        }
        aitests[2].value = 1;
        yield return new WaitForSeconds(.2f);
        while (Training.TopScore < 4 * MinimumPassingScore)
        {
            float fraction = Training.TopScore / (MinimumPassingScore * 4);
            aitests[3].value = fraction;
            yield return null;
        }
        aitests[3].value = 1;
        yield return null;
        AIscreen.SetActive(false);
        firstGameObject.gameObject.SetActive(true);
    }
}

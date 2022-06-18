using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public Text oxygenText;
    public Text dayText;
    public Text dialogueText;
    public Text logText;
    public Text statsText;
    public Text deltaText;

    public int maxOxygen = 100;
    public int oxygenDailyConsumption = 3;
    public int singleTileOxygenVal = 3;
    public int singleTileWasteVal = 2;
    public int singleTileOxygenValPlus = 0;

    public GameObject endGamePanel;
    public GameObject windowPanel;
    public GameObject homePanel;

    public GameObject shipPanel;
    public GameObject buttonEndLeave;

    public bool enableClickRock = false;
    public bool enableDecreaseRow = false;

    public string[] chipMsg;
    public string introMsg;
    public string endMsg;

    // goalMsgs
    public string goalMsgArea;
    public string goalMsgGear;

    private Board board;
    private int currOxygen;
    private int currDay;

    private SoundManagement soundManagement;

    private bool endGame = false;

    private int wasteCount = 0;
    public int wasteCountEnd = 200;

    private int gearCount = 0;
    //public int gearCountEnd = 100;

    public float hardRockProbVal = 0.2f;
    public float shipProbVal = 0.2f;

    private int buildShipVal = 5; // 5
    private int buildExpVal = 0;
    private int buildExpDelta = 5; // 5
    private int buildExpMax = 25; // 25
    private int buildShipGearNum = 30; // 5

    public Text feedbackText;
    public Text resourceText;
    public Text equationText;

    public int oxygenItemNum;
    public int wasteItemNum;
    public int itemOxygenVal = 20;
    public int itemWasteVal = 10;

    public int crystalItemNum = 1;

    public int oxygenConsumptionMulti = 1;
    public int toxicValMulti = 1;

    public int eventThreshold = 4;

    public int nextEventCount = 0;
    public int currExploreCount = 0;
    public int currExploreMoveCount = 0;
    public int eventMoveCount = 0;
    public int currMoveCount = 0;

    public int sectorSize = 100;
    public int sectorLevel = 1;

    public int gasFilterVal = 20;
    public int gasFilterInc = 10;
    public int gasCollectedVal = 0;
    public int filterResultVal = 10;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        soundManagement = FindObjectOfType<SoundManagement>();

        // Initialise stats
        currOxygen = maxOxygen;
        currDay = 0;
        nextEventCount = sectorSize;

        logText.text = "";
        DisplayLogText(introMsg + "\n");
        IncreaseDay(); // so that actually starting from day 1

    }

    // Update is called once per frame
    void Update()
    {
        if (endGame == false)
        {
            oxygenText.text = "O2: " + currOxygen;
            dayText.text = "Day " + currDay;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    public void ConsumeOxygen(int amount)
    {
        currOxygen -= amount;
        if (currOxygen > maxOxygen)
        {
            currOxygen = maxOxygen;
        }
        if (currOxygen <= 0)
        {
            currOxygen = 0;
            oxygenText.text = "O2: " + currOxygen;


            string temp = "I have consumed all my oxygen… If I could have the change to do it again, I would collect oxygen first.";
            temp += "\n";
            DisplayLogText(temp); // need to be before SetGameEnd()
            SetGameEnd();
            soundManagement.PlayRandomLoseSound();
        }
    }

    public void IncreaseDay()
    {
        ElemType elemToAdd = ElemType.Void;

        if (nextEventCount > 0 && currExploreCount >= nextEventCount) // to start an event
        {
            currExploreCount = 0;
            nextEventCount = -1;

            currMoveCount = 0;
            toxicValMulti = Random.Range(1,10);
            eventMoveCount = 1;
            currMoveCount = eventMoveCount;

            elemToAdd = (ElemType)Random.Range(2, 5);
            currExploreMoveCount = 0;
        }

        if (nextEventCount == -1) // during event
        {
            currMoveCount--;
            if (currMoveCount < 1) // end event
            {
                toxicValMulti = 1;
                nextEventCount = sectorSize;
                eventMoveCount = -1;

                sectorLevel++;
                gasFilterVal += gasFilterInc;
            }
        }

        //board.scannerList.Add(elemToAdd);
        //board.scannerList.RemoveAt(0);

        UpdateDiary();
        currDay++;
        currExploreMoveCount++;
    }

    private void UpdateDiary()
    {
        string thisDiary = "";

        if (gearCount > 0)
        {
            thisDiary += goalMsgGear + ": " + gearCount;
            //thisDiary += "/" + gearCountEnd;
            thisDiary += "\n";
        }

        // thisDiary += goalMsgArea + ": " + board.exploredAreaCount;
        // //thisDiary += "/" + (board.width * board.height);
        // thisDiary += "\n";

        thisDiary += "Sector Level " + sectorLevel;
        thisDiary += "\n";

        if (board.scannerList.Count > 0)
        {
            foreach (ElemType s in board.scannerList)
            {
                thisDiary += s;
            }
            thisDiary += "\n";
        }

        if (nextEventCount > 0)
        {
            thisDiary += "Gas until next filtered oxygen" + ": " + gasCollectedVal + "/" + gasFilterVal;
            thisDiary += "\n";
        }

        if (nextEventCount > 0)
        {
            thisDiary += "Explore until next sector" + ": " + currExploreCount + "/" + nextEventCount;
            thisDiary += "\n";
        }

        if (eventMoveCount > 0)
        {
            thisDiary += "Event toxic level multiple: " + toxicValMulti;
            thisDiary += "\n";
            thisDiary += "Move count until event end" + ": " + currMoveCount + "/" + eventMoveCount;
            thisDiary += "\n";
        }

        if (thisDiary == "")
        {
            // to add some random sentences
            //thisDiary = "Nothing special today.";
            //DisplayLogText(thisDiary);
        }
        else
        {
            DisplayLogText(thisDiary);
        }

    }

    public void SetGameEnd()
    {
        endGame = true;
        board.currentState = GameState.pause;

        EndGameDisplay();
        soundManagement.DisableBGM();
    }

    public bool CheckIfGameEnd()
    {
        return endGame;
    }

    public void DisplayDialogueText(string displayText)
    {
        dialogueText.text = displayText;
    }

    public void DisplayLogText(string displayText)
    {
        if (!endGame)
        {
            string temp = "<Day " + currDay + ">\n";
            logText.text = temp + displayText + logText.text;
        }
    }

    public void EndGameDisplay()
    {
        //Debug.Log("End game");

        endGamePanel.SetActive(true);

        //board.currentState = GameState.lose;
    }

    public void CollectWaste(int num)
    {
        wasteCount += num;

        //statsText.text = "Pollutants cleared: " + wasteCount;
        Debug.Log("Pollutants cleared: " + wasteCount);

        if (wasteCount >= wasteCountEnd)
        {
            string temp = "I have cleared enough pollutants, it seems the environment has been improved. This planet has become a suitable place to live. Would I like to stay here?.";
            temp += "\n";
            DisplayLogText(temp); // need to be before SetGameEnd()
            SetGameEnd();
            soundManagement.PlayRandomWinSound();
        }
    }

    public void CollectGear(int num)
    {
        gearCount += num;
        //if (gearCount >= gearCountEnd)
        //{
        //    string temp = "I have collected enough gears to build a new spaceship! I am ready for the new journey!";
        //    temp += "\n";
        //    DisplayLogText(temp); // need to be before SetGameEnd()
        //    SetGameEnd();
        //    soundManagement.PlayRandomWinSound();
        //}
    }

    public void DisplayDeltaText(string displayText)
    {
        deltaText.text = displayText;
    }

    public void BuildShip()
    {
        if (gearCount < buildShipGearNum)
        {
            feedbackText.text = "No enough resources";
        }
        else
        {
            gearCount -= buildShipGearNum;

            bool isSuccessful = BuildShipActual();
            if (isSuccessful)
            {
                feedbackText.text = "Successful";

                CloseWindow();
                OpenHome();

                shipPanel.SetActive(true);

                buttonEndLeave.SetActive(true);
                shipPanel.transform.GetChild(0).gameObject.GetComponent<Image>().color = Color.white;
            }
            else
            {
                feedbackText.text = "Failure";
                buildExpVal = Mathf.Min(buildExpVal + buildExpDelta, buildExpMax);
            }

            UpdateResourceText();
            UpdateEquationText();
        }
    }

    private bool BuildShipActual()
    {
        float buildProb = GetBuildProb();
        if (Random.Range(0.0f, 1.0f) <= buildProb)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private float GetBuildProb()
    {
        return (buildShipVal + buildExpVal) / 100.0f;
    }

    private void UpdateResourceText()
    {
        if (resourceText != null)
        {
            resourceText.text = "Gear: " + gearCount;
            resourceText.text += "/" + buildShipGearNum;
        }
    }

    private void UpdateEquationText()
    {
        if (equationText != null)
        {
            int probVal = buildShipVal + buildExpVal;

            // right-aligned width 2
            string temp = string.Format("{0,2}% + {1,2}% = {2,2}%", buildShipVal.ToString(), buildExpVal.ToString(), probVal.ToString());
            equationText.text = temp;
        }
    }

    public void OpenWindow()
    {
        if (windowPanel != null)
        {
            windowPanel.SetActive(true);
            board.currentState = GameState.pause;
            UpdateResourceText();
            UpdateEquationText();
            feedbackText.text = "";
        }
    }

    public void CloseWindow()
    {
        if (windowPanel != null)
        {
            windowPanel.SetActive(false);
            board.currentState = GameState.move;
        }
    }

    public void EnterZone()
    {
        if (homePanel != null)
        {
            homePanel.SetActive(false);
            board.currentState = GameState.move;
            board.InitBoard();
        }
    }

    public void OpenHome()
    {
        if (homePanel != null)
        {
            homePanel.SetActive(true);
            board.currentState = GameState.pause;
            board.DestroyAll();
        }
    }

    public void EndLeave()
    {
        string temp = "I made a spaceship! I am ready for the new journey!";
        temp += "\n";
        DisplayLogText(temp); // need to be before SetGameEnd()

        SetGameEnd();
        soundManagement.PlayRandomWinSound();
    }

    public void RefillOxygen()
    {
        currOxygen = maxOxygen;
        Debug.Log("Refilled oxygen");
    }
}

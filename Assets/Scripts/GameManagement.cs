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

    public bool enableClickRock = false;

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

    private int buildShipVal = 5;
    private int buildExpVal = 0;
    private int buildExpDelta = 5;
    private int buildExpMax = 25;
    private int buildShipGearNum = 50;

    public Text feedbackText;
    public Text resourceText;
    public Text equationText;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        soundManagement = FindObjectOfType<SoundManagement>();

        // Initialise stats
        currOxygen = maxOxygen;
        currDay = 0;

        // popup display to be added
        Debug.Log(introMsg);

        logText.text = "";
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
        UpdateDiary();
        currDay++;
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

        thisDiary += goalMsgArea + ": " + board.exploredAreaCount;
        //thisDiary += "/" + (board.width * board.height);
        thisDiary += "\n";

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
        //    string temp = "I have collected enough gears to build a new spaceship! I am ready for the new Journey!";
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

                string temp = "I built a spaceship! I am ready for the new Journey!";
                temp += "\n";
                DisplayLogText(temp); // need to be before SetGameEnd()
                SetGameEnd();
                soundManagement.PlayRandomWinSound();
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
}

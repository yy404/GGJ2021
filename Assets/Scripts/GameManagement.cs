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
    public int gearCountEnd = 100;

    public float hardRockProbVal = 0.2f;
    public float shipProbVal = 0.2f;

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

        if (board.CalFreeTileRatio() >= 0.1f)
        {
            thisDiary += goalMsgGear + ": " + gearCount;
            thisDiary += "/" + gearCountEnd;
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
            string temp = "Day " + currDay + ":\n";
            logText.text = temp + displayText + "\n" + logText.text;
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
        if (gearCount >= gearCountEnd)
        {
            string temp = "I have collected enough gears to build a new spaceship! I am ready for the new Journey!";
            temp += "\n";
            DisplayLogText(temp); // need to be before SetGameEnd()
            SetGameEnd();
            soundManagement.PlayRandomWinSound();
        }
    }

    public void DisplayDeltaText(string displayText)
    {
        deltaText.text = displayText;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public Text oxygenText;
    public Text dayText;
    public Text dialogueText;
    public Text logText;

    public int maxOxygen;
    public int oxygenDailyConsumption;

    public GameObject endGamePanel;

    public int chipNum;

    public bool enableClickRock = false;

    public string[] chipMsg;
    public string introMsg;
    public string endMsg;

    private Board board;
    private int currOxygen;
    private int currDay;

    private SoundManagement soundManagement;

    private bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        soundManagement = FindObjectOfType<SoundManagement>();

        chipNum = chipMsg.Length;

        // Initialise stats
        currOxygen = maxOxygen;
        currDay = 0;

        DisplayLogText(introMsg);
    }

    // Update is called once per frame
    void Update()
    {
        if (endGame == false)
        {
            oxygenText.text = "O2: " + currOxygen;
            dayText.text = "Day: " + currDay;
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

            DisplayLogText("I have consumed all my oxygen… If I could have the change to do it again, I would collect oxygen first."); // need to be before SetGameEnd()
            SetGameEnd();
            soundManagement.PlayRandomLoseSound();
        }
    }

    public void IncreaseDay()
    {
        currDay++;
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
            logText.text = displayText;
        }
    }

    public void EndGameDisplay()
    {
        //Debug.Log("End game");

        endGamePanel.SetActive(true);

        //board.currentState = GameState.lose;
    }
}

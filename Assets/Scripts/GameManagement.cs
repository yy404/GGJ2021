﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Box,
    //Blueprint,
    Chip,
    //Seed,
    Ship,
    Radar,
    None,
}

public class GameManagement : MonoBehaviour
{
    public Text oxygenText;
    public Text dayText;
    public Text dialogueText;
    public Text logText;

    public int maxOxygen;
    public int oxygenDailyConsumption;

    public GameObject endGamePanel;

    public ItemType[,] ItemMap;
    public int[] seqArray;
    public int itemNum = 0;

    private Board board;
    private int currOxygen;
    private int currDay;

    private bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

        ItemMap = new ItemType[board.width, board.height];

        // Initialise a sequence array
        seqArray = new int[board.width * board.height];
        for (int i = 0; i < seqArray.Length; i++)
        {
            seqArray[i] = i;
        }

        ShuffleSeqArray();
        SetupItemMap(); // need to be after sequence array initialisation and shuffling

        // Initialise stats
        currOxygen = maxOxygen;
        currDay = 0;
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
            endGame = true;
            oxygenText.text = "O2: " + currOxygen;
            dialogueText.text = "I END my journey because of no Oxygen..." + "\n(May I have a different ending next time?)";
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
    }

    public bool CheckIfGameEnd()
    {
        return endGame;
    }

    public void DisplayDialogueText(string displayText)
    {
        dialogueText.text = displayText;
    }

    private void SetupItemMap()
    {

        // initialise item map
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                ItemMap[i, j] = ItemType.None;
            }
        }

        // adding radar(s)
        for (int i = 0; i < 1; i++)
        {
            AddItemToMap(ItemType.Radar);
        }

        // adding ship(s)
        for (int i = 0; i < 1; i++)
        {
            AddItemToMap(ItemType.Ship);
        }

        // adding chips
        for (int i = 0; i < 6; i++)
        {
            AddItemToMap(ItemType.Chip);
        }
    }

    public void EndGameDisplay()
    {
        //Debug.Log("End game");
        endGamePanel.SetActive(true);

        //board.currentState = GameState.lose;
    }

    public void DisplayRadar()
    {
        //Debug.Log("DisplayHints");
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (ItemMap[i, j] != ItemType.None)
                {
                    board.rockTiles[i, j].DisplaySpecialRock();
                }
            }
        }
    }

    private void ShuffleSeqArray()
    {
        // Shuffle the array
        for (int i = 0; i < seqArray.Length; i++)
        {
            int temp = seqArray[i];
            int randomIndex = Random.Range(i, seqArray.Length);
            seqArray[i] = seqArray[randomIndex];
            seqArray[randomIndex] = temp;
        }
    }

    private void AddItemToMap(ItemType itemType)
    {
        // Check if any space to add item
        if (itemNum < seqArray.Length)
        {
            // Add the item
            int currSeqVal = seqArray[itemNum];
            int currX = currSeqVal % board.width;
            int currY = currSeqVal / board.width;
            ItemMap[currX, currY] = itemType;
            itemNum++;
        }
        else
        {
            Debug.Log("Added too many items");
            itemNum = 0;
        }
    }
}

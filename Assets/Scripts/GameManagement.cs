using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Box,
    Ship,
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

    private Board board;
    private int currOxygen;
    private int currDay;

    private bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

        ItemMap = new ItemType[board.width, board.height];
        SetupItemMap();

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
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if ((i == 0) && (j == 0))
                {
                    ItemMap[i, j] = ItemType.Box;
                }
                else
                {
                    ItemMap[i, j] = ItemType.None;
                }
            }
        }
    }

    public void EndGameDisplay()
    {
        Debug.Log("End game");
        endGamePanel.SetActive(true);

        //board.currentState = GameState.lose;
    }
}

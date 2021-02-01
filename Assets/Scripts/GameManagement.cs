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

    public int maxOxygen;
    public int oxygenDailyConsumption;

    private Board board;
    private int currOxygen;
    private int currDay;

    private bool endGame = false;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();

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

    public bool CheckIfGameEnd()
    {
        return endGame;
    }

    public void DisplayDialogueText(string displayText)
    {
        dialogueText.text = displayText;
    }
}

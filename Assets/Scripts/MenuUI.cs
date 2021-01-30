using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void StartGame()
    {
        //Debug.Log("StartGame button was clicked");
        SceneManager.LoadScene("MainScene");
    }

    public void ExitGame()
    {
        //Debug.Log("ExitGame button was clicked");
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButton: MonoBehaviour
{

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

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

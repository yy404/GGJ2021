using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagement : MonoBehaviour
{
    public AudioSource[] destroyNoise;
    public AudioSource[] pickupNoise;
    public AudioSource[] winSound;
    public AudioSource[] loseSound;

    public GameObject bgmObj;

    //// Start is called before the first frame update
    //void Start()
    //{
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void PlayRandomDestroyNoise()
    {
        //Choose a random number
        int clipToPlay = Random.Range(0, destroyNoise.Length);
        //Play that clip
        destroyNoise[clipToPlay].Play();
    }

    public void PlayRandomPickupNoise()
    {
        //Choose a random number
        int clipToPlay = Random.Range(0, pickupNoise.Length);
        //Play that clip
        pickupNoise[clipToPlay].Play();
    }

    public void PlayRandomWinSound()
    {
        //Choose a random number
        int clipToPlay = Random.Range(0, winSound.Length);
        //Play that clip
        winSound[clipToPlay].Play();
    }

    public void PlayRandomLoseSound()
    {
        //Choose a random number
        int clipToPlay = Random.Range(0, loseSound.Length);
        //Play that clip
        loseSound[clipToPlay].Play();
    }

    public void DisableBGM()
    {
        if (bgmObj != null)
        {
            bgmObj.SetActive(false);
        }
    }


    

}

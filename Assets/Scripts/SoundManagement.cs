using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagement : MonoBehaviour
{
    public AudioSource[] destroyNoise;

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
}

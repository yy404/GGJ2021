using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab; // The background tile prefab
    public GameObject[] dots; // A list of dot prefabs
    public GameObject[,] allDots;

    // Start is called before the first frame update
    void Start()
    {
        allDots = new GameObject[width, height];
        SetUp();
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    // Initialise the board with tiles and dots
    private void SetUp()
    { 
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Create a background tile at (i,j) position
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab,tilePosition, Quaternion.identity) as GameObject; //
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                // Create a dot at (i,j) position
                int dotToUse = Random.Range(0, dots.Length); // Choose a random type of dots 
                Vector2 tempPosition = new Vector2(i, j + offset);
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";

                // Initialise dot variables
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;

                allDots[i, j] = dot;
            }
        }
    }
}

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

                // Choose a random type of dots 
                int dotToUse = Random.Range(0, dots.Length); 
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    //Debug.Log(maxIterations);
                }

                // Create a dot at (i,j) position
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

    // Check if a type of dot at (col,row) has any match
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag
                && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag
                && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag
                    && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag
                    && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
    }

}

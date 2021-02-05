using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    pause,
}

public enum TileKind
{
    //SpecialRock,
    Rock,
    Normal,
}

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab; // The background tile prefab
    public GameObject rockTilePrefab;
    public GameObject[] dots; // A list of dot prefabs
    public GameObject[,] allDots; //

    public BackgroundTile[,] rockTiles;
    public int rockTileCount = 48;

    public float refillDelay = 0.5f;

    public GameState currentState = GameState.move;

    private FindMatches findMatches;
    private GameManagement gameManagement;
    private SoundManagement soundManagement;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        gameManagement = FindObjectOfType<GameManagement>();
        soundManagement = FindObjectOfType<SoundManagement>();
        allDots = new GameObject[width, height];
        rockTiles = new BackgroundTile[width, height];
        SetUp();
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    // Initialise the board with tiles and dots
    private void SetUp()
    {
        GenerateRockTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // Create a background tile at (i,j) position
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject; //
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                if (rockTiles[i,j] == null)
                {
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

            DamageRock(column, row);

            if (allDots[column, row].tag == "TileOxygen")
            {
                gameManagement.ConsumeOxygen(-3); // add 3
            }

            if (soundManagement != null)
            {
                soundManagement.PlayRandomDestroyNoise();
            }

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
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // If the current spot is empty
                if (!rockTiles[i, j] && allDots[i, j] == null)
                {
                    // Loop from the space above to the top of column
                    for (int k = j + 1; k < height; k++)
                    {
                        // If a dot is found
                        if (allDots[i, k] != null)
                        {
                            // Move that dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            // Set that spot to be null
                            allDots[i, k] = null;
                            // Break out of the loop;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);
        RefillBoard();

        while (MatchesOnBoard())
        {
            DestroyMatches(); // this is recursion
            yield return new WaitForSeconds(2 * refillDelay);
        }
        findMatches.currentMatches.Clear();
        yield return new WaitForSeconds(refillDelay);

        if (IsDeadlocked())
        {
            ShuffleBoard();
            Debug.Log("Deadlocked!!!");
        }

        currentState = GameState.move;
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!rockTiles[i, j] && allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offset);
                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]))
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                        if (maxIterations > 100)
                        {
                            break;
                        }
                    }

                    GameObject piece = Instantiate(dots[dotToUse],
                      tempPosition, Quaternion.identity);
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                    piece.transform.parent = this.transform;
                    piece.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = piece;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        // Take the first piece and save it in a holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        // Switching the first dot to be the second position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        // Set the first dot to be the second dot
        allDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    // Make sure that one and two to the right are in the board
                    if (i < width - 2)
                    {
                        // Check if the dots to the right exist
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                                && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    if (j < height - 2)
                        // Check if the dots above exist
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                                && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                }
            }
        }
        return false;
    }

    private void ShuffleBoard()
    {
        // Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        // Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        // For every spot on the board
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // If this spot is not rock
                if (!rockTiles[i, j])
                {
                    // pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        // Debug.Log(maxIterations);
                    }
                    // Make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    // Assign the column/row to the piece
                    piece.column = i;
                    piece.row = j;
                    // Fill in the dots array with this new piece
                    allDots[i, j] = newBoard[pieceToUse];
                    // Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }
        // Check if it's still deadlocked
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }

    private void GenerateRockTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if ((i > 3) || (j < 4))
                if (j < height - 1) // leave one line above
                {
                    Vector2 tempPosition = new Vector2(i, j);
                    GameObject tile = Instantiate(rockTilePrefab, tempPosition, Quaternion.identity);
                    rockTiles[i, j] = tile.GetComponent<BackgroundTile>();
                    rockTiles[i, j].column = i;
                    rockTiles[i, j].row = j;
                    tile.transform.parent = this.transform;
                }
            }
        }
    }

    private void DamageRock(int column, int row)
    {
        if (column > 0)
        {
            if (rockTiles[column - 1, row])
            {
                rockTiles[column - 1, row].TakeDamage(Random.Range(1, 3));
                if (rockTiles[column - 1, row].hitPoints <= 0)
                {
                    rockTiles[column - 1, row] = null;
                    rockTileCount--;
                }
            }
        }
        if (column < width - 1)
        {
            if (rockTiles[column + 1, row])
            {
                rockTiles[column + 1, row].TakeDamage(Random.Range(1, 3));
                if (rockTiles[column + 1, row].hitPoints <= 0)
                {
                    rockTiles[column + 1, row] = null;
                    rockTileCount--;
                }
            }
        }
        if (row > 0)
        {
            if (rockTiles[column, row - 1])
            {
                rockTiles[column, row - 1].TakeDamage(Random.Range(1, 3));
                if (rockTiles[column, row - 1].hitPoints <= 0)
                {
                    rockTiles[column, row - 1] = null;
                    rockTileCount--;
                }
            }
        }
        if (row < height - 1)
        {
            if (rockTiles[column, row + 1])
            {
                rockTiles[column, row + 1].TakeDamage(Random.Range(1, 3));
                if (rockTiles[column, row + 1].hitPoints <= 0)
                {
                    rockTiles[column, row + 1] = null;
                    rockTileCount--;
                }
            }
        }
    }

}

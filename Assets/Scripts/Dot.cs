using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column; //x
    public int row; //y
    public int prevColumn;
    public int prevRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;
    private FindMatches findMatches;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;

        // X movement
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject) // Keep the record correct
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        // Y movement
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    // This is for testing and Debug only.
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Mouse was clicked");
        }
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Debug.Log(firstTouchPosition);
    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle(); // and may move pieces
    }

    void CalculateAngle()
    {
        // Move only if swipe enough
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist
        || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(
            finalTouchPosition.y - firstTouchPosition.y,
            finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            // Debug.Log(swipeAngle);
            MovePieces();
        }
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            //Right Swipe;
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            //Up Swipe
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            //Left Swipe
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -145 && row > 0)
        {
            //Down Swipe
            MovePiecesActual(Vector2.down);
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        int x = (int)direction.x;
        int y = (int)direction.y;
        otherDot = board.allDots[column + x, row + y];
        prevRow = row;
        prevColumn = column;
        if (otherDot != null)
        {
            otherDot.GetComponent<Dot>().column += -1 * x;
            otherDot.GetComponent<Dot>().row += -1 * y;
            column += x;
            row += y;
            StartCoroutine(CheckMoveCo());
        }
    }

    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(0.5f);
        if (otherDot != null)
        {
            // Move back if no match
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = prevRow;
                column = prevColumn;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }

    }
}

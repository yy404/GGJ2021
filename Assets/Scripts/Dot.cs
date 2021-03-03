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
    private GameManagement gameManagement;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    public bool marked = false;
    public SpriteRenderer spriteRend;

    public ElemType elemType = ElemType.Void;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        gameManagement = FindObjectOfType<GameManagement>();
        spriteRend = GetComponent<SpriteRenderer>();

        if (this.tag == "TileElem")
        {
            // int currTileTypeNum = board.CalTileTypeNum();
            // elemType = (ElemType) Random.Range(0, Mathf.Min(5, currTileTypeNum));

            elemType = (ElemType) Random.Range(0,5);

            SetColorByElemType();
        }
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

    private void OnMouseOver()
    {
        if (board.dotMarkCount == 0)
        {
            if (board.currentState == GameState.move)
            {
                marked = true;
                MarkIt(this.tag, elemType);
            }
        }

        if (gameManagement.dialogueText.text == "")
        {
            gameManagement.DisplayDialogueText(GeneDialogueText());
        }
    }

    private void OnMouseDown()
    {
        //if (board.currentState == GameState.move)
        //{
        //    //firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    ////Debug.Log(firstTouchPosition);

        //    isMatched = true;
        //}

    }

    private void OnMouseUp()
    {
        //if (board.currentState == GameState.move)
        //{
        //    //finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    //if (gameManagement.CheckIfGameEnd() == false)
        //    //{
        //    //    CalculateAngle(); // and may move pieces
        //    //}

        //    board.DestroyMatches();

        //    gameManagement.IncreaseDay();
        //    gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption);
        //}

        // bool isValid = board.dotMarkCount > 1 || elemType == ElemType.Void;
        bool isValid = board.dotMarkCount > 0;

        if (isValid)
        {
            // decrease oxygen firstly to make the logic correct
            if (board.dotMarkCount > 1)
            {
                // normal consumption
                gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption);
                board.DestroyAllMarked();
            }
            else
            {
                // penalty consumption
                gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption * gameManagement.oxygenConsumptionMulti);
                board.DestroyAllMarked();

                // TransformElemType();
                // board.ClearDotMark();
                // board.ClearRockMark();
                // board.seedMarkCount = 0;
                // gameManagement.DisplayDeltaText("");
                // gameManagement.DisplayDialogueText("");
            }

            gameManagement.IncreaseDay(); // need to be after DestroyAllMarked() to avoid incorrect display
        }
        else
        {
            // can't click
            // sound
        }


    }

    private void OnMouseEnter()
    {
        if (board.currentState == GameState.move)
        {
            marked = true;
            MarkIt(this.tag, elemType);

            gameManagement.DisplayDialogueText(GeneDialogueText());
        }
    }

    private void OnMouseExit()
    {
        board.ClearDotMark();
        board.ClearRockMark();
        board.seedMarkCount = 0;
        gameManagement.DisplayDeltaText("");
        gameManagement.DisplayDialogueText("");
    }

    void CalculateAngle()
    {
        // Move only if swipe enough
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist
        || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(
            finalTouchPosition.y - firstTouchPosition.y,
            finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            // Debug.Log(swipeAngle);
            MovePieces();

            gameManagement.IncreaseDay();
            gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption);
        }
        else
        {
            board.currentState = GameState.move;
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
        else
        {
            board.currentState = GameState.move;
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
        else
        {
            board.currentState = GameState.move;
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

                yield return new WaitForSeconds(.5f);
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }

    }

    public void MarkIt(string thisTag, ElemType thisElemType)
    {
        SetColorAlphaVal(0.5f);

        board.dotMarkCount++;

        if (elemType != ElemType.Void)
        {
            board.MarkRock(column, row);
        }

        if (column > 0)
        {
            // board.allDots[column - 1, row]
            GameObject theOtherDot = board.allDots[column - 1, row];
            MarkTheOtherDot(theOtherDot, thisTag, thisElemType);
        }
        if (column < board.width - 1)
        {
            // board.allDots[column + 1, row]
            GameObject theOtherDot = board.allDots[column + 1, row];
            MarkTheOtherDot(theOtherDot, thisTag, thisElemType);
        }
        if (row > 0)
        {
            // board.allDots[column, row - 1]
            GameObject theOtherDot = board.allDots[column, row - 1];
            MarkTheOtherDot(theOtherDot, thisTag, thisElemType);
        }
        if (row < board.height - 1)
        {
            // board.allDots[column, row + 1]
            GameObject theOtherDot = board.allDots[column, row + 1];
            MarkTheOtherDot(theOtherDot, thisTag, thisElemType);
        }

        // to be optimised
        // if (board.dotMarkCount > 1)
        // {
        //     UpdateDeltaText(thisTag);
        // }
    }

    private void MarkTheOtherDot(GameObject theOtherDot, string thisTag, ElemType thisElemType)
    {
        if (theOtherDot != null)
        {
            Dot theOtherDotComp = theOtherDot.GetComponent<Dot>();
            if (!theOtherDotComp.marked)
            {
                if (thisTag == "TileElem")
                {
                    if (thisElemType == ElemType.Void)
                    {
                        // pass
                    }
                    else if (thisElemType == theOtherDotComp.elemType)
                    {
                        theOtherDotComp.marked = true;
                        theOtherDotComp.MarkIt(thisTag, thisElemType);
                    }
                }
                else if (theOtherDotComp.tag == thisTag)
                {
                    theOtherDotComp.marked = true;
                    theOtherDotComp.MarkIt(thisTag, thisElemType);
                }
                else if (theOtherDotComp.tag == "TileSeed")
                {
                    theOtherDotComp.marked = true;
                    board.seedMarkCount++;
                    theOtherDotComp.MarkIt(thisTag, thisElemType);
                }
            }
        }
    }

    private void UpdateDeltaText(string thisTag)
    {
        int tempOxygenVal = -1 * gameManagement.oxygenDailyConsumption;
        if (thisTag == "TileOxygen")
        {
            tempOxygenVal += (board.dotMarkCount - board.seedMarkCount) * gameManagement.singleTileOxygenVal;
            //if (board.seedMarkCount > 0)
            //{
            //    // bonus adding 1 for each tile
            //    tempOxygenVal += (board.dotMarkCount - board.seedMarkCount) * 1;
            //}
        }
        else if (thisTag == "TileWaste")
        {
            tempOxygenVal -= (board.dotMarkCount - board.seedMarkCount) * gameManagement.singleTileWasteVal;
            if (board.seedMarkCount > 0)
            {
                // bonus adding 1 for each tile
                tempOxygenVal += (board.dotMarkCount - board.seedMarkCount) * 1;
            }
        }
        else if (thisTag == "TileSeed")
        {
            tempOxygenVal += board.dotMarkCount * gameManagement.singleTileOxygenValPlus;
        }

        if (tempOxygenVal > 0)
        {
            gameManagement.DisplayDeltaText("+" + tempOxygenVal);
        }
        else
        {
            gameManagement.DisplayDeltaText("" + tempOxygenVal);
        }
    }

    private string GeneDialogueText()
    {
        string answerStr = "";

        if (this.tag == "TileOxygen")
        {
            answerStr = "Oxygen: refilling O2 (necessary for living)";
        }
        else if (this.tag == "TileWaste")
        {
            answerStr = "Pollutant: consuming more oxygen this day";
        }
        else if (this.tag == "TileGear")
        {
            answerStr = "Gear: useful for crafting";
        }
        else if (this.tag == "TileSeed")
        {
            answerStr = "Seed: magic power connecting other objects";
        }
        else if (this.tag == "TileElem")
        {
            answerStr = "Gear: collect for crafting";
        }
        else
        {
            answerStr = "N/A";
        }

        return answerStr;
    }

    public void SetColorByElemType()
    {
        Color color = Color.white;

        if (elemType == ElemType.Metal)
        {
            color = Color.black;
        }
        else if (elemType == ElemType.Water)
        {
            color = Color.blue;
        }
        else if (elemType == ElemType.Wood)
        {
            color = Color.green;
        }
        else if (elemType == ElemType.Fire)
        {
            color = Color.red;
        }
        else if (elemType == ElemType.Soil)
        {
            color = Color.yellow;
        }
        else
        {
            // pass
        }

        spriteRend.material.color = color;

        // SetColorAlphaVal();
    }

    private void SetColorAlphaVal(float alphaVal = 0.3f)
    {
        if (spriteRend != null)
        {
            Color tempColor = spriteRend.material.color;
            tempColor.a = alphaVal;
            spriteRend.material.color = tempColor;
        }
    }

    public void TransformElemType()
    {
        // if (elemType == ElemType.Metal)
        // {
        //     elemType = ElemType.Water;
        // }
        // else if (elemType == ElemType.Water)
        // {
        //     elemType = ElemType.Wood;
        // }
        // else if (elemType == ElemType.Wood)
        // {
        //     elemType = ElemType.Fire;
        // }
        // else if (elemType == ElemType.Fire)
        // {
        //     elemType = ElemType.Soil;
        // }
        // else if (elemType == ElemType.Soil)
        // {
        //     elemType = ElemType.Metal;
        // }
        // else
        // {
        //     // pass
        // }

        ElemType tempType = elemType;
        while (tempType == elemType)
        {
            tempType = (ElemType) Random.Range(0,5);
        }
        elemType = tempType;

        SetColorByElemType();
    }
}

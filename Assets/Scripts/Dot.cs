using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    private TextMeshPro textMeshComp;
    private int damageVal = 1;

    public Sprite gear;
    public Sprite oxygen;
    public Sprite waste;
    public Sprite battery;
    public Sprite ship;
    public Sprite rock;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        gameManagement = FindObjectOfType<GameManagement>();
        spriteRend = GetComponent<SpriteRenderer>();

        GameObject thisTextObject = gameObject.transform.GetChild(0).gameObject;
        textMeshComp = thisTextObject.GetComponent<TextMeshPro>();
        textMeshComp.text = "";

        if (this.tag == "TileElem")
        {
            // int currTileTypeNum = board.CalTileTypeNum();
            // elemType = (ElemType) Random.Range(0, Mathf.Min(5, currTileTypeNum));

            if ( IsShipTile() )
            {
                elemType = ElemType.Void;
            }
            else
            {
                elemType = (ElemType)Random.Range(0, 2);
            }

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
                if (damageVal > 1)
                {
                    SetColorAlphaVal(0.5f);
                    board.MarkRock(column, row);
                    board.dotMarkCount = 1;
                }
                else
                {
                    MarkIt(this.tag, elemType);
                }
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

        bool isValid = !IsShipTile() && board.dotMarkCount > 1 && ( elemType == ElemType.Void || elemType == ElemType.Metal );

        if ( isValid || IsShipNeighbour() )
        {
            bool isEventReset = false;
            //int dotCount = board.damageMarkCount;
            bool shouldGenerate = board.damageMarkCount > gameManagement.uncertaintyCap && this.tag == "TileElem" && elemType == ElemType.Metal;

            ElemType previousElemType = elemType;

            if (this.tag == "TileElem" && elemType != ElemType.Metal && elemType != ElemType.Void && board.dotMarkCount == gameManagement.eventThreshold)
            {
                // board.isAnEvent = true;

                // board.eventElemType = (ElemType) Random.Range(2,5);
                board.eventElemType = elemType;
            }
            else if (this.tag == "TileElem" && elemType == ElemType.Void && board.isAnEvent)
            {
                isEventReset = true;
            }

            //if (this.tag == "TileElem" && elemType == ElemType.Fire)
            //{
            //    gameManagement.ConsumeOxygen((board.toxicTileCount - board.dotMarkCount) * gameManagement.toxicValMulti);
            //}
            //else
            //{
            //    gameManagement.ConsumeOxygen(board.toxicTileCount * gameManagement.toxicValMulti);
            //}

            if (this.tag == "TileElem" && elemType == ElemType.Fire)
            {
                gameManagement.gasCollectedVal += board.dotMarkCount;

                while (gameManagement.gasCollectedVal >= gameManagement.gasFilterVal)
                {
                    gameManagement.ConsumeOxygen(-1 * gameManagement.filterResultVal);
                    gameManagement.gasCollectedVal -= gameManagement.gasFilterVal;
                }
            }

            // first release the previous ship tile for destroy
            if (board.isShipClick)
            {
                board.shipPosition.x = column;
                board.shipPosition.y = row;
            }

            // decrease oxygen firstly to make the logic correct
            if (board.dotMarkCount > 1)
            {
                // normal consumption
                gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption);

                // damageVal = board.damageMarkCount;
                // textMeshComp.text = "" + damageVal;

                // marked = false;
                // board.DestroyAllMarked(0);
                board.DestroyAllMarked(damageVal);
                board.damageMarkCount = 0;
            }
            else
            {
                // penalty consumption
                // if (damageVal > 1)
                // {
                //     gameManagement.ConsumeOxygen(0);
                // }
                // else
                {
                    gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption * gameManagement.oxygenConsumptionMulti);
                }

                board.DestroyAllMarked(damageVal);
                board.damageMarkCount = 0;

                // TransformElemType();
                // board.ClearDotMark();
                // board.ClearRockMark();
                // board.seedMarkCount = 0;
                // gameManagement.DisplayDeltaText("");
                // gameManagement.DisplayDialogueText("");
            }

            // then setup the new ship tile
            if (board.isShipClick)
            {
                elemType = ElemType.Void;
                SetColorByElemType();

                board.isShipClick = false;
            }

            // override the results of DestroyAllMarked()
            if ( shouldGenerate )
            {
                elemType = board.scannerList[0];
                SetColorByElemType();
            }

            // ensure the clicked tile changed
            if (!IsShipTile() && elemType == previousElemType)
            {
                elemType = previousElemType == ElemType.Void ? ElemType.Metal : ElemType.Void;
                SetColorByElemType();
            }

            if (isEventReset)
            {
                board.isAnEvent = false;
                board.eventElemType = ElemType.Void;
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
            if (damageVal > 1)
            {
                SetColorAlphaVal(0.5f);
                board.MarkRock(column, row);
                board.dotMarkCount = 1;
                board.damageMarkCount = damageVal;
            }
            else
            {
                MarkIt(this.tag, elemType);
            }

            gameManagement.DisplayDialogueText(GeneDialogueText());
        }
    }

    private void OnMouseExit()
    {
        board.ClearDotMark();
        board.ClearRockMark();
        board.seedMarkCount = 0;
        board.damageMarkCount = 0;
        board.isShipClick = false;
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
        board.damageMarkCount += damageVal;

        if ( IsShipTile() )
        {
            board.isShipClick = true;
        }

        // if (elemType != ElemType.Void)
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
        bool isValid = !IsShipTile() && board.dotMarkCount > 1 && (elemType == ElemType.Void || elemType == ElemType.Metal);

        if (isValid || IsShipNeighbour())
        {
            UpdateDeltaText(thisTag, thisElemType);
        }
        else
        {
            gameManagement.DisplayDeltaText("X");
        }
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
                    // if (thisElemType == ElemType.Void)
                    // {
                    //     // pass
                    // }
                    // else 
                    
                    if (thisElemType == theOtherDotComp.elemType)
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

    private void UpdateDeltaText(string thisTag, ElemType thisElemType)
    {
        int tempOxygenVal = -1 * (gameManagement.oxygenDailyConsumption + board.toxicTileCount * gameManagement.toxicValMulti);
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
        else if (thisTag == "TileElem")
        {
            if (thisElemType == ElemType.Water)
            {
                tempOxygenVal += board.dotMarkCount * gameManagement.singleTileOxygenVal;
            }
            else if (thisElemType == ElemType.Fire)
            {
                tempOxygenVal += board.dotMarkCount * gameManagement.toxicValMulti;
            }
        }

        string eventSign = "";
        //if (thisTag == "TileElem" && elemType != ElemType.Metal && elemType != ElemType.Void && board.dotMarkCount == gameManagement.eventThreshold)
        //{
        //    eventSign += " +EVENT";
        //}
        //else if (thisTag == "TileElem" && thisElemType == ElemType.Void && board.isAnEvent)
        //{
        //    eventSign += " " + board.eventElemType;
        //}

        if (tempOxygenVal > 0)
        {
            gameManagement.DisplayDeltaText("+" + tempOxygenVal + eventSign);
        }
        else if (tempOxygenVal == 0)
        {
            gameManagement.DisplayDeltaText("-" + tempOxygenVal + eventSign);
        }
        else
        {
            gameManagement.DisplayDeltaText("" + tempOxygenVal + eventSign);
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
            answerStr = "Action: explore the nearby area";
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
        Sprite spriteTemp = gear;

        if ( IsShipTile() )
        {
            spriteRend.sprite = ship;
            spriteRend.material.color = Color.red;

            return;
        }

        if (elemType == ElemType.Metal)
        {
            color = Color.black;
        }
        else if (elemType == ElemType.Water)
        {
            color = Color.blue;
            spriteTemp = oxygen;
        }
        else if (elemType == ElemType.Wood)
        {
            color = Color.green;
            spriteTemp = battery;
        }
        else if (elemType == ElemType.Fire)
        {
            color = Color.red;
            spriteTemp = waste;
        }
        else if (elemType == ElemType.Soil)
        {
            color = Color.yellow;
            spriteTemp = rock;
        }
        else
        {
            // pass
        }

        spriteRend.sprite = spriteTemp;
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

    public bool IsShipTile()
    {
        return board.shipPosition.x == column && board.shipPosition.y == row;
    }

    public bool IsShipNeighbour()
    {
        return Mathf.Abs(board.shipPosition.x - column) + Mathf.Abs(board.shipPosition.y - row) == 1;
    }
}

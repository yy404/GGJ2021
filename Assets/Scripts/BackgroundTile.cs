using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints = 1;
    public Sprite wholeRock;
    public Sprite specialRock;
    public Sprite box;
    public Sprite ship;
    public Sprite chip;
    public Sprite oxygen;
    public Sprite waste;
    public Sprite spriteTool;
    public Sprite crystal;

    public Texture2D cursorTextureOver;
    public Texture2D cursorTextureDown;
    public Texture2D cursorTextureUp;
    public GameObject chipParticle;
    public GameObject specialParticle;

    public int column; //x
    public int row; //y

    public SpriteRenderer spriteRend;
    private GameManagement gameManagement;
    private SoundManagement soundManagement;
    private Board board;

    public bool marked = false;
    private GameObject specialParticleVar = null;

    private bool isWorkstation = false;
    private bool isCrystal = false;

    private TextMeshPro textMeshComp;

    // Start is called before the first frame update
    void Start()
    {
        gameManagement = FindObjectOfType<GameManagement>();
        soundManagement = FindObjectOfType<SoundManagement>();
        board = FindObjectOfType<Board>();

        spriteRend = GetComponent<SpriteRenderer>();

        GameObject thisTextObject = gameObject.transform.GetChild(0).gameObject;
        textMeshComp = thisTextObject.GetComponent<TextMeshPro>();
        textMeshComp.text = "";

        // default settings
        spriteRend.sprite = specialRock;
        spriteRend.material.color = Color.black;

        if (board.ItemMap[column, row] == ItemType.None)
        {
            if (Random.Range(0.0f, 1.0f) <= gameManagement.hardRockProbVal) // to be improved
            {
                //spriteRend.material.color = Color.white;
                hitPoints = 999;
                spriteRend.sprite = wholeRock;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPoints <= 0 && board.ItemMap[column, row] == ItemType.None)
        {
            DestroyBackgroundTile();
            board.DecreaseRowFunc();
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isWorkstation && !isCrystal)
        {
            if (spriteRend.material.color == Color.black)
            {
                spriteRend.material.color = Color.white;
                board.exploredAreaCount++;

                hitPoints = Random.Range(1, 6);
                textMeshComp.text = "" + hitPoints;

                //if (board.ItemMap[column, row] == ItemType.None)
                //{
                //    hitPoints -= damage;
                //}
            }
            else
            {
                hitPoints -= damage;

                textMeshComp.text = "";
                if (hitPoints > 0)
                {
                    textMeshComp.text += hitPoints;
                }
            }

            if ((hitPoints == 0) && (board.ItemMap[column, row] != ItemType.None))
            {
                if (board.ItemMap[column, row] == ItemType.Ship)
                {
                    spriteRend.sprite = ship;
                }
                else if (board.ItemMap[column, row] == ItemType.Chip)
                {
                    spriteRend.sprite = chip;
                }
                else if (board.ItemMap[column, row] == ItemType.Oxygen)
                {
                    spriteRend.sprite = oxygen;
                }
                else if (board.ItemMap[column, row] == ItemType.Waste)
                {
                    spriteRend.sprite = waste;
                }
                else if (board.ItemMap[column, row] == ItemType.Workstation)
                {
                    isWorkstation = true;
                    spriteRend.sprite = spriteTool;

                    if (specialParticleVar == null)
                    {
                        specialParticleVar = Instantiate(specialParticle, this.gameObject.transform.position, Quaternion.identity);
                        var thisMain = specialParticleVar.GetComponent<ParticleSystem>().main;
                        thisMain.startColor = new Color(1, 0, 1, .9f);
                    }
                }
                else if (board.ItemMap[column, row] == ItemType.Crystal)
                {
                    isCrystal = true;
                    spriteRend.sprite = crystal;

                    if (specialParticleVar == null)
                    {
                        specialParticleVar = Instantiate(specialParticle, this.gameObject.transform.position, Quaternion.identity);
                        var thisMain = specialParticleVar.GetComponent<ParticleSystem>().main;
                        thisMain.startColor = new Color(0, 0, 1, .9f);
                    }
                }
                else
                {
                    spriteRend.sprite = box;
                }
                if (specialParticleVar == null)
                {
                    specialParticleVar = Instantiate(specialParticle, this.gameObject.transform.position, Quaternion.identity);
                }
            }
        }
    }

    private void OnMouseOver()
    {
        if (gameManagement.dialogueText.text == "")
        {
            gameManagement.DisplayDialogueText(GeneDialogueText());
        }
    }

    private void OnMouseDown()
    {
        if (gameManagement != null && gameManagement.enableClickRock)
        {
            //Cursor.SetCursor(cursorTextureDown, Vector2.zero, CursorMode.Auto);
            // play sound
        }
    }

    private void OnMouseUp()
    {
        if (isWorkstation)
        {
            if (gameManagement != null)
            {
                gameManagement.OpenWindow();
            }
        }
        else if (isCrystal)
        {
            if (gameManagement != null)
            {
                gameManagement.OpenHome();
            }
        }
        else
        {
            // debug only
            if (gameManagement != null && gameManagement.enableClickRock)
            {
                //gameManagement.IncreaseDay();
                //gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption);

                //if (soundManagement != null)
                //{
                //    soundManagement.PlayRandomDestroyNoise();
                //}

                //Cursor.SetCursor(cursorTextureUp, Vector2.zero, CursorMode.Auto);

                TakeDamage(1);
            }

            if (hitPoints <= 0 && board.ItemMap[column, row] != ItemType.None)
            {
                PerformItem();
                DestroyBackgroundTile();
                board.DecreaseRowFunc();
            }
        }
    }

    private void OnMouseEnter()
    {
        if (gameManagement != null)
        {
            gameManagement.DisplayDialogueText(GeneDialogueText());

            if (gameManagement.enableClickRock)
            {
                //if(spriteRend != null)
                //{
                //    spriteRend.material.color = Color.white;
                //}
                //Cursor.SetCursor(cursorTextureOver, Vector2.zero, CursorMode.Auto); // don't change cursor
            }
        }
    }

    private void OnMouseExit()
    {
        if (gameManagement != null)
        {
            gameManagement.DisplayDialogueText("");

            if (gameManagement.enableClickRock)
            {
                //if (spriteRend != null)
                //{
                //    //spriteRend.material.color = Color.black;
                //}
                Cursor.SetCursor(cursorTextureUp, Vector2.zero, CursorMode.Auto); // reset to default cursor
            }
        }
    }

    public void DisplaySpecialRock()
    {
        if (specialParticleVar == null)
        {
            specialParticleVar = Instantiate(specialParticle, this.gameObject.transform.position, Quaternion.identity);
        }
    }

    public void DestroyBackgroundTile()
    {
        board.rockTiles[column, row] = null;
        board.rockTileCount--;
        board.UpdateDepth(board.height - row);

        if (specialParticleVar != null)
        {
            Destroy(specialParticleVar);
        }
        Destroy(this.gameObject);
    }

    private void PerformItem()
    {
        if (board.ItemMap[column, row] == ItemType.Chip)
        {
            // popup display to be added
            //Debug.Log(board.msgMap[column, row]);

            gameManagement.DisplayLogText(board.msgMap[column, row] + "\n"); // need to be before SetGameEnd()
        }
        else
        {
            gameManagement.DisplayLogText(board.msgMap[column, row] + "\n"); // need to be before SetGameEnd()
        }

        if (board.ItemMap[column, row] == ItemType.Ship)
        {
            gameManagement.SetGameEnd();
            soundManagement.PlayRandomWinSound();
        }
        else if (board.ItemMap[column, row] == ItemType.Chip)
        {
            Instantiate(chipParticle, this.gameObject.transform.position, Quaternion.identity);

            if (soundManagement != null)
            {
                soundManagement.PlayRandomPickupNoise();
            }
        }
        else if (board.ItemMap[column, row] == ItemType.Radar)
        {
            Instantiate(chipParticle, this.gameObject.transform.position, Quaternion.identity);
            board.DisplayRadar();

            if (soundManagement != null)
            {
                soundManagement.PlayRandomPickupNoise();
            }
        }
        else if (board.ItemMap[column, row] == ItemType.Oxygen)
        {
            gameManagement.ConsumeOxygen(gameManagement.itemOxygenVal * -1);

            if (soundManagement != null)
            {
                soundManagement.PlayRandomPickupNoise();
            }
        }
        else if (board.ItemMap[column, row] == ItemType.Waste)
        {
            gameManagement.ConsumeOxygen(gameManagement.itemWasteVal * 1);

            if (soundManagement != null)
            {
                soundManagement.PlayRandomPickupNoise();
            }
        }
    }

    private string GeneDialogueText()
    {
        string answerStr = "";

        if (spriteRend.material.color == Color.black)
        {
            answerStr = "Unexplored area";
        }
        else if (spriteRend.sprite == specialRock)
        {
            answerStr = "Rock: breakable";
        }
        else if (spriteRend.sprite == wholeRock)
        {
            answerStr = "Rock: NOT breakable";
        }
        else if (spriteRend.sprite == box)
        {
            answerStr = "Box: click to open";
        }
        else if (spriteRend.sprite == ship)
        {
            answerStr = "Ship: click to leave this planet";
        }
        else if (spriteRend.sprite == chip)
        {
            answerStr = "Data Chip: click to open";
        }
        else if (spriteRend.sprite == spriteTool)
        {
            answerStr = "Workstation: click for crafting";
        }
        else if (spriteRend.sprite == oxygen)
        {
            answerStr = "Oxygen: oxygen value +" + gameManagement.itemOxygenVal;
        }
        else if (spriteRend.sprite == waste)
        {
            answerStr = "Pollutant: oxygen value -" + gameManagement.itemWasteVal;
        }
        else if (spriteRend.sprite == crystal)
        {
            answerStr = "Crystal: teleportation";
        }
        else
        {
            answerStr = "N/A";
        }

        return answerStr;
    }
}

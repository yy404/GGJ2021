using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    public Sprite specialRock;
    public Sprite box;
    public Texture2D cursorTextureOver;
    public Texture2D cursorTextureDown;
    public Texture2D cursorTextureUp;
    public GameObject chipParticle;

    public int column; //x
    public int row; //y

    private SpriteRenderer spriteRend;
    private GameManagement gameManagement;
    private SoundManagement soundManagement;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        //spriteRend.sprite = specialRock;
        gameManagement = FindObjectOfType<GameManagement>();
        soundManagement = FindObjectOfType<SoundManagement>();
        board = FindObjectOfType<Board>();

        if (board.ItemMap[column, row] != ItemType.None)
        {
            hitPoints = 2;
        }
        else
        {
            hitPoints = 1;
        }

        spriteRend.material.color = Color.black;
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPoints <= 0)
        {
            gameManagement.DisplayLogText(board.msgMap[column, row]); // need to be before SetGameEnd()

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

            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if (spriteRend.material.color == Color.black)
        {
            spriteRend.material.color = Color.white;
        }
        else
        {
            hitPoints -= damage;
        }
    }

    private void OnMouseDown()
    {
        if (gameManagement != null && gameManagement.enableClickRock)
        {
            Cursor.SetCursor(cursorTextureDown, Vector2.zero, CursorMode.Auto);
            // play sound
        }
    }

    private void OnMouseUp()
    {
        if (gameManagement != null && gameManagement.enableClickRock)
        {
            gameManagement.IncreaseDay();
            gameManagement.ConsumeOxygen(gameManagement.oxygenDailyConsumption);

            if (soundManagement != null)
            {
                soundManagement.PlayRandomDestroyNoise();
            }

            TakeDamage(1);
            if ((hitPoints == 1) && (board.ItemMap[column, row] != ItemType.None))
            {
                spriteRend.sprite = box;
            }

            Cursor.SetCursor(cursorTextureUp, Vector2.zero, CursorMode.Auto);
        }
    }

    private void OnMouseEnter()
    {
        if (gameManagement != null && gameManagement.enableClickRock)
        {
            if (spriteRend != null)
            {
                //spriteRend.material.color = Color.white;
                Cursor.SetCursor(cursorTextureOver, Vector2.zero, CursorMode.Auto); // don't change cursor
            }
        }
    }

    private void OnMouseExit()
    {
        if (gameManagement != null && gameManagement.enableClickRock)
        {
            if (spriteRend != null)
            {
                //spriteRend.material.color = Color.black;
            }
            Cursor.SetCursor(cursorTextureUp, Vector2.zero, CursorMode.Auto); // reset to default cursor
        }
    }

    public void DisplaySpecialRock()
    {
        if (spriteRend.sprite != box)
        {
            spriteRend.sprite = specialRock;
        }
    }
}

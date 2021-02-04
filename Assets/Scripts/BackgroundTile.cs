using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    public Sprite specialRock;
    public Sprite box;
    public Texture2D cursorTextureDown;
    public Texture2D cursorTextureUp;
    public GameObject chipParticle;

    public int column; //x
    public int row; //y

    private SpriteRenderer spriteRend;
    private GameManagement gameManagement;

    // Start is called before the first frame update
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        //spriteRend.sprite = specialRock;
        gameManagement = FindObjectOfType<GameManagement>();

        if (gameManagement.ItemMap[column, row] != ItemType.None)
        {
            hitPoints = 2;
        }
        else
        {
            hitPoints = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPoints <= 0)
        {
            if (gameManagement.ItemMap[column, row] == ItemType.Ship)
            {
                gameManagement.SetGameEnd();
            }
            else if (gameManagement.ItemMap[column, row] == ItemType.Chip)
            {
                Instantiate(chipParticle, this.gameObject.transform.position, Quaternion.identity);
            }
            else if (gameManagement.ItemMap[column, row] == ItemType.Radar)
            {
                Instantiate(chipParticle, this.gameObject.transform.position, Quaternion.identity);
                gameManagement.DisplayRadar();
            }

            gameManagement.DisplayLogText(gameManagement.msgMap[column, row]);

            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;
    }

    private void OnMouseDown()
    {
        Cursor.SetCursor(cursorTextureDown, Vector2.zero, CursorMode.Auto);
        // play sound
    }

    private void OnMouseUp()
    {
        TakeDamage(1);

        if ((hitPoints == 1) && (gameManagement.ItemMap[column, row] != ItemType.None))
        {
            spriteRend.sprite = box;
        }

        Cursor.SetCursor(cursorTextureUp, Vector2.zero, CursorMode.Auto);
    }

    public void DisplaySpecialRock()
    {
        if (spriteRend.sprite != box)
        {
            spriteRend.sprite = specialRock;
        }
    }
}

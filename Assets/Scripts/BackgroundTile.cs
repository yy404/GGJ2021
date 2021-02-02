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

    public int column; //x
    public int row; //y

    private SpriteRenderer spriteRend;
    private GameManagement gameManagement;

    // Start is called before the first frame update
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        spriteRend.sprite = specialRock;
        gameManagement = FindObjectOfType<GameManagement>();
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

        if (gameManagement.ItemMap[column, row] == ItemType.Box)
        {
            spriteRend.sprite = box;
            hitPoints += 1;
            gameManagement.ItemMap[column, row] = ItemType.Ship;
        }

        Cursor.SetCursor(cursorTextureUp, Vector2.zero, CursorMode.Auto);
    }
}

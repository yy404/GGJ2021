using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    public Sprite specialRock;
    public Sprite box;

    private SpriteRenderer spriteRend;

    // Start is called before the first frame update
    void Start()
    {
        spriteRend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hitPoints <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        hitPoints -= damage;

        if (hitPoints == 1)
        {
            spriteRend.sprite = specialRock;
            //spriteRend.sprite = box;
        }
    }
}

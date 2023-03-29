using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealthBar : NetworkBehaviour
{
    [SyncVar]
    private Transform bar;
    private float sizeNormalized;

    private void Start()
    {
        bar = transform.Find("HealthActual");
        SetColor(Color.green);
    }
    public void SetHealth(int maxHealth, int health)
    {
        if(health <= 0){
            sizeNormalized = 0;
        }else{
            sizeNormalized = (float)health/(float)maxHealth;
        }
        bar.localScale = new Vector3(sizeNormalized, 1f);
        if(sizeNormalized <= .33f){
            SetColor(Color.yellow);
        }else{
            SetColor(Color.green);
        }
    }
    public void SetColor(Color color)
    {
        bar.Find("HBASprite").GetComponent<SpriteRenderer>().color = color;
    }
}

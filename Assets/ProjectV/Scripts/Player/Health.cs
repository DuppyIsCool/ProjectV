using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Health : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChange))]
    public int health;

    [SyncVar(hook = nameof(OnHealthChange))]
    public int maxHealth;
    
    [SerializeField]
    private GameObject healthBarPrefab;
    private GameObject healthBar;
    private SpriteRenderer barRender;

    private Vector2 tempScale;
    void Start()
    {
        if (isServer) 
        {
            health = maxHealth;
        }

        healthBar = Instantiate(healthBarPrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + (gameObject.GetComponent<Renderer>().bounds.size.y), 0), Quaternion.identity);
        healthBar.transform.SetParent(gameObject.transform);   
        barRender = healthBar.transform.GetChild(0).GetComponent<SpriteRenderer>();
        UpdatePlayerHealthBar();
    }

    void OnHealthChange(int oldHealth, int newHealth)
    {
        if (isClientOnly)
            print("Client: My health changed from " + oldHealth + " to " + newHealth);

        if (isServer)
        {
            print("Server: Player object health changed from " + oldHealth + " to " + newHealth);
        }

        UpdatePlayerHealthBar();
    }

    void OnMaxHealthChange(int oldMaxHealth, int newMaxHealth) 
    {
        if (isClientOnly)
            print("Client: My max health changed from " + oldMaxHealth + " to " + newMaxHealth);

        if (isServer)
        {
            print("Server: Player object max health changed from " + oldMaxHealth + " to " + newMaxHealth);
        }

        UpdatePlayerHealthBar();
    }

    void UpdatePlayerHealthBar() 
    {
        tempScale = healthBar.transform.GetChild(0).localScale;

        if (health <= 0)
        {
            tempScale.x = 0;
        }
        else
        {
            tempScale.x = (float)health / (float)maxHealth;
        }
        if (tempScale.x > 0.66)
        {
            barRender.color = Color.green;
        }
        else if (tempScale.x > 0.33f)
        {
            barRender.color = Color.yellow;
        }
        else 
        {
            barRender.color = Color.red;
        }
        
        healthBar.transform.GetChild(0).transform.localScale = tempScale;

    }

    [Server]
    public void ApplyDamage(int amount)
    {
        if ((health - amount) <= 0)
        {
            print("Player object has died. Setting health back to full");
            health = maxHealth;
        }
        else 
        {
            health -= amount;
            print("Player object took " + amount + " damage");
        }
    }

    [Server]
    public void Heal(int amount) 
    {
        if ((health + amount) >= maxHealth)
        {
            print("Player object was healed to full died.");
            health = maxHealth;
        }
        else
        {
            health += amount;
            print("Player object healed " + amount + " damage");
        }
    }
}

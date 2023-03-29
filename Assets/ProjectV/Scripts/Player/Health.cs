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
    
    public GameObject healthBar;

    
    void Start()
    {
        if (isServer) 
        {
            health = maxHealth;
        }
        healthBar = Instantiate(healthBar, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y + (gameObject.GetComponent<Renderer>().bounds.size.y), 0), Quaternion.identity);
        healthBar.transform.SetParent(gameObject.transform);
        healthBar.transform.localScale = new Vector3 (gameObject.GetComponent<Renderer>().bounds.size.x * .008f, gameObject.GetComponent<Renderer>().bounds.size.y * .008f);
        healthBar.GetComponent<HealthBar>().SetHealth(maxHealth, health);
    }

    void OnHealthChange(int oldHealth, int newHealth)
    {
        if (isClientOnly)
            print("Client: My health changed from " + oldHealth + " to " + newHealth);

        if (isServer)
        {
            print("Server: Player object health changed from " + oldHealth + " to " + newHealth);
        }
    }

    void OnMaxHealthChange(int oldMaxHealth, int newMaxHealth) 
    {
        if (isClientOnly)
            print("Client: My max health changed from " + oldMaxHealth + " to " + newMaxHealth);

        if (isServer)
        {
            print("Server: Player object max health changed from " + oldMaxHealth + " to " + newMaxHealth);
        }
    }

    [Server]
    public void ApplyDamage(int amount)
    {
        if ((health - amount) <= 0)
        {
            print("Player object has died. Setting health back to full");
            health = maxHealth;
            healthBar.GetComponent<HealthBar>().SetHealth(maxHealth, health);
        }
        else 
        {
            health -= amount;
            healthBar.GetComponent<HealthBar>().SetHealth(maxHealth, health);
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
            healthBar.GetComponent<HealthBar>().SetHealth(maxHealth, health);
        }
        else
        {
            health += amount;
            healthBar.GetComponent<HealthBar>().SetHealth(maxHealth, health);
            print("Player object healed " + amount + " damage");
        }
    }
}

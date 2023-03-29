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
    public GameObject headHealthBar;
    
    void Start()
    {
        if (isServer) 
        {
            health = maxHealth;
        }
        headHealthBar = Instantiate(headHealthBar, new Vector3(0, 0, 0), Quaternion.identity);
        headHealthBar.transform.SetParent(GameObject.Find("Canvas").transform);
        headHealthBar.transform.position = new Vector3(0, 0, 0);
        healthBar = GameObject.Find("HealthBar");
        healthBar.GetComponent<HealthBar>().SetMaxHealth(maxHealth);
        headHealthBar.GetComponent<HealthBar>().SetMaxHealth(maxHealth);
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
        }
        else 
        {
            health -= amount;
            healthBar.GetComponent<HealthBar>().SetHealth(health);
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
            healthBar.GetComponent<HealthBar>().SetHealth(health);
            print("Player object healed " + amount + " damage");
        }
    }
}

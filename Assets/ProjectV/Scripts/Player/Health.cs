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

    void Start()
    {
        if (isServer) 
        {
            health = maxHealth;
        }
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
        if ((health -= amount) <= 0)
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
        if ((health += amount) >= maxHealth)
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

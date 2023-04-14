using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Health : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChange))]
    public int health;

    [SyncVar(hook = nameof(OnHealthChange))]
    public int maxHealth;
    
    
    [SerializeField]
    private SpriteRenderer callerRender;

    private Color originalColor;
    private GameObject healthbarUI;
    private bool colorSaved;
    private Slider slider;
    private Image fill;

    void Start()
    {
        if (isServer) 
        {
            health = maxHealth;
        }
        if (isLocalPlayer)
        {
            healthbarUI = GameObject.Find("HealthBar 1");
            slider = healthbarUI.GetComponent<Slider>();
            fill = healthbarUI.transform.GetChild(0).GetComponent<Image>();
            SetMaxHealth(maxHealth);
            //healthbarUI = GameObject.Find("HealthBar 1");
            //healthbarUI.GetComponent<UIHealthBar>().SetMaxHealth(maxHealth);
        }
        /*healthbarUI = GameObject.Find("HealthBar 1");
        slider = healthbarUI.GetComponent<Slider>();
        fill = healthbarUI.transform.GetChild(0).GetComponent<Image>();
        callerRender = gameObject.transform.GetComponent<SpriteRenderer>();
        SetMaxHealth(maxHealth);*/
        callerRender = gameObject.transform.GetComponent<SpriteRenderer>();
        colorSaved = false;
    }
    void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    void SetHealth(int health)
    {
        slider.value = health;
        if (slider.value <= (slider.maxValue * .25f)){
            fill.color = Color.red;
        }else if(slider.value <= (slider.maxValue * .5f)){
            fill.color = Color.yellow;
        }else{
            fill.color = Color.green;
        }
    }
    void OnHealthChange(int oldHealth, int newHealth)
    {
        if (isClientOnly)
            print("Client: My health changed from " + oldHealth + " to " + newHealth);

        if (isServer)
        {
            print("Server: Object health changed from " + oldHealth + " to " + newHealth);
        }
        if (colorSaved == false)
        {
            originalColor = gameObject.transform.GetComponent<SpriteRenderer>().color;
            colorSaved = true;
        }
        healthChangeAnimations(oldHealth, newHealth);
        SetHealth(health);
        //healthbarUI.GetComponent<UIHealthBar>().SetHealth(newHealth);
    }

    void OnMaxHealthChange(int oldMaxHealth, int newMaxHealth) 
    {
        if (isClientOnly)
            print("Client: My max health changed from " + oldMaxHealth + " to " + newMaxHealth);

        if (isServer)
        {
            print("Server: Object max health changed from " + oldMaxHealth + " to " + newMaxHealth);
        }
        SetMaxHealth(maxHealth);
        //healthbarUI.GetComponent<UIHealthBar>().SetMaxHealth(newMaxHealth);
    }

    void healthChangeAnimations(int oldHealth, int newHealth) 
    {
        Color tmp = originalColor;
        callerRender.color = tmp;
        if (newHealth <= 0)
        {
            DeathAnimation();
        }
        else if (newHealth < oldHealth)
        {
            tmp.r = tmp.r * 1.5f;
            tmp.g = tmp.g / 1.5f;
            tmp.b = tmp.b / 1.5f;
            callerRender.color = tmp;
            HealthDownAnimation();
        }
        else if (newHealth > oldHealth)
        {
            tmp.g = tmp.g * 1.5f;
            tmp.r = tmp.r / 1.5f;
            tmp.b = tmp.b / 1.5f;
            callerRender.color = tmp;
            HealthUpAnimation();
        }
    }
    void HealthUpAnimation()
    {
        Color tmp = callerRender.color;
        tmp.g = tmp.g * .90f;
        tmp.r = tmp.r / .90f;
        tmp.b = tmp.b / .90f;
        callerRender.color = tmp;
        if (tmp.g <= originalColor.g)
        {
            callerRender.color = originalColor;
        }
        else
        {
            Invoke("HealthUpAnimation", .075f);
        }
    }
    void HealthDownAnimation()
    {
        Color tmp = callerRender.color;
        tmp.r = tmp.r * .90f;
        tmp.g = tmp.g / .90f;
        tmp.b = tmp.b / .90f;
        callerRender.color = tmp;
        if (tmp.r <= originalColor.r)
        {
            callerRender.color = originalColor;
        }
        else
        {
            Invoke("HealthDownAnimation", .075f);
        }
    }
    void DeathAnimation()
    {
        Color tmp = callerRender.color;
        tmp.a = tmp.a * .9f;
        tmp.r = tmp.r * 1.5f;
        tmp.g = tmp.g * .5f;
        tmp.b = tmp.b * .5f;
        callerRender.color = tmp;
        if (tmp.a < .1)
        {
            //Destroy(gameObject);
            callerRender.color = originalColor;//For Testing

        }
        else
        {
            Invoke("DeathAnimation", .075f);
        }
    }
    [Server]
    public void ApplyDamage(int amount)
    {
        if ((health - amount) <= 0)
        {
            print("Player object has died. Setting health back to full");
            health -= amount;
            //health = maxHealth;
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

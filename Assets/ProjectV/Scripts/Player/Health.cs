using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField]
    [SyncVar(hook = nameof(OnHealthChanged))] private int currentHealth;

    [SerializeField] private Slider healthSlider;
    [SerializeField] private Gradient healthGradient;
    [SerializeField] private Image fill;

    private Material originalMaterial;
    private Color originalColor;
    [SerializeField] private Color damageColor = new Color(1, 0, 0, 0.3f);
    [SerializeField] private float damageFlashDuration = 0.3f;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GameObject canvasSlider = GameObject.Find("Canvas").transform.Find("HealthBar").gameObject;
        canvasSlider.SetActive(true);
        healthSlider = canvasSlider.GetComponent<Slider>();
        fill = canvasSlider.transform.GetChild(0).GetComponent<Image>();

        UpdateHealthUI();
    }

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;
        originalColor = originalMaterial.color;
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            // Handle player death here (e.g., respawn, disable movement, etc.)
        }

        StartCoroutine(FlashDamage());
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if(isLocalPlayer)
            UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthSlider.value = (float)currentHealth / maxHealth;
        fill.color = healthGradient.Evaluate(healthSlider.normalizedValue);
    }

    private IEnumerator FlashDamage()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = damageColor;

        yield return new WaitForSeconds(damageFlashDuration);

        renderer.material.color = originalColor;
    }

    [Server]
    public void ApplyDamage(int amount)
    {
        if ((currentHealth - amount) <= 0)
        {
            print("Player object has died. Setting health back to full");
            currentHealth = 100;  
            //health = maxHealth;
        }
        else 
        {
            currentHealth -= amount;
            print("Player object took " + amount + " damage");
            RpcTakeDamage(currentHealth);
        }
    }

    [Server]
    public void Heal(int amount) 
    {
        if ((currentHealth + amount) >= maxHealth)
        {
            print("Player object was healed to full died.");
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
            print("Player object healed " + amount + " damage");
        }
    }
}

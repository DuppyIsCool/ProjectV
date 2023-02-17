using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerProjectile : MonoBehaviour
{
    private int damage;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If we collided with a player
        if (collision.collider.CompareTag("Player")) 
        {
            collision.collider.gameObject.GetComponent<Health>().ApplyDamage(damage);
        }
        
        DestroySelf();
    }



    private void Start()
    {
        //We want to destroy the projectile if it lasts for longer than 8 seconds
        Invoke(nameof(DestroySelf), 8);
    }

    public void SetDamage(int damage) 
    {
        this.damage = damage;
    }

    public void ApplyForce(Vector2 direction, float speed)
    {
        GetComponent<Rigidbody2D>().AddForce(speed * direction, ForceMode2D.Impulse);
    }

    // destroy for everyone on the server
    void DestroySelf()
    {
        Destroy(gameObject);
    }
}

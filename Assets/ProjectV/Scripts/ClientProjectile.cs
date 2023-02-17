using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientProjectile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DestroySelf();
    }


     
    private void Start()
    {
        //We want to destroy the projectile if it lasts for longer than 8 seconds
        Invoke(nameof(DestroySelf), 8);
    }

    public void ApplyForce(Vector2 direction, float speed)
    {
        GetComponent<Rigidbody2D>().AddForce(speed * direction, ForceMode2D.Impulse);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }
}

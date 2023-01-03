using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Projectile : NetworkBehaviour
{
    public int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*
        if (isServer)
        {
            if (collision.GetComponent<Health>() != null)
            {
                Health health = collision.GetComponent<Health>();
                health.ApplyDamage(damage);
            }
        }

        Destroy(this.gameObject);*/
    }



    private void Start()
    {
        Invoke(nameof(DestroySelf), 8);
    }

    public void ApplyForce(Vector2 direction, float speed) 
    {
        GetComponent<Rigidbody2D>().AddForce(speed * direction, ForceMode2D.Impulse);
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}

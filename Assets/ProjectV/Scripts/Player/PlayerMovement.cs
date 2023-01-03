using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 movement; 

    [SerializeField]
    private float speed = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        //Calculate velocity
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
}

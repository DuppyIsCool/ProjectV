using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class PlayerMovement : NetworkBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    private Vector2 movement; 
    public Animator animator;
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

        animator.SetFloat("horizontal", movement.x);
        animator.SetFloat("vertical", movement.y);
        animator.SetFloat("speed", movement.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        //Calculate velocity
        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }
}

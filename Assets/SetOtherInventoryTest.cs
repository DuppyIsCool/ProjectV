using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class SetOtherInventoryTest : NetworkBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
            Inventory pInv = collision.gameObject.GetComponent<Inventory>();
            
        }
    }
}

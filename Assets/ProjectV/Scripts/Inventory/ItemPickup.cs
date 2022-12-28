using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemPickup : NetworkBehaviour
{
    [SerializeField]
    [Tooltip("This represents the type of item")]
    private Item item;

    [SerializeField]
    [Tooltip("This represents the item amount")]
    private int amount;

    public void Start()
    {
        this.GetComponent<SpriteRenderer>().sprite = item.sprite;
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag.Equals("Player"))
        {
            //Get the player's inventory
            Inventory playerInventory = collider.gameObject.GetComponent<Inventory>();

            //If the inventory doesn't exist, return and print an error
            if (playerInventory == null)
            {
                print("Encountered error when retrieving player inventory");
                return;
            }
            
            //If this is the server
            if (isServer)
            {
                //Try adding the item to the inventory
                if (playerInventory.AddItem(item, amount))
                {
                    //If it works, destroy this object on the network
                    NetworkServer.Destroy(this.gameObject);
                }
            }
            else if(isLocalPlayer)
            {
                //If we're a client, and we can pickup the item
                if (playerInventory.CanAddItem(item, amount)) 
                {
                    //We only want to set it active as false, as the server will delete it when it detects the pickup
                    this.gameObject.SetActive(false);
                }
            }
        }  
    }
}
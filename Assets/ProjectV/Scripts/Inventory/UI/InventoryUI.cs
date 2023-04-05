using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : NetworkBehaviour
{
    private InventoryContainerUI playerInventoryContainer,otherInventoryContainer;
    public Inventory currentInventory;
    public Inventory otherInventory;

    public void Update()
    {
        //Code for toggling the panel
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            playerInventoryContainer.ToggleVisibility();
        }
    }
    public override void OnStartLocalPlayer() 
    {
        //Get the UI gameobject
        playerInventoryContainer = GameObject.Find("PlayerInventoryContainerUI").GetComponent<InventoryContainerUI>();
        otherInventoryContainer = GameObject.Find("OtherInventoryContainerUI").GetComponent<InventoryContainerUI>();
        currentInventory = gameObject.GetComponent<Inventory>();
        currentInventory.content.Callback += PlayerInventoryUIUpdates;

        //If its first time setup, add new empty elements.
        if (playerInventoryContainer.transform.childCount == 0)
        {
            playerInventoryContainer.Setup(currentInventory);
        }
        

        // Process initial SyncList payload
        for (int index = 0; index < currentInventory.content.Count; index++)
        {
            //This calls the InventoryUIUpdates, adding every item that is in the currentInventory
            PlayerInventoryUIUpdates(SyncList<InventoryItem>.Operation.OP_SET, index, new InventoryItem(), currentInventory.content[index]);
        }

        base.OnStartLocalPlayer();

    }
    public override void OnStopClient()
    {
        //Unsubscribe from callback
        currentInventory.content.Callback -= PlayerInventoryUIUpdates;
        base.OnStopClient();
    }

    void PlayerInventoryUIUpdates(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem)
    {
        //Test code printing for inventory status
        if (isLocalPlayer)

            switch (op)
            {
                case SyncList<InventoryItem>.Operation.OP_ADD:
                    break;

                case SyncList<InventoryItem>.Operation.OP_INSERT:
                    break;

                case SyncList<InventoryItem>.Operation.OP_SET:
                    playerInventoryContainer.SetItem(oldItem, newItem, index);
                    break;

                case SyncList<InventoryItem>.Operation.OP_REMOVEAT:
                    break;

                case SyncList<InventoryItem>.Operation.OP_CLEAR:
                    playerInventoryContainer.Clear();
                    break;


            }
    }
}

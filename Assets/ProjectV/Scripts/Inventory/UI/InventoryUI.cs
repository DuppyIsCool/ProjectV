using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : NetworkBehaviour
{
    private GameObject playerInventory, otherInventory;
    private CanvasGroup playerGroup, otherGroup;
    [SerializeField]
    private Inventory playerContent, otherContent;
    public void Update()
    {
        //Code for toggling the panel
        if (Input.GetKeyDown(KeyCode.E) && isLocalPlayer) 
        {

            if (playerGroup.interactable == false)
            {
                playerGroup.alpha = 1;
                playerGroup.interactable = true;
            }
            else 
            {
                playerGroup.alpha = 0;
                playerGroup.interactable = false;
            }

        }
    }
    public override void OnStartLocalPlayer() 
    {
        if (isLocalPlayer)
        {
            //Get the UI gameobject
            playerInventory = GameObject.Find("PlayerInventory");
            otherInventory = GameObject.Find("OtherInventory");
            playerGroup = playerInventory.GetComponent<CanvasGroup>();
            otherGroup = otherInventory.GetComponent<CanvasGroup>();

            playerContent.content.Callback += PlayerInventoryUIUpdates;
            //If its first time setup, add new empty elements.
            if (playerInventory.transform.childCount == 0)
            {
                playerInventory.GetComponent<InventoryContainerUI>().Setup(playerInventory.GetComponent<Inventory>().size);
            }

            // Process initial SyncList payload
            for (int index = 0; index < gameObject.GetComponent<Inventory>().size; index++)
            {
                //This calls the InventoryUIUpdates, adding every item that is in the currentInventory
                PlayerInventoryUIUpdates(SyncList<InventoryItem>.Operation.OP_SET, index, new InventoryItem(), playerContent.content[index]);
            }

        }
        base.OnStartLocalPlayer();
    }

    public void OpenOtherInventory(Inventory inv) 
    {
        if (isLocalPlayer)
        {
            //Set the inventory
            otherContent = inv;

            //Setup the UI
            otherContent.content.Callback += OtherInventoryUIUpdates;

            if (otherInventory.transform.childCount == 0)
            {
                otherInventory.GetComponent<InventoryContainerUI>().Setup(playerInventory.GetComponent<Inventory>().size);
            }

            // Process initial SyncList payload
            for (int index = 0; index < gameObject.GetComponent<Inventory>().size; index++)
            {
                OtherInventoryUIUpdates(SyncList<InventoryItem>.Operation.OP_SET, index, new InventoryItem(), otherContent.content[index]);
            }

            //Show the inventory
            otherGroup.alpha = 1;
            otherGroup.interactable = true;
        }
    }

    public void CloseOtherInventory() 
    {
        if (isLocalPlayer)
        {
            otherContent.content.Callback -= OtherInventoryUIUpdates;
            otherInventory.GetComponent<InventoryContainerUI>().Clear();
            otherContent = null;
            otherGroup.alpha = 0;
            otherGroup.interactable = false;
        }
    }

    public void MoveItem(GameObject senderContainer, GameObject receiverContainer, int fromSlot, int toSlot) 
    {
        if (isLocalPlayer)
        {
            Inventory sender = null, receiver = null;

            if (senderContainer.Equals(playerInventory))
            {
                sender = playerContent;
            }
            else if (senderContainer.Equals(otherInventory))
            {
                sender = otherContent;
            }

            if (receiverContainer.Equals(playerInventory))
            {
                receiver = playerContent;
            }
            else if (receiverContainer.Equals(otherInventory))
            {
                receiver = otherContent;
            }

            MoveItemCMD(sender, receiver, fromSlot, toSlot);
        }     
    }

    [Command]
    private void MoveItemCMD(Inventory sender, Inventory receiver, int fromSlot, int toSLot) 
    {
        if (sender != null && receiver != null) 
        {
            //Out of bounds check
            if ((sender.size > fromSlot && fromSlot >= 0) && (receiver.size > toSLot && toSLot >= 0)) 
            {



            }
        }
    }

    public override void OnStopClient()
    {
        if (isLocalPlayer)
        {
            //Unsubscribe from callback
            gameObject.GetComponent<Inventory>().content.Callback -= PlayerInventoryUIUpdates;
        }
        base.OnStopClient();
    }

    void PlayerInventoryUIUpdates(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem)
    {
        //Test code printing for inventory status
        if (isLocalPlayer)
        {

            switch (op)
            {
                case SyncList<InventoryItem>.Operation.OP_ADD:
                    break;

                case SyncList<InventoryItem>.Operation.OP_INSERT:
                    break;

                case SyncList<InventoryItem>.Operation.OP_SET:
                    playerInventory.GetComponent<InventoryContainerUI>().SetItem(oldItem, newItem, index);
                    break;

                case SyncList<InventoryItem>.Operation.OP_REMOVEAT:
                    break;

                case SyncList<InventoryItem>.Operation.OP_CLEAR:
                    playerInventory.GetComponent<InventoryContainerUI>().Clear();
                    break;
            }
        }
    }

    void OtherInventoryUIUpdates(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem)
    {
        //Test code printing for inventory status
        if (isLocalPlayer)
        {

            switch (op)
            {
                case SyncList<InventoryItem>.Operation.OP_ADD:
                    break;

                case SyncList<InventoryItem>.Operation.OP_INSERT:
                    break;

                case SyncList<InventoryItem>.Operation.OP_SET:
                    otherInventory.GetComponent<InventoryContainerUI>().SetItem(oldItem, newItem, index);
                    break;

                case SyncList<InventoryItem>.Operation.OP_REMOVEAT:
                    break;

                case SyncList<InventoryItem>.Operation.OP_CLEAR:
                    otherInventory.GetComponent<InventoryContainerUI>().Clear();
                    break;
            }
        }
    }
}

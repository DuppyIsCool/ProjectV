using UnityEngine;
using Mirror;

public class InventoryUI : NetworkBehaviour
{
    private GameObject playerInventory, otherInventory;
    private CanvasGroup playerGroup, otherGroup;
    public enum ContainerType { Player, Other }
    [SerializeField]
    public Inventory playerContent, otherContent;
    public float interactionRange = 5f; // Add a customizable interaction range
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

        if (Input.GetKeyDown(KeyCode.Escape) && isLocalPlayer) 
        {
            RequestCloseOtherInventory();
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
            playerInventory.GetComponent<InventoryContainerUI>().inventoryUI = this;
            otherInventory.GetComponent<InventoryContainerUI>().inventoryUI = this;
            if (playerInventory.transform.childCount == 0)
            {
                playerInventory.GetComponent<InventoryContainerUI>().Setup(GetComponent<Inventory>().size);
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
            CmdCheckProximityAndOpen(inv.gameObject);
        }
    }
    public InventoryItem GetItemAtSlot(int slot, ContainerType containerType)
    {
        if (containerType == ContainerType.Player)
        {
            return playerContent.content[slot];
        }
        else if (containerType == ContainerType.Other)
        {
            return otherContent.content[slot];
        }
        return new InventoryItem().GetEmptyItem();
    }

    [Command]
    private void CmdCheckProximityAndOpen(GameObject otherInventoryObj)
    {
        if (Vector3.Distance(gameObject.transform.position, otherInventoryObj.transform.position) <= interactionRange)
        {
            TargetOpenOtherInventory(connectionToClient, otherInventoryObj.GetComponent<Inventory>());
        }
    }

    [TargetRpc]
    private void TargetOpenOtherInventory(NetworkConnection target, Inventory inv)
    {
        if (isLocalPlayer)
        {
            // Set the inventory
            otherContent = inv;

            // Setup the UI
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

            // Show the inventory
            otherGroup.alpha = 1;
            otherGroup.interactable = true;
        }
    }

    public void RequestCloseOtherInventory()
    {
        if (isLocalPlayer)
        {
            CmdRequestCloseOtherInventory();
        }
    }

    [Command]
    private void CmdRequestCloseOtherInventory()
    {
        TargetCloseOtherInventory(connectionToClient);
    }

    [TargetRpc]
    private void TargetCloseOtherInventory(NetworkConnection target)
    {
        if (isLocalPlayer)
        {
            if (otherContent != null)
            {
                otherContent.content.Callback -= OtherInventoryUIUpdates;
                otherInventory.GetComponent<InventoryContainerUI>().Clear();
                otherContent = null;
                otherGroup.alpha = 0;
                otherGroup.interactable = false;
            }
        }
    }

    public void MoveItem(ContainerType senderType, ContainerType receiverType, int fromSlot, int toSlot, int moveAmount)
    {
        if (isLocalPlayer)
        {
            Inventory sender = null, receiver = null;

            if (senderType == ContainerType.Player)
            {
                sender = playerContent;
            }
            else if (senderType == ContainerType.Other)
            {
                sender = otherContent;
            }

            if (receiverType == ContainerType.Player)
            {
                receiver = playerContent;
            }
            else if (receiverType == ContainerType.Other)
            {
                receiver = otherContent;
            }

            MoveItemCMD(sender, receiver, fromSlot, toSlot, moveAmount);
        }
    }

    [Command]
    private void MoveItemCMD(Inventory sender, Inventory receiver, int fromSlot, int toSlot, int moveAmount)
    {
        if (sender != null && receiver != null)
        {
            // Out of bounds check
            if ((sender.size > fromSlot && fromSlot >= 0) && (receiver.size > toSlot && toSlot >= 0))
            {
                // Check if the destination slot is empty or the same item type and not exceeding the stack limit
                if (receiver.content[toSlot].item == null ||
                    (receiver.content[toSlot].item.id == sender.content[fromSlot].item.id &&
                    receiver.content[toSlot].amount + moveAmount <= receiver.content[toSlot].item.stacklimit))
                {
                    // Add sender's item to the receiver's slot
                    if (receiver.content[toSlot].item == null)
                    {
                        receiver.content[toSlot] = receiver.content[toSlot].ChangeItem(sender.content[fromSlot].item, moveAmount);
                    }
                    else
                    {
                        receiver.content[toSlot] = receiver.content[toSlot].ChangeQuantity(receiver.content[toSlot].amount + moveAmount);
                    }

                    // Update the sender's slot
                    if (sender.content[fromSlot].amount - moveAmount <= 0)
                    {
                        sender.content[fromSlot] = sender.content[fromSlot].GetEmptyItem();
                    }
                    else
                    {
                        sender.content[fromSlot] = sender.content[fromSlot].ChangeQuantity(sender.content[fromSlot].amount - moveAmount);
                    }
                }
                else
                {
                    // Swap items between sender's and receiver's slots
                    InventoryItem tempItem = sender.content[fromSlot];
                    sender.content[fromSlot] = receiver.content[toSlot];
                    receiver.content[toSlot] = tempItem;
                }
            }
        }
    }

    public int FindEmptySlot(ContainerType containerType)
    {
        Inventory targetInventory = containerType == ContainerType.Player ? playerContent : otherContent;
        for (int i = 0; i < targetInventory.size; i++)
        {
            if (targetInventory.content[i].item == null)
            {
                return i;
            }
        }
        return -1;
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
                    playerInventory.GetComponent<InventoryContainerUI>().SetItem(newItem, index);
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
                    otherInventory.GetComponent<InventoryContainerUI>().SetItem(newItem, index);
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

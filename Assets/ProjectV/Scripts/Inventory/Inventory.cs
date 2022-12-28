using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class Inventory : NetworkBehaviour
{
    public readonly SyncList<InventoryItem> inventory = new SyncList<InventoryItem>();
    [SerializeField] [SyncVar]
    private int size;

    [Server]
    private void SetupInventory() 
    {
        //Initializing the Inventory would go here
        print("Setup inventory here!");
    }

    public override void OnStartClient() 
    {
        inventory.Callback += OnInventoryUpdated;

        // Process initial SyncList payload
        for (int index = 0; index < inventory.Count; index++)
        {
            OnInventoryUpdated(SyncList<InventoryItem>.Operation.OP_ADD, index, new InventoryItem(), inventory[index]);
        }

}

    void OnInventoryUpdated(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem) 
    {
        //Test code printing for inventory status
        if(isLocalPlayer)

            switch (op) 
            {
                case SyncList<InventoryItem>.Operation.OP_ADD:
                    print(newItem.amount + " of " + newItem.item.id + " was added to my inventory as a new stack");
                    break;

                case SyncList<InventoryItem>.Operation.OP_INSERT:

                    break;

                case SyncList<InventoryItem>.Operation.OP_SET:
                    print("My " + oldItem.amount + " of " + oldItem.item.id + " became a " + newItem.amount + " of " + newItem.item.id);
                    break;

                case SyncList<InventoryItem>.Operation.OP_REMOVEAT:

                    break;

                case SyncList<InventoryItem>.Operation.OP_CLEAR:

                    break;


            }
    }

    [Server]
    public bool AddItem(Item item, int amount) 
    {
        //Loop to see if we can fit the item into a current stack
        for(int i = 0; i < inventory.Count; i++)
        {
            //Check if the ids are the same and if the amount that would be added does not exceed the stack limit.
            if (inventory[i].item.id == item.id && inventory[i].amount + amount <= inventory[i].item.stacklimit) 
            {
                inventory[i] = inventory[i].ChangeQuantity(inventory[i].amount + amount);
                return true;
            }
        }

        //If not, see if we can fit into its own stack in the inventory
        if (inventory.Count + 1 <= size) 
        {
            InventoryItem newItem = new InventoryItem { item = item, amount = amount };
            inventory.Add(newItem);
            return true;
        }
        return false;
    }

    public bool CanAddItem(Item item, int amount) 
    {
        foreach (InventoryItem invitem in inventory)
        {
            if (invitem.amount + amount <= invitem.item.stacklimit && invitem.item.id == item.id)
            {
                return true;
            }
        }

        if (inventory.Count + 1 <= size)
        {
            return true;
        }

        return false;
    }

    public void Awake()
    {
        if(isServer)
            SetupInventory();
    }

}

public struct InventoryItem 
{
    public Item item;
    public int amount;

    public InventoryItem ChangeQuantity(int newAmount) 
    {
        return new InventoryItem
        {
            item = this.item,
            amount = newAmount
        };
    }

    public InventoryItem GetEmptyItem()
        => new InventoryItem
        {
            item = null,
            amount = 0
        };
}

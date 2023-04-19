using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;
public class Inventory : NetworkBehaviour
{
    public readonly SyncList<InventoryItem> content = new SyncList<InventoryItem>();
    [SerializeField] [SyncVar] public int size;
    [SyncVar] public InventoryItem equippedItem = new InventoryItem().GetEmptyItem();

    [Server]
    private void SetupInventory() 
    {
        for (int i = 0; i < size; i++)
            content.Add(new InventoryItem().GetEmptyItem());
    }

    public override void OnStartServer()
    {
        if (isServer && content.Count == 0) 
        {
            SetupInventory();
        }
        base.OnStartServer();
    }

    [Command]
    public void EquipItemCmd(int index) 
    {
        if (index <= size && index >= 0)
        {
            equippedItem = content[index];
            print("Player has equipped item: " + equippedItem.item.id);           
        }
    }

    [Server]
    public bool AddItem(Item item, int amount) 
    {

        //Loop to see if we can fit the item into a current stack or an empty slot
        for(int i = 0; i < content.Count; i++)
        {
            if (content[i].item != null)
            {
                //Check if the ids are the same and if the amount that would be added does not exceed the stack limit.
                if (content[i].item.id == item.id && content[i].amount + amount <= content[i].item.stacklimit)
                {
                    content[i] = content[i].ChangeQuantity(content[i].amount + amount);
                    return true;
                }
            }

        }

        //Loop to see if we can fit it into an empty slot
        for (int i = 0; i < content.Count; i++)
        {
            //Empty slot
            if (content[i].item == null)
            {
                content[i] = content[i].ChangeItem(item, amount);
                return true;
            }
        }

        return false;
    }

    public bool CanAddItem(Item item, int amount) 
    {
        foreach (InventoryItem invitem in content)
        {
            if (invitem.item == null)
            {
                return true;
            }
            else if (invitem.amount + amount <= invitem.item.stacklimit && invitem.item.id == item.id)
            {
                return true;
            }
        }

        return false;
    }

    public InventoryItem GetItemAtSlot(int slot) 
    {
        if (slot >= 0 && slot < content.Count)
            return content[slot];
        else
        {
            Debug.LogError("Error grabbing item and out-of-bounds slot");
            return new InventoryItem().GetEmptyItem();
        }
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
    public InventoryItem ChangeItem(Item newItem, int newAmount) 
    {
        return new InventoryItem
        {
            item = newItem,
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

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate () {
            OnClick(param);
        });
    }
}

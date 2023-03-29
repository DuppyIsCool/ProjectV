using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;
public class Inventory : NetworkBehaviour
{
    public readonly SyncList<InventoryItem> content = new SyncList<InventoryItem>();
    [SerializeField] [SyncVar] private int size;
    [SyncVar] public InventoryItem equippedItem = new InventoryItem().GetEmptyItem();

    [Server]
    private void SetupInventory() 
    {
        //Initializing the Inventory would go here
        print("Setup inventory here!");
    }

    private void Start()
    {
        if (isServer) 
        {
            SetupInventory();
        }
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

        //Loop to see if we can fit the item into a current stack
        for(int i = 0; i < content.Count; i++)
        {
            //Check if the ids are the same and if the amount that would be added does not exceed the stack limit.
            if (content[i].item.id == item.id && content[i].amount + amount <= content[i].item.stacklimit) 
            {
                content[i] = content[i].ChangeQuantity(content[i].amount + amount);
                return true;
            }
        }

        //If not, see if we can fit into its own stack in the inventory
        if (content.Count + 1 <= size) 
        {
            InventoryItem newItem = new InventoryItem { item = item, amount = amount };
            content.Add(newItem);
            return true;
        }
        return false;
    }

    public bool CanAddItem(Item item, int amount) 
    {
        foreach (InventoryItem invitem in content)
        {
            if (invitem.amount + amount <= invitem.item.stacklimit && invitem.item.id == item.id)
            {
                return true;
            }
        }

        if (content.Count + 1 <= size)
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

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate () {
            OnClick(param);
        });
    }
}

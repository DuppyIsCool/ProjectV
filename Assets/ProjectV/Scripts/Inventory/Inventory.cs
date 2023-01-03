using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;
using System;
public class Inventory : NetworkBehaviour
{
    public readonly SyncList<InventoryItem> inventory = new SyncList<InventoryItem>();
    [SerializeField] [SyncVar] private int size;
    [SerializeField] [SyncVar] public InventoryItem equippedItem = new InventoryItem().GetEmptyItem();
    [SerializeField] private GameObject itemButtonPrefab;

    private GameObject inventoryUI,g;
    [Server]
    private void SetupInventory() 
    {
        //Initializing the Inventory would go here
        print("Setup inventory here!");
    }

    public override void OnStartClient() 
    {
        //Get the UI gameobject
        inventoryUI = GameObject.Find("InventoryUI");
        inventory.Callback += InventoryUIUpdates;    
        
        // Process initial SyncList payload
        for (int index = 0; index < inventory.Count; index++)
        {
            InventoryUIUpdates(SyncList<InventoryItem>.Operation.OP_ADD, index, new InventoryItem(), inventory[index]);
        }

    }

    public override void OnStopClient()
    {
        //Test code: may need to be changed if fix is found for synclist clearing on scene transfer
        //When stopping the client, clear the UI as the inventory is cleared
        for (int i = 0; i < inventoryUI.transform.childCount; i++)
        {
            Destroy(inventoryUI.transform.GetChild(i).gameObject);
        }

        //Unsubscribe from callback
        inventory.Callback -= InventoryUIUpdates;
        base.OnStopClient();
    }

    private void Start()
    {
        if (isServer) 
        {
            SetupInventory();
        }
    }

    void InventoryUIUpdates(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem) 
    {
        //Test code printing for inventory status
        if(isLocalPlayer)

            switch (op) 
            {
                case SyncList<InventoryItem>.Operation.OP_ADD:
                    //Create the Item UI in the Inventory
                    g = Instantiate(itemButtonPrefab,inventoryUI.transform);
                    g.transform.GetChild(0).GetComponent<TMP_Text>().text = newItem.item.displayName;
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = newItem.item.description;
                    g.transform.GetChild(2).GetComponent<Image>().sprite = newItem.item.sprite;
                    g.GetComponent<Button>().AddEventListener(index, EquipItemCmd);
                    break;

                case SyncList<InventoryItem>.Operation.OP_INSERT:

                    break;

                case SyncList<InventoryItem>.Operation.OP_SET:
                    //Edit the Item UI in the Inventory
                    g = inventoryUI.transform.GetChild(index).gameObject;
                    g.transform.GetChild(0).GetComponent<TMP_Text>().text = newItem.item.displayName;
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = newItem.item.description;
                    g.transform.GetChild(2).GetComponent<Image>().sprite = newItem.item.sprite;
                    break;

                case SyncList<InventoryItem>.Operation.OP_REMOVEAT:
                    //Index in the inventory syncs with the UI index, so delete the UI index
                    Destroy(inventoryUI.transform.GetChild(index).gameObject);
                    break;

                case SyncList<InventoryItem>.Operation.OP_CLEAR:
                    //Clear all ItemUI elements
                    for (int i = 0; i < inventoryUI.transform.childCount; i++) 
                    {
                        Destroy(inventoryUI.transform.GetChild(i).gameObject);
                    }
                    break;


            }
    }

    [Command]
    public void EquipItemCmd(int index) 
    {
        if (index <= size && index >= 0)
        {
            equippedItem = inventory[index];
            print("Player has equipped item: " + equippedItem.item.id);           
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

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate () {
            OnClick(param);
        });
    }
}

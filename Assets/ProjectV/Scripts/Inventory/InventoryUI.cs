using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
public class InventoryUI : NetworkBehaviour
{
    [SerializeField] private GameObject itemButtonPrefab;
    private GameObject inventoryUI, g;
    private Inventory currentInventory;
    public override void OnStartClient() 
    {
        //Get the UI gameobject
        inventoryUI = GameObject.Find("InventoryUI");
        currentInventory = gameObject.GetComponent<Inventory>();
        gameObject.GetComponent<Inventory>().content.Callback += InventoryUIUpdates;    
        
        // Process initial SyncList payload
        for (int index = 0; index < currentInventory.content.Count; index++)
        {
            InventoryUIUpdates(SyncList<InventoryItem>.Operation.OP_ADD, index, new InventoryItem(), currentInventory.content[index]);
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
        currentInventory.content.Callback -= InventoryUIUpdates;
        base.OnStopClient();
    }

    void InventoryUIUpdates(SyncList<InventoryItem>.Operation op, int index, InventoryItem oldItem, InventoryItem newItem)
    {
        //Test code printing for inventory status
        if (isLocalPlayer)

            switch (op)
            {
                case SyncList<InventoryItem>.Operation.OP_ADD:
                    //Create the Item UI in the Inventory
                    g = Instantiate(itemButtonPrefab, inventoryUI.transform);
                    g.transform.GetChild(0).GetComponent<TMP_Text>().text = newItem.item.displayName;
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = newItem.item.description;
                    g.transform.GetChild(2).GetComponent<Image>().sprite = newItem.item.sprite;
                    //Register Equip item to this button
                    g.GetComponent<Button>().AddEventListener(index, currentInventory.EquipItemCmd);
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
}

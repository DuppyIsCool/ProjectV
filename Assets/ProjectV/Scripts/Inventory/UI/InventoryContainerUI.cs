using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
public class InventoryContainerUI : MonoBehaviour
{
    private GameObject currentItem;
    public Inventory currentInventory;
    [SerializeField] private GameObject itemSlotPrefab;
    CanvasGroup panelCanvas;
    public void Setup(Inventory panelInventory) 
    {
        panelCanvas = transform.parent.gameObject.GetComponent<CanvasGroup>();
        currentInventory = panelInventory;
        for (int i = 0; i < currentInventory.size; i++)
        {
            currentItem = Instantiate(itemSlotPrefab, transform);
            currentItem.transform.Find("Item").GetComponent<Image>().sprite = null;
            currentItem.transform.Find("Item").Find("Amount").GetComponent<TMP_Text>().text = "";
        }
    }

    public void ToggleVisibility() 
    {
        if (panelCanvas.alpha == 1)
        {

            panelCanvas.alpha = 0;
            panelCanvas.interactable = false;
        }
        else 
        {
            panelCanvas.alpha = 1;
            panelCanvas.interactable = true;
        }
    }

    public void MoveItem(int mySlot, InventoryContainerUI otherInventory, int otherSlot) 
    {
        //If moving to self
        if (otherInventory = this)
        {
            Debug.Log("MYSELF");
        }
        Debug.Log("IS OTHER OWNED: " + otherInventory.currentInventory.isOwned);
        Debug.Log("AM I OWNED?: " + currentInventory.isOwned);
    }

    public void SetItem(InventoryItem oldItem, InventoryItem newItem, int slot) 
    {
        currentItem = transform.GetChild(slot).gameObject;
        if (newItem.item != null)
        {
            currentItem.transform.Find("Item").GetComponent<Image>().sprite = newItem.item.sprite;
            currentItem.transform.Find("Item").Find("Amount").GetComponent<TMP_Text>().text = newItem.amount.ToString();
            currentItem.transform.Find("Item").gameObject.SetActive(true);
        }
        else 
        {
            currentItem.transform.Find("Item").GetComponent<Image>().sprite = null;
            currentItem.transform.Find("Item").Find("Amount").GetComponent<TMP_Text>().text = null;
            currentItem.transform.Find("Item").gameObject.SetActive(false);
        }
    }
    
    public void Clear() 
    {
        //Clear all ItemUI elements
        for (int i = 0; i < transform.childCount; i++)
        {
            currentItem = transform.GetChild(i).gameObject;
            currentItem.transform.Find("Item").GetComponent<Image>().sprite = null;
            currentItem.transform.Find("Item").Find("Amount").GetComponent<TMP_Text>().text = null;
            currentItem.transform.Find("Item").gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedSlot = eventData.pointerDrag;
        DraggableUIItem dragItem = droppedSlot.GetComponent<DraggableUIItem>();

        InventoryContainerUI originPanel = dragItem.parentAfterDrag.parent.GetComponent<InventoryContainerUI>();
        
       // dragItem.parentAfterDrag = transform;

        //Swapping (Moving the full origin stack to an empty slot)
        if (dragItem.parentAfterDrag.childCount == 0)
        {
            //Move empty slot to origin
            transform.GetChild(0).parent = dragItem.parentAfterDrag;
            dragItem.parentAfterDrag = transform;
            originPanel.MoveItem(transform.GetSiblingIndex(), originPanel, dragItem.parentAfterDrag.GetSiblingIndex());
        }
        /*Stacking
        else if ()
        {

        }
        //Splitting
        else 
        {
        }*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Mirror;
public class InventorySlot : NetworkBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        GameObject droppedSlot = eventData.pointerDrag;
        DraggableUIItem dragItem = droppedSlot.GetComponent<DraggableUIItem>();

        InventoryContainerUI originPanel = dragItem.parentAfterDrag.parent.GetComponent<InventoryContainerUI>();
        
        dragItem.parentAfterDrag = transform;
    }
}

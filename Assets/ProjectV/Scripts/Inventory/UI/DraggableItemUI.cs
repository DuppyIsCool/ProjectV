using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItemUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private InventoryUI inventoryUI;
    private InventoryContainerUI containerUI;
    private Vector2 originalPosition;
    private int moveAmount;
    private bool isRightClick;
    int fromSlotIndex;
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        containerUI = GetComponentInParent<InventoryContainerUI>();
        inventoryUI = containerUI.inventoryUI;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        int slotIndex = transform.parent.GetSiblingIndex();
        fromSlotIndex = transform.parent.GetSiblingIndex();
        // Left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            moveAmount = inventoryUI.GetItemAtSlot(slotIndex, containerUI.containerType).amount;
            isRightClick = false;
        }
        // Right click
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            moveAmount = Mathf.CeilToInt(inventoryUI.GetItemAtSlot(slotIndex, containerUI.containerType).amount / 2f);
            isRightClick = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Check if the pointer is on a valid item slot
        GameObject targetObj = eventData.pointerEnter;
        if (targetObj != null && targetObj.transform.parent.GetComponent<InventoryContainerUI>() != null)
        {
            int fromSlot = fromSlotIndex;
            int toSlot = targetObj.transform.GetSiblingIndex();
            Debug.Log(fromSlot + " to " + toSlot);
            InventoryUI.ContainerType fromContainerType = containerUI.containerType;
            InventoryUI.ContainerType toContainerType = targetObj.transform.parent.GetComponent<InventoryContainerUI>().containerType;

            // Move the item
            inventoryUI.MoveItem(fromContainerType, toContainerType, fromSlot, toSlot, moveAmount);

            // If right-clicked, handle item splitting
            if (isRightClick)
            {
                // If the target slot is empty or contains a different item, move half of the stack back to the original slot
                InventoryItem targetItem = inventoryUI.GetItemAtSlot(toSlot, toContainerType);
                if (targetItem.item == null || targetItem.item.id != inventoryUI.GetItemAtSlot(fromSlot, fromContainerType).item.id)
                {
                    inventoryUI.MoveItem(toContainerType, fromContainerType, toSlot, fromSlot, moveAmount);
                }
            }
            fromSlotIndex = 0;
        }
    }
}

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

    private float doubleClickThreshold = 0.5f;
    private float lastClickTime;

    private Transform originalParent;
    private int originalSiblingIndex;

    public static GameObject itemBorder;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        containerUI = GetComponentInParent<InventoryContainerUI>();
        inventoryUI = containerUI.inventoryUI;
        itemBorder = inventoryUI.border;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        int slotIndex = transform.parent.GetSiblingIndex();
        fromSlotIndex = transform.parent.GetSiblingIndex();

        // Left click
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            float currentTime = Time.time;
            if (currentTime - lastClickTime <= doubleClickThreshold)
            {
                OnItemDoubleClicked(slotIndex);
                MoveBorderToSlot(this);
                lastClickTime = 0;
                return;
            }
            else
            {
                lastClickTime = currentTime;
            }

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

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        transform.SetParent(canvas.transform); // Move the item to the root of the canvas
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Revert the parent and sibling index back to their original values
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);

        // Check if the pointer is on a valid item slot
        GameObject targetObj = eventData.pointerEnter;
        if (targetObj != null && targetObj.transform.parent.GetComponent<InventoryContainerUI>() != null)
        {
            int fromSlot = fromSlotIndex;
            int toSlot = targetObj.transform.GetSiblingIndex();
            InventoryUI.ContainerType fromContainerType = containerUI.containerType;
            InventoryUI.ContainerType toContainerType = targetObj.transform.parent.GetComponent<InventoryContainerUI>().containerType;

            // Move the item
            inventoryUI.MoveItem(fromContainerType, toContainerType, fromSlot, toSlot, moveAmount);

            // If right-clicked, handle item splitting
            if (isRightClick)
            {
                // If the target slot is empty or contains a different item, move half of the stack back to the original slot
                InventoryItem targetItem = inventoryUI.GetItemAtSlot(toSlot, toContainerType);
                if (targetItem.item == null || targetItem.item.id != inventoryUI.GetItemAtSlot(fromSlot, fromContainerType).item?.id)
                {
                    inventoryUI.MoveItem(toContainerType, fromContainerType, toSlot, fromSlot, moveAmount);
                }
            }
            fromSlotIndex = 0;
        }
    }

    private void MoveBorderToSlot(DraggableItemUI item)
    {
        if (itemBorder != null)
        {
            if(itemBorder.GetComponent<CanvasGroup>().alpha == 0)
                itemBorder.GetComponent<CanvasGroup>().alpha = 1;

            itemBorder.transform.SetParent(item.transform, false);
            itemBorder.transform.SetSiblingIndex(0);
            itemBorder.SetActive(true);
        }
    }

    public void OnItemDoubleClicked(int slot)
    {
        inventoryUI.EquipItem(slot);
    }
}

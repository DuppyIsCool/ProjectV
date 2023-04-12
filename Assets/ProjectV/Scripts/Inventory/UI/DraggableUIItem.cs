using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class DraggableUIItem : MonoBehaviour, IPointerClickHandler,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(parentAfterDrag);
        GetComponent<Image>().raycastTarget = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //If picking up
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root.Find("PointerSlot"));
        transform.SetAsLastSibling();
        GetComponent<Image>().raycastTarget = false;
    }
}

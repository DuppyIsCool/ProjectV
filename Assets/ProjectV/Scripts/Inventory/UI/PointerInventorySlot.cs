using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerInventorySlot : MonoBehaviour
{
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items")]
public class Item : ScriptableObject
{
    public string id;
    public int value;
    public bool isConsumable;
    public bool isUsable;
    public int useCooldownTime;
    public int stacklimit;
    public string displayName;
    public string description;
    public Sprite sprite;
}

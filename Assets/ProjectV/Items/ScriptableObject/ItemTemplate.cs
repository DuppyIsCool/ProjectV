using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemTemplate : ScriptableObject
{
    public string id;
    public int value;
    public bool isConsumable;
    public int stacklimit;
    public string displayName;
    public string description;
    public Sprite sprite;
    public abstract void useItem();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "Items/TestItems", order = 1)]
public class Item : ScriptableObject
{
    public string id;
    public int value;
    public int stacklimit;
    public Sprite sprite;
}

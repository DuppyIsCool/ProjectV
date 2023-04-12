using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SwordItem", menuName = "Items/Weapon/Sword")]
public class SwordItem : Item
{
    public GameObject clientPrefab;
    public GameObject serverPrefab;
    public float speed;
    public int damage;
}

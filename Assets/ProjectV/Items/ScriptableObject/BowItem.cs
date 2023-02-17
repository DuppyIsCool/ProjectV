using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BowItem", menuName = "Items/Weapon")]
public class BowItem : Item
{
    public GameObject clientPrefab;
    public GameObject serverPrefab;
    public float speed;
    public float cooldown;
    public int damage;
}

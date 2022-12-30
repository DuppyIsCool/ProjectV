using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GenericItem : ItemTemplate
{
    [Command]
    public override void useItem()
    {
        Debug.Log("hi?");
    }
}

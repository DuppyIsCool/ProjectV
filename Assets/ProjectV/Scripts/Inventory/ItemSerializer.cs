using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public static class ItemSerializer
{
    public static void WriteInventoryItem(this NetworkWriter writer, InventoryItem inventoryItem)
    {
        writer.WriteString(inventoryItem.item.id);
        writer.WriteInt(inventoryItem.amount);
    }

    public static InventoryItem ReadInventoryItem(this NetworkReader reader)
    {
        string itemid = reader.ReadString();
        int amount = reader.ReadInt();
        return new InventoryItem
        {
            item = ItemDatabase.GetItem(itemid),
            amount = amount
        };
    }
}

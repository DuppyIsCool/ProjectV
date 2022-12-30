using Mirror;
public static class ItemSerializer
{
    public static void WriteInventoryItem(this NetworkWriter writer, InventoryItem inventoryItem)
    {
        if (inventoryItem.item != null)
        {
            writer.WriteString(inventoryItem.item.id);
            writer.WriteInt(inventoryItem.amount);
        }
        else 
        {
            writer.WriteString("NULL");
            writer.WriteInt(-1);
        }
    }

    public static InventoryItem ReadInventoryItem(this NetworkReader reader)
    {
        string itemid = reader.ReadString();
        int amount = reader.ReadInt();

        if (itemid.Equals("NULL"))
        {
            return new InventoryItem().GetEmptyItem();
        }
        else
        {
            return new InventoryItem
            {
                item = ItemDatabase.GetItem(itemid),

                amount = amount
            };
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase database { get; private set; }
    private static Dictionary<string, Item> ItemList;

    public void Awake()
    {
        if (database == null)
        {
            database = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
        ItemList = new Dictionary<string, Item>();
        List<Item> tempItems = Resources.LoadAll<Item>("TestItems/").ToList();

        foreach (Item item in tempItems)
        {
            Debug.Log("Loading item with id:" + item.id);
            ItemList.Add(item.id, item);
        }
    }

    public static Item GetItem(string itemID)
    {
        if (ItemList.ContainsKey(itemID))
            return ItemList[itemID];
        else
        {
            Debug.Log("Could not find item by the ID of: " + itemID);
            return null;
        }
    }
}

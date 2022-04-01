using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteItemOnEvent : MonoBehaviour
{
    [Tooltip("Event which should set this object to become inactive")]
    public string deleteEvent = "";
    
    [Tooltip("Item to delete.")]
    public string itemName = "";
    
    // Start is called before the first frame update
    void Start()
    {
        if (deleteEvent != string.Empty)
        {
            EventManager.Sub(deleteEvent, () => { DeleteItem(); });
        }
    }

    void DeleteItem()
    {
        Inventory inventory = Inventory.GetInstance();
        if ( !inventory.DeleteItem(itemName) )
        {
            Debug.Log("Couldn't delete item with name: " + itemName);
        }
    }
}

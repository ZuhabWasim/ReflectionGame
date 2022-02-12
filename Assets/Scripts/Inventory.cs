using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemPickupResult
{
    SUCCESS = 0,
    FAIL_INVENTORY_FULL,
    FAIL_ERROR
}

// Singleton Class, should not be instantiated!! Use Inventory.GetInstance instead
public class Inventory
{
    public int inventorySize = 8;
    private static PickupItem[] empty = {null, null, null, null, null, null, null, null};
    private List<PickupItem> m_items = new List<PickupItem>(empty);
    private static Inventory m_instance = new Inventory();
    private int cursor = 2;
    private InventoryDisplay inventoryDisplay = GameObject.Find("UI_Canvas").GetComponent<InventoryDisplay>();
    public static ref Inventory GetInstance()
    {
        return ref m_instance;
    }

    public ItemPickupResult PickupItem( ref PickupItem item )
    {
        if ( item == null )
        {
            return ItemPickupResult.FAIL_ERROR;
        }

        for (int i = 0; i < inventorySize; i++){
            if (m_items[i] == null){
                m_items[i] = item;
                item.OnPickup();
                inventoryDisplay.addItem(item, i);
                return ItemPickupResult.SUCCESS;
            }
        }
        return ItemPickupResult.FAIL_INVENTORY_FULL;
    }

    public void MoveCursor( int newPos )
    {
        if ( newPos < 0) {
            cursor = inventorySize + newPos;
        } else if ( newPos < inventorySize )
        {
            cursor = newPos;
        } else {
            cursor = newPos - inventorySize;
        }
    }
    public void openInventory()
    {
        inventoryDisplay.openInventory();
        if (m_items[cursor] != null){
            PickupItem item = m_items[cursor];
            inventoryDisplay.showItemName(item.itemName);
        } else {
            inventoryDisplay.hideItemName();
        }
    }    
    public void closeInventory()
    {
        inventoryDisplay.closeInventory();
    }
    public void spinInventory(int spin)
    {
        MoveCursor( cursor + spin );
        inventoryDisplay.spinInventory(spin);
    }
    public int GetCursor()
    {
        return cursor;
    }

    public bool DropItem( Vector3 dropPosition )
    {
        if ( cursor < m_items.Count )
        {
            PickupItem item = m_items[ cursor ];
            item.OnDrop( dropPosition );
            m_items[cursor] = null;
            inventoryDisplay.dropItem(cursor);
            return true;
        }
        Debug.Log( "Tried to drop item from a slot that doesn't have a valid item" );
        return false;
    }
}

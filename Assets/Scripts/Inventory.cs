using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton Class, should not be instantiated!! Use Inventory.GetInstance instead
public class Inventory
{
    public int inventorySize = 9;
    private static PickupItem[] empty = {null, null, null, null, null, null, null, null, null};
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
        if (m_items[cursor] == null) {
            m_items[cursor] = item;
            item.OnPickup();
            inventoryDisplay.addItem(item, cursor);
            PlayerController.PlaySound( "object_obtained" );
            return ItemPickupResult.SUCCESS;
        }
        for (int i = 1; i < 5; i ++) {
            int pos1 = cursor + i;
            if (pos1 >= inventorySize) {
                pos1 -= inventorySize;
            }
            int pos2 = cursor - i;
            if (pos2 < 0) {
                pos2 += inventorySize;
            }
            if (m_items[pos2] == null){
                m_items[pos2] = item;
                item.OnPickup();
                inventoryDisplay.addItem(item, pos2);
                PlayerController.PlaySound( "object_obtained" );
                return ItemPickupResult.SUCCESS;
            }
            if (m_items[pos1] == null){
                m_items[pos1] = item;
                item.OnPickup();
                inventoryDisplay.addItem(item, pos1);
                PlayerController.PlaySound( "object_obtained" );
                return ItemPickupResult.SUCCESS;
            }
        }
        PlayerController.PlaySound( "non_interactable" );
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
    public void CloseInventory()
    {
        inventoryDisplay.CloseInventory();
    }
    public void SpinInventory(int spin)
    {
        MoveCursor( cursor + spin );
        inventoryDisplay.SpinInventory(spin);
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

    public string GetSelectedItem()
    {
        if (m_items[cursor] != null)
        {
            return m_items[cursor].itemName;
        }

        return "";
    }
    public bool HasItem(string itemName)
    {
        foreach (PickupItem item in m_items)
        {
            if (item.itemName == itemName)
            {
                return true;
            }
        }

        return false;
    }
    
    public bool DeleteItem(string itemName)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (m_items[i] != null && m_items[i].itemName == itemName)
            {
                // Delete item after using it
                m_items[cursor] = null;
                inventoryDisplay.dropItem(cursor);
                return true;
            } 
        }
        // Return false if the deletion of this item was unsuccessful
        return false;
    }
}

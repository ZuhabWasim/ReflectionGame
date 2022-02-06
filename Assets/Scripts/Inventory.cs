using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton Class, should not be instantiated!! Use Inventory.GetInstance instead
public class Inventory
{
    public int inventorySize = 5;
    private List<PickupItem> m_items = new List<PickupItem>();
    private static Inventory m_instance = new Inventory();
    private int cursor = 0;
    
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

        if ( m_items.Count == inventorySize )
        {
            return ItemPickupResult.FAIL_INVENTORY_FULL;
        }

        m_items.Add( item );
        item.OnPickup();
        return ItemPickupResult.SUCCESS;
    }

    public void MoveCursor( int newPos )
    {
        if ( newPos < m_items.Count )
        {
            cursor = newPos;
        }
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
            m_items.RemoveAt( cursor );
            if ( cursor > 0 )
            {
                cursor--;
            }

            return true;
        }

        Debug.Log( "Tried to drop item from a slot that doesn't have a valid item" );
        return false;
    }
}

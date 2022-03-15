using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DadPuzzle1Canvas : InteractableAbstract
{

    public string OnPaintEvent = string.Empty;
    private bool m_canUsePaintBrush = false;
    private const string PAINT_BRUSH = "Paint Brush";
	protected override void OnStart()
	{
		desiredItem = PAINT_BRUSH;
        EventManager.Sub( Globals.Events.PAINT_BRUSH_WET, () => {
            m_canUsePaintBrush = true;
        });
	}

	protected override void OnUseItem()
	{
		if ( !m_canUsePaintBrush ) return;
        Inventory.GetInstance().DeleteItem( PAINT_BRUSH );
        EventManager.Fire( OnPaintEvent );
	}
}

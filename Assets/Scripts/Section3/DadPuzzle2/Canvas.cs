using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum CanvasState
{
	CLEAN = 0,
	REFLECTIVE,
	PORTAL
}

public class Canvas : InteractableAbstract
{
	public CanvasState state = CanvasState.CLEAN;
	public Light outgoingLight = null;
	public ColorFilter outgoingFilter = null;
	public GameObject[] activateOnPortal;

	public MirrorConnector mirrorConnector;
	
	private GameObject m_mirrorPlane;
	private float m_lightRange;
	
	protected override void OnStart()
	{
		if ( outgoingLight ) m_lightRange = outgoingLight.range;
		desiredItem = Globals.Misc.WET_PAINT_BRUSH;

		m_mirrorPlane = GetComponentInChildren<MirrorPlane>( true ).gameObject; // By default, many canvas mirrors are inactive
		// TODO: Make it so StudioLight.cs fires an event that turns on the outgoing light of all canvases.
	}

	protected override void OnUserInteract()
	{
		if (state == CanvasState.CLEAN)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section3.WHITE_CANVAS, Globals.Tags.DIALOGUE_SOURCE);
		} else if (state == CanvasState.REFLECTIVE)
		{
			AudioPlayer.Play(Globals.VoiceLines.Section3.REFLECTIVE_CANVAS, Globals.Tags.DIALOGUE_SOURCE);
		}
	}
	
	protected override void OnUseItem()
	{
		PaintBrush brush = Inventory.GetInstance().GetSelectedPickupItem().GetComponent<PaintBrush>();
		switch ( brush.paint )
		{
			case PaintType.WHITE:
				SetState( CanvasState.CLEAN );
				break;
			case PaintType.REFLECTIVE:
				if ( state == CanvasState.PORTAL ) return;
				SetState( CanvasState.REFLECTIVE );
				break;
			case PaintType.PORTAL:
				SetState( CanvasState.PORTAL );
				break;
			case PaintType.NONE:
			default:
				return;
		}
	}

	void OnPortal()
	{
		ToPortal();
		foreach ( GameObject obj in activateOnPortal )
		{
			obj.SetActive( true );
		}
	}

	public void DisableLight()
	{
		if ( !outgoingLight ) return;
		outgoingLight.enabled = false;
	}

	public void EnableLight()
	{
		if ( !outgoingLight ) return;
		outgoingLight.enabled = true;
		outgoingLight.range = m_lightRange;
	}

	public void SetState( CanvasState newState, bool notifyChange = true )
	{
		UpdateMirror( newState );
		this.state = newState;
		if ( state == CanvasState.PORTAL ) OnPortal();
		if ( notifyChange )
		{
			EventManager.Fire( Globals.Events.CANVAS_STATE_CHANGE, this.gameObject );
		}
	}

	private void UpdateMirror( CanvasState newState )
	{
		switch ( newState )
		{
			case CanvasState.CLEAN:
				ToClean();
				break;
			case CanvasState.REFLECTIVE:
				ToReflective();
				break;
			case CanvasState.PORTAL:
				ToPortal();
				break;
			default:
				break;
		}
	}

	public void SetFilter( ColorFilter filter )
	{
		this.outgoingFilter = filter;
		EventManager.Fire( Globals.Events.CANVAS_STATE_CHANGE );
	}

	private void ToPortal()
	{
		m_mirrorPlane.SetActive( true );
		mirrorConnector.Activate();
	}

	private void ToReflective()
	{
		m_mirrorPlane.SetActive( true );
		mirrorConnector.Deactivate();
	}

	private void ToClean()
	{
		
		m_mirrorPlane.SetActive( false );
	}
}

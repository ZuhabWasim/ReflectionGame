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

	public AudioClip onPaintVoiceLine;

	protected override void OnStart()
	{
		if ( outgoingLight ) m_lightRange = outgoingLight.range;
		//desiredItem = Globals.Misc.WET_PAINT_BRUSH;

		m_mirrorPlane = GetComponentInChildren<MirrorPlane>( includeInactive: true ).gameObject;
		// By default, many canvas mirrors are inactive
	}

	protected override void OnUserInteract()
	{
		if ( state == CanvasState.CLEAN )
		{
			AudioPlayer.Play( Globals.VoiceLines.Section3.WHITE_CANVAS, Globals.Tags.DIALOGUE_SOURCE );
		}
		else if ( state == CanvasState.REFLECTIVE )
		{
			AudioPlayer.Play( Globals.VoiceLines.Section3.REFLECTIVE_CANVAS, Globals.Tags.DIALOGUE_SOURCE );
		}

		if (voiceLine != null)
		{
			AudioPlayer.Play( voiceLine, Globals.Tags.DIALOGUE_SOURCE );
		}
	}

	protected override void OnUseItem()
	{
		// If this was OnUseItemUnfiltered, the default OnUseItem() method in PlayerController overrides this dialogue
		// But since this override OnUseItem(), I need to write all the cases of what to do here.
		// I need desiredItem = "" so I can do checking on multiple brushes.
		PickupItem item = Inventory.GetInstance().GetSelectedPickupItem();
		if (item == null)
		{
			AudioPlayer.Play( Globals.VoiceLines.General.NOT_HOLDING_ANYTHING, Globals.Tags.DIALOGUE_SOURCE );
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
			return;
		}
		
		PaintBrush brush = item.GetComponent<PaintBrush>();
		if (brush == null)
		{
			AudioPlayer.Play( Globals.VoiceLines.General.CANT_USE_ITEM, Globals.Tags.DIALOGUE_SOURCE );
			AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
			return;
		}

		switch (brush.paint)
		{
			case PaintType.WHITE:
				if (state == CanvasState.CLEAN)
				{
					AudioPlayer.Play(Globals.VoiceLines.Section3.ALREADY_BLANK, Globals.Tags.DIALOGUE_SOURCE);
					AudioPlayer.Play(Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE);
					return;
				}

				SetState(CanvasState.CLEAN);
				break;
			case PaintType.REFLECTIVE:
				if (state == CanvasState.REFLECTIVE || state == CanvasState.PORTAL)
				{
					AudioPlayer.Play(Globals.VoiceLines.Section3.ALREADY_REFLECTIVE, Globals.Tags.DIALOGUE_SOURCE);
					AudioPlayer.Play(Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE);
					return;
				}

				SetState(CanvasState.REFLECTIVE);
				break;
			case PaintType.PORTAL:
				SetState(CanvasState.PORTAL);
				AudioPlayer.Play(onPaintVoiceLine, Globals.Tags.DIALOGUE_SOURCE);
				// Set the state of the brush to Reflective (one-time use)
				brush.paint = PaintType.REFLECTIVE;
				break;
			case PaintType.NONE:
				AudioPlayer.Play( Globals.VoiceLines.General.CANT_USE_ITEM, Globals.Tags.DIALOGUE_SOURCE );
				AudioPlayer.Play( Globals.AudioFiles.General.NON_INTERACTABLE, Globals.Tags.MAIN_SOURCE );
				break;
			default:
				return;
		}
	}

	public void Paint()
    {
	    OnUseItem();
	}

	void OnPortal()
	{
		foreach ( GameObject obj in activateOnPortal )
		{
			obj.SetActive( true );
		}

		ToPortal();
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
		// Play painting sound
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
		GetComponentInChildren<MirrorPlane>().SetNormalTexture();
		mirrorConnector.Activate();
	}

	private void ToReflective()
	{
		m_mirrorPlane.SetActive( true );
		GetComponentInChildren<MirrorPlane>().SetOpaqueTexture();
		mirrorConnector.Deactivate();
	}

	private void ToClean()
	{

		m_mirrorPlane.SetActive( false );
	}
}

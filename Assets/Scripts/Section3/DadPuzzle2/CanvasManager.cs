#undef DEBUGGING_LIGHT_TRACE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

struct CanvasPair
{
	public readonly Canvas present;
	public readonly Canvas past;

	public CanvasPair( Canvas pres, Canvas past )
	{
		this.present = pres;
		this.past = past;
	}
}

public class CanvasManager : MonoBehaviour
{
	[Tooltip( "Canvases in the present world, should be the same order as their counterparts in the other time period" )]
	public Canvas[] presentCanvases;
	[Tooltip( "Canvases in the past world, should be the same order as their counterparts in the other time period" )]
	public Canvas[] pastCanvases;
	public ColorFilter[] filters;
	public Light initialLightSource;
	private List<CanvasPair> m_canvasPairs;
	void Start()
	{
		Assert.IsTrue( presentCanvases.Length == pastCanvases.Length );
		m_canvasPairs = new List<CanvasPair>();
		for ( uint i = 0; i < presentCanvases.Length; i++ )
		{
			m_canvasPairs.Add( new CanvasPair( presentCanvases[ i ], pastCanvases[ i ] ) );
		}

		initialLightSource.enabled = true;
		GlobalState.AddVar<Color>( Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR, Color.black );
#if DEBUGGING_LIGHT_TRACE
		StartCoroutine( TestLightTrace() );
#else
		ComputeLightPath();
		EventManager.Sub( Globals.Events.CANVAS_STATE_CHANGE, HandleCanvasStateChange );
#endif
	}

	IEnumerator TestLightTrace()
	{
		while ( true )
		{
			ComputeLightPath();
			yield return new WaitForSecondsRealtime( 1.0f );
		}
	}

	void HandleCanvasStateChange( GameObject canvas )
	{
		// retrace only if this canvas is one of the canvases we're managing
		foreach ( CanvasPair pair in m_canvasPairs )
		{
			if ( pair.present.gameObject == canvas || pair.past.gameObject == canvas )
			{
				PrepareCanvasPairsForLightTrace();
				ComputeLightPath();
			}
		}
	}

	void PrepareCanvasPairsForLightTrace()
	{
		foreach ( CanvasPair pair in m_canvasPairs )
		{
			if ( pair.past.state == CanvasState.REFLECTIVE && ( pair.past.state == pair.present.state ) )
			{
				pair.past.SetState( CanvasState.PORTAL, notifyChange: false );
				pair.present.SetState( CanvasState.PORTAL, notifyChange: false );
			}
			else if ( ( pair.past.state == CanvasState.PORTAL || pair.present.state == CanvasState.PORTAL ) && pair.past.state != pair.present.state )
			{
				if ( pair.past.state == CanvasState.PORTAL ) pair.past.SetState( CanvasState.REFLECTIVE, notifyChange: false );
				else pair.present.SetState( CanvasState.REFLECTIVE, notifyChange: false );
			}
		}
	}

	void DisableAllLights()
	{
		foreach ( CanvasPair pair in m_canvasPairs )
		{
			pair.past.DisableLight();
			pair.present.DisableLight();
		}

		foreach ( ColorFilter filter in filters )
		{
			filter.DisableLight();
		}
	}

	void DisableLightFromIndex( uint startIndex, bool isPresent )
	{
		for ( int i = ( (int)startIndex ); i < m_canvasPairs.Count; i++ )
		{
			Canvas canvas = isPresent ? m_canvasPairs[ i ].present : m_canvasPairs[ i ].past;
			canvas.DisableLight();
		}
	}

	void ComputeLightPath()
	{
		DisableAllLights(); // reset light states since trace will figure out correct states anyways
		if ( !initialLightSource.enabled )
		{
			return;
		}

		TracePath( 0, false, initialLightSource.color );
#if DEBUGGING_LIGHT_TRACE
		PrintCanvasDebugInfo();
#endif
		Debug.Log("Global Colour: " + GlobalState.GetVar<Color>(Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR));
	}

	void TracePath( uint startIndex, bool isPresent, Color incomingLightColor )
	{
		// last canvas, light stops here
		if ( startIndex > m_canvasPairs.Count - 1 )
		{
			GlobalState.SetVar<Color>( Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR, incomingLightColor );
			return;
		}

		CanvasPair pair = m_canvasPairs[ ( (int)startIndex ) ];
		Canvas currCanvas = isPresent ? pair.present : pair.past;
		Canvas otherCanvas = isPresent ? pair.past : pair.present;

		if ( currCanvas.state == CanvasState.CLEAN )
		{
			GlobalState.SetVar<Color>( Globals.Vars.DAD_PUZZLE_2_FINAL_LIGHT_COLOR, Color.black );
			return;
		}
		else if ( currCanvas.state == CanvasState.PORTAL )
		{
			// portal => switch worlds
			otherCanvas.EnableLight();
			Color outgoingColor = incomingLightColor;
			if ( otherCanvas.outgoingFilter )
			{
				outgoingColor = otherCanvas.outgoingFilter.FilterColor( incomingLightColor );
				// need to add delta so light stops at surface
				otherCanvas.outgoingLight.range = ( otherCanvas.outgoingFilter.transform.position - otherCanvas.outgoingLight.transform.position ).magnitude - 0.5f;
			}
			otherCanvas.outgoingLight.color = incomingLightColor;
			TracePath( startIndex + 1, !isPresent, outgoingColor );
		}
		else
		{
			// reflective => continue trace in curr world
			currCanvas.EnableLight();
			Color outgoingColor = incomingLightColor;
			if ( currCanvas.outgoingFilter )
			{
				outgoingColor = currCanvas.outgoingFilter.FilterColor( incomingLightColor );
				// need to add delta so light stops at surface
				currCanvas.outgoingLight.range = ( currCanvas.outgoingFilter.transform.position - currCanvas.outgoingLight.transform.position ).magnitude - 0.5f;
			}

			currCanvas.outgoingLight.color = incomingLightColor;
			TracePath( startIndex + 1, isPresent, outgoingColor );
		}
	}

#if DEBUGGING_LIGHT_TRACE
	void PrintCanvasDebugInfo()
	{
		uint i = 0;
		foreach ( CanvasPair pair in m_canvasPairs )
		{
			Debug.LogFormat( pair.present.name + "Present{0}: State ({1}) | LightEnabled ({2}) | LightColor({3})", i, pair.present.state, pair.present.outgoingLight.enabled, pair.present.outgoingLight.color );
			Debug.LogFormat( pair.past.name + "Past{0}: State ({1}) | LightEnabled ({2}) | LightColor({3})", i, pair.past.state, pair.past.outgoingLight.enabled, pair.past.outgoingLight.color );
		}
	}
#endif
}

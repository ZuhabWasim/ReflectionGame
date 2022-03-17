using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ColorFilter : MonoBehaviour
{
	public enum ColorFilterMode
	{
		SUBTRACTIVE = 0,
		ADDITIVE,
		[Tooltip( "Same as subtractive, but removes 1 - color (i.e., keeps the given color, removes rest)" )]
		INVERT_SUBTRACTIVE
	};

	public Light outgoingLight;
	public Color color;
	[Range( 0.0f, 1.0f )]
	public float factor = 0.9f;
	public ColorFilterMode filterMode = ColorFilterMode.SUBTRACTIVE;

	public Color FilterColor( Color input )
	{
		EnableLight();
		switch ( filterMode )
		{
			case ColorFilterMode.SUBTRACTIVE:
				outgoingLight.color = FSubtractive( input );
				break;
			case ColorFilterMode.ADDITIVE:
				outgoingLight.color = FAdditive( input );
				break;
			case ColorFilterMode.INVERT_SUBTRACTIVE:
				outgoingLight.color = FInvertedSubtractive( input );
				break;
		}

		return outgoingLight.color;
	}

	private Color FSubtractive( Color input )
	{
		return input - ( factor * color );
	}

	private Color FInvertedSubtractive( Color input )
	{
		input = new Color( 1, 1, 1, 1 ) - input;
		return Abs( input - ( factor * color ) );
	}

	private Color FAdditive( Color input )
	{
		return input + ( factor * color );
	}

	private Color Abs( Color c )
	{
		c.r = Mathf.Abs( c.r );
		c.g = Mathf.Abs( c.g );
		c.b = Mathf.Abs( c.b );
		c.a = Mathf.Abs( c.a );
		return c;
	}

	public void DisableLight()
	{
		outgoingLight.enabled = false;
	}

	public void EnableLight()
	{
		outgoingLight.enabled = true;
	}
}

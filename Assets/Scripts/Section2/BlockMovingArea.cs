using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MoveDirect
{
	UP,
	RIGHT,
	DOWN,
	LEFT
}
public class BlockMovingArea : MonoBehaviour
{

	private float tileWidth;
	private float tileHeight;
	private float tilePadding;
	private int numTilesX;
	private int numTilesY;

	private enum TileStatus
	{
		BOX,
		SPACE,
		BLOCKED
	}

	private List<List<TileStatus>> tileGrid;
	private List<List<Transform>> spaceGrid;

	public GameObject mirrorConnector;
	private MirrorConnector[] mcList;

	// Start is called before the first frame update
	void Start()
	{
		tileGrid = new List<List<TileStatus>>();
		spaceGrid = new List<List<Transform>>();

		InitGrid();
		if (mirrorConnector != null) {
			mcList = mirrorConnector.GetComponents<MirrorConnector>();
		}
		//PrintBoxGrids();
	}

	private bool init = false;
    void Update()
    {
        if (!init)
        {
			RefreshMCList();
			init = true;
		}
    }

    public void MoveBox( int x, int y, MoveDirect dir )
	{
		int targetX = x;
		int targetY = y;
		switch ( dir )
		{
			case MoveDirect.UP:
				targetY -= 1;
				break;

			case MoveDirect.RIGHT:
				targetX += 1;
				break;

			case MoveDirect.DOWN:
				targetY += 1;
				break;

			default:
				targetX -= 1;
				break;
		}

		tileGrid[ y ][ x ] = TileStatus.SPACE;
		tileGrid[ targetY ][ targetX ] = TileStatus.BOX;

		Transform box = spaceGrid[ y ][ x ].transform.GetChild( 0 );
		box.SetParent( spaceGrid[ targetY ][ targetX ], false );
	}

	public bool BoxAtPosition( int x, int y )
	{
		return ( x >= 0 && x < numTilesX
			&& y >= 0 && y < numTilesY
			&& tileGrid[ y ][ x ] == TileStatus.BOX );
	}

	public bool CanMoveBox( int x, int y, MoveDirect dir )
	{
		switch ( dir )
		{
			case MoveDirect.UP:
				return ( y >= 1 && tileGrid[ y - 1 ][ x ] == TileStatus.SPACE );

			case MoveDirect.RIGHT:
				return ( x < numTilesX - 1 && tileGrid[ y ][ x + 1 ] == TileStatus.SPACE );

			case MoveDirect.DOWN:
				return ( y < numTilesY - 1 && tileGrid[ y + 1 ][ x ] == TileStatus.SPACE );

			case MoveDirect.LEFT:
				return ( x >= 1 && tileGrid[ y ][ x - 1 ] == TileStatus.SPACE );

			default:
				return ( x >= 1 && tileGrid[ y ][ x - 1 ] == TileStatus.SPACE );
		}
	}

	public (int, int) GetBoxIndices( Transform box )
	{
		for ( int i = 0; i < numTilesY; i++ )
		{
			for ( int j = 0; j < numTilesX; j++ )
			{
				if ( spaceGrid[ i ][ j ].childCount > 0 && spaceGrid[ i ][ j ].GetChild( 0 ) == box )
				{
					return (j, i);
				}
			}
		}
		return (-1, -1);
	}

	void InitGrid()
	{
		int i = 0;
		int j = 0;
		float padd = 0;
		bool paramSetA = false;
		bool paramSetB = false;
		foreach ( Transform currRow in transform )
		{
			if ( currRow.CompareTag( "MomRow" ) )
			{
				j = 0;
				List<TileStatus> tileRow = new List<TileStatus>();
				List<Transform> spaceRow = new List<Transform>();
				foreach ( Transform child1 in currRow.transform )
				{
					if ( child1.CompareTag( "MomSpace" ) )
					{
						if ( paramSetA && !paramSetB )
						{
							tilePadding = Mathf.Abs( child1.position.x - padd ) - tileWidth;
							paramSetB = true;
						}
						if ( !paramSetA )
						{
							tileWidth = child1.GetComponent<Renderer>().bounds.size.x;
							tileHeight = child1.GetComponent<Renderer>().bounds.size.z;
							padd = child1.position.x;
							paramSetA = true;
						}
						spaceRow.Add( child1 );
						if ( child1.childCount > 0 )
						{
							Transform child2 = child1.GetChild( 0 );
							if ( child2.GetComponent<MoveableBlock>() != null )
							{
								//child2.GetComponent<MoveableBlock>().SetBoxArea(this);
								tileRow.Add( TileStatus.BOX );
							}
							else
							{
								tileRow.Add( TileStatus.BLOCKED );
							}
						}
						else
						{
							tileRow.Add( TileStatus.SPACE );
						}
						j++;
					}
				}
				//assumes rows are sorted top->bottom
				//assumes spaces are sorted left->right
				tileGrid.Add( tileRow );
				spaceGrid.Add( spaceRow );
				i++;
			}
		}
		numTilesX = j;
		numTilesY = i;
	}

	public void RefreshMCList()
    {
		if (mirrorConnector != null)
		{
			for (int i = 0; i < mcList.Length; i++)
			{
				mcList[i].CheckForDeactivateZone();
			}
		}
	}

	private void PrintBoxGrids()
	{
		Debug.Log( "x: " + numTilesX + ", y: " + numTilesY );

		for ( int i = 0; i < numTilesY; i++ )
		{
			for ( int j = 0; j < numTilesX; j++ )
			{
				Debug.Log( "Row " + i + ", Col " + j
					+ ":\t" + tileGrid[ i ][ j ] + ", " + spaceGrid[ i ][ j ] );
			}
		}
	}

}

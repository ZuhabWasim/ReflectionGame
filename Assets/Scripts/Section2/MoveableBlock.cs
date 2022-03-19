using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : InteractableAbstract
{
	public BlockMovingArea bma;

	public MoveableBlock correspondingBlock;

	public void SetBoxArea( BlockMovingArea area )
	{
		bma = area;
	}

	protected override void OnUserInteract()
	{
		(int x, int y) pos;
		pos = bma.GetBoxIndices( transform );
		bool achieved = false;

		if ( bma.CanMoveBox( pos.x, pos.y, MoveDirect.UP ) )
		{
			achieved = TryMoveBox( MoveDirect.UP );
		}
		if ( !achieved && bma.CanMoveBox( pos.x, pos.y, MoveDirect.RIGHT ) )
		{
			achieved = TryMoveBox( MoveDirect.RIGHT );
		}
		if ( !achieved && bma.CanMoveBox( pos.x, pos.y, MoveDirect.DOWN ) )
		{
			achieved = TryMoveBox( MoveDirect.DOWN );
		}
		if ( !achieved && bma.CanMoveBox( pos.x, pos.y, MoveDirect.LEFT ) )
		{
			achieved = TryMoveBox( MoveDirect.LEFT );
		}
		if ( achieved && bma.transform.parent.name == Globals.Misc.SCISSOR_PUZZLE)
		{
			if (bma.BoxAtPosition(0, 0)) // If Mirror B position is blocked.
			{
				EventManager.Fire(Globals.Events.BLOCKING_MIRROR_B);
			}
			else
			{
				EventManager.Fire(Globals.Events.UNBLOCKING_MIRROR_B);
			}
		}
	}

	bool TryMoveBox( MoveDirect dir )
	{
		(int x, int y) pos;
		pos = bma.GetBoxIndices( transform );
		if ( bma.BoxAtPosition( pos.x, pos.y ) && bma.CanMoveBox( pos.x, pos.y, dir ) )
		{
			bma.MoveBox( pos.x, pos.y, dir );
			if (correspondingBlock != null)
            {
				correspondingBlock.TryMoveBox(dir);
				bma.RefreshMCList();
			}
			AudioPlayer.Play( Globals.AudioFiles.Section2.CARDBOARD_SLIDE, Globals.Tags.MAIN_SOURCE );
			return true;
		}
		return false;
	}

}

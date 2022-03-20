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
		MoveDirect dir;

		Vector3 f = GameObject.FindObjectOfType<PlayerController>().transform.forward;

		if (Mathf.Abs(Vector3.Dot(bma.leftVec, f)) > Mathf.Abs(Vector3.Dot(bma.topVec, f)))
        {
			//left-right
			if (Vector3.Dot(bma.leftVec, f) > 0)
            {
				dir = MoveDirect.LEFT;
            } else
            {
				dir = MoveDirect.RIGHT;
			}
        } else
        {
			//up-down
			if (Vector3.Dot(bma.topVec, f) > 0)
			{
				dir = MoveDirect.UP;
			}
			else
			{
				dir = MoveDirect.DOWN;
			}
		}

		TryMoveBoxPrecursor( pos.x, pos.y, dir );
	}

	void TryMoveBoxPrecursor( int x, int y, MoveDirect dir )
    {
		bool achieved = false;
		int dirSize = System.Enum.GetValues(typeof(MoveDirect)).Length;

		if (bma.CanMoveBox(x, y, dir))
		{
			achieved = TryMoveBox(dir);
		}
		dir = (MoveDirect)((int)(dir + 1) % dirSize);
		if (!achieved && bma.CanMoveBox(x, y, dir))
		{
			achieved = TryMoveBox(dir);
		}
		dir = (MoveDirect)((int)(dir - 2) % dirSize);
		if (!achieved && bma.CanMoveBox(x, y, dir))
		{
			achieved = TryMoveBox(dir);
		}
		dir = (MoveDirect)((int)(dir - 1) % dirSize);
		if (!achieved && bma.CanMoveBox(x, y, dir))
		{
			achieved = TryMoveBox(dir);
		}
		if (achieved && bma.transform.parent.name == Globals.Misc.SCISSOR_PUZZLE)
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

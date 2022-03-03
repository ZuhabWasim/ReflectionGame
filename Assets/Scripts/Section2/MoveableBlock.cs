using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBlock : InteractableAbstract
{
    public BlockMovingArea bma;

    public void SetBoxArea(BlockMovingArea area)
    {
        bma = area;
    }

    protected override void OnUserInteract()
    {
        TryMoveBox(MoveDirect.DOWN);
    }

    bool TryMoveBox(MoveDirect dir)
    {
        (int x, int y) pos;
        pos = bma.GetBoxIndices(transform);
        if (bma.BoxAtPosition(pos.x, pos.y) && bma.CanMoveBox(pos.x, pos.y, dir))
        {
            Debug.Log("BOX MOVE ACCEPTED");
            bma.MoveBox(pos.x, pos.y, dir);
            return true;
        }
        Debug.Log("BOX MOVE BLOCKED: " + pos.x + ", " + pos.y + ", " + dir);
        Debug.Log("BOX AT POS: " + bma.BoxAtPosition(pos.x, pos.y) + ", BOX CAN MOVE: " + bma.CanMoveBox(pos.x, pos.y, dir));
        return false;
    }

}

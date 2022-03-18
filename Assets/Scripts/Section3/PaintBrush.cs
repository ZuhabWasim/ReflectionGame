using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintBrush : PickupItem
{
    public PaintType paint { get; set; } = PaintType.NONE;
}

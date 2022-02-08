using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    public class Events
    {
        // All events used by event manager should go here
        public static string TELEPORT = "teleport";
    }

    public class Tags
    {
        public static string PICKUP_ITEM = "PickupItem";
        public static string PLAYER = "Player";
    }

    public class Misc
    {
        public static string H_AXIS = "Horizontal";
        public static string V_AXIS = "Vertical";
        public static string MOUSE_X = "Mouse X";
        public static string MOUSE_Y = "Mouse Y";
    }

    public class UIStrings
    {
        public static string INTERACT_MIRROR = "Interact With Mirror";
    }

    public class Vars
    {
        public static string TELEPORTING = "teleporting";
    }
}

public enum ItemPickupResult
{
    SUCCESS = 0,
    FAIL_INVENTORY_FULL,
    FAIL_ERROR
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals
{
    [RuntimeInitializeOnLoadMethod]
    public static void RegisterGlobalStateVars()
    {
        // All global state var should be initialized here
        GlobalState.AddVar<bool>( Globals.Vars.INTERACTABLES_ENABLED, false );
        GlobalState.AddVar<bool>( Globals.Vars.IS_PRESENT_WORLD, true );
        Debug.Log( "Loaded global state vars" );
    }

    public class Events
    {
        // All events used by event manager should go here
        public static string TELEPORT = "teleport";
        public static string INTERACT_KEY_PRESSED = "InteractKeyPress";
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
        
        // For Animations
        public static string IS_WALKING = "IsWalking";
        public static string IS_BACKING_UP = "IsBackingUp"; 
        public static string IS_JUMPING = "IsJumping";
        public static string IS_INTERACTING = "IsInteracting";
        public static float MAX_INTERACT_DISTANCE = 3.5f;
    }

    public class UIStrings
    {
        public static string INTERACT_MIRROR = "Interact With Mirror";
    }

    public class Vars
    {
        public static string TELEPORTING = "teleporting";
        public static string INTERACTABLES_ENABLED = "interactables_enabled";
        public static string IS_PRESENT_WORLD = "isPresent";
    }

    public class Keybinds
    {
        public static KeyCode InteractKey = KeyCode.F;
        public static KeyCode PickupKey = KeyCode.E;
        public static KeyCode DropKey = KeyCode.G;
        public static KeyCode InventoryKey = KeyCode.LeftShift;
    }
}

public enum ItemPickupResult
{
    SUCCESS = 0,
    FAIL_INVENTORY_FULL,
    FAIL_ERROR
}

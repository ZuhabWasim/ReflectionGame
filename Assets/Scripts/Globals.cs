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

        // Teleporting variables
        GlobalState.AddVar<bool>(Globals.Vars.TELEPORTING, false);
        GlobalState.AddVar<bool>(Globals.Vars.CAN_TELEPORT, false);

        Debug.Log( "Loaded global state vars" );
    }

    [RuntimeInitializeOnLoadMethod]
    public static void RegisterGlobalEventListeners()
    {
        EventManager.Sub( Globals.Events.TELEPORT, UpdateWorldOnTeleport );
    }

    private static void UpdateWorldOnTeleport()
    {
        GlobalState.SetVar<bool>( Globals.Vars.IS_PRESENT_WORLD, !GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) );
        // bool isPresent = GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD );
    }   

    public class Events
    {
        // All events used by event manager should go here
        public static string TELEPORT = "teleport";
        public static string LIGHTS_TURN_ON = "TurnOnLights";
        public static string LIGHTS_TURN_OFF = "TurnOffLights";
        public static string MILLIE_KEY_INTERACT = "MillieKeyInteract";
    }

    public class Tags
    {
        public static string PICKUP_ITEM = "PickupItem";
        public static string INTERACTABLE = "Interactable";
        public static string PLAYER = "Player";
        public static string DIALOGUE_SOURCE = "DialogueSource";
        public static string MAIN_SOURCE = "MainSource";
    }

    public class Misc
    {
        public static string H_AXIS = "Horizontal";
        public static string V_AXIS = "Vertical";
        public static string MOUSE_X = "Mouse X";
        public static string MOUSE_Y = "Mouse Y";
        public static string UI_Canvas = "UI_Canvas";
        
        // For Animations
        public static string IS_WALKING = "IsWalking";
        public static string IS_BACKING_UP = "IsBackingUp"; 
        public static string IS_JUMPING = "IsJumping";
        public static string IS_INTERACTING = "IsInteracting";
        public static float MAX_INTERACT_DISTANCE = 3.5f;
    }

    public class UIStrings
    {
        public static string INTERACT_ITEM = "Interact with ";
        public static string PICKUP_ITEM = "Pick up ";
        public static string MOVE_ITEM = "Move ";
        public static string OPEN_ITEM = "Open ";
        public static string CLOSE_ITEM = "Close ";

        public static string USE_ITEM_A = "Use ";
        public static string USE_ITEM_B = " on ";

        public static string MIRROR_ITEM = "Mirror";
        public static string DRAWER_ITEM = "Drawer";
        public static string NOTE_ITEM = "Note";

        public static string HANDKERCHIEF_ITEM = "Handkerchief";
        public static string MUSICBOXMILLIE_ITEM = "Millie's Key";

        public static string USE_HANDKERCHIEF = "Use Handkerchief";
    }

    public class Vars
    {
        public static string TELEPORTING = "teleporting";
        public static string CAN_TELEPORT = "canTeleport";

        public static string INTERACTABLES_ENABLED = "interactables_enabled";
        public static string IS_PRESENT_WORLD = "isPresent";
    }

    public class Teleporting
    {
        public const float TELEPORTER_COOLDOWN = 1.75f;
        public const float INPUT_LOCK_COOLDOWN = 0.5f;
    }

    public class AudioFiles
    {
        public static string PAPER_UNRAVEL = "paper_unravel";
        public static string OBJECT_OBTAINED = "object_obtained";
        public static string NON_INTERACTABLE = "non_interactable";
        public static string MAIN_DOOR = "main_door";
        public static string LIGHTS_TURN_ON_ROOM = "seeing_lit_room";
        public static string ENTERING_ROOM = "its_really_dark";
        public static string DIRTY_MIRROR = "dirty_mirror";
    }

    public class EnvironmentParams
    {
        public static SunConfig presentSunConfig = new SunConfig( 10000.0f, 10000.0f );
    }
}

public enum ItemPickupResult
{
    SUCCESS = 0,
    FAIL_INVENTORY_FULL,
    FAIL_ERROR
}

public enum Keybinds
{
    INTERACT_KEY = KeyCode.F,
    USE_ITEM_KEY = KeyCode.E,
    DROP_KEY = KeyCode.G,
    INVENTORY_KEY = KeyCode.LeftShift,

    // This is for debug only, should be removed in release
    DEBUG_TRIGGER = KeyCode.Alpha0
}

public struct SunConfig
{
    public readonly float colorTemp; // temp in K
    public readonly float intensity;

    public SunConfig( float temp, float intensity )
    {
        colorTemp = temp;
        this.intensity = intensity;
    }
};

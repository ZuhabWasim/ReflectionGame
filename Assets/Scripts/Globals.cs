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

    [RuntimeInitializeOnLoadMethod]
    public static void RegisterGlobalEventListeners()
    {
        EventManager.Sub( Globals.Events.TELEPORT, UpdateWorldOnTeleport );
    }

    private static void UpdateWorldOnTeleport()
    {
        GlobalState.SetVar<bool>( Globals.Vars.IS_PRESENT_WORLD, !GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) );
        bool isPresent = GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD );
        // !! This is broken rn
        // GameObject sun = GameObject.Find( "Sun" );
        // GameObject moon =  GameObject.Find( "Moon" );
        // moon.SetActive( isPresent );
        // sun.SetActive( !isPresent );
    }   

    public class Events
    {
        // All events used by event manager should go here
        public static string TELEPORT = "teleport";
        public static string INTERACT_KEY_PRESSED = "InteractKeyPress";
        public static string LIGHTS_TURN_ON = "TurnOnLights";
        public static string LIGHTS_TURN_OFF = "TurnOffLights";
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
        
        // For Animations
        public static string IS_WALKING = "IsWalking";
        public static string IS_BACKING_UP = "IsBackingUp"; 
        public static string IS_JUMPING = "IsJumping";
        public static string IS_INTERACTING = "IsInteracting";
        public static float MAX_INTERACT_DISTANCE = 3.5f;
    }

    public class UIStrings
    {
        public static string INTERACT_MIRROR = "Interact with Mirror";

        public static string PICKUP_HANDKERCHIEF = "Pick up Handkerchief";
        public static string USE_HANDKERCHIEF = "Use Handkerchief";

        public static string INTERACT_DRAWER = "Interact with Drawer";
        public static string INTERACT_SWITCH = "Interact with Switch";
        public static string INTERACT_NOTE = "Interact with Note";
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

    public class AssetPaths
    {
        public static string MAIN_DOOR_SOUND = "Assets/Audio/Section 1/main_door.wav";
    }
}

public enum ItemPickupResult
{
    SUCCESS = 0,
    FAIL_INVENTORY_FULL,
    FAIL_ERROR
}

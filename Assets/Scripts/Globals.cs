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
		GlobalState.AddVar<bool>( Globals.Vars.TELEPORTING, false );
		GlobalState.AddVar<bool>( Globals.Vars.CAN_TELEPORT, false );

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
		public static string HAS_MILLIE_KEY = "HasMillieKey";
		public static string LOCK_MOM_DOOR = "LockMomDoor";
		public static string LOCK_DAD_DOOR = "LockDadDoor";
		public static string BOOKSHELF_BOOK_PLACED = "BookshelfBookPlaced";
		public static string BOOKSHELF_BOOK_PICKED_UP = "BookshelfBookPickedUp";
		public static string PAINT_BRUSH_WET = "PaintBrushWet";
		public static string DAD_PUZZLE_1_MILLIE_PAINT = "DadPuzzle1_MillePaint";
		public static string DAD_PUZZLE_1_BOOKSHELF_SOLVED = "DadPuzzle1_BookshelfSolved";
		public static string CANVAS_STATE_CHANGE = "CanvasStateChange";

		public static string OBTAINED_SCISSORS = "ObtainedScissors";
		public static string CUTTING_CLOTHING_RACK = "CuttingClothingRack";
		public static string SWEPT_DEBRIS = "SweptDebris";
		public static string MOVE_MIRROR_B = "MoveMirrorB";
		public static string HAS_MOM_KEY = "HasMomKey";
		public static string UNBLOCKING_MIRROR_B = "UnblockingMirrorB";
		public static string BLOCKING_MIRROR_B = "BlockingMirrorB";
		public static string READ_MOM_NOTE = "ReadMomNote";


	}

	public class Tags
	{
		public static string PICKUP_ITEM = "PickupItem";
		public static string INTERACTABLE = "Interactable";
		public static string PLAYER = "Player";
		public static string DIALOGUE_SOURCE = "DialogueSource";
		public static string MAIN_SOURCE = "MainSource";
		public static string AMBIENCE_SOURCE = "AmbienceSource";
		public static string MUSIC_SOURCE = "MusicSource";
		public static string INTERACTABLE_BOOK = "InteractableBook";
		public static string BOOK_SLOT = "BookSlot";
		public static string MUSIC_BOX = "MusicBox";

	}

	public class Misc
	{
		public static string H_AXIS = "Horizontal";
		public static string V_AXIS = "Vertical";
		public static string MOUSE_X = "Mouse X";
		public static string MOUSE_Y = "Mouse Y";
		public static string UI_Canvas = "UI_Canvas";
		public static string PAINT_BRUSH = "Paint Brush";
		public static string SCISSOR_PUZZLE = "ScissorPuzzle";

		// For Animations
		public static string IS_WALKING = "IsWalking";
		public static string IS_BACKING_UP = "IsBackingUp";
		public static string IS_JUMPING = "IsJumping";
		public static string IS_INTERACTING = "IsInteracting";
		public static float MAX_INTERACT_DISTANCE = 3f;
		public static float DEFAULT_PROXIMITY_TRIGGER_DIST = 3.5f;
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
		public static string SCISSORS_ITEM = "Scissors";
		public static string BROOM_ITEM = "Broom";

		public static string HANDKERCHIEF_ITEM = "Handkerchief";
		public static string MUSICBOXMILLIE_ITEM = "Millie's Key";

		public static string USE_HANDKERCHIEF = "Use Handkerchief";
		public static string NOTE_PLACEHOLDER = "This note has not yet been collected.";
		public static string NOTE_TITLE_PLACEHOLDER = "Unknown Note";
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

		public class Ambience
		{
			public static string PRESENT_AMBIENCE = "amb_rainceiling_loop";
			public static string PAST_AMBIENCE = "amb_past_birds_wind_loop";
		}

		public class Music
		{
			// Music goes here
		}

		public class General
		{
			public static string PAPER_UNRAVEL = "paper_unravel";
			public static string OBJECT_OBTAINED = "object_obtained";
			public static string MUSICBOXKEY_OBTAINED = "key_acquired";
			public static string NON_INTERACTABLE = "non_interactable";
		}

		public class Section1
		{
			public static string MAIN_DOOR = "main_door";
			public static string FUSE_BOX_TOGGLETICK = "fusebox_toggle_tick";
			public static string FUSE_BOX = "fusebox";
			public static string HANDKERCHIEF = "handkerchief";

		}

		public class Section2
		{
			public static string CUTTING_DRESS = "sfx_scissors_cutting_dress";
			public static string MOVING_MIRROR = "mirror_b_move_placeholder";
			public static string SAFE_UNLOCK = "sfx_safe_unlock_open_sequence";
			public static string SWEEP_DEBRIS = "sfx_broom_sweep_debris";
			public static string CARDBOARD_SLIDE = "sfx_cardboard_box_slide";

		}
	}

	public class SoundEffects
	{

	}
	public class VoiceLines
	{
		public class General
		{
			public static string CANT_USE_ITEM = "1.14__hmm_that_doesnt_seem_right";
			public static string NOT_HOLDING_ANYTHING = "not_holding_anything";
			public static string PLACEHOLDER = "placeholder";
			public static string PLACEHOLDER_1 = "placeholder1";
			public static string PLACEHOLDER_2 = "placeholder2";
			public static string PLACEHOLDER_3 = "placeholder3";
		}
		public class Section1
		{
			public static string DARK_IN_HERE = "1.03__dark_in_here";
			public static string TOO_DARK_TO_SEE = "1.04__too_dark_to_see";
			public static string DOOR_AM_I_LOCKED_IN = "1.05__door_am_i_locked_in";
			public static string DOOR_NOT_BUDGING = "1.05__door_am_i_locked_in_short";
			public static string LOCKED_FROM_THE_INSIDE = "1.06__locked_from_the_inside";

			public static string JOHNSON_LETTER = "1.11__johnson_letter";
			public static string MILLE_POV_INTRO = "1.12__mille_pov_intro";
			public static string HMM_THAT_DOESNT_SEEM_RIGHT = "1.14__hmm_that_doesnt_seem_right";
			public static string HMM_CODE = "1.15__0_hmm_1";
			public static string FILTHY_MIRROR = "1.18__filthy_mirror";
			public static string CANT_SEE_REFLECTION_ROOM_DARK = "1.20__cant_see_reflection_room_dark";
			public static string CODE_DIFFERENT = "1.25__code_different";
			public static string CODE_FLIPPED = "code_flipped";

			public static string HOUSE_FEELS_UNFAMILIAR = "1.28__house_feels_unfamiliar";
			public static string STILL_CANT_SEE_MYSELF = "1.30__still_cant_see_myself";
			public static string ITS_MY_MUSICBOX = "1.32__its_my_musicbox";
			public static string DISCOVER_BOX_FIRST = "1.33__discover_box_first";
			public static string DISCOVER_KEY_FIRST = "1.33__discover_key_first";
			public static string DISCOVER_KEY_FIRST__OH_THERE_IT_IS = "1.35__discover_key_first__oh_there_it_is";
			public static string BOX_NEEDS_TWO_OTHER_KEYS = "1.36__box_needs_two_other_keys";

			public static string IS_THE_MIRROR_GLOWING = "1.38__is_the_mirror_glowing";
			public static string WHY_AM_I_SO_SMALL = "1.42__why_am_i_so_small";
			public static string I_M_TOO_SMALL = "1.43__i_m_too_small";
			public static string MOTHERS_NAGGING = "1.48__mothers_nagging";
			public static string FATHERS_VOICE = "1.49__fathers_voice";
		}

		public class Section2
		{
			public static string CONTAINER_TOO_FAR = "container_too_far";
			public static string CONTAINER_TOO_HIGH = "container_too_high";
			public static string LAVISH_CLOTHES = "lavish_clothes";
			public static string TONE_DOWN = "tone_down";
			public static string MIRROR_A_RIGHT_SPOT = "mirror_a_right_spot";
			public static string NO_MORE_OBSTRUCTIONS = "no_more_obstructions";
			public static string ANOTHER_BOX_BLOCKING = "another_box_blocking";
			public static string MOM_NOTE = "mom_note";
		}
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
	ESCAPE_KEY = KeyCode.Escape,

	// This is for debug only, should be removed in release
	DEBUG_TRIGGER = KeyCode.Alpha0
}

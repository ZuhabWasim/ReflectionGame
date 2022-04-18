using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals
{
	[RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
	public static void RegisterOnSceneLoadCallback()
	{
		SceneManager.sceneLoaded += ( Scene s, LoadSceneMode mode ) =>
		{
			if ( s.buildIndex != MAIN_SCENE )
			{
				return;
			}

			Debug.Log( "calling on scene loaded callbacks" );
			RegisterGlobalStateVars();
			RegisterGlobalEventListeners();
			Objectives.PopulateObjectives();
			ForkList.InitForks();
			Inventory.InitInventory();
			InteractNote.InitJournal();
		};
	}

	public static void RegisterGlobalStateVars()
	{
		// All global state var should be initialized here
		GlobalState.AddVar<bool>( Globals.Vars.INTERACTABLES_ENABLED, false );
		GlobalState.AddVar<bool>( Globals.Vars.IS_PRESENT_WORLD, true );

		// Teleporting variables
		GlobalState.AddVar<bool>( Globals.Vars.TELEPORTING, false );
		GlobalState.AddVar<bool>( Globals.Vars.CAN_TELEPORT, false );
		
		Time.timeScale = 1f;
		Debug.Log( "Loaded global state vars" );
	}

	public static void RegisterGlobalEventListeners()
	{
		EventManager.Sub( Globals.Events.TELEPORT, UpdateWorldOnTeleport );

		// Mom Closet disabling
		EventManager.Sub( Globals.Events.LOCK_MOM_DOOR, DisableMainRooms );
		EventManager.Sub( Globals.Events.HAS_MOM_KEY, EnableMainRooms );

		EventManager.Sub( Globals.Events.LOCK_MOM_DOOR, DisableDadCloset );
		EventManager.Sub( Globals.Events.HAS_MOM_KEY, EnableDadCloset );

		// Dad closet disabling
		EventManager.Sub( Globals.Events.LOCK_DAD_DOOR, DisableMainRooms );
		EventManager.Sub( Globals.Events.LOCK_DAD_DOOR, DisableMomCloset );
		EventManager.Sub( Globals.Events.HAS_DAD_KEY, EnableMainRooms );
		EventManager.Sub( Globals.Events.HAS_DAD_KEY, EnableMomCloset );

		EventManager.Sub( Globals.Events.GAME_RESTART, AudioPlayer.OnExit );
		EventManager.Sub( Globals.Events.GAME_RESTART, PickupItem.OnExit );
		EventManager.Sub( Globals.Events.GAME_RESTART, Objectives.OnExit );
		EventManager.Sub( Globals.Events.GAME_RESTART, InteractNote.OnExit );
		EventManager.Sub( Globals.Events.GAME_RESTART, GlobalState.OnExit );
		EventManager.Sub( Globals.Events.GAME_RESTART, ForkList.OnExit );
	}

	private static void UpdateWorldOnTeleport()
	{
		GlobalState.SetVar<bool>( Globals.Vars.IS_PRESENT_WORLD, !GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD ) );
		// bool isPresent = GlobalState.GetVar<bool>( Globals.Vars.IS_PRESENT_WORLD );
	}

	private static void DisableMainRooms()
	{
		Debug.Log( "Disabling Main Room" );
		EventManager.Fire( Globals.Events.DEACTIVATE_MAIN_ROOM );
	}

	private static void EnableMainRooms()
	{
		Debug.Log( "Enabling Main Room" );
		EventManager.Fire( Globals.Events.ACTIVATE_MAIN_ROOM );
	}

	private static void DisableMomCloset()
	{
		Debug.Log( "Disabling Mom Closet" );
		EventManager.Fire( Globals.Events.DEACTIVATE_MOM_CLOSET );
	}

	private static void EnableMomCloset()
	{
		Debug.Log( "Enabling Mom Closet" );
		EventManager.Fire( Globals.Events.ACTIVATE_MOM_CLOSET );
	}

	private static void DisableDadCloset()
	{
		Debug.Log( "Disabling Dad Closet" );
		EventManager.Fire( Globals.Events.DEACTIVATE_DAD_CLOSET );
	}

	private static void EnableDadCloset()
	{
		Debug.Log( "Enabling Dad Closet" );
		EventManager.Fire( Globals.Events.ACTIVATE_DAD_CLOSET );
	}

	public static int MAIN_SCENE = 1;
	public static int MENU_SCENE = 0;

	// All events used by event manager should go here
	// Do not add spaces between the words of the string, the event system will split it into multiple events
	public class Events
	{
		// General events
		public static string TELEPORT = "teleport";
		public static string ACTIVATE_MOM_CLOSET = "ActivateMomCloset";
		public static string DEACTIVATE_MOM_CLOSET = "DeactivateMomCloset";
		public static string ACTIVATE_DAD_CLOSET = "ActivateDadCloset";
		public static string DEACTIVATE_DAD_CLOSET = "DeactivateDadCloset";
		public static string ACTIVATE_MAIN_ROOM = "ActivateMainRoom";
		public static string DEACTIVATE_MAIN_ROOM = "DeactivateMainRoom";
		public static string GAME_RESTART = "GAME_RESTART";

		// Section 1
		public static string LIGHTS_TURN_OFF = "TurnOffLights";
		public static string GO_ENTER_CODE = "GoEnterCode";
		//public static string OPEN_FUSEBOX = "OpenFusebox";
		public static string GO_CLEAN_MIRROR = "GoCleanMirror";
		public static string DONE_CLEAN_MIRROR = "DoneCleanMirror";
		public static string DONE_INVESTIGATE = "DoneInvestigate";
		public static string LIGHTS_TURN_ON = "TurnOnLights";
		public static string MILLIE_KEY_INTERACT = "MillieKeyInteract";
		public static string HAS_MILLIE_KEY = "HasMillieKey";
		public static string FIRST_TELEPORT = "OnFirstTeleport";

		// Section 2
		public static string LOCK_MOM_DOOR = "LockMomDoor";
		public static string OBTAINED_SCISSORS = "ObtainedScissors";
		public static string CUTTING_CLOTHING_RACK = "CuttingClothingRack";
		public static string SWEPT_DEBRIS = "SweptDebris";
		public static string MOVE_MIRROR_B = "MoveMirrorB";
		public static string HAS_MOM_KEY = "HasMomKey";
		public static string UNBLOCKING_MIRROR_B = "UnblockingMirrorB";
		public static string BLOCKING_MIRROR_B = "BlockingMirrorB";
		public static string READ_MOM_NOTE = "ReadMomNote";
		public static string BLOCKING_SECRET_LACIS = "BlockingSecretLacis";
		public static string UNBLOCKING_SECRET_LACIS = "UnblockingSecretLacis";

		// Section 3
		public static string LOCK_DAD_DOOR = "LockDadDoor";
		public static string LOCK_PAST_DAD_SHELF = "LockPastDadShelf";
		public static string BOOKSHELF_BOOK_PLACED = "BookshelfBookPlaced";
		public static string BOOKSHELF_BOOK_PICKED_UP = "BookshelfBookPickedUp";
		public static string PAINT_BRUSH_WET = "PaintBrushWet";
		public static string DAD_PUZZLE_1_MILLIE_PAINT = "DadPuzzle1_MillePaint";
		public static string DAD_PUZZLE_1_BOOKSHELF_SOLVED = "DadPuzzle1_BookshelfSolved";
		public static string CANVAS_STATE_CHANGE = "CanvasStateChange";
		public static string RECOMPUTED_LIGHT = "RECOMPUTED_LIGHT";
		public static string HAS_DAD_KEY = "HasDadKey";
		public static string UPDATE_MOVEMENT = "UpdateShelfMover";
		public static string DAD_PUZZLE_2_SPOTLIGHT_INSTALLED = "DadPuzzle2_SpotlightInstalled";
		public static string DAD_PUZZLE_2_LIGHTPUZZLE_SOLVED = "DadPuzzle2_LightPuzzleSolved";

		// Objective-related
		public static string LOCK_EITHER_DOOR = "LockEitherDoor";
		public static string PICKUP_EITHER_KEY = "PickupEitherKey";
		public static string PICKUP_BOTH_KEY = "PickupBothKey";
		public static string EXIT_BOTH = "ExitedBothClosets";

		public static string GO_CHECK_MUSIC_BOX = "GoCheckBox";

		public static string MOM_AT_SECOND_PART = "AtMomPartTwo";
		public static string NEED_SCISSORS = "NeedScissors";
		public static string NEED_BROOM = "NeedBroom";
		public static string NEED_WHEEL = "NeedWheel";
		public static string USED_WHEEL = "UsedWheel";
		public static string EXIT_MOM = "ExitedMomCloset";

		public static string NEED_BRUSH1 = "NeedBrushOne";
		public static string NEED_PAINT_CANVAS = "NeedPaintCanvas";
		public static string NEED_STEP = "NeedStep";
		public static string NEED_BOOK_CODE = "NeedBookCode";
		public static string NEED_BRUSH2 = "NeedBrushTwo";
		public static string BRUSH_WET_REFL = "BrushWetRefl";
		public static string BRUSH_WET_WHITE = "BrushWetWhite";
		public static string NEED_BULB = "NeedBulb";
		public static string DONE_DAD_EXPLORE = "DoneDadExplore";
		public static string SOLVE_DAD_PUZZLE = "SolveDadPuzzle";
		public static string NEED_DAD_SAFE_CODE = "NeedDadSafeCode";
		public static string EXIT_DAD = "ExitedDadCloset";

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
		public static string PAST_FIREPLACE_MIRROR = "PastFireplaceMirror";
		public static string COLOUR_FILTER = "ColourFilter";

		public static string PRESENT_MOM_DOOR = "PresentMomDoor";
		public static string PRESENT_DAD_DOOR = "PresentDadDoor";
		public static string PRESENT_DAD_SHELF = "PresentDadShelf";
	}

	public class Misc
	{
		public static int HIGHLIGHT_LAYER = 7;
		public static int DEFAULT_LAYER = 0;
		public static string H_AXIS = "Horizontal";
		public static string V_AXIS = "Vertical";
		public static string MOUSE_X = "Mouse X";
		public static string MOUSE_Y = "Mouse Y";
		public static string UI_Canvas = "UI_Canvas";
		public static string PAINT_BRUSH = "Dry Paint Brush";
		public static string WET_PAINT_BRUSH = "Reflective Paint Brush";
		public static string WHITE_PAINT_BRUSH = "White Paint Brush";
		public static string SCISSOR_PUZZLE = "ScissorPuzzle";
		public static string MIRROR_C_PAST_TILES = "MirrorCPastTiles";
		public static string EMPTY_BUCKET = "Empty Bucket (E)";

		// For Animations
		public static string IS_WALKING = "IsWalking";
		public static string IS_BACKING_UP = "IsBackingUp";
		public static string IS_JUMPING = "IsJumping";
		public static string IS_INTERACTING = "IsInteracting";
		public static float MAX_INTERACT_DISTANCE = 3.5f;
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
		public static string DRAWER_PULL_ITEM = "Drawer Pull";
		public static string CROWBAR_ITEM = "Crowbar";
		public static string ALICE_IN_WONDERLAND = "Alice in Wonderland";

		public static string HANDKERCHIEF_ITEM = "Handkerchief";
		public static string MUSICBOXMILLIE_ITEM = "Millie's Key";
		public static string MUSICBOXMOM_ITEM = "Mom's Key";
		public static string MUSICBOXDAD_ITEM = "Dad's Key";

		public static string USE_HANDKERCHIEF = "Use Handkerchief";
		public static string NOTE_PLACEHOLDER = "This note has not yet been collected.";
		public static string NOTE_TITLE_PLACEHOLDER = "Unknown Note";

		// Objectives
		public static string OBJECTIVE_TURN_ON_POWER = "Turn on the lights";
			public static string SUB_OBJ_INVESTIGATE_ROOM = "Search the room for clues";
			//public static string SUB_OBJ_INVESTIGATE_BOX = "Investigate the fusebox";
			public static string SUB_OBJ_FIND_BOX_CODE = "Find the code and enter it at the fusebox";
			//public static string SUB_OBJ_FIND_HANDKERCHIEF = "Find something to clean the vanity mirror";
			public static string SUB_OBJ_FIND_CLEAN_MIRROR = "Clean the vanity mirror";

		public static string OBJECTIVE_EXPLORE = "Explore the bedroom";
			public static string SUB_OBJ_MUSIC_KEY1 = "Use music box key on music box above the fireplace";
			public static string SUB_OBJ_CHECK_MIRROR = "Investigate the fireplace mirror";

		public static string OBJECTIVE_FIND_TWO_KEYS = "Find 2 more music box keys";
			public static string SUB_OBJ_INVESTIGATE_MOM_CLOSET = "Search Mom's closet in the past";
			public static string SUB_OBJ_INVESTIGATE_DAD_CLOSET = "Search Dad's closet in the past";

		public static string OBJECTIVE_FIND_MOMS_KEY = "Find Mom's music box key";
			public static string SUB_OBJ_FIND_SCISSORS = "Use scissors to cut the dresses";
			public static string SUB_OBJ_FIND_BROOM = "Use a broom to clear the debris";
			public static string SUB_OBJ_GET_TO_SECOND = "Find a way to get to the second half of the closet";
			public static string SUB_OBJ_MOM_USE_WHEEL = "Find and use a wheel to repair the dress form";
			public static string SUB_OBJ_USE_SAFE = "Find and enter the code for the safe";
			
		public static string OBJECTIVE_LEAVE_MOM = "Leave Mom's closet";

		public static string OBJECTIVE_FIND_ONE_KEY = "Find the final music box key";

		public static string OBJECTIVE_FIND_DADS_KEY = "Find Dad's music box key";
			public static string SUB_OBJ_BRUSH = "Apply paint to a paint brush";
			public static string SUB_OBJ_PAINT = "Paint the blank canvas";
			public static string SUB_OBJ_FIND_BOOK = "Find the missing book";
			public static string SUB_OBJ_FIND_STEP = "Find a way to reach the bookshelf";
			public static string SUB_OBJ_FIND_BOOK_CODE = "Find the correct order of the books on the shelf";
			public static string SUB_OBJ_EXPLORE2 = "Explore the secret room";
			public static string SUB_OBJ_BRUSH2 = "Apply white paint to the second paint brush";
			public static string SUB_OBJ_TURN_ON_PROJECTOR = "Turn on the light projector";
			public static string SUB_OBJ_SOLVE_PUZZLE = "Direct the light through each filter with the paint brushes";
			public static string SUB_OBJ_USE_SAFE2 = "Enter the code for the safe";

		public static string OBJECTIVE_LEAVE_DAD = "Leave Dad's closet";

		public static string OBJECTIVE_USE_KEYS = "Use the keys on the music box";
	}

	public class Vars
	{
		public static string TELEPORTING = "teleporting";
		public static string CAN_TELEPORT = "canTeleport";

		public static string INTERACTABLES_ENABLED = "interactables_enabled";
		public static string IS_PRESENT_WORLD = "isPresent";
		public static string DAD_PUZZLE_2_FINAL_LIGHT_COLOR = "FinalCanvasLightColor";
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
			public static string OPEN_DRAWER = "open_drawer";
			public static string CLOSE_DRAWER = "close_drawer";
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
			public static string CANT_USE_ITEM = "extra.1.066.i_dont_think_i_can_use_this_here";
			public static string NOT_HOLDING_ANYTHING = "extra.1.065.not_holding_anything";
			public static string CAN_USE_THIS_HERE = "extra.1.066.i_dont_think_i_can_use_this_here";
			public static string PLACEHOLDER = "placeholder";
			public static string PLACEHOLDER_1 = "placeholder1";
			public static string PLACEHOLDER_2 = "placeholder2";
			public static string PLACEHOLDER_3 = "placeholder3";
		}
		public class Section1
		{
			public static string DARK_IN_HERE = "1.003.why_is_it_so_dark";
			public static string TOO_DARK_TO_SEE = "1.004.too_dark_should_turn_on_light";
			public static string DOOR_AM_I_LOCKED_IN = "1.005.am_i_locked_in";
			public static string DOOR_NOT_BUDGING = "1.005.am_i_locked_in_short";
			public static string LOCKED_FROM_THE_INSIDE = "1.006.locked_from_inside";
			public static string MILLE_POV_INTRO = "1.012.after_johnson_letter";
			public static string HMM_THAT_DOESNT_SEEM_RIGHT = "1.014.doesn_t_seem_right";
			public static string HMM_CODE = "1.015.can_t_make_out_code";
			public static string FILTHY_MIRROR = "1.018.filthy_mirror";
			public static string CANT_SEE_REFLECTION_ROOM_DARK = "1.020.can_t_see_reflection";
			public static string CODE_DIFFERENT = "1.025.is_that_the_code";
			public static string HOUSE_FEELS_UNFAMILIAR = "1.028.has_it_been_20_years";
			public static string STILL_CANT_SEE_MYSELF = "1.030.still_can_t_see_myself";
			public static string ITS_MY_MUSICBOX = "1.032.oh_it_s_my_music_box";
			public static string DISCOVER_KEY_FIRST = "1.033.discover_key_first";
			public static string DISCOVER_BOX_FIRST = "1.033.discover_music_box_first";
			public static string DISCOVER_KEY_FIRST__OH_THERE_IT_IS = "1.035.oh_there_it_is";
			public static string BOX_NEEDS_TWO_OTHER_KEYS = "1.036.forgot_needs_two_other_keys";
			public static string IS_THE_MIRROR_GLOWING = "1.038.mirrorr_glowing";
			public static string WHY_AM_I_SO_SMALL = "1.042.why_am_i_so_small";
			public static string I_M_TOO_SMALL = "1.043.i_m_too_small";
			public static string MOTHERS_NAGGING = "1.048.can_hear_mother_s_nagging";
			public static string FATHERS_VOICE = "1.049.haven_t_heard_father_s_voice_in_so_long";
			
			public static string WOAH_WHAT_JUST_HAPPENED_TO_THE_MIRROR = "extra.1.054.woah_what_just_happened_to_the_mirror";
			public static string HM_THAT_DOESN_T_SEEM_RIGHT = "extra.1.055.hm_that_doesn_t_seem_right";
			public static string AM_I_IN_THE_PAST = "extra.1.056.am_i_in_the_past";
			public static string GREAT_AGAIN = "extra.1.057.great_again";
			public static string LIGHTS_ARE_ALREADY_ON = "extra.1.058.lights_are_already_on";
			public static string MOTHER_S_VANITY_SET = "extra.1.059.mother_s_vanity_set";
			public static string COMPLETE_CODE = "extra.1.060.complete_code";
			public static string ROOMS_LOOKS_EMPTY = "extra.1.061.rooms_looks_empty";
			public static string FATHER_LOVED_TO_READ = "extra.1.062.father_loved_to_read";
			public static string SOUND_OF_THE_RAIN = "extra.1.063.sound_of_the_rain";
			public static string GOLDEN_LIGHT = "extra.1.064.golden_light";
			public static string NOT_HOLDING_ANYTHING = "extra.1.065.not_holding_anything";
			public static string I_DONT_THINK_I_CAN_USE_THIS_HERE = "extra.1.066.i_dont_think_i_can_use_this_here";
		}

		public class Section2
		{
			public static string EXPENSIVE_GOWNS = "2.003.woah_expensive_gowns";
			public static string AM_I_LOCKED_IN = "2.004.am_i_locked_in";
			public static string TOO_HEAVY_EASY = "2.005.gowns_too_heavy_to_move";
			public static string SEWING_TOOLS_KEPT_HIGH_UP = "2.010.sewing_tools_kept_high_up";
			public static string THESE_LOOK_SHARP_ENOUGH = "2.014.these_look_sharp_enough";
			public static string CAN_T_REACH_MIRROR = "2.016.can_t_reach_mirror";
			public static string MOTHER_S_GOING_TO_SCOLD_ME = "2.017.mother_s_going_to_scold_me";
			public static string CAN_T_BELIEVE_I_DID_THAT = "2.022.can_t_believe_i_did_that";
			public static string ANOTHER_MIRROR = "2.026.another_mirror";
			public static string CAN_T_GO_THROUGH_THIS_WAY = "2.027.can_t_go_through_this_way";
			public static string LOOK_AT_ALL_THESE_BOOKS = "2.027.look_at_all_these_books";
			public static string FATHER_READ_TO_ME_NOT_SEEN_BOOK_YET = "2.028.father_read_to_me_not_seen_book_yet";
			public static string PASSAGE_ON_OTHER_SIDE = "2.028.passage_on_other_side";
			public static string CAN_I_MOVE_THIS = "2.029.can_i_move_this";
			public static string FATHER_READ_BOOK_FOUND_WHITE_BOOK_ALREADY = "2.029.father_read_book_found_white_book_already";
			public static string MIRROR_HAS_BEEN_MOVED_MANY_TIMES = "2.031.mirror_has_been_moved_many_times";
			public static string STUFF_IN_THE_WAY = "2.032.stuff_in_the_way";
			public static string CLEAR_DEBRIS = "2.035.clear_debris";
			public static string THAT_SHOULD_DO_IT = "2.039.that_should_do_it";
			public static string I_CAN_MOVE_THROUGH_IT_NOW = "2.040.i_can_move_through_it_now";
			public static string THUMP_GOOD_HEAVENS = "2.053.thump_good_heavens";
			public static string STILL_TOO_HIGH = "2.057.still_too_high";
			public static string THIS_COULD_WORK_BUT = "2.059.this_could_work_but";
			public static string EASY_WAY_OUT = "2.061.easy_way_out";
			public static string ONLY_WAY_FORWARD_IS_THROUGH_MIRROR = "2.062.only_way_forward_is_through_mirror";
			public static string MIRROR_DOUBLE_SIDED = "2.065.mirror_double_sided";
			public static string IT_S_ON_WHEELS = "2.066.it_s_on_wheels";
			public static string MOTHER_S_OLD_SAFE = "2.071.mother_s_old_safe";
			public static string LOCKED = "2.072.locked";
			public static string LET_S_SEE_WHERE_THIS_LEADS = "2.077.let_s_see_where_this_leads";
			public static string NOTE_OVER_THERE = "2.080.note_over_there";
			public static string IT_WON_T_ROLL_FOUND_WHEEL_ALREADY = "2.081.it_won_t_roll_found_wheel_already";
			public static string IT_WON_T_ROLL_HAVEN_T_FOUND_WHEEL_YET = "2.081.it_won_t_roll_haven_t_found_wheel_yet";
			public static string BLOCKED = "2.087.blocked";
			public static string I_CAN_MOVE_DRESS_FORM_NOW = "2.090.i_can_move_dress_form_now";
			public static string THIS_LOOKS_IMPORTANT = "2.091.this_looks_important";
			public static string MOTHER_SEEMED_COLD_AND_STERN = "2.094.mother_seemed_cold_and_stern";
			public static string THAT_SHOULD_DO_IT_MOVE = "2.104.that_should_do_it";
			public static string PERFECT = "2.105.perfect";
			public static string TRY_USING_BIRTH_YEAR = "2.117.try_using_birth_year";
			public static string FEELS_LIKE_I_SHOULDN_T_BE_HERE = "2.124.feels_like_i_shouldn_t_be_here";
			public static string OBTAINED_MOTHER_KEY = "2.125.obtained_mother_key";
			public static string A_DRAWER_PULL = "2.126.a_drawer_pull";
			public static string CAN_T_TRAVEL_THROUGH_MIRRORS_ANYMORE = "2.131.can_t_travel_through_mirrors_anymore";
			public static string USE_DRAWER_PULL = "2.133.use_drawer_pull";
			public static string FINALLY_I_CAN_LEAVE = "2.134.finally_i_can_leave";
			public static string CHANGES_IN_PAST_AFFECT_PRESENT = "2.144.changes_in_past_affect_present";
			public static string IS_IT_MOVING_IN_PRESENT = "2.145.is_it_moving_in_present";
			
			public static string DEBRIS_TOUGHER_TIME = "extra.2.147.debris_tougher_time";
			public static string ANOTHER_BOX_BLOCKING_MY_WAY = "extra.2.148.another_box_blocking_my_way";
			public static string BUNCH_OF_BOXES_BLOCKING_THE_WAY = "extra.2.149.bunch_of_boxes_blocking_the_way";
			//public static string I_THINK_I_CAN_LEAVE_THE_CLOSET_NOW = "extra.2.150.i_think_i_can_leave_the_closet_now";
			public static string LET_S_CLEAN_THIS_UP_MOTHER_ALWAYS_HATED_MESSES = "extra.2.151.let_s_clean_this_up_mother_always_hated_messes";
			public static string MIRROR_SEEMS_CLOUDY = "extra.2.152.mirror_seems_cloudy";
			public static string MOTHER_CURSE_BREAKING_THINGS = "extra.2.153.mother_curse_breaking_things";
			public static string I_NEED_TO_GET_A_LITTLE_CLOSER = "extra.2.154.i_need_to_get_a_little_closer";
			public static string GLASS_CONTAINER_TOO_HIGH = "extra.2.155.glass_container_too_high";
			public static string I_NEED_SCISSORS = "extra.2.157.i_need_scissors";
			public static string I_THINK_I_KNOW_THE_SAFE_CODE_NOW = "extra.2.158.i_think_i_know_the_safe_code_now";
			public static string THIS_MUST_BE_THE_SAME_ROOM = "extra.2.159.this_must_be_the_same_room";
			public static string NO_MORE_OBSTRUCTIONS = "extra.2.160.no_more_obstructions";
			public static string REFLECTION_TAKES_ME_BACK = "extra.2.161.reflection_takes_me_back";
			public static string RAILS_ATTACHED_TO_MIRROR = "extra.2.162.rails_attached_to_mirror";
			public static string SEEM_TO_BE_A_WAY_THROUGH = "extra.2.164.seem_to_be_a_way_through";
			public static string SUMMIT_OF_BOOKSHELF = "extra.2.165.summit_of_bookshelf";
			public static string ROOM_LOOKS_OLDER = "extra.2.166.room_looks_older";
			public static string MOTHER_S_RAPACIOUSNESS = "extra.2.167.mother_s_rapaciousness";
			public static string DRAWER_BLOCKING_ME_FROM_SAFE = "extra.2.168.drawer_blocking_me_from_safe";
			public static string I_THINK_I_CAN_LEAVE_THE_CLOSET_NOW = "extra.2.169.i_think_i_can_leave_the_closet_now";
		}

		public class Section3
		{
			public static string FRESH_COAT_OF_PAINT = "3.003.fresh_coat_of_paint";
			public static string AM_I_LOCKED_IN = "3.004.am_i_locked_in";
			public static string BLANK_CANVAS_RARE_SIGHT = "3.008.blank_canvas_rare_sight";
			public static string FIND_SOME_PAINT = "3.008.find_some_paint";
			public static string COVER_PAINTS_DRY_OUT = "3.012.cover_paints_dry_out";
			public static string NEED_BRUSH = "3.014.need_brush";
			public static string PAINT_MY_HEART_OUT = "3.018.paint_my_heart_out";
			public static string REFLECTIVE_PAINT_CREATE_MIRROR = "3.022.reflective_paint_create_mirror";
			public static string FATHER_S_WORK_COLLECTING_DUST = "3.023.father_s_work_collecting_dust";
			public static string NOTES_WRITTEN_BY_FATEHR_IN_HERE = "3.032.notes_written_by_fatehr_in_here";
			public static string STRUGGLING_ARTIST = "3.035.struggling_artist";
			public static string ALICE_IN_WONDERLAND = "3.043.alice_in_wonderland";
			public static string SEEMS_STUCK = "3.044.seems_stuck";
			public static string PULL_OFF_SHEET = "3.053.pull_off_sheet";
			public static string LET_S_STAND_ON_THIS = "3.055.let_s_stand_on_this";
			public static string OOF = "3.058.oof";
			public static string THAT_DIDN_T_WORK = "3.059.that_didn_t_work";
			public static string CAN_STILL_SEE_COLOURS_BUT_BROKEN = "3.060.can_still_see_colours_but_broken";
			public static string DID_MOTHER_DO_THIS_ANGRY = "3.065.did_mother_do_this_angry";
			public static string MOTHER_CHANGED_THE_ORDER = "3.066.mother_changed_the_order";
			public static string BOOKSHELF_SEEMS_OFF = "3.070.bookshelf_seems_off";
			public static string FIND_SOMETHING_TO_PROP_ME_UP = "3.071.find_something_to_prop_me_up";
			public static string A_STOOL = "3.073.a_stool";
			public static string CANT_REACH_IRONIC = "3.074.cant_reach_ironic";
			public static string WHAT_ELSE_CAN_I_USE_AS_STOOL = "3.080.what_else_can_i_use_as_stool";
			public static string PAINT_ALL_DRIED_OUT = "3.083.paint_all_dried_out";
			public static string COULD_USE_THIS_AS_STOOL_TAKE_WITH_ME = "3.084.could_use_this_as_stool_take_with_me";
			public static string TOO_SMALL_PROP_ME_UP = "3.090.too_small_prop_me_up";
			public static string MISSING_A_BOOK = "3.094.missing_a_book";
			public static string NOTHING_HAPPENED_CORRECT_ORDER = "3.098.nothing_happened_correct_order";
			public static string THAT_DOESN_T_SEEM_RIGHT = "3.099.that_doesn_t_seem_right";
			public static string AHA_THAT_DID_THE_TRICK = "3.100.aha_that_did_the_trick";
			public static string SECRET_ROOM = "3.101.secret_room";
			public static string SO_DARK_IN_HERE = "3.105.so_dark_in_here";
			public static string FATHER_HAD_A_SAFE = "3.107.father_had_a_safe";
			public static string MISSING_A_BULB = "3.108.missing_a_bulb";
			public static string HE_ONLY_SHOWED_POSITIVE_SIDE = "3.112.he_only_showed_positive_side";
			public static string ANOTHER_PAINT_BRUSH = "3.113.another_paint_brush";
			public static string FATHER_TEACH_COLOUR_THEORY = "3.115.father_teach_colour_theory";
			public static string WORKING_WITH_COLOUR_FILTERS = "3.116.working_with_colour_filters";
			public static string I_BET_I_COULD_PAINT_ON_THESE_WITH_SOMETHING = "3.117.i_bet_i_could_paint_on_these_with_something";
			public static string DO_I_STILL_HAVE_BRUSH = "3.118.do_i_still_have_brush";
			public static string NOW_I_CAN_PAINT_THOSE_BLANK_CANVASES = "3.122.now_i_can_paint_those_blank_canvases";
			public static string OH_I_CAN_MOVE_THROUGH_THIS_ONE = "3.128.oh_i_can_move_through_this_one";
			public static string PAINTED_IN_BOTH_TIMELINES = "3.131.painted_in_both_timelines";
			public static string ANOTHER_NOTE_MISSED_IT = "3.132.another_note_missed_it";
			public static string IF_ONLY_THINGS_HAD_BEEN_DIFFERENT = "3.134.if_only_things_had_been_different";
			public static string LET_SGO_TURN_ON_THAT_LIGHT = "3.135.let_sgo_turn_on_that_light";
			public static string BROKEN_LIGHT_UNTOUCHED = "3.138.broken_light_untouched";
			public static string FAINT_LINES_HERE = "3.139.faint_lines_here";
			public static string HERE_ARE_THE_OTHER_FILMS = "3.140.here_are_the_other_films";
			public static string IF_I_HAD_A_PAINT_BRUSH = "3.141.if_i_had_a_paint_brush";
			public static string PAINT_OVER_THE_REFLECTIVE_PAINT = "3.150.paint_over_the_reflective_paint";
			public static string WOW_THAT_S_BRIGHT = "3.159.wow_that_s_bright";
			public static string DOESN_T_SEEM_RIGHT = "3.160.doesn_t_seem_right";
			public static string THAT_WORKED = "3.160.that_worked";
			public static string I_UNDERSTAND_FATHER = "3.162.i_understand_father";
			public static string CODE_WAS_IN_PAINTING = "3.163.code_was_in_painting";
			public static string A_CROWBAR = "3.166.a_crowbar";
			public static string FATHER_S_MUSIC_BOX_KEY = "3.166.father_s_music_box_key";
			public static string CAN_T_GO_THROUGH_MIRRORS_ANYMORE = "3.170.can_t_go_through_mirrors_anymore";
			public static string LET_S_USE_CROWBAR = "3.171.let_s_use_crowbar";
			public static string FINALLY_GET_OUT_OF_HERE = "3.175.finally_get_out_of_here";
			public static string AMAZING_BUT_INCOMPLETE = "3.186.amazing_but_incomplete";
			public static string IT_S_EMPTY = "3.187.it_s_empty";
			public static string SHOULDN_T_LET_LIGHT_IN = "3.188.shouldn_t_let_light_in";
			public static string STUCK_NEED_TOOK_TO_UNSEAL = "3.189.stuck_need_took_to_unseal";
			
			public static string NOTHING_HAPPENED = "extra.3.191.nothing_happened";
			public static string LETS_STAND_ON_THIS = "extra.3.194.lets_stand_on_this";
			//public static string BOOKSHELF_SEEMS_OFF = "extra.3.195.bookshelf_seems_off";
			public static string MEMENTO = "extra.3.198.memento.wav";
			public static string I_DONT_KNOW_THE_COMBINATION = "extra.3.199.i_dont_know_the_combination.wav";
			public static string LIGHT_GOES_THROUGH_ALL_FILTERS = "extra.3.200.light_goes_through_all_filters";
			public static string I_THINK_THAT_WORKED_CANVAS = "extra.3.201.i_think_that_worked_canvas";
			public static string CANT_GO_THROUGH_OTHER_SIDE_NOT_PAINTED = "extra.3.202.cant_go_through_other_side_not_painted";
			public static string CANVAS_ALREADY_BLANK = "extra.3.205.canvas_already_blank";
			public static string CANVAS_ALREADY_REFLECTIVE = "extra.3.206.canvas_already_reflective";
			public static string BRUSH_ALREADY_WET_WITH_PAINT = "extra.3.207.brush_already_wet_with_paint";
			public static string DONT_WANT_TO_TOUCH_THIS_ANYMORE = "extra.3.208.dont_want_to_touch_this_anymore";
			public static string PULL_OFF_SHEET_STANDING_ON_BUCKET = "extra.3.209.pull_off_sheet_standing_on_bucket";
			public static string STILL_TOO_HIGH_USE_BUCKET = "extra.3.213.still_too_high_use_bucket";
			public static string TOO_SMALL_TO_GO_THROUGH_ONLY_LOOK = "extra.3.214.too_small_to_go_through_only_look";
			public static string MOTHER_MOVED_BUSTS_AROUND_IN_PRESENT = "extra.3.215.mother_moved_busts_around_in_present";
		}

		public class Section4
		{
			public static string THATS_ONE = "thats_one";

			public static string ALL_THREE_KEYS = "4.004.all_three_keys";
			public static string HAVE_MY_KEY_NOW = "extra.4.035.have_my_key_now";
			public static string FOUND_MOTHERS_KEY_LOOK_FOR_FATHERS = "extra.4.037.found_mothers_key_look_for_fathers";
			public static string WAIT_FOR_ALL_THREE_KEYS = "extra.4.038.wait_for_all_three_keys";
			public static string OBTAINED_FATHERS_KEY_MOTHER_KEY_LEFT = "extra.4.039.obtained_fathers_key_mother_key_left";
			public static string THATS_TWO = "extra.4.041.thats_two";
			public static string THATS_THREE = "extra.4.042.thats_three";
			public static string IT_OPENED = "extra.4.044.it_opened";
			public static string GO_BACK_TO_MUSIC_BOX = "extra.4.045.go_back_to_music_box";
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
	REFLECT_KEY = KeyCode.R,
	DROP_KEY = KeyCode.G,
	INVENTORY_KEY = KeyCode.LeftShift,
	ESCAPE_KEY = KeyCode.Escape,
	TAB_KEY = KeyCode.Tab,

	// This is for debug only, should be removed in release
	DEBUG_TRIGGER = KeyCode.Alpha0
}

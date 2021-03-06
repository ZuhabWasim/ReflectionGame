using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitObjectives
{

	public static void InitAllObjectives( List<Objective> m_objectives )
	{
		m_objectives.Add( InitObjectives.InitSection1Objectives() );
		m_objectives.Add( InitObjectives.InitSection11Objectives() );
		m_objectives.Add( InitObjectives.InitSection12Objectives() );

		m_objectives.Add( InitObjectives.InitSection2Objectives() );
		m_objectives.Add( InitObjectives.InitSection21Objectives() );

		m_objectives.Add(InitObjectives.InitSection3Objectives());
		m_objectives.Add(InitObjectives.InitSection31Objectives());

		m_objectives.Add( InitObjectives.InitSection22Objectives() );
		
		m_objectives.Add( InitObjectives.InitSection4Objectives() );
	}
	static Objective InitSection1Objectives()
	{
		Objective ob_main = new Objective( Globals.UIStrings.OBJECTIVE_TURN_ON_POWER, completeEvent: Globals.Events.LIGHTS_TURN_ON, isActive: true );
		ob_main.AddSubObjective( new Objective( Globals.UIStrings.SUB_OBJ_INVESTIGATE_ROOM, completeEvent: Globals.Events.DONE_INVESTIGATE, isActive: true ) );
		ob_main.AddSubObjective( new Objective( Globals.UIStrings.SUB_OBJ_FIND_BOX_CODE, activateEv: Globals.Events.GO_ENTER_CODE, completeEvent: Globals.Events.LIGHTS_TURN_ON ) );
		ob_main.AddSubObjective( new Objective( Globals.UIStrings.SUB_OBJ_FIND_CLEAN_MIRROR, activateEv: Globals.Events.GO_CLEAN_MIRROR, completeEvent: Globals.Events.DONE_CLEAN_MIRROR ) );
		return ob_main;
	}

	static Objective InitSection11Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_EXPLORE, activateEv: Globals.Events.LIGHTS_TURN_ON, completeEvent: Globals.Events.FIRST_TELEPORT);
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_MUSIC_KEY1, activateEv: Globals.Events.GO_CHECK_MUSIC_BOX, completeEvent: Globals.Events.MILLIE_KEY_INTERACT));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_CHECK_MIRROR, activateEv: Globals.Events.MILLIE_KEY_INTERACT, completeEvent: Globals.Events.FIRST_TELEPORT));
		return ob_main;
	}

	static Objective InitSection12Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_FIND_TWO_KEYS, activateEv: Globals.Events.FIRST_TELEPORT, completeEvent: Globals.Events.LOCK_EITHER_DOOR);
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_INVESTIGATE_MOM_CLOSET, completeEvent: Globals.Events.LOCK_EITHER_DOOR, isActive: true));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_INVESTIGATE_DAD_CLOSET, completeEvent: Globals.Events.LOCK_EITHER_DOOR, isActive: true));
		return ob_main;
	}

	static Objective InitSection2Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_FIND_MOMS_KEY, activateEv: Globals.Events.LOCK_MOM_DOOR, completeEvent: Globals.Events.HAS_MOM_KEY);
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_GET_TO_SECOND, activateEv: Globals.Events.LOCK_MOM_DOOR, completeEvent: Globals.Events.MOM_AT_SECOND_PART));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_FIND_SCISSORS, activateEv: Globals.Events.NEED_SCISSORS, completeEvent: Globals.Events.CUTTING_CLOTHING_RACK));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_FIND_BROOM, activateEv: Globals.Events.NEED_BROOM, completeEvent: Globals.Events.SWEPT_DEBRIS));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_MOM_USE_WHEEL, activateEv: Globals.Events.NEED_WHEEL, completeEvent: Globals.Events.USED_WHEEL));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_USE_SAFE, activateEv: Globals.Events.MOM_AT_SECOND_PART, completeEvent: Globals.Events.HAS_MOM_KEY));
		return ob_main;
	}

	static Objective InitSection21Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_LEAVE_MOM, activateEv: Globals.Events.HAS_MOM_KEY, completeEvent: Globals.Events.EXIT_MOM);
		return ob_main;
	}

	static Objective InitSection3Objectives()
	{
		//TODO replace active & complete events
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_FIND_DADS_KEY, activateEv: Globals.Events.LOCK_DAD_DOOR, completeEvent: Globals.Events.HAS_DAD_KEY);
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_BRUSH, activateEv: Globals.Events.NEED_BRUSH1, completeEvent: Globals.Events.BRUSH_WET_REFL));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_PAINT, activateEv: Globals.Events.NEED_PAINT_CANVAS, completeEvent: Globals.Events.CANVAS_STATE_CHANGE));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_FIND_STEP, activateEv: Globals.Events.NEED_STEP, completeEvent: Globals.Events.BOOKSHELF_BOOK_PLACED));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_FIND_BOOK_CODE, activateEv: Globals.Events.NEED_BOOK_CODE, completeEvent: Globals.Events.DAD_PUZZLE_1_BOOKSHELF_SOLVED));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_EXPLORE2, activateEv: Globals.Events.DAD_PUZZLE_1_BOOKSHELF_SOLVED, completeEvent: Globals.Events.DONE_DAD_EXPLORE));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_BRUSH2, activateEv: Globals.Events.NEED_BRUSH2, completeEvent: Globals.Events.BRUSH_WET_WHITE));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_TURN_ON_PROJECTOR, activateEv: Globals.Events.NEED_BULB, completeEvent: Globals.Events.DAD_PUZZLE_2_SPOTLIGHT_INSTALLED));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_USE_SAFE2, activateEv: Globals.Events.NEED_DAD_SAFE_CODE, completeEvent: Globals.Events.HAS_DAD_KEY));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_SOLVE_PUZZLE, activateEv: Globals.Events.SOLVE_DAD_PUZZLE, completeEvent: Globals.Events.DAD_PUZZLE_2_LIGHTPUZZLE_SOLVED));
		return ob_main;
	}

	static Objective InitSection31Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_LEAVE_DAD, activateEv: Globals.Events.HAS_DAD_KEY, completeEvent: Globals.Events.EXIT_DAD);
		return ob_main;
	}

	static Objective InitSection22Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_FIND_ONE_KEY, activateEv: Globals.Events.PICKUP_EITHER_KEY, completeEvent: Globals.Events.EXIT_BOTH);
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_INVESTIGATE_MOM_CLOSET, completeEvent: Globals.Events.LOCK_MOM_DOOR, isActive: true));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_INVESTIGATE_DAD_CLOSET, completeEvent: Globals.Events.LOCK_DAD_DOOR, isActive: true));
		return ob_main;
	}

	
	static Objective InitSection4Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_USE_KEYS, activateEv: Globals.Events.EXIT_BOTH);
		return ob_main;
	}


}

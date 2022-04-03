using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InitObjectives
{

	public static void InitAllObjectives(List<Objective> m_objectives)
	{
		m_objectives.Add(InitObjectives.InitSection1Objectives());
	}
	static Objective InitSection1Objectives()
	{
		Objective ob_main = new Objective(Globals.UIStrings.OBJECTIVE_TURN_ON_POWER, completeEvent: Globals.Events.LIGHTS_TURN_ON, isActive: true);
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_INVESTIGATE_ROOM, completeEvent: Globals.Events.DONE_INVESTIGATE, isActive: true));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_FIND_BOX_CODE, activateEv: Globals.Events.GO_ENTER_CODE, completeEvent: Globals.Events.LIGHTS_TURN_ON));
		ob_main.AddSubObjective(new Objective(Globals.UIStrings.SUB_OBJ_FIND_CLEAN_MIRROR, activateEv: Globals.Events.GO_CLEAN_MIRROR, completeEvent: Globals.Events.DONE_CLEAN_MIRROR));
		return ob_main;
	}

	static Objective InitSection2Objectives()
	{
		return null;
	}

	static Objective InitSection3Objectives()
	{
		return null;
	}

	static Objective InitSection4Objectives()
	{
		return null;
	}


}

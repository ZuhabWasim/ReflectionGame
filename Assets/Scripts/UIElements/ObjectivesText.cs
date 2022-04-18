using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectivesText : MonoBehaviour
{

    public Text CurrObjective;
    public Text SubObjectiveList;

    void Update()
    {
        RefreshText();
    }

    void RefreshText()
    {
        Objective ob = Objectives.GetCurrentObjective();
        if (ob != null)
        {
            CurrObjective.text = "-" + ob.description;
            SubObjectiveList.text = "";
            int activeCount = 0;
            for (int i = 0; i < ob.subObjectives.Count && activeCount < 3; i++)
            {
                Objective sob = ob.subObjectives[i];
                if (sob.isActive)
                {
                    activeCount++;
                    SubObjectiveList.text += "-" + sob.description + "\n";
                }
            }
        } else
        {
            CurrObjective.text = "";
            SubObjectiveList.text = "";
        }
    }
}

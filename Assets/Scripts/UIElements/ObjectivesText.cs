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
            foreach (Objective sob in ob.subObjectives)
            {
                if (sob.isActive)
                {
                    SubObjectiveList.text += "-" + sob.description + "\n";
                }
            }
        } else
        {
            CurrObjective.text = "ERROR";
            SubObjectiveList.text = "no active objective";
        }
    }
}

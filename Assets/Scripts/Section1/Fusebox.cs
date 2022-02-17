using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fusebox : MonoBehaviour
{

    public bool[] switchPositions;
    public bool[] answer;

    public Material light_on;
    public Material light_off;
    public GameObject[] lights;

    // Start is called before the first frame update
    void Start()
    {

    }

    void CheckInput()
    {
        //TODO
        //Check if code input matches solution
    }

    void UpdateLight(int n)
    {
        if (switchPositions[n])
        {
            //set light to on
            lights[n].GetComponent<Renderer>().material = light_on;
        } else
        {
            //set light to off
            lights[n].GetComponent<Renderer>().material = light_off;
        }
    }

    public void switchLight(int n)
    {
        switchPositions[n] = !switchPositions[n];
        UpdateLight(n);
    }
}

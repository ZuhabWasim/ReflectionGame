using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DrawerPassageD : InteractableAbstract
{
    public GameObject drawerOne;
    public GameObject drawerTwo;
    public GameObject rampObject;
    private const string PASSAGE_D_SOURCE = "PassageDAudioSource";
    protected override void OnStart()
    {
        desiredItem = Globals.UIStrings.DRAWER_PULL_ITEM;
        AudioPlayer.RegisterAudioPlayer( PASSAGE_D_SOURCE, GetComponent<AudioSource>() );
    }

    protected override void OnUseItem()
    {
        drawerOne.GetComponent<InterpolateTransform>().TriggerMotion();
        drawerTwo.GetComponent<InterpolateTransform>().TriggerMotion();
        rampObject.SetActive(true);
        AudioPlayer.Play(Globals.AudioFiles.General.OPEN_DRAWER, PASSAGE_D_SOURCE);
    }
}

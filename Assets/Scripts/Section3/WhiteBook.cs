using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBook : PickupItem
{
    public InteractNote hiddenNote;
    public AudioClip onFirstPickupVoiceLine;
    public AudioClip onPuzzleSolvedVoiceLine;
    private bool puzzleSolved;
    private bool onFirstPickup;
    
    void Start()
    {
        base.OnStart();
        EventManager.Sub(Globals.Events.DAD_PUZZLE_1_BOOKSHELF_SOLVED, () => { puzzleSolved = true; });
        puzzleSolved = false;
        onFirstPickup = true;
    }

    protected override void OnUserInteract()
    {
        this.gameObject.SetActive( false );
        
        if ( onFirstPickup )
        {
            AudioPlayer.Play(onFirstPickupVoiceLine, Globals.Tags.DIALOGUE_SOURCE);
            // call the hidden note to be picked up.
            onFirstPickup = false;
        } else if ( puzzleSolved )
        {
            AudioPlayer.Play(onPuzzleSolvedVoiceLine, Globals.Tags.DIALOGUE_SOURCE);
        }
        EventManager.Fire(Globals.Events.BOOKSHELF_BOOK_PICKED_UP, this.gameObject);
    }
}

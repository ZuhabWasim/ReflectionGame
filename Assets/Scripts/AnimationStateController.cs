using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Input = UnityEngine.Windows.Input;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;

   private int isWalkingHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("IsWalking");
    }

    // Update is called once per frame
    void Update()
    {

        bool isWalking = animator.GetBool(isWalkingHash);
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        bool forwardPress = input.magnitude >= 0.1;
        
        //Debug.Log("isWalking: " + isWalking + "," + "forwardPress: " + forwardPress + ", " + "(!isWalking && forwardPress): " + (!isWalking && forwardPress));
        if (!isWalking && forwardPress)
        {
            animator.SetBool(isWalkingHash, true);
        }
        
        if (isWalking && !forwardPress)
        {
            animator.SetBool(isWalkingHash, false);
        }
    }
}

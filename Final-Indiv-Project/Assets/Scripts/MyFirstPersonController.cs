using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstPersonController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    public RuntimeAnimatorController animatorControllerIdle;
    public RuntimeAnimatorController animatorControllerWalk;
    public RuntimeAnimatorController animatorControllerRun;
    public RuntimeAnimatorController animatorControllerJump;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)){
            animator.runtimeAnimatorController = animatorControllerWalk;
        }
        else if ( Input.GetKey(KeyCode.LeftShift) ){
            animator.runtimeAnimatorController = animatorControllerRun;
        }
        else if ( Input.GetKey(KeyCode.Space) ){
            animator.runtimeAnimatorController = animatorControllerJump;
        }
        else {
            animator.runtimeAnimatorController = animatorControllerIdle;
        }

        

    }
}

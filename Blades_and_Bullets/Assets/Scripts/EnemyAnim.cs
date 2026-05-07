using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EnemyAnim : MonoBehaviour
{
    //private const string IS_WALKING_LEFT = "isMovingLeft";
    //private const string IS_WALKING_RIGHT = "isMovingRight";
    //private const string IS_WALKING_UP = "isMovingUp";
    //private const string IS_WALKING_DOWN = "isMovingDown";
    public Animator animator;
    int m_BounceStateHash;
    int IS_WALKING_RIGHT;
    int IS_WALKING_LEFT;
    int IS_WALKING_UP;
    int IS_WALKING_DOWN;
    void Start()
    {
        animator = GetComponent<Animator>();
        //m_BounceStateHash = Animator.StringToHash("Base Layer.Bounce");
        IS_WALKING_RIGHT = Animator.StringToHash("isMovingRight");
        IS_WALKING_LEFT = Animator.StringToHash("isMovingLeft");
        IS_WALKING_UP = Animator.StringToHash("isMovingUp");
        IS_WALKING_DOWN = Animator.StringToHash("isMovingDown");
    }
    void Update()
    {
        if (Keyboard.current.rightArrowKey.isPressed)
        {
            animator.SetBool(IS_WALKING_RIGHT, true);
        }
        else
        {
            animator.SetBool(IS_WALKING_RIGHT, false);
        }

        if (Keyboard.current.leftArrowKey.isPressed)
        {
            animator.SetBool(IS_WALKING_LEFT, true);
        }
        else
        {
            animator.SetBool(IS_WALKING_LEFT, false);
        }

        if (Keyboard.current.upArrowKey.isPressed)
        {
            animator.SetBool(IS_WALKING_UP, true);
        }
        else
        {
            animator.SetBool(IS_WALKING_UP, false);
        }

        if (Keyboard.current.downArrowKey.isPressed)
        {
            animator.SetBool(IS_WALKING_DOWN, true);
        }
        else
        {
            animator.SetBool(IS_WALKING_DOWN, false);
        }
    }

}

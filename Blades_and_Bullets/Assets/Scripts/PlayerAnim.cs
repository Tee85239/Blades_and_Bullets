using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System;

public class PlayerAnim : MonoBehaviour
{
    private const string IS_WALKING_LEFT = "isMovingLeft";
    private const string IS_WALKING_RIGHT = "isMovingRight";
    private const string IS_UNFOCUSED = "isAttackUnfocused"; // trigger version
    private const string IS_UNFOCUSED1 = "isAttackUnfocus";
    private const string IS_FOCUSED = "isAttackFocused";
    private const string IS_SPECIAL = "isSpecial";
    private const string IS_SPECIALABILITY = "isSpecialAbility";
    private const string IS_KILLED = "isKilled";
    public Animator animator;
    private bool specialSlashActive;

    void Start()
    {
        GameControllerScript.AbilityActiveStatus += AbilityActiveStatus;

    }

    private void AbilityActiveStatus(object sender, EventArgs e)
    {
        specialSlashActive = true;
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

        if ((Keyboard.current.spaceKey.isPressed || Keyboard.current.zKey.isPressed || Keyboard.current.slashKey.isPressed) && Keyboard.current.shiftKey.isPressed)
        {
            animator.SetBool(IS_FOCUSED, true);
        }
        else
        {
            animator.SetBool(IS_FOCUSED, false);
        }

        if ((Keyboard.current.spaceKey.isPressed || Keyboard.current.zKey.isPressed || Keyboard.current.slashKey.isPressed) && !Keyboard.current.shiftKey.isPressed)
        {
            animator.SetBool(IS_UNFOCUSED1, true);
        }
        else
        {
            animator.SetBool(IS_UNFOCUSED1, false);
        }

        if (Keyboard.current.cKey.wasPressedThisFrame || Keyboard.current.commaKey.wasPressedThisFrame )
        {
            animator.SetBool(IS_SPECIAL, true);
            specialSlashActive = true;
        }
        else
        {
            animator.SetBool(IS_SPECIAL, false);
        }

        // if (Keyboard.current.zKey.wasPressedThisFrame && specialSlashActive)
        // {
        //     animator.SetBool(IS_SPECIALABILITY, true);
        //     specialSlashActive = false;
        // }
        // else
        // {
        //     animator.SetBool(IS_SPECIALABILITY, false);
        // }
        if (Player.Instance.moveState == Player.MoveState.Death)
        {
            animator.SetBool(IS_KILLED, true);
            //transform.position = new Vector3(-3f, -4f, transform.position.z);
        } else
        {
            animator.SetBool(IS_KILLED, false);
        }
    }

    IEnumerator RespawnPoint()
    {
        Debug.Log("Waiting starts now");
        yield return new WaitForSeconds(3f);  // Pause for 2 seconds
        Debug.Log("2 seconds have passed");
    }

}

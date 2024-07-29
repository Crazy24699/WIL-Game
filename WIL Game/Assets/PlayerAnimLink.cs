using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimLink : MonoBehaviour
{

    private Animator PlayerAnimation;

    //Change this to a custom start
    private void Start()
    {
        PlayerAnimation = GetComponent<Animator>();
    }

    [SerializeField]
    private void LockAttack()
    {
        PlayerAnimation.SetBool("IsAttacking", true);
    }

    [SerializeField]
    private void UnlockAttack()
    {
        PlayerAnimation.SetBool("IsAttacking", false);
    }

}

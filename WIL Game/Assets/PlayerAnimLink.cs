using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimLink : MonoBehaviour
{


    private PlayerAttacks PlayerAttackScript;

    //Change this to a custom start
    private void Start()
    {
        PlayerAttackScript = FindAnyObjectByType<PlayerAttacks>();
    }

    [SerializeField]
    private void LockAttack()
    {
        
    }

    [SerializeField]
    private void UnlockAttack()
    {

    }

}

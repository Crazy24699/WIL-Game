using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimLink : MonoBehaviour
{

    private Animator PlayerAnimation;
    private PlayerAttacks PlayerAttackScript;

    //Change this to a custom start
    private void Start()
    {
        PlayerAnimation = GetComponent<Animator>();
        PlayerAttackScript = FindObjectOfType<PlayerAttacks>();
    }


}

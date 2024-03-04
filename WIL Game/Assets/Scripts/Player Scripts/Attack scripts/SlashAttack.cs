using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class SlashAttack : PlayerAttacks
{
    public GameObject WolfRef;
    public GameObject ClawRef;

    public void Attack()
    {
        AttackAnimation.SetBool("PlaySlash", true);
        StartCoroutine(AttackReset());
    }

    public IEnumerator AttackReset()
    {
        yield return new WaitForSeconds(0.25f);
        AttackAnimation.SetBool("PlaySlash", false);
    }

}

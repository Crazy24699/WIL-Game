using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebbinAnimLink : MonoBehaviour
{
    private WebbinEnemy WebbinScript;

    // Start is called before the first frame update
    void Start()
    {
        WebbinScript = transform.parent.root.GetComponent<WebbinEnemy>();
    }


    private void StartAttackCooldown()
    {
        WebbinScript.StartCoroutine(WebbinScript.WebAttack.AttackCooldown());
    }

    private void StartWebBarrage()
    {
        StartCoroutine(WebbinScript.WebAttack.SpitBurst());
        WebbinScript.HandleMovingState(false);
    }

    private void StartActionDelay()
    {

    }

}

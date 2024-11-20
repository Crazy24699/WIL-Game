using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebbinAnimLink : MonoBehaviour
{
    private WebbinEnemy WebbinScript;

    private bool RollAttacking = false;
    private bool ScuttleMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        WebbinScript = transform.parent.root.GetComponent<WebbinEnemy>();
    }


    private void StartScuttle()
    {
        if(ScuttleMoving) { return; }

        ScuttleMoving = true;
        WebbinScript.BossSoundManagerScript.PlaySound(BossSoundManager.SoundOptions.Moving);
    }

    private void EndScuttle()
    {
        ScuttleMoving = false;
        WebbinScript.BossSoundManagerScript.PlaySound(BossSoundManager.SoundOptions.Silence);
    }

    private void StartAttackCooldown()
    {
        WebbinScript.StartCoroutine(WebbinScript.WebAttack.AttackCooldown());
        Debug.Log("runs");
        StartCoroutine(WebbinScript.AttackCooldown());
    }

    private void StartWebBarrage()
    {
        StartCoroutine(WebbinScript.WebAttack.SpitBurst());
        WebbinScript.HandleMovingState(false);
        Debug.Log("Web barrage");
    }

    [SerializeField]
    private void StartRollAttack()
    {
        if(RollAttacking) { return; }
        RollAttacking = true;
        WebbinScript.BossSoundManagerScript.PlaySound(BossSoundManager.SoundOptions.Attack1);
    }

    [SerializeField] 
    private void EndRollAttack()
    {
        RollAttacking = false;
        //WebbinScript.BossSoundManagerScript.PlaySound(BossSoundManager.SoundOptions.Silence);
    }

}

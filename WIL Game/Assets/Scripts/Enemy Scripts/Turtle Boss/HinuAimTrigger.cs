using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HinuAimTrigger : MonoBehaviour
{

    private bool HitBoxEngaged = false;

    [SerializeField] private Collider PunchHitBox;
    [SerializeField] private TurtleBossAI TurtleAttackScript;

    // Start is called before the first frame update
    void Start()
    {
        if(PunchHitBox == null)
        {
            Debug.LogError("Punch box not set");
        }

        TurtleAttackScript = FindObjectOfType<TurtleBossAI>();
    }

    #region BashAttack
    [SerializeField]
    private void EndBashAttack()
    {
        Debug.Log("Run when");

        HitBoxEngaged = false;

        
        PunchHitBox.enabled = HitBoxEngaged;
        TurtleAttackScript.ChangeLockState();
    }

    [SerializeField]
    private void StartBashAttack()
    {
        HitBoxEngaged = true;
        StartCoroutine(TurtleAttackScript.BucketAttackClass.AttackCooldown());
        TurtleAttackScript.ChangeLockState();
    }
    #endregion


    #region Bubble Attack
    [SerializeField]
    private void StartBubbleAttack()
    {
        TurtleAttackScript.BubbleAttackClass.SpewBubbles = true;
        TurtleAttackScript.ChangeLockState();
    }
    #endregion

    [SerializeField]
    private void EndBubbleAttack()
    {
        TurtleAttackScript.BubbleAttackClass.SpewBubbles = false;
        TurtleAttackScript.ChangeLockState();
    }

    [SerializeField]
    private void PlayWalkSound()
    {

    }
}

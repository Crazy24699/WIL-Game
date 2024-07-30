using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HinuAimTrigger : MonoBehaviour
{

    private bool BubbleAttackEngaged = false;
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

    [SerializeField]
    private void TriggerBashAttack()
    {
        Debug.Log("Run when");
        switch (HitBoxEngaged)
        {
            case true:
                HitBoxEngaged = false;
                StartCoroutine(TurtleAttackScript.BucketAttackClass.AttackCooldown());

                break;

            case false:
                HitBoxEngaged = true;
                break;
        }
        
        PunchHitBox.enabled = HitBoxEngaged;
        TurtleAttackScript.ChangeLockState();
    }


    [SerializeField]
    private void TriggerBubbleAttack()
    {
        switch (BubbleAttackEngaged)
        {
            case true:
                BubbleAttackEngaged = false;
                break;

            case false:
                BubbleAttackEngaged = true;
                break;
        }
        TurtleAttackScript.BubbleAttackClass.SpewBubbles = BubbleAttackEngaged;
    }

}

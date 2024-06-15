using UnityEngine;

public class BTTurtleAttack : BTNodeBase
{
    private TurtleBossAI BossScript;
    private GameObject BossObjectRef;



    public BTTurtleAttack(GameObject EnemyAIRef)
    {
        BossScript = EnemyAIRef.GetComponent<TurtleBossAI>();
        BossObjectRef = EnemyAIRef;
    }

    public override NodeStateOptions RunLogicAndState()
    {
        //Debug.Log("Nullberries");
        if (BossScript.CanPerformAction && BossScript.Distance <= 70 && !BossScript.PerformingAttack) 
        {
            SetAttack();
            Debug.Log("Nullberries");
            return NodeStateOptions.Passed;
        }

        return NodeStateOptions.Failed;
    }

    private void SetAttack()
    {
        float PlayerDistance = Vector3.Distance(BossScript.PlayerRef.transform.position, BossObjectRef.transform.position);
        BossScript.MoveToPlayer = false;
        switch (PlayerDistance)
        {
            case <= 10.0f:

                if (!BossScript.BucketAttackClass.AttackCooldownActive)
                {
                    BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BucketBasher;
                }
                else if (BossScript.BucketAttackClass.AttackCooldownActive)
                {
                    BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BubbleBlast;
                }

                break;

            case > 10.0f:

                if (!BossScript.BubbleAttackClass.AttackCooldownActive)
                {
                    BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BubbleBlast;
                }
                else if (BossScript.BubbleAttackClass.AttackCooldownActive)
                {
                    BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BucketBasher;
                }

                break;
        }

        BossScript.PerformingAttack = true;
    }

}

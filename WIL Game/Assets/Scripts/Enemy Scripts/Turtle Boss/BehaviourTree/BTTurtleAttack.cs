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
        if (BossScript.CanPerformAction && BossScript.DistanceToPlayer <= 100) 
        {
            SetAttack();
            Debug.Log("Nullberries");
            return NodeStateOptions.Passed;
        }

        return NodeStateOptions.Failed;
    }

    private void SetAttack()
    {
        float PlayerDistance = BossScript.DistanceToPlayer;
        
        switch (PlayerDistance)
        {
            case <= 10.0f:

                
                if (BossScript.BucketAttackClass.AttackCooldownActive)
                {
                    BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BubbleBlast;
                    return;

                }
                BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BucketBasher;


                break;

            case > 10.0f:

                if (BossScript.BubbleAttackClass.AttackCooldownActive)
                {
                    BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BucketBasher;
                    return;
                }
                BossScript.ChosenAttack = TurtleBossAI.TurtleAttacks.BubbleBlast;

                break;
        }
        BossScript.PerformingAttack = true;

    }

}

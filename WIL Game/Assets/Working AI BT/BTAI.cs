using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTAI : BaseEnemy
{

    public ShortAttack SAttack;
    public LongAttack LAttack;

    public bool CanAttack;
    public bool Moving;

    public bool AttacksDown;
    public bool LongAttackVar;

    public bool CanPerformAction;
    public bool PerformingAttack;

    public bool ShortAttackVar;


    public float LockoutTime;

    public float Distance;

    // Start is called before the first frame update
    void Start()
    {
        CreateBehaviourTree();
        LockoutTime = 2;
    }

    // Update is called once per frame
    void Update()
    {

        if (!PerformingAttack)
        {
            if (Distance > 10)
            {
                LongAttackVar = true;
                ShortAttackVar = false;
                LockoutTime = LAttack.ActionLockout;

            }
            if (Distance < 10)
            {
                ShortAttackVar = true;
                LockoutTime = SAttack.ActionLockout;
                LongAttackVar = false;
            }
        }

        if (CanPerformAction)
        {
            StartCoroutine(ActionLockoutTimer());
            PerformingAttack = true;
            return;
            
        }

        if (PerformingAttack)
        {
            PerformAction();
            BehaviourTreeUpdate();
        }
        
    }



    private void CreateBehaviourTree()
    {

        //Attack1 Attack1Node = new Attack1(gameObject);
        //Attack2 MoveNode = new Attack2(gameObject);

        //BTNodeSequence Attack1Seq = new BTNodeSequence();
        //BTNodeSequence MoveSeq = new BTNodeSequence();

        //Attack1Seq.SetSequenceValues(new List<BTNodeBase> { Attack1Node });
        //MoveSeq.SetSequenceValues(new List<BTNodeBase> { MoveNode });


        //RootNode=new BTNodeSelector(new List<BTNodeBase> { Attack1Seq,MoveSeq});
    }

    public void BehaviourTreeUpdate()
    {
        //RootNode.RunLogicAndState();
        return;
    }

    public void PerformAction()
    {
        
        if (LockoutTime > 0)
        {
            LockoutTime -= Time.deltaTime;
            if(LockoutTime <= 0)
            {
                PerformingAttack = false;
                LockoutTime = 0;
                return;
            }
        }
    }

    public IEnumerator ActionLockoutTimer()
    {
        CanPerformAction = false;
        yield return new WaitForSeconds(LockoutTime);
        CanPerformAction = true;
    }

    [System.Serializable]
    public class ShortAttack
    {
        public bool CooldownActive;

        public float ActionLockout;

        public IEnumerator HandleCooldown()
        {
            yield return new WaitForSeconds(1.5f);
        }
    }


    [System.Serializable]
    public class LongAttack
    {
        public bool CooldownActive;

        public float ActionLockout;

        public IEnumerator HandleCooldown()
        {
            yield return new WaitForSeconds(5.5f);
        }
    }

}

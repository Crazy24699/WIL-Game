using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WebbinEnemy : BossBase
{
    [Space(2), Header("Bools")]
    public bool PerformingAttack;
    public bool CustomLocationChosen = false;
    public bool CanMove=true;
    public bool ActionAvaliable = true;
    public bool EngagePlayer = false;
    public bool ActionDelayActive;
    public bool AttackChosen;
    public bool CanAttack = true;

    [Space(5), Header("Floats")]
    public float CurrentPlayerDistance;
    public float StoppingDistance;
    [SerializeField] private float ActionLockoutTime;
    [SerializeField] private float AttackLockoutTime;
    [Space(2)]
    public float BashAttackRange;
    public float WebSpitAttack;
    [SerializeField] private float MaxAttackDistance;
    public float AttackWaitTime;

    [Space(5)]
    public int CurrentContactDamage;


    //Script area 
    public enum AttackOptions
    {
        WebSpit,
        RollBash
    };

    [Space(5)]

    public AttackOptions ChosenAttack;
    public WebSpit WebAttack;



    private void Start()
    {
        BossStartup();
    }

    public override void BossStartup()
    {

        PlayerRef = GameObject.FindObjectOfType<PlayerInteraction>().gameObject;
        NavMeshRef = GetComponent<NavMeshAgent>();
        MaxHealth = 8;
        CurrentHealth = MaxHealth;
        CreateBehaviourTree();
    }

    private void CreateBehaviourTree()
    {
        //BTWebChoice ChoiceNode = new BTWebChoice(gameObject);
        BTWebMove MoveNode = new BTWebMove(gameObject);
        BTWebAttack AttackNode = new BTWebAttack(gameObject);

        BTNodeSequence AttackSequence = new BTNodeSequence();
        AttackSequence.SetSequenceValues(new List<BTNodeBase> {AttackNode });

        BTNodeSequence MoveSequence = new BTNodeSequence();
        MoveSequence.SetSequenceValues(new List<BTNodeBase> { MoveNode });

        RootNode = new BTNodeSelector(new List<BTNodeBase>() { MoveSequence, AttackSequence });

    }

    private void FixedUpdate()
    {
        if (!Alive) { return; }

        RootNode.RunLogicAndState();

    }
    private void UpdateDistance()
    {
        CurrentPlayerDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
    }
    public IEnumerator ActionDelay()
    {
        yield return new WaitForSeconds(2.52f);
    }

    public void HandleEnemySpeed(bool OutOfRange)
    {
        switch (OutOfRange)
        {
            case true:
                NavMeshRef.speed = NavMeshRef.speed * 2;
                break;

            case false:
                NavMeshRef.speed = NavMeshRef.speed / 2;
                break;
        }
    }

    public void PerformSelectedAttack()
    {

        PerformingAttack = true;
    }


    public bool BeyondMaxRange()
    {
        return !(CurrentPlayerDistance < MaxAttackDistance);
    }

    private void Update()
    {
        UpdateDistance();
    }

    public class WebSpit
    {
        public bool AttackCoodldownActive = false;

        public float ActionDelayTime;
        public void AttackPerform()
        {
            
        }
    }

    public class RollBash
    {
        public bool AttackCoodldownActive = false;

        public float ActionDelayTime;
        public void AttackPerform()
        {

        }

        

    }

}

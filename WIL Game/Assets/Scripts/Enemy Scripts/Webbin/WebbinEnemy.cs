using Cinemachine.Utility;
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
    [SerializeField] private float LockoutTimer;

    [Space(2)]
    public float BashAttackRange;
    public float WebSpitRange;
    [SerializeField] public float MaxAttackDistance;
    public float AttackWaitTime;
    public float BaseMoveSpeed;
    public float CurrentAttackDistance;

    [Space(5), Header("Integers")]
    public int CurrentContactDamage;

    [Space(5), Header("Scripts")]
    public WebSpit WebAttack;
    public RollBash BashAttack;



    //Script area 
    public enum AttackOptions
    {
        WebSpit,
        RollBash
    };

    [Space(5)]

    public AttackOptions ChosenAttack;

    private void SetMoveState(bool MoveState)
    {
        CanMove = MoveState;
        switch (MoveState)
        {
            case true:
                NavMeshRef.speed = BaseMoveSpeed;
                NavMeshRef.isStopped = false;
                break;

            case false:
                NavMeshRef.enabled = true;
                NavMeshRef.speed = 0;
                NavMeshRef.ResetPath();
                NavMeshRef.isStopped = true;
                //StartCoroutine(MoveDelay());
                break;
        }
    }

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
        BaseMoveSpeed = NavMeshRef.speed;
        LockoutTimer = ActionLockoutTime;

        WebAttack.AttackStartup(this);
        BashAttack.AttackStartup(this);
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

        if (ActionAvaliable)
        {
            RootNode.RunLogicAndState();
        }


    }

    private bool CheckRollAttack()
    {
        Vector3 CurrentPosition = transform.position.RoundVector(2);
        Vector3 EndPosition = BashAttack.EndingPosition.RoundVector(2);

        return CurrentPosition == EndPosition;
    }

    private void HandleLockoutTime()
    {
        if (PerformingAttack) { return; }
        if (!ActionAvaliable)
        {
            if (LockoutTimer <= 0)
            {
                Debug.Log("action true      ;");
                ActionAvaliable = true;
                LockoutTimer = ActionLockoutTime;
                AttackChosen = false;
                return;
            }
            if (LockoutTimer > 0)
            {
                LockoutTimer -= Time.deltaTime;
            }
        }
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

    public void HandleMovingState(bool StopMoving)
    {
        switch (StopMoving)
        {
            case true:
                NavMeshRef.isStopped = false;
                NavMeshRef.speed = BaseMoveSpeed;
                break;

            case false:
                NavMeshRef.isStopped = true;
                NavMeshRef.ResetPath();
                break;
        }
    }

    public bool BeyondMaxRange()
    {
        return !(CurrentPlayerDistance < MaxAttackDistance);
    }

    private void Update()
    {
        UpdateDistance();
        if (AttackChosen && ActionAvaliable)
        {
            RunChosenAttack();
        }
        if(ChosenAttack==AttackOptions.RollBash && CheckRollAttack())
        {
            BashAttack.AttackActive = false;
            PerformingAttack = false;
        }
        HandleLockoutTime();
    }

    private void RunChosenAttack()
    {
        PerformingAttack = true;
        ActionAvaliable = false;

        switch (ChosenAttack)
        {
            case AttackOptions.WebSpit:
                ActionLockoutTime = WebAttack.ActionLockoutTime;
                ActionAvaliable = false;
                PerformingAttack = true;
                WebAttack.AttackPerform();
                break;

            case AttackOptions.RollBash:
                ActionLockoutTime = BashAttack.ActionLockoutTime;
                ActionAvaliable = false;
                PerformingAttack = true;

                //HandleMovingState(false);
                BashAttack.AttackPerform();
                break;
        }
        AttackChosen = true;
        LockoutTimer = ActionLockoutTime;

    }

    public void BashHit()
    {
        HandleMovingState(false);
        PerformingAttack = false;
        ActionAvaliable = false;
        BashAttack.AttackActive = false;
        NavMeshRef.SetDestination(transform.position);
    }



    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            BashHit();
            Debug.Log("from above");
            Vector3 KnockbackDirection = ( Collision.transform.position- transform.position);
            KnockbackDirection.Normalize();
            Debug.Log(KnockbackDirection);
            KnockbackDirection *= 75;
            KnockbackDirection.y = 5;
            PlayerRef.GetComponent<Rigidbody>().velocity=(KnockbackDirection );
            //apply the knockback to the player


            if(Collision.GetComponent<PlayerInteraction>() == null)
            {
                Collision.transform.parent.root.GetComponent<PlayerInteraction>().TakeHit(-20);
                return;
            }
            Collision.GetComponent<PlayerInteraction>().TakeHit(-20);
        }
    }

    #region Attacks
    [System.Serializable]
    public class WebSpit
    {
        public bool AttackCoodldownActive = false;
        public bool AttackActive;

        public float ActionLockoutTime;

        public int SpitCounter;
        private WebbinEnemy ParentScriptLink;


        public void AttackStartup(WebbinEnemy LinkedScript)
        {
            ParentScriptLink = LinkedScript;
        }

        public IEnumerator SpitBurst()
        {
            while (AttackActive)
            {
                yield return new WaitForSeconds(0.78f);
                //Play animation
                //fire web spit
                SpitCounter++;
                if (SpitCounter == 3)
                {
                    AttackActive = false;
                    ParentScriptLink.PerformingAttack = false;
                    yield break;
                }
            }
        }

        private IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(3.5f);
        }

        public void AttackPerform()
        {
            
            Debug.Log("worthy of affection");
        }
    }

    [System.Serializable]
    public class RollBash
    {
        public bool AttackCoodldownActive = false;
        public bool AttackActive = false;

        public float ActionLockoutTime;

        private WebbinEnemy ParentScriptLink;
        public Vector3 StartingPosition;
        public Vector3 EndingPosition;


        public void AttackStartup(WebbinEnemy LinkedScript)
        {
            ParentScriptLink = LinkedScript;
        }
        public void AttackPerform()
        {
            if (AttackActive) { return; }
            Debug.Log("where i bleong");
            EndingPosition = EndBashLocation();
            ParentScriptLink.NavMeshRef.SetDestination(EndingPosition);
            ParentScriptLink.NavMeshRef.speed = ParentScriptLink.BaseMoveSpeed * 1.75f;

            AttackActive = true;
        }

        

        public Vector3 EndBashLocation()
        {
            StartingPosition = ParentScriptLink.transform.position;
            float Distance = ParentScriptLink.CurrentPlayerDistance;
            Vector3 Direction = (ParentScriptLink.PlayerRef.transform.position- StartingPosition).normalized;
            Direction.y = StartingPosition.y;

            EndingPosition = StartingPosition+(Direction * (Distance*2));
            EndingPosition.y = StartingPosition.y;

            return EndingPosition;
        }

        private IEnumerator AttackCooldown()
        {
            yield return new WaitForSeconds(5.5f);
        }

    }
    #endregion

}

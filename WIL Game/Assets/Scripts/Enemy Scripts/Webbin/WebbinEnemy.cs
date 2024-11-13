using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static WebbinEnemy;

public class WebbinEnemy : BossBase
{
    [Space(2), Header("Bools")]
    public bool PerformingAttack;
    public bool CanMove=true;
    public bool ActionAvaliable = true;
    public bool EngagePlayer = false;
    public bool ActionDelayActive;
    public bool AttackChosen;
    public bool CanAttack = true;
    public bool BeyondCurrentAttackRange;
    public bool Override;

    private bool ApplySlowdown = false;

    [Space(5), Header("Floats")]
    public float CurrentPlayerDistance;
    public float StoppingDistance;
    [SerializeField] private float ActionLockoutTime;
    [SerializeField] private float LockoutTimer;
    public float RotationOffset;


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
    private Animator WebbinAnimations;


    //Script area 
    public enum AttackOptions
    {
        WebSpit,
        RollBash
    };

    [Space(5)]

    public AttackOptions ChosenAttack;

    private void Start()
    {
        BossStartup();
        WebbinAnimations = transform.GetComponentInChildren<Animator>();
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

        StartupRan = true;
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
        if (!Alive || Override) { return; }

        UpdateDistance();
        if (ActionAvaliable && StartupRan)
        {
            RootNode.RunLogicAndState();
        }

        BeyondCurrentAttackRange = TrackAttackRange();
    }

    private bool TrackAttackRange()
    {
        if (CurrentPlayerDistance>CurrentAttackDistance && AttackChosen)
        {
            return true;
        }
        return false;
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
        Debug.Log("pay me wher ei belong");
        CurrentPlayerDistance = Vector3.Distance(PlayerRef.transform.position, transform.position);
    }
    public IEnumerator ActionDelay()
    {
        yield return new WaitForSeconds(2.52f);
        ActionAvaliable = true;
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

    public void HandleMovingState(bool Moving)
    {
        switch (Moving)
        {
            case true:
                NavMeshRef.isStopped = false;
                NavMeshRef.speed = BaseMoveSpeed;
                break;

            case false:
                NavMeshRef.isStopped = true;
                NavMeshRef.ResetPath();
                NavMeshRef.speed = 0;
                NavMeshRef.velocity = Vector3.zero;
                break;
        }
    }

    public bool BeyondMaxRange()
    {
        return !(CurrentPlayerDistance < MaxAttackDistance);
    }

    private void HandleAnimationSwitching()
    {
        if (NavMeshRef.velocity.magnitude > 2)
        {
            WebbinAnimations.SetBool("Moving", true);
            return;
        }
        else if(NavMeshRef.velocity.magnitude<=1)
        {
            WebbinAnimations.SetBool("Moving", false);
            return;
        }



    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            WebbinAnimations.SetTrigger("WebAttack");

        }
        if (Override) { return; }
        UpdateDistance();
        if (AttackChosen && ActionAvaliable && CanAttack)
        {
            RunChosenAttack();
        }
        if (ChosenAttack == AttackOptions.RollBash && CheckRollAttack() && BashAttack.AttackActive) 
        {
            BashAttack.AttackActive = false;
            PerformingAttack = false;
            StartCoroutine(BashAttack.AttackCooldown());
            BashAttack.DisableCollider();
            //Debug.Log("tales as old as time you know what i mean begs you the question what are theese jokers in the court you are holding");
            StartCoroutine(AttackCooldown());
        }
        HandleLockoutTime();
        RotateToTarget();

        HandleAnimationSwitching();
    }



    private void RunChosenAttack()
    {
        PerformingAttack = true;
        ActionAvaliable = false;

        switch (ChosenAttack)
        {
            case AttackOptions.WebSpit:
                if (WebAttack.AttackActive) { return; }
                ActionLockoutTime = WebAttack.ActionLockoutTime;
                ActionAvaliable = false;
                PerformingAttack = true;
                WebbinAnimations.SetTrigger("WebAttack");
                break;

            case AttackOptions.RollBash:
                ActionLockoutTime = BashAttack.ActionLockoutTime;
                ActionAvaliable = false;
                PerformingAttack = true;
                BashAttack.AttackPerform();
                WebbinAnimations.SetTrigger("RollAttack");
                break;
        }
        AttackChosen = true;
        LockoutTimer = ActionLockoutTime;
        CanAttack = false;
    }

    public void BashHit()
    {
        HandleMovingState(false);
        PerformingAttack = false;
        ActionAvaliable = false;

        BashAttack.AttackActive = false;
        NavMeshRef.SetDestination(transform.position);
        StartCoroutine(BashAttack.AttackCooldown());
        BashAttack.DisableCollider();
        StartCoroutine(AttackCooldown());
    }

    public IEnumerator AttackCooldown()
    {
        Debug.Log("cant attack");
        yield return new WaitForSeconds(5.5f);
        CanAttack = true;
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
                Collision.transform.parent.root.GetComponent<PlayerInteraction>().TakeHit(-20,transform.position);
                return;
            }
            Collision.GetComponent<PlayerInteraction>().TakeHit(-20,transform.position);
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
        [SerializeField] private Transform AttackPoint;
        [SerializeField] private GameObject WebShotPrefab;

        public void AttackStartup(WebbinEnemy LinkedScript)
        {
            ParentScriptLink = LinkedScript;
        }

        public IEnumerator SpitBurst()
        {
            
            Debug.Log("Spit");
            AttackActive = true;
            while (AttackActive)
            {
                yield return new WaitForSeconds(0.05f);
                //Play animation
                //fire web spit
                GameObject SpawnedWebShot= Instantiate(WebShotPrefab, AttackPoint.transform.position, Quaternion.identity);
                SpawnedWebShot.GetComponent<ProjectileBase>().LifeStartup(AttackPoint.transform.forward, 125f);
                Debug.Log("this spit");


                SpitCounter++;
                if (SpitCounter >= 5)
                {
                    AttackActive = false;
                    ParentScriptLink.PerformingAttack = false;
                    SpitCounter = 0;
                    yield break;
                }
            }
        }


        public IEnumerator AttackCooldown()
        {
            AttackCoodldownActive = true;
            yield return new WaitForSeconds(7.5f);
            AttackCoodldownActive = false;
        }
        
    }

    private void RotateToTarget()
    {
        Vector3 TargetDirection = PlayerRef.transform.position - this.transform.position;
        TargetDirection.y = 0.0f;
        Quaternion TargetRotation = Quaternion.LookRotation(TargetDirection + new Vector3(RotationOffset, 0, 0));

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, TargetRotation, (35f + 100) * Time.deltaTime);
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
        public Collider RollBashCollider;


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
            
            EnableCollider();

            AttackActive = true;
        }

        public void EnableCollider()
        {
            RollBashCollider.enabled = true;
        }

        public void DisableCollider()
        {
            RollBashCollider.enabled = false;
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

        public IEnumerator AttackCooldown()
        {
            AttackCoodldownActive = true;
            yield return new WaitForSeconds(12.5f);
            AttackCoodldownActive = false;
        }

    }
    #endregion

}

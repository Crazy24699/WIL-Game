using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class TurtleBossAI : BossBase
{


    [SerializeField]public BubbleAttack BubbleAttackClass;
    public BucketAttack BucketAttackClass;
    [SerializeField] private NavMeshAgent NavMeshRef;

    [SerializeField]private Vector3 PositionLockCords;
    public Vector3 CurrentVelocity;

    #region Bools
    public bool Move;
    public bool CanPerformAction = true;
    public bool MoveToPlayer;
    public bool PerformingAttack = false;
    public bool AttacksAvaliable = true;
    [SerializeField] private bool LockMovement = false;
    [SerializeField] private bool AttackActive;
    #endregion

    #region Floats
    private float ActionCooldown = 1.5f;
    private float CurrentActionCooldown = 0.0f;

    [SerializeField] private float TurnSpeed = 0.0f;
    public float MaxAttackRange;
    public float DistanceToPlayer;
    public float CurrentMagnitude;

    private float BubbleShotDelay=0.01f;
    private float CurrentDelay;

    #endregion

    [SerializeField]private Animator TurtleAnimation;

    public enum TurtleAttacks
    {
        None,
        BubbleBlast,
        BucketBasher
    }

    public TurtleAttacks ChosenAttack;

    private void Start()
    {
        BossStartup();
    }

    public override void BossStartup()
    {
        MaxHealth = 16;
        if (TurnSpeed == 0.0f)
        {
            TurnSpeed = 4.5f;
        }
        CurrentHealth = MaxHealth;

        TurtleAnimation = transform.GetComponentInChildren<Animator>();
        PlayerRef = FindObjectOfType<PlayerInteraction>().gameObject;

        Alive = true;
        HealthBar.maxValue = MaxHealth;
        StartupRan = true;
        CreateBehaviourTree();
    }

    private void FixedUpdate()
    {
        if (Alive == false)
        {
            return;
        }
        
        UpdateDistance();
        RootNode.RunLogicAndState();
    }


    private void CreateBehaviourTree()
    {
        BTTurtleMove MoveNode = new BTTurtleMove(gameObject);
        BTTurtleAttack AttackNode = new BTTurtleAttack(gameObject);

        BTNodeSequence MoveSequence = new BTNodeSequence();
        BTNodeSequence AttackSequence = new BTNodeSequence();

        MoveSequence.SetSequenceValues(new List<BTNodeBase> { MoveNode });
        AttackSequence.SetSequenceValues(new List<BTNodeBase> { AttackNode });

        RootNode = new BTNodeSelector(new List<BTNodeBase>() { MoveSequence, AttackSequence });
        
    }



    // Update is called once per frame
    void Update()
    {
        if (!StartupRan)
        {
            return;
        }
        ValueTracking();

        BubbleAttackMethod();
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    Debug.Log("Hello");
        //    //BubbleAttackMethod();
        //    ActiveAttack("BubbleAttack");

        //    //PerformingAttack = true;
        //}

        if (Input.GetKeyDown(KeyCode.I))
        {
            BucketBash();
            
        }

        SetDestination(PlayerRef.transform);

        EnforcePositionLock();

        if (CurrentHealth <= 0)
        {
            Alive = false;
            Destroy(gameObject);
        }

        HandleAttacks();

    }

    private void HandleAttacks()
    {
        if (!PerformingAttack)
        {
            return;
        }
        switch (ChosenAttack)
        {
            case TurtleAttacks.BubbleBlast:
                ActiveAttack("BubbleAttack");
                StartCoroutine(BubbleAttackClass.AttackCooldown());
                break;

            case TurtleAttacks.BucketBasher:
                BucketBash();
                StartCoroutine(BucketAttackClass.AttackCooldown());

                break;
        }
        ChosenAttack = TurtleAttacks.None;
        PerformingAttack = false;

    }

    private void ValueTracking()
    {
        CurrentVelocity = NavMeshRef.velocity;
        CurrentMagnitude = NavMeshRef.velocity.normalized.magnitude;
        TurtleAnimation.SetBool("IsMoving", CurrentMagnitude != 0);
        NavMeshRef.isStopped = LockMovement;
        AttacksAvaliable = !BubbleAttackClass.AttackCooldownActive || !BucketAttackClass.AttackCooldownActive;

        HealthBar.value = CurrentHealth;
    }

    public void ChangeLockState()
    {
        Debug.Log("Eve");
        switch (LockMovement)
        {
            case true:
                PositionLockCords = Vector3.zero;
                LockMovement = false;
                break;
                
            case false:
                PositionLockCords = transform.position;
                LockMovement = true;
                break;
        }

        Debug.Log("huh when reun");
        MoveToPlayer = LockMovement;

    }


    private void EnforcePositionLock()
    {
        if (LockMovement && PositionLockCords != Vector3.zero)
        {
            Debug.Log("Wolves");
            gameObject.transform.position = PositionLockCords;
            return;
        }
    }

    public void BucketBash()
    {
        ActiveAttack("BucketAttack");
       

    }

    private void BubbleAttackMethod()
    {
        if (BubbleAttackClass.SpewBubbles)
        {
            CurrentDelay -= Time.deltaTime;
            if (CurrentDelay <= 0)
            {
                CurrentDelay = BubbleShotDelay;
                BubbleAttackClass.ShootingThing();
                Debug.Log("Run mf run");
            }

        }
    }


    #region Turtle Logic

    public void SetDestination(Transform ObjectLocation)
    {
        NavMeshRef.SetDestination(ObjectLocation.transform.position);
    }

    private void UpdateDistance()
    {
        DistanceToPlayer = Vector3.Distance(PlayerRef.transform.position, transform.position);
    }

    #endregion

    private void ActiveAttack(string AttackTrigger)
    {
        TurtleAnimation.SetTrigger(AttackTrigger);
    }

    [System.Serializable]
    public class BubbleAttack
    {

        public Vector3[] ShootPointRotations;

        [Space(1)]
        public GameObject ShootPoint;
        public GameObject BubblePrefab;

        [Space(1)]
        public bool AttackCooldownActive = false;
        public bool SpewBubbles = false;

        public float AttackRange;

        public void BubbleShot(int ShotIndex, string NameNumbers)
        {
            
            //ShootPoint.transform.localPosition = BubbleShootPoints[ShotIndex];
            //ShootPoint.transform.localRotation = Quaternion.Euler(BubbleShootRotations[ShotIndex]);

            GameObject BubbleShotObejct = Instantiate(BubblePrefab, ShootPoint.transform.position, ShootPoint.transform.rotation);
            BubbleShotObejct.GetComponent<ProjectileBase>().LifeStartup(ShootPoint.transform.forward, 300);
            BubbleShotObejct.name = "BubbleShot" + NameNumbers;

        }

        public void ShootingThing()
        {
            GameObject BubbleShotObejct = Instantiate(BubblePrefab, ShootPoint.transform.position, ShootPoint.transform.rotation);
            BubbleShotObejct.GetComponent<ProjectileBase>().LifeStartup(ShootPoint.transform.forward, 300);
            BubbleShotObejct.name = "BubbleShot";
        }

        public IEnumerator AttackCooldown()
        {
            if (!AttackCooldownActive)
            {
                AttackCooldownActive = true;
                yield return new WaitForSeconds(7.5f);
                AttackCooldownActive = false;
            }
        }

    }

    [System.Serializable]
    public class BucketAttack
    {
        public float AttackRange;

        public bool AttackCooldownActive = false;

        [SerializeField]private Collider BucketCollider;
        
        //public Vector3 HitPosition;
        public Vector3 ViewLock;

        public void BucketBash()
        {

            BucketCollider.enabled = true;

            Debug.Log("Love Bites");
            //BucketCollider.enabled = false;
        }

        public IEnumerator AttackCooldown()
        {
            if (!AttackCooldownActive)
            {
                AttackCooldownActive = true;
                yield return new WaitForSeconds(0.75f);
                AttackCooldownActive = false;
            }

        }

    }

}


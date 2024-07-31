using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TurtleBossAI : BossBase
{


    [SerializeField]public BubbleAttack BubbleAttackClass;
    public BucketAttack BucketAttackClass;
    [SerializeField] private NavMeshAgent NavMeshRef;

    [SerializeField]private Vector3 PositionLockCords;
    public Vector3 CurrentVelocity;

    public bool Move;
    public bool CanPerformAction = true;
    public bool MoveToPlayer;
    public bool PerformingAttack = false;
    [SerializeField] private bool LockMovement = false;
    [SerializeField] private bool AttackActive;

    private float ActionCooldown = 1.5f;
    private float CurrentActionCooldown = 0.0f;

    [SerializeField] private float TurnSpeed = 0.0f;
    public float DistanceToPlayer;
    public float CurrentMagnitude;

    private float BubbleShotDelay=0.01f;
    private float CurrentDelay;

    [SerializeField]private Animator TurtleAnimation;

    public enum TurtleAttacks
    {
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
        ValueTracking();

        BubbleAttackMethods();
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Hello");
            //BubbleAttackMethod();
            ActiveAttack("BubbleAttack");

            //PerformingAttack = true;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            BucketBash();
            
        }


        if (PerformingAttack)
        {
            //Debug.Log("Sneeze");
            switch (ChosenAttack)
            {
                default:
                case TurtleAttacks.BubbleBlast:
                    ActiveAttack("BubbleAttack");
                    StartCoroutine(BubbleAttackClass.AttackCooldown());
                    //BubbleAttackMethod();
                    break;


                case TurtleAttacks.BucketBasher:
                    BucketBash();
                    StartCoroutine(BucketAttackClass.AttackCooldown());
                    //BucketBashMethod();
                    break;

            }
            PerformingAttack = false;
        }

        EnforcePositionLock();

        if (CurrentHealth <= 0)
        {
            Alive = false;
            Destroy(gameObject);
        }

        #region Active Action Cooldown
        if (!CanPerformAction && CurrentActionCooldown > 0)
        {
            CurrentActionCooldown -= Time.deltaTime;
        }
        else if(!CanPerformAction && CurrentActionCooldown <= 0)
        {
            CurrentActionCooldown = 0;
            CanPerformAction = true;
            PositionLockCords = Vector3.zero;
            Debug.Log("Thrown");
        }
        #endregion

    }

    private void ValueTracking()
    {
        CurrentVelocity = NavMeshRef.velocity;
        CurrentMagnitude = NavMeshRef.velocity.normalized.magnitude;
        TurtleAnimation.SetBool("IsMoving", CurrentMagnitude != 0);
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
        NavMeshRef.isStopped = LockMovement;
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


    protected void BubbleAttackMethod()
    {
        if (AttackActive || !CanPerformAction)
        {
            return;
        }
        //ChangeLockState();
        Debug.Log("Coral high");
        Vector3 RotationChange = PlayerRef.transform.position - transform.position;
        RotationChange.y = 0;
        Quaternion RotateTrackPlayer = Quaternion.LookRotation(RotationChange);
        transform.rotation = Quaternion.Slerp(transform.rotation, RotateTrackPlayer, Time.deltaTime * TurnSpeed);

        if (!AttackActive)
        {
            Debug.Log("Ran");
            StartCoroutine(BubbleShotFunctionality());
            AttackActive = true;
        }


    }

    public void BucketBash()
    {
        ActiveAttack("BucketAttack");
        //ChangeLockState();

    }

    private void BubbleAttackMethods()
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

    protected void BucketBashMethod()
    {
        if (AttackActive ||!CanPerformAction)
        {
            return;
        }
        //ChangeLockState();

        MoveToPlayer = false;
        Debug.Log("Afluvian");
        BucketAttackClass.BucketBash();
        ActiveAttack("BucketAttack");

        CanPerformAction = false;
        CurrentActionCooldown = ActionCooldown;
        //PerformingAttack = false;
        AttackActive = false;
        TurtleAnimation.SetBool("IsMoving", true);

        StartCoroutine(BucketAttackClass.AttackCooldown());

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
        TurtleAnimation.SetBool("IsMoving", false);

    }

    private IEnumerator BubbleShotFunctionality()
    {
        ActiveAttack("BubbleAttack");
        yield return new WaitForSeconds(1.75f);
        string ShotNumber = "";
        for (int i = 0; i < 4; i++)
        {
            for (int y = 0; y < 10;)
            {
                BubbleAttackClass.ShootPoint.transform.LookAt(PlayerRef.transform.position);
                ShotNumber = i.ToString() + y.ToString();
                BubbleAttackClass.BubbleShot(y, ShotNumber);
                yield return new WaitForSeconds(0.05f);

                y++;
            }
        }

        CanPerformAction = false;
        CurrentActionCooldown = ActionCooldown;
        //PerformingAttack = false;
        AttackActive = false;
        StartCoroutine(BubbleAttackClass.AttackCooldown());
        TurtleAnimation.SetBool("IsMoving", true);

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


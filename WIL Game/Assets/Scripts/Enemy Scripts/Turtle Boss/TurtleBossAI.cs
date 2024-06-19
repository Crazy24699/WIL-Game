using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TurtleBossAI : BossBase
{


    [SerializeField]public BubbleAttack BubbleAttackClass;
    public BucketAttack BucketAttackClass;
    [SerializeField] private NavMeshAgent NavMeshRef;

    private Vector3 PositionLockCords;

    public bool CanPerformAction = true;
    public bool MoveToPlayer;
    public bool PerformingAttack = false;
    [SerializeField] private bool AttackActive;

    private float ActionCooldown = 1.5f;
    private float CurrentActionCooldown = 0.0f;

    [SerializeField] private float TurnSpeed = 0.0f;
    public float Distance;

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
        UpdateRange();
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
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Hello");
            BubbleAttackMethod();
            PerformingAttack = true;
        }

        if (PerformingAttack) 
        {
            //Debug.Log("Sneeze");
            switch (ChosenAttack)
            {
                default:
                case TurtleAttacks.BubbleBlast:
                    BubbleAttackMethod();
                    break;


                case TurtleAttacks.BucketBasher:
                    BucketBashMethod();
                    break;

            }
        }

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

    private void LockPosition()
    {
        if (PositionLockCords == Vector3.zero) 
        {
            Debug.Log("Wolves");
            PositionLockCords = gameObject.transform.position;
            return;
        }

    }

    protected void BubbleAttackMethod()
    {
        if (AttackActive || !CanPerformAction)
        {
            return;
        }
        LockPosition();
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

    protected void BucketBashMethod()
    {
        if (AttackActive ||!CanPerformAction)
        {
            return;
        }
        LockPosition();

        MoveToPlayer = false;
        Debug.Log("Afluvian");
        BucketAttackClass.BucketBash();
        ActiveAttack("BucketAttack");

        CanPerformAction = false;
        CurrentActionCooldown = ActionCooldown;
        PerformingAttack = false;
        AttackActive = false;
        TurtleAnimation.SetBool("IsMoving", true);

        StartCoroutine(BucketAttackClass.AttackCooldown());

    }

    #region Turtle Logic

    public void SetDestination(Transform ObjectLocation)
    {
        NavMeshRef.SetDestination(ObjectLocation.transform.position);
    }

    private void UpdateRange()
    {
        Distance = Vector3.Distance(PlayerRef.transform.position, transform.position);
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
        PerformingAttack = false;
        AttackActive = false;
        StartCoroutine(BubbleAttackClass.AttackCooldown());
        TurtleAnimation.SetBool("IsMoving", true);

    }


    [System.Serializable]
    public class BubbleAttack
    {
        //private Vector3[] ShootPointCords =
        //{
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (-0.225f, 0.75f, 0.765f),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),

        //};
        //public Vector3[] BubbleShootPoints;

        private Vector3[] BubbleShootRotations =
        {
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (0, 0, 0),
            //new Vector3 (-10, 0, 0),
            //new Vector3 (-10, -10, 0),
            //new Vector3 (0, -10, 0),
            //new Vector3 (10, -10, 0),
            //new Vector3 (10, 0, 0),
            //new Vector3 (10, 10, 0),
            //new Vector3 (0, 10, 0),
            //new Vector3 (-10, 10, 0),
        };

        public Vector3[] ShootPointRotations;

        [Space(1)]
        public GameObject ShootPoint;
        public GameObject BubblePrefab;

        [Space(1)]
        public bool AttackCooldownActive = false;

        public void BubbleShot(int ShotIndex, string NameNumbers)
        {
            if (ShootPointRotations.Length != BubbleShootRotations.Length)
            {
                ShootPointRotations = BubbleShootRotations;
            }
            //ShootPoint.transform.localPosition = BubbleShootPoints[ShotIndex];
            //ShootPoint.transform.localRotation = Quaternion.Euler(BubbleShootRotations[ShotIndex]);

            GameObject BubbleShotObejct = Instantiate(BubblePrefab, ShootPoint.transform.position, ShootPoint.transform.rotation);
            BubbleShotObejct.GetComponent<ProjectileBase>().LifeStartup(ShootPoint.transform.forward, 300);
            BubbleShotObejct.name = "BubbleShot" + NameNumbers;

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
                yield return new WaitForSeconds(1.5f);
                AttackCooldownActive = false;
            }

        }

    }

}


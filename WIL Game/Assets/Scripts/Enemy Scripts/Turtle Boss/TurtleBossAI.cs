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
    public bool AttackActive;

    private float ActionCooldown = 1.5f;
    private float CurrentActionCooldown = 0.0f;

    [SerializeField] private float TurnSpeed = 0.0f;
    public float Distance;



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
        if (TurnSpeed == 0.0f)
        {
            TurnSpeed = 4.5f;
        }
        CurrentHealth = MaxHealth;
        CreateBehaviourTree();
    }

    private void FixedUpdate()
    {
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
            Debug.Log("Sneeze");
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
        LockPosition();

        MoveToPlayer = false;
        Debug.Log("Afluvian");
        BucketAttackClass.BucketBash();
        StartCoroutine(BucketAttackClass.AttackCooldown());
        CanPerformAction = false;
        CurrentActionCooldown = ActionCooldown;
        PerformingAttack = false;
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



    private IEnumerator BubbleShotFunctionality()
    {
        //if (AttackActive)
        //{
        //    yield return null;
        //}
        string ShotNumber = "";
        for (int i = 0; i < 4; i++)
        {
            for (int y = 0; y < BubbleAttackClass.ShootPointRotations.Length;)
            {
                ShotNumber = i.ToString() + y.ToString();
                BubbleAttackClass.BubbleShot(y, ShotNumber);
                yield return new WaitForSeconds(0.075f);

                y++;
            }
        }

        CanPerformAction = false;
        CurrentActionCooldown = ActionCooldown;
        PerformingAttack = false;
        StartCoroutine(BubbleAttackClass.AttackCooldown());
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
            new Vector3 (0, 0, 0),
            new Vector3 (-10, 0, 0),
            new Vector3 (-10, -10, 0),
            new Vector3 (0, -10, 0),
            new Vector3 (10, -10, 0),
            new Vector3 (10, 0, 0),
            new Vector3 (10, 10, 0),
            new Vector3 (0, 10, 0),
            new Vector3 (-10, 10, 0),
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
            ShootPoint.transform.localRotation = Quaternion.Euler(BubbleShootRotations[ShotIndex]);

            GameObject BubbleShotObejct = Instantiate(BubblePrefab, ShootPoint.transform.position, ShootPoint.transform.rotation);
            BubbleShotObejct.GetComponent<ProjectileBase>().LifeStartup(ShootPoint.transform.forward, 300);
            BubbleShotObejct.name = "BubbleShot" + NameNumbers;

        }


        public IEnumerator AttackCooldown()
        {
            if (!AttackCooldownActive)
            {
                yield return new WaitForSeconds(3.5f);
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
            BucketCollider.enabled = false;
        }

        public IEnumerator AttackCooldown()
        {
            if (!AttackCooldownActive)
            {
                yield return new WaitForSeconds(1.5f);
                AttackCooldownActive = false;
            }

        }

    }

}

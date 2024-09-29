using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GenSwarmLogic : MonoBehaviour
{
    [SerializeField] private GameObject GeneratorEnemy;
    private GameObject PlayerTarget;
    [SerializeField] private List<Transform> SwarmLocations = new List<Transform>();


    [SerializeField] private float MinPlayerDistance;
    [SerializeField] private float MaxPlayerDistance;

    //What amount of added distance the swarm needs when orbiting the player
    //this is used when the swarm can not attack and needs to keep distance
    [SerializeField] private float ExtraOrbitDistance;
    [SerializeField]private float CurrentPlayerDistance;

    public Vector3 PlayerDirection;

    [SerializeField]private float OrbitingMoveMultiplier;

    [SerializeField]private float Radius;
    [SerializeField] private float Angle;

    [SerializeField]private float CloseRangeTimer;
    const float MaxCloseRangeTime = 1.25f;
    private float RetreatSpeed;

    private float AttackCooldownTime = 3.5f;
    [SerializeField]private float CurrentWaitTime; 
    public float CloseRangeSpeedMulti;

    public float PathfindAwayDistance;
    public float OrbitingDistance;
    public float BaseMoveSpeed;
    

    public HashSet<Enim2PH> GeneratorSwarm = new HashSet<Enim2PH>();
    public List<Enim2PH> ShownSwarm;
    private Enim2PH CurrentSelectedDrone;
    private WorldHandler WorldHandlerScript;

    private Vector3 CheckingCord;
    Vector3 RndPoint;
    [SerializeField]private Vector3 NewRetreatPosition;
    [SerializeField] private Vector3 OldPosition;
    [SerializeField] private Vector3 ThisPosition;
    public Vector3 CurrentPosition;
    public Vector3 RetreatPosition;

    private int SwarmNum = 7;
    private int LocationCheckCounter;
    private int CurrentDroneIndex=0;
    public int MaxFollowDistance;

    [SerializeField] private LayerMask SpawningMask;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private LayerMask ObstacleMask;

    private Rigidbody RigidbodyRef;
    private NavMeshAgent NavMeshRef;

    [SerializeField] private bool CanAttack;
    [SerializeField] private bool InAttackRange;
    [SerializeField]private bool ChangePosition = false;
    [SerializeField] private bool GeneralStartupRan = false;

    [SerializeField] private bool OnAttackList = false;
    [SerializeField] private bool SeenPlayer = false;
    public bool DevAttackOverride = false;
    public bool AdvancedRetreat;
    public bool UpdateRetretPosition;
    public bool Orbiting = false;


    private void Start()
    {
        EnemyStartup();
    }   

    private void EnemyStartup()
    {
        PlayerTarget = FindObjectOfType<PlayerMovement>().gameObject;
        RigidbodyRef = GetComponent<Rigidbody>();

        CloseRangeTimer = MaxCloseRangeTime;
        CanAttack = true;
        //StartCoroutine();
        SpawnSwarm();
        NavMeshRef = GetComponent<NavMeshAgent>();
        WorldHandlerScript = FindObjectOfType<WorldHandler>();
    }

    private void SpawnSwarm()
    {
        for (int i = 0; i < SwarmNum; i++)
        {
            if (LocationCheckCounter >= 20)
            {
                break;
            }
            
            GameObject SpawnedDrone;
            Vector3 PossiblePosition = RandomizeSpawnPoint();
            if (!Physics.CheckSphere(PossiblePosition, 1.55f, SpawningMask))
            {
                SpawnedDrone = Instantiate(GeneratorEnemy, PossiblePosition, Quaternion.identity);
                SpawnedDrone.gameObject.name = "Drone " + i;

                SwarmLocations.Add(new GameObject().transform);
                SwarmLocations[i].transform.position = PossiblePosition;
                SwarmLocations[i].transform.parent = this.transform;
                SwarmLocations[i].gameObject.name = "Swarmer Location: " + i;

                SpawnedDrone.GetComponent<Enim2PH>().SwarmTransformLocation = SwarmLocations[i];
                SpawnedDrone.transform.parent = this.transform;

                GeneratorSwarm.Add(SpawnedDrone.GetComponent<Enim2PH>());
                SpawnedDrone.GetComponent<Enim2PH>().BaseStartup();
                Debug.Log("Run");
            }
            else
            {
                i--;
            }
            LocationCheckCounter++;
        }
        GeneralStartupRan = true;
    }

    //Possible Future Performance Issue 
    private Vector3 RandomizeSpawnPoint()
    {
        //spherecast to check if the area is clear

        float X_Coord = SetRandomCoorindates(transform.position.x,2.65f);
        float Y_Coord = SetRandomCoorindates(transform.position.y,1.5f);
        float Z_Coord = SetRandomCoorindates(transform.position.z,2.65f);

        Vector3 DroneSpawnPos = new Vector3(X_Coord, Y_Coord, Z_Coord);
        CheckingCord = DroneSpawnPos;

        Physics.SphereCastAll(DroneSpawnPos, 1.55f, Vector3.forward);


        //CreatedDrone.transform.position = DroneSpawnPos;

        return DroneSpawnPos;
    }

    private float SetRandomCoorindates(float CurrentCord, float MinCoordRange)
    {
        bool NegativeAxisValue = Random.Range(0, 2) == 0 ? true : false;

        float ChangedCoord = 0.0f;
        if (NegativeAxisValue) { MinCoordRange *= -1; }
        ChangedCoord = Random.Range(CurrentCord + MinCoordRange, CurrentCord + (MinCoordRange * 3.25f));


        return ChangedCoord;
    }

    private void KeepDistanceRange()
    {
        CurrentPlayerDistance = Vector3.Distance(transform.position, PlayerTarget.transform.position);
        InAttackRange = (CurrentPlayerDistance < MaxPlayerDistance);

        if (!OnAttackList && SeenPlayer)
        {
            NavMeshRef.speed = 3.5f * OrbitingMoveMultiplier;
            OrbitPlayer();
            return;
        }

        if (CurrentPlayerDistance > MaxPlayerDistance )
        {
            //Debug.Log("at the alter we start to pray");
            NavMeshRef.SetDestination(PlayerTarget.transform.position);
            NavMeshRef.isStopped = false;
        }
        if (CurrentPlayerDistance < MinPlayerDistance + 15)
        {
            //Debug.Log(CurrentPlayerDistance + "Love bites" + MinPlayerDistance + 15.00);
            //ChangePosition = true;
            NavMeshRef.isStopped = true;
        }

        //if (!InAttackRange) { return; }


        if (CurrentPlayerDistance < MinPlayerDistance && !ChangePosition)
        {
            Vector3 PlayerDirection = (transform.position - PlayerTarget.transform.position).normalized;
            RigidbodyRef.velocity = new Vector3(PlayerDirection.x, 0, PlayerDirection.z) * 5;

            if (CloseRangeTimer > 0)
            {
                CloseRangeTimer -= Time.deltaTime ;
                Debug.Log("eternal luliby");
                if (CloseRangeTimer <= 0)
                {
                    Debug.Log("Heartbear");
                    FindRetreatPosition();
                    CloseRangeTimer = MaxCloseRangeTime;
                }
            }
        }
        else if (CurrentPlayerDistance > MinPlayerDistance)
        {
            RigidbodyRef.velocity = Vector3.zero;
        }
    }

    private void OrbitPlayer()
    {
        Debug.Log("Orbiting");
        float NewFollowDistance = MaxPlayerDistance + ExtraOrbitDistance;

        if (CurrentPlayerDistance < NewFollowDistance)
        {
            if(NavMeshRef.isStopped) { NavMeshRef.isStopped = false; }

            NavMeshRef.SetDestination(PlayerTarget.transform.position);
        }
        else if (CurrentPlayerDistance < NewFollowDistance - 5) 
        {
            NavMeshRef.isStopped = true;
        }
        //this moves the swarm out of range of the player faster
        if (CurrentPlayerDistance < MaxPlayerDistance && !ChangePosition)
        {
            Vector3 PlayerDirection = (transform.position - PlayerTarget.transform.position).normalized;
            RigidbodyRef.velocity = new Vector3(PlayerDirection.x, 0, PlayerDirection.z) * 15;
        }

        //if the player somehow manages to corner the swarm
        if (CurrentPlayerDistance < MaxPlayerDistance - 10)
        {
            Debug.Log("Heartbear");
            FindRetreatPosition();
            CloseRangeTimer = MaxCloseRangeTime;
        }

    }

    
    private void Attack()
    {
        if (DevAttackOverride) { return; }
        if (!OnAttackList && WorldHandlerScript.EnemiesAttacking.Count < 2)
        {
            WorldHandlerScript.EnemiesAttacking.Add(this.gameObject);
            OnAttackList = true;
        }

        if ((!InAttackRange || GeneratorSwarm.Count <= 0) || !OnAttackList )
        {
            return;
        }

        if (!CanAttack)
        {
            CurrentWaitTime -= Time.deltaTime;
            if (CurrentWaitTime <= 0)
            {
                CanAttack = true;
            }
        }

        if (CanAttack && GeneratorSwarm.Count > 0) 
        {
            SendNextDrone();
            CanAttack = false;
            CurrentWaitTime = AttackCooldownTime;
        }

    }

    private void SendNextDrone()
    {
        //the drone needs to do a check that it belongs to the parent before its launched, that is causing an error

        if (CurrentDroneIndex >= GeneratorSwarm.Count) { CurrentDroneIndex = 0; }
        //Debug.Log(GeneratorSwarm.ElementAt(CurrentDroneIndex));
        //Debug.Log(GeneratorSwarm.ElementAt(CurrentDroneIndex).name);
        if (GeneratorSwarm.ElementAt(CurrentDroneIndex).GetComponent<Enim2PH>().AttatchedToParent)
        {
            GeneratorSwarm.ElementAt(CurrentDroneIndex).GetComponent<Enim2PH>().SwarmAttack(this.transform.gameObject, PlayerTarget);
            //Debug.Log("Fore Drone out");
            CurrentDroneIndex++;
            CanAttack = false;
        }
        CurrentDroneIndex++;
    }

    private void FindRetreatPosition()
    {
        OldPosition = transform.position;
        
        float X_Value = RoundNumbersToDecimal(OldPosition.x);
        float Y_Value = RoundNumbersToDecimal(OldPosition.y);
        float Z_Value = RoundNumbersToDecimal(OldPosition.z);

        OldPosition = new Vector3(X_Value, Y_Value, Z_Value);
        //RandomRetreatPosition = Random.Range(MinPlayerDistance, MaxPlayerDistance);
        RndPoint = Random.insideUnitCircle;


        Vector3 RetreatDirection = new Vector3(RndPoint.x, 0, RndPoint.y).normalized;
        NewRetreatPosition = PlayerTarget.transform.position + RetreatDirection * MaxPlayerDistance;

        X_Value = RoundNumbersToDecimal(NewRetreatPosition.x);
        Y_Value = RoundNumbersToDecimal(NewRetreatPosition.y);
        Z_Value = RoundNumbersToDecimal(NewRetreatPosition.z);

        NewRetreatPosition = new Vector3(X_Value, Y_Value, Z_Value);

        ChangePosition = true;
    }

    private void CheckDeath()
    {
        if(GeneratorSwarm.Count == 0)
        {
            //play death anim
            Destroy(this.gameObject);
        }
    }

    private void Retreat()
    {
        if (!ChangePosition) { return; }

        if (transform.position == NewRetreatPosition) { ChangePosition = false; RetreatSpeed = 0; return; }
        RetreatSpeed += 0.425f * Time.deltaTime;

        Vector3 CurrentPosition = Vector3.Slerp(OldPosition, NewRetreatPosition, RetreatSpeed);

        float X_Value = RoundNumbersToDecimal(CurrentPosition.x);
        float Y_Value = RoundNumbersToDecimal(CurrentPosition.y);
        float Z_Value = RoundNumbersToDecimal(CurrentPosition.z);

        transform.position = new Vector3(X_Value, Y_Value, Z_Value);
        if (transform.position == NewRetreatPosition)
        {
            ChangePosition = false;
            RetreatSpeed = 0;
        }
    }

    private float RoundNumbersToDecimal(float BaseNum)
    {
        float RoundedNum = Mathf.Round(BaseNum * 100) / 100;
        return RoundedNum;
    }

    public void RotateToTarget()
    {
        Vector3 TargetDirection = PlayerTarget.transform.position - this.transform.position;
        TargetDirection.y = 0.0f;
        Quaternion TargetRotation = Quaternion.LookRotation(TargetDirection);

        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, TargetRotation, 35f * Time.deltaTime);
    }

    private void Update()
    {
        if (!GeneralStartupRan)
        {
            return;
        }

        //if (!StartupRan) { return; }

        
        if (Orbiting)
        {
            RotateToTarget();
            KeepOrbitDistance();
            return;
        }

        ShownSwarm = GeneratorSwarm.ToList();
        KeepDistanceRange();
        Attack();

        if(CanAttack)
        {
            SendNextDrone();
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            FindRetreatPosition();

        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            DevAttackOverride = !DevAttackOverride;
            
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SendNextDrone();
        }
    }

    private void FixedUpdate()
    {
        Retreat();
        CheckDeath();
    }


    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            if (Collision.transform.root.root.GetComponent<PlayerInteraction>())
            {
                SeenPlayer = true;
            }
        }
    }

    private void OnTriggerExit(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            if (Collision.transform.root.root.GetComponent<PlayerInteraction>())
            {
                SeenPlayer = false;
            }
        }
    }

    protected void KeepOrbitDistance()
    {
        CurrentPlayerDistance = Vector3.Distance(this.transform.position, PlayerTarget.transform.position);
        PlayerDirection = (this.transform.position - PlayerTarget.transform.position).normalized;
        //PlayerDirection = PlayerDirection.RoundVector(0);

        if (CurrentPlayerDistance <= PathfindAwayDistance && UpdateRetretPosition)
        {
            PathfindingRetreat();
            StartCoroutine(RetreatCooldown());

            return;
        }
        if (AdvancedRetreat)
        {
            if (CurrentPosition == RetreatPosition)
            {
                if (RigidbodyRef.velocity != Vector3.zero)
                {
                    RigidbodyRef.velocity = Vector3.zero;
                }

                AdvancedRetreat = false;
            }
            return;
        }

        if (CurrentPlayerDistance < OrbitingDistance)
        {
            Debug.Log(":the whispers");
            NavMeshRef.isStopped = false;
            RigidbodyRef.velocity = new Vector3(PlayerDirection.x, 0, PlayerDirection.z) * 20;
            if (CurrentPlayerDistance < MaxFollowDistance - 10)
            {
                UpdateRetretPosition = true;
                CloseRangeSpeedMulti = 2.5f;
                NavMeshRef.speed = BaseMoveSpeed * CloseRangeSpeedMulti;
                return;
            }

            CloseRangeSpeedMulti = 1f;
            UpdateRetretPosition = false;
            NavMeshRef.speed = BaseMoveSpeed * CloseRangeSpeedMulti;
        }
        else
        {
            Debug.Log("thriller");
            NavMeshRef.isStopped = true;
            if (RigidbodyRef.velocity != Vector3.zero)
            {

                RigidbodyRef.velocity = Vector3.zero;
            }
        }
    }

    private void PathfindingRetreat()
    {
        CurrentPosition = transform.position.RoundVector(2);
        RetreatPosition = (PlayerDirection * 20) + CurrentPosition;
        RetreatPosition = RetreatPosition.RoundVector(2);

        NavMeshRef.SetDestination(RetreatPosition);
        AdvancedRetreat = true;
    }

    private IEnumerator RetreatCooldown()
    {
        UpdateRetretPosition = false;
        yield return new WaitForSeconds(1.75f);
        UpdateRetretPosition = true;
    }
}

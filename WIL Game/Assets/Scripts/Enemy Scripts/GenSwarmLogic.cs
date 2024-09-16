using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenSwarmLogic : MonoBehaviour
{
    [SerializeField] private GameObject GeneratorEnemy;
    [SerializeField] private GameObject PlayerTarget;
    [SerializeField] private GameObject LookatPoint;


    [SerializeField] private float MinPlayerDistance;
    [SerializeField] private float MaxPlayerDistance;
    private float CurrentPlayerDistance;
    private float RandomRetreatPosition;

    public HashSet<Enim2PH> GeneratorSwarm = new HashSet<Enim2PH>();
    private Enim2PH CurrentSelectedDrone;

    private Vector3 CheckingCord;
    Vector3 RndPoint;

    private int SwarmNum = 7;
    private int LocationCheckCounter;
    private int CurrentDroneIndex=0;
    
    [SerializeField] private LayerMask SpawningMask;

    private bool CanAttack;
    private bool InAttackRange;
    public bool ChangePoint;


    private void EnemyStartup()
    {
        PlayerTarget = FindObjectOfType<PlayerMovement>().gameObject;
    }

    private IEnumerator SpawnSwarm()
    {
        for (int i = 0; i < SwarmNum; i++)
        {
            if (LocationCheckCounter >= 20)
            {
                break;
            }

            yield return new WaitForSeconds(0.25f);
            GameObject SpawnedDrone;
            Vector3 PossiblePosition = RandomizeSpawnPoint();
            if (!Physics.CheckSphere(PossiblePosition, 1.55f, SpawningMask))
            {
                SpawnedDrone = Instantiate(GeneratorEnemy, PossiblePosition, Quaternion.identity);
                SpawnedDrone.GetComponent<Enim2PH>().BaseStartup();
                GeneratorSwarm.Add(SpawnedDrone.GetComponent<Enim2PH>());
                Debug.Log("Run");
            }
            else
            {
                i--;
            }
            LocationCheckCounter++;
        }
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
        InAttackRange = (CurrentPlayerDistance > MinPlayerDistance && CurrentPlayerDistance < MaxPlayerDistance);

        if(InAttackRange)
        {
            if(CurrentPlayerDistance < MinPlayerDistance)
            {

            }
        }

    }

    private void Attack()
    {

    }

    private void SendNextDrone()
    {
        if (CurrentDroneIndex >= GeneratorSwarm.Count) { CurrentDroneIndex = 0; }
        Debug.Log(GeneratorSwarm.ElementAt(CurrentDroneIndex));
        GeneratorSwarm.ElementAt(CurrentDroneIndex).GetComponent<Enim2PH>().SwarmAttack(this.transform.gameObject, PlayerTarget);

        CurrentDroneIndex++;
        StartCoroutine(AttackCooldown());
    }

    private void OnDrawGizmos()
    {
        Vector3 DirectionPoint = (LookatPoint.transform.position - PlayerTarget.transform.position).normalized;
        Gizmos.DrawWireSphere(PlayerTarget.transform.position, 1.55f);
        Gizmos.color = Color.blue;
        Vector3 Point = PlayerTarget.transform.position+DirectionPoint * MinPlayerDistance;

        Gizmos.DrawSphere(Point, 2.5f);
        Vector3 MaxPoint = PlayerTarget.transform.position+ DirectionPoint * MaxPlayerDistance;
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(MaxPoint, 2.5f);


        if (ChangePoint)
        {
            RandomRetreatPosition = Random.Range(MinPlayerDistance, MaxPlayerDistance);
            RndPoint = Random.insideUnitCircle;
            ChangePoint = false;
        }
        Vector3 SpawnDirection = new Vector3(RndPoint.x, 0, RndPoint.y).normalized;
        Vector3 SpawnPos = PlayerTarget.transform.position + SpawnDirection * RandomRetreatPosition;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SpawnPos, 2.5f);


    }

    private IEnumerator AttackCooldown()
    {
        CanAttack = false;
        yield return new WaitForSeconds(2.55f);
        CanAttack = true;
    }

    private void Update()
    {
        if(CanAttack)
        {
            SendNextDrone();
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            EnemyStartup();
        }

        if(Input.GetKeyDown(KeyCode.J))
        {

            StartCoroutine(SpawnSwarm());
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SendNextDrone();
        }
    }

    

}

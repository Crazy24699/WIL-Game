using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenSwarmLogic : MonoBehaviour
{
    [SerializeField] private GameObject GeneratorEnemy;
    [SerializeField] private GameObject PlayerTarget;

    
    public HashSet<Enim2PH> GeneratorSwarm = new HashSet<Enim2PH>();
    private Enim2PH CurrentSelectedDrone;

    private Vector3 CheckingCord;

    private int SwarmNum = 7;
    private int LocationCheckCounter;
    private int CurrentDroneIndex=0;
    
    [SerializeField] private LayerMask SpawningMask;


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

    }

    private void SendNextDrone()
    {
        if (CurrentDroneIndex >= GeneratorSwarm.Count) { CurrentDroneIndex = 0; }
        Debug.Log(GeneratorSwarm.ElementAt(CurrentDroneIndex));
        GeneratorSwarm.ElementAt(CurrentDroneIndex).GetComponent<Enim2PH>().SwarmAttack(this.transform.gameObject, PlayerTarget);

        CurrentDroneIndex++;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(CheckingCord, 1.55f);
    }

    private void Update()
    {
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

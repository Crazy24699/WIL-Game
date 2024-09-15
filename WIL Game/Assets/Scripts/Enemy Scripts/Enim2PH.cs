using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generator guy
public class Enim2PH : EnemyBase
{
    private Vector3 SwarmPosition;
    private Vector3 OutOfRangePosition;
    private Vector3 RejoinPathOffset;

    [SerializeField] private Transform CenterSlerpPoint;

    private GenSwarmLogic SwarmParent;
    private int MissCount=3;
    private GameObject PlayerObjectRef;

    [SerializeField]private bool Confused = true;

    [SerializeField]private int InternalCounter;

    private float Speed;

    public void SwarmAttack(GameObject Parent, GameObject PlayerObject)
    {
        transform.LookAt(PlayerObject.transform.position);
        SwarmParent = Parent.GetComponent<GenSwarmLogic>();
        InvokeRepeating(nameof(CheckMiss), 0.25f, 0.15f);

        PlayerObjectRef = PlayerObject;
        SwarmPosition = this.transform.position;
        Rigidbody=GetComponent<Rigidbody>();
        Rigidbody.AddForce(transform.forward * 1000);
        Debug.Log("aaaa");
    }

    protected override void CustomStartup()
    {
        MaxHealth = 30;
        BaseMoveSpeed = 16;

    }

    private void Start()
    {
        OutOfRangePosition = this.transform.position;
    }

    private void HandleDeath()
    {
        SwarmParent.GeneratorSwarm.Remove(this);
    }

    private void OnTriggerEnter(Collider Collision)
    {
        HandleDeath();
    }

    private void FixedUpdate()
    {
        if(!Confused && this.transform.position!=Vector3.zero)
        {
            Speed += 0.250f * Time.deltaTime;
            Vector3 CenterPivotPoint = new Vector3(49, 10, -25);
            //x=4
            //Vector3(49,10,-25)
            //CenterPivotPoint = new Vector3(SwarmPosition.x, SwarmPosition.y, SwarmPosition.z);
            //transform.position = Vector3.Slerp(OutOfRangePosition - new Vector3(49, 10, -25), new Vector3(50, 12, -29) - new Vector3(49, 10, -25), Speed) + new Vector3(49, 10, -25);
            transform.position = Vector3.Slerp(OutOfRangePosition, SwarmPosition, Speed);
        }


    }

    private void CheckMiss()
    {
        float PlayerDistance = Vector3.Distance(transform.position, PlayerObjectRef.transform.position);
        float SwarmParentDistance = Vector3.Distance(transform.position, SwarmParent.transform.position);
        if (PlayerDistance >= 3.25f && SwarmParentDistance >= 16.75f) 
        {
            Debug.Log("Bleh" + PlayerDistance + "       " + SwarmParentDistance);
            if (MissCount > 0)
            {
                MissCount--;
                CancelInvoke(nameof(CheckMiss));
                StartCoroutine(ConfusedCooldown());
                return;
            }
            if (MissCount == 0)
            {
                HandleDeath();
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(ConfusedCooldown());
        }
        
    }

    private IEnumerator ConfusedCooldown()
    {
        OutOfRangePosition = transform.position;
        yield return new WaitForSeconds(1.75f);
        Confused = false;
    }
}

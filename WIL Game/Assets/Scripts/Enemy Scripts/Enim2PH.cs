using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Generator guy
public class Enim2PH : EnemyBase
{
    [SerializeField]private Vector3 SwarmPosition;
    [SerializeField] private Vector3 OutOfRangePosition;
    private Vector3 RejoinPathOffset;
    public Vector3 CurrentPosition;

    [SerializeField] private Transform CenterSlerpPoint;

    private GenSwarmLogic SwarmParent;
    [SerializeField]private int MissCount=3;
    private GameObject PlayerObjectRef;

    [SerializeField]private bool Confused = true;
    [SerializeField] private bool Attacking = false;
    public bool InPos;
    private bool GeneralStartupRan = false;
    private bool WaitTimeFinished = false;

    [SerializeField]private int InternalCounter;

    public float PlayerDistance;
    public float SwarmParentDistance;
    private float Speed;

    public void SwarmAttack(GameObject Parent, GameObject PlayerObject)
    {
        Attacking = true;


        if (!GeneralStartupRan)
        {
            Rigidbody = GetComponent<Rigidbody>();


            SwarmParent = Parent.GetComponent<GenSwarmLogic>();


            PlayerObjectRef = PlayerObject;
            SwarmPosition = this.transform.position;
            SwarmPosition = SwarmPosition.RoundVector(2);
            GeneralStartupRan = true;
        }
        transform.LookAt(PlayerObject.transform.position);
        Rigidbody.AddForce(transform.forward * 1000);

        WaitTimeFinished = false;
        InvokeRepeating(nameof(CheckMiss), 0.25f, 0.15f);

        Speed = 0;
        Debug.Log("aaaa");
        StartCoroutine(AttackDoneWaitTime());
    }

    protected override void CustomStartup()
    {
        MaxHealth = 30;
        BaseMoveSpeed = 16;

    }

    private void Start()
    {
        OutOfRangePosition = this.transform.position.RoundVector(2);
    }

    private void HandleDeath()
    {
        SwarmParent.GeneratorSwarm.Remove(this);
    }

    private void OnTriggerEnter(Collider Collision)
    {
        //HandleDeath();
    }

    private void CheckMiss()
    {
        if (!Attacking)
        {
            Attacking = true;
        }

        //Debug.Log("Hit it");
        PlayerDistance = Vector3.Distance(transform.position, PlayerObjectRef.transform.position);
        SwarmParentDistance = Vector3.Distance(transform.position, SwarmParent.transform.position);
        if (PlayerDistance >= 20.25f && SwarmParentDistance >= 40.75f) 
        {
            //Debug.Log("Bleh" + PlayerDistance + "       " + SwarmParentDistance);
            if (MissCount > 0)
            {
                Debug.Log("Invoke");
                Rigidbody.velocity = Vector3.zero;
                CancelInvoke(nameof(CheckMiss));
                MissCount--;
                StartCoroutine(ConfusedCooldown());
                Confused = true;
                return;
            }
            if (MissCount == 0)
            {
                //HandleDeath();
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(ConfusedCooldown());
        }

        CurrentPosition = transform.position.RoundVector(2);
        transform.position=transform.position.RoundVector(2);
        if (!Confused && this.transform.position.RoundVector(2) != SwarmPosition && Attacking && WaitTimeFinished && OutOfRangePosition!=Vector3.zero)
        {
            Speed += 0.250f * Time.deltaTime;
            Vector3 CurrentPosition = Vector3.Slerp(OutOfRangePosition, SwarmPosition, Speed);
            transform.position = CurrentPosition.RoundVector(2);
        }
        //InPos = this.transform.position.RoundVector(2) == SwarmPosition;
        if (!Confused && this.transform.position.RoundVector(2) == SwarmPosition && Rigidbody.velocity==Vector3.zero && Attacking && WaitTimeFinished)
        {
            Debug.Log("Is not true");
            Attacking = false;
            OutOfRangePosition = Vector3.zero;
        }
    }

    private IEnumerator AttackDoneWaitTime()
    {
        yield return new WaitForSeconds(0.25f);
        WaitTimeFinished = true;
    }

    private IEnumerator ConfusedCooldown()
    {
        OutOfRangePosition = transform.position.RoundVector(2);
        yield return new WaitForSeconds(1.75f);
        Confused = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



//Generator guy
public class Enim2PH : BaseEnemy
{
    public Transform SwarmTransformLocation;
    [SerializeField] private Transform CenterSlerpPoint;

    private GameObject PlayerObjectRef;

    [SerializeField]private Vector3 SwarmPosition;
    [SerializeField] private Vector3 OutOfRangePosition;
    private Vector3 RejoinPathOffset;

    private GenSwarmLogic SwarmParent;

    [SerializeField]private int MissCount=3;

    [SerializeField]private bool Confused = true;
    [SerializeField] private bool Attacking = false;

    private bool GeneralStartupRan = false;
    [SerializeField] private bool WaitTimeFinished = false;
    [SerializeField] public bool AttatchedToParent;

    [SerializeField]private int InternalCounter;

    public float PlayerDistance;
    private float SwarmParentDistance;
    private float Speed;
    private float MaxTravelDistance = 50;

    public void SwarmAttack(GameObject Parent, GameObject PlayerObject)
    {
        RigidbodyRef.isKinematic = false;
        if (!GeneralStartupRan)
        {
            RigidbodyRef = GetComponent<Rigidbody>();

            SwarmParent = Parent.GetComponent<GenSwarmLogic>();

            PlayerObjectRef = PlayerObject;
            SwarmPosition = this.transform.position;

            SwarmPosition = SwarmPosition.RoundVector(2);
            GeneralStartupRan = true;
        }
        Attacking = true;
        //this.transform.SetParent(null);
        //Debug.Log("Thrown " + transform.parent.name);
        this.transform.parent = null;

        transform.LookAt(PlayerObject.transform.position);
        RigidbodyRef.AddForce(transform.forward * 1250);

        WaitTimeFinished = false;
        InvokeRepeating(nameof(CheckMiss), 0.25f, 0.15f);

        Speed = 0;
        //Debug.Log("aaaa");
        StartCoroutine(AttackDoneWaitTime());
        //Physics.IgnoreLayerCollision(12, 13, true);
    }
    

    protected override void CustomStartup()
    {
        RigidbodyRef.isKinematic = true;
        MaxHealth = 4;
        CurrentHealth = MaxHealth;
        //BaseMoveSpeed = 16;
        HealthBar = transform.GetComponentInChildren<Slider>();

        ImmunityTime = 0.25f;

        CanTakeDamage = true;
    }

    public void HandleGeneralStartup()
    {
        OutOfRangePosition = this.transform.position.RoundVector(2);
        BaseStartup();
    }

    private void HandleDeath()
    {
        SwarmParent.GeneratorSwarm.Remove(this);
    }


    protected override void Death()
    {
        //Explosion animation
        HandleDeath();
        Destroy(this.gameObject);
    }

    //maybe put this into a larger checking method for the update method. 
    private IEnumerator TakeDamageCooldown()
    {
        CanTakeDamage = false;
        yield return new WaitForSeconds(1.25f);
        CanTakeDamage = true;
    }

    private void CheckMiss()
    {
        if (!Attacking)
        {
            Attacking = true;
        }

        //Debug.Log("Hit it");
        CheckDistances();
        if (PlayerDistance >= 20.25f && SwarmParentDistance >= 40.75f || SwarmParentDistance >= MaxTravelDistance) 
        {
            //Debug.Log("Bleh" + PlayerDistance + "       " + SwarmParentDistance);
            if (MissCount > 0)
            {
                //Debug.Log("Invoke");
                RigidbodyRef.velocity = Vector3.zero;
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

    private void CheckDistances()
    {
        PlayerDistance = Vector3.Distance(transform.position, PlayerObjectRef.transform.position);
        SwarmParentDistance = Vector3.Distance(transform.position, SwarmTransformLocation.transform.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(ConfusedCooldown());
        }

        AttatchedToParent = transform.parent != null;

        if (!GeneralStartupRan) { return; }

        if (AttatchedToParent)
        {
            RigidbodyRef.isKinematic = true;
        }


        CurrentPosition = transform.position.RoundVector(2);
        transform.position=transform.position.RoundVector(2);
        SwarmPosition = SwarmTransformLocation.position.RoundVector(2);

        Retreval();
        //InPos = this.transform.position.RoundVector(2) == SwarmPosition;
        if (!Confused && this.transform.position.RoundVector(2) == SwarmPosition && RigidbodyRef.velocity==Vector3.zero && Attacking && WaitTimeFinished)
        {
            //Debug.Log("Is not true");
            Attacking = false;
            OutOfRangePosition = Vector3.zero;
        }
    }

    private void Retreval()
    {
        if (!Attacking)
        {
            return;
        }

        CheckDistances();

        if (!Confused && this.transform.position.RoundVector(2) != SwarmPosition && Attacking && WaitTimeFinished && OutOfRangePosition != Vector3.zero)
        {
            Speed += 0.250f * Time.deltaTime;
            Vector3 CurrentPosition = Vector3.Slerp(OutOfRangePosition, SwarmTransformLocation.position, Speed);
            transform.position = CurrentPosition.RoundVector(2);
        }

        if (SwarmParentDistance <= 3.5f && WaitTimeFinished)
        {
            if (transform.parent == null) 
            {
                transform.parent = SwarmParent.transform;
                RigidbodyRef.velocity = Vector3.zero;
            }
            //Speed += 0.250f * Time.deltaTime;
            //Vector3 CurrentPosition = Vector3.Lerp(OutOfRangePosition, SwarmPosition, Speed);
            //transform.position = CurrentPosition.RoundVector(2);
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
        yield return new WaitForSeconds(2.75f);
        Confused = false;
    }

    private void OnTriggerEnter(Collider Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            if (!Attacking)
            {
                return;
            }
            FindObjectOfType<PlayerInteraction>().HandleHealth(-3);
            //Play explosion animation
            //Play explosion sound
            Destroy(this.gameObject);
            SwarmParent.GeneratorSwarm.Remove(this);
        }
        if (Collision.CompareTag("Ground"))
        {
            MissCount--;
            RigidbodyRef.velocity = Vector3.zero;
        }
    }
}

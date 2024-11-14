using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialProjectile : ProjectileBase
{
    private float CurrentLifeTime = 0.0f;
    private Vector3 IntialCoalSize;
    private Vector3 FinalCoalSize;

    public GameObject PlayerRef;

    public bool StartupRan=false;
    private bool OnGround = false;

    private float VelocityReductionCap = -75.5f;

    protected override void CustomBehaviour()
    {
        Damage = 1;
        LifeTime = 20;

        IntialCoalSize = this.transform.localScale;
        FinalCoalSize = IntialCoalSize * 3;

        CustomLifeTimer = true;

        InvokeRepeating("VelocityReduction", 0, 0.15f);
    }

    private void Update()
    {
        if (!StartupRan) { return; }
        transform.position=Vector3.MoveTowards(transform.position, PlayerRef.transform.position, 150*Time.deltaTime);
    }

    private void VelocityReduction()
    {
        if (RigidBodyRef.velocity.y <= VelocityReductionCap)
        {
            CancelInvoke("VelocityReduction");
            return;
        }
        Vector3 CurrentVelocity = RigidBodyRef.velocity;
        Vector3 ChangedVelocity = new Vector3(CurrentVelocity.x, CurrentVelocity.y - 6.5f, CurrentVelocity.z);
        RigidBodyRef.velocity = ChangedVelocity;
    }

    private void OnCollisionEnter(Collision ObjectCollision)
    {
        Debug.Log(ObjectCollision.gameObject.tag);
        if (ObjectCollision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Hit Ground");
            Destroy(this.gameObject);
        }

        if (ObjectCollision.gameObject.CompareTag("Player"))
        {
            ObjectCollision.gameObject.GetComponentInParent<PlayerInteraction>().HandleHealth(-10);
            Destroy(this.gameObject);
        }
    }

}

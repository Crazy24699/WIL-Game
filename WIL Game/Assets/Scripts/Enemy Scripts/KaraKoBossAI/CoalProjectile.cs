using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoalProjectile : ProjectileBase
{
    private float CurrentLifeTime = 0.0f;
    private Vector3 IntialCoalSize;
    private Vector3 FinalCoalSize;

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

    private void Update()
    {
        if (CurrentLifeTime <= 0)
        {
            CurrentLifeTime = 0;
            //Destroy(gameObject);
            return;
        }

        CurrentLifeTime -= Time.deltaTime;
        if(transform.localScale.x < FinalCoalSize.x)
        {
            float SizeIncreaseTime = 2f;
            float XScaleIncrease = Mathf.Lerp(IntialCoalSize.x, FinalCoalSize.x, SizeIncreaseTime);

            Vector3 CoalScaleChange = Vector3.one * XScaleIncrease;
            transform.localScale = CoalScaleChange;

        }


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
            ObjectCollision.gameObject.GetComponentInParent<PlayerInteraction>().HandleHealth(-1);
        }
    }

}

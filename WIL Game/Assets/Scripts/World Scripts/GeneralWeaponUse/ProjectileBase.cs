using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    private bool LifeStarted = false;
    protected bool CustomLifeTimer = false;

    protected Rigidbody RigidBodyRef;
    [SerializeField] private Vector3 ForceChanges;

    protected float LifeTime;
    protected int Damage;
    protected bool CanDestory = false;

    [SerializeField]private float AdditionalForce;

    public void LifeStartup(Vector3 DirectionalForce, float InitalForce)
    {
        CustomBehaviour();
        if (LifeStarted)
        {
            return;
        }
        RigidBodyRef = GetComponent<Rigidbody>();
        if (Damage == 0)
        {
            Damage = 5;
        }

        if (RigidBodyRef == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        //Debug.Log(DirectionalForce + "  " + InitalForce);

        if (LifeTime == 0 && !CustomLifeTimer)
        {
            LifeTime = 15;
        }
        RigidBodyRef.AddForce((DirectionalForce * 5 + ForceChanges) * (InitalForce + AdditionalForce));
        StartCoroutine(LifeTimer());

        if (!CustomLifeTimer)
        {
            InvokeRepeating("VelocityReduction", 0, 0.25f);
        }
        LifeStarted = true;
    }

    protected virtual void CustomBehaviour()
    {

    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(LifeTime);

        if (gameObject.activeSelf)
        {
            Destroy(this.gameObject);
        }
    }

    private void VelocityReduction()
    {
        if (!this.gameObject.activeSelf)
        {
            CancelInvoke("VelocityReduction");
            return;
        }
        Vector3 CurrentVelocity = RigidBodyRef.velocity;
        RigidBodyRef.velocity = new Vector3(CurrentVelocity.x, CurrentVelocity.y - 1.0f, CurrentVelocity.z);
    }
}

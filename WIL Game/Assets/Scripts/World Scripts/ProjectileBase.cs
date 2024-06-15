using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    private bool LifeStarted = false;

    private Rigidbody RigidBodyRef;
    [SerializeField] private Vector3 ForceChanges;

    protected float LifeTime;
    protected bool CustomLifeTimer = false;

    public void LifeStartup(Vector3 DirectionalForce, float InitalForce)
    {
        if (DirectionalForce == Vector3.zero)
        {
            DirectionalForce = Vector3.forward;
        }
        CustomBehaviour();
        if (LifeStarted)
        {
            return;
        }
        RigidBodyRef = GetComponent<Rigidbody>();

        if (RigidBodyRef == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        Debug.Log(DirectionalForce + "  " + InitalForce);

        if (LifeTime == 0)
        {
            LifeTime = 15;
        }
        RigidBodyRef.AddForce((DirectionalForce * 5 + ForceChanges) * InitalForce);
        StartCoroutine(LifeTimer());

        if (!CustomLifeTimer)
        {
            InvokeRepeating("VelocityReduction", 0, 0.25f);
        }
        LifeStarted = true;
        CustomBehaviour();
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

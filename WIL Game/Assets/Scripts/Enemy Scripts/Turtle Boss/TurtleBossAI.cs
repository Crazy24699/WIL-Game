using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleBossAI : BossBase
{


    [SerializeField]private BubbleAttack BubbleAttackClass;

    public enum TurtleAttacks
    {
        BubbleBlast,
        BucketBasher
    }

    public TurtleAttacks ChosenAttack;


    public override void BossStartup()
    {

        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("Hello");
            Attack();
        }
    }

    protected override void Attack()
    {
        StartCoroutine(ShotDelay());

    }

    private IEnumerator ShotDelay()
    {
        string ShotNumber = "";
        for (int i = 0; i < 3; i++)
        {
            for (int y = 0; y < BubbleAttackClass.ShootPointRotations.Length;)
            {
                ShotNumber = i.ToString() + y.ToString();
                BubbleAttackClass.ChangeBubbleShot(y, ShotNumber);
                yield return new WaitForSeconds(0.2f);

                y++;
            }
        }

    }

    private void CreateBehaviourTree()
    {

    }

    [System.Serializable]
    private class BubbleAttack
    {
        //private Vector3[] ShootPointCords =
        //{
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (-0.225f, 0.75f, 0.765f),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),
        //    new Vector3 (0, 0, 0),

        //};
        //public Vector3[] BubbleShootPoints;
        private Vector3[] BubbleShootRotations =
        {
            new Vector3 (0, 0, 0),
            new Vector3 (-10, 0, 0),
            new Vector3 (-10, -10, 0),
            new Vector3 (0, -10, 0),
            new Vector3 (10, -10, 0),
            new Vector3 (10, 0, 0),
            new Vector3 (10, 10, 0),
            new Vector3 (0, 10, 0),
            new Vector3 (-10, 10, 0),
        };
        public Vector3[] ShootPointRotations;

        public GameObject ShootPoint;
        public GameObject BubblePrefab;

        public void ChangeBubbleShot(int ShotIndex, string NameNumbers)
        {
            if (ShootPointRotations.Length != BubbleShootRotations.Length)
            {
                ShootPointRotations = BubbleShootRotations;
            }
            //ShootPoint.transform.localPosition = BubbleShootPoints[ShotIndex];
            ShootPoint.transform.localRotation = Quaternion.Euler(BubbleShootRotations[ShotIndex]);

            GameObject BubbleShotObejct = Instantiate(BubblePrefab, ShootPoint.transform.position, ShootPoint.transform.rotation);
            BubbleShotObejct.GetComponent<ProjectileBase>().LifeStartup(ShootPoint.transform.forward, 300);
            

        }
    }

    [System.Serializable]
    private class BucketAttack
    {

    }

}


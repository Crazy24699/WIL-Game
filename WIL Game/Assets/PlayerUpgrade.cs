using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerUpgrade : MonoBehaviour
{
    [SerializeField] private UpgradeTracker[] AllUpgrades;


    private int AbilityPointCount;
    

    public enum Upgrades
    {
        None,
        Heath,
        Attack1,
        Attack2,
        Attack3,
    }



    // Start is called before the first frame update
    void Start()
    {
        AllUpgrades[1].UpgradeAbility(0);
        AllUpgrades[2].UpgradeAbility(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AbilityPointUpdate(int PointCount)
    {
        AbilityPointCount = PointCount;
    }

    public void HandleUpgrade(Upgrades ChosenUpgrade)
    {

        switch (ChosenUpgrade)
        {
            case Upgrades.None:
                break;

            case Upgrades.Heath:
                HealthUpgrade();
                break;

            case Upgrades.Attack1:
                break;

            case Upgrades.Attack2:
                break;

            case Upgrades.Attack3:
                break;
        }

    }

    #region Upgrades

    private void HealthUpgrade()
    {
        if (AllUpgrades[1].UpgradeCost>AbilityPointCount)
        AllUpgrades[1].UpgradeAbility(1);



        if (AllUpgrades[1].UpgradeLevel == 3)
        {
            AllUpgrades[1].UpgradeCost += 2;
        }
    }

    private void SlashUpgrade()
    {

    }

    private void BiteUpgrade()
    {

    }

    private void TailUpgrade()
    {

    }

    #endregion

}

[Serializable]
public class UpgradeTracker
{
    public string UpgradeName;

    [SerializeField]private bool Unlocked = false;

    public int UpgradeCost;
    public int UpgradeLevel;

    public void UpgradeAbility(int AddedCost)
    {
        UpgradeLevel++;
        if (AddedCost == 0) { Unlocked = true; AddedCost = 2; return; }

        UpgradeCost += AddedCost;

    }

}
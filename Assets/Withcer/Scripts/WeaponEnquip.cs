using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnquip : MonoBehaviour
{
    public GameObject weaponHolder;
    public GameObject weapon;
    public GameObject weaponSheath;

    GameObject currentWeaponInHand;
    GameObject currentWeaponInSheath;

    void Start()
    {
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
    }

    public void WeaponDraw()
    {
        currentWeaponInHand = Instantiate(weapon, weaponHolder.transform);
        Destroy(currentWeaponInSheath);
    }

    public void WeaponSheath()
    {
        currentWeaponInSheath = Instantiate(weapon, weaponSheath.transform);
        Destroy(currentWeaponInHand);
    }

    public void StartDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().StartDamage();
    }

    public void EndDealDamage()
    {
        currentWeaponInHand.GetComponentInChildren<DamageDealer>().EndDamage();
    }
}

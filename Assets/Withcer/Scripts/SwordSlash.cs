using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    private InputController inputActions;
    private StateAttack stateAttack;
    private CombatManager combatManager;
    public GameObject swordSlash;
    
    void Awake()
    {
        inputActions = new InputController();
        stateAttack = GameObject.Find("Witcher").GetComponent<StateAttack>();
        combatManager = GameObject.Find("Witcher").GetComponent<CombatManager>();
    }

    private void Start()
    {
        swordSlash.SetActive(false);
    }

    private void Update()
    {
        AttackSlash();
    }

    private void AttackSlash()
    {
        if (stateAttack.isAttack == true)
        {
            swordSlash.SetActive(true);
        }

        else if (stateAttack.isAttack == false)
        {
            swordSlash.SetActive(false);
        }

        if (combatManager.drawWeapon == false)
        {
            swordSlash.SetActive(false);
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}

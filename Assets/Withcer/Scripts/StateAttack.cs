using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : MonoBehaviour
{
    private Animator animator;
    public int countAttackClick;
    private InputController inputActions;

    void Awake()
    {
        inputActions = new InputController();
        animator = GetComponent<Animator>();
        countAttackClick = 0;
    }

    private void Update()
    {
        if (inputActions.Player.Attack.triggered)
        {
            ButtonAttack();
        }
    }

    void ButtonAttack()
    {
        countAttackClick++;
        if (countAttackClick == 1) animator.SetInteger("attack", 1);
    }

    public void CheckAttackPhase()
    {
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("Combat 1"))
        {
            if (countAttackClick > 1) animator.SetInteger("attack", 2);
            else ResetAttackPhase();
        }
        else if (animator.GetCurrentAnimatorStateInfo(1).IsName("Combat 2"))
        {
            if (countAttackClick >= 2) ResetAttackPhase();
        }
    }

    private void ResetAttackPhase()
    {
        countAttackClick = 0;
        animator.SetInteger("attack", 0);
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

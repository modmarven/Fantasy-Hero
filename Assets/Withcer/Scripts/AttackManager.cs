using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private Animator animator;
    private StateAttack attack;
   
    void Awake()
    {
        animator = GetComponent<Animator>();
        attack = GameObject.Find("Witcher").GetComponent<StateAttack>();
    }

    public void AttackEvent()
    {
        attack.CheckAttackPhase();
    }
}

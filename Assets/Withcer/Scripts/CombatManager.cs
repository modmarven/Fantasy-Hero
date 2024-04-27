using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    private CharacterMovementManager character;
    private InputController inputActions;

    public bool drawWeapon;

    // Start is called before the first frame update
    void Awake()
    {
        character = GetComponent<CharacterMovementManager>();
        inputActions = new InputController();
        drawWeapon = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActions.Player.Sheath.triggered)
        {
            if (drawWeapon)
            {
                SheathWeapon();
            }
            else
            {
                DrawWeapon();
            }

        }       
    }

    private void DrawWeapon()
    {
        character.animator.SetTrigger("drawWeapon");
        drawWeapon = true;
    }

    private void SheathWeapon()
    {
        character.animator.SetTrigger("sheathWeapon");
        drawWeapon = false;
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

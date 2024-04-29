using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    private bool canDamage;
    List<GameObject> hasDamage;

    public float weaponLenght;
    public float weaponDamage;

    void Start()
    {
        canDamage = false;
        hasDamage = new List<GameObject>();
    }

    
    void Update()
    {
        if (canDamage)
        {
            RaycastHit hit;

            int layerMask = 1 << 9;
            if (Physics.Raycast(transform.position, -transform.forward, out hit, weaponLenght, layerMask))
            {
                if (!hasDamage.Contains(hit.transform.gameObject))
                {
                    Debug.Log("Damage");
                    hasDamage.Add(hit.transform.gameObject);
                }
            }
        }
    }

    public void StartDamage()
    {
        canDamage = true;
        hasDamage.Clear();
    }

    public void EndDamage()
    {
        canDamage = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward * weaponLenght);
    }
}

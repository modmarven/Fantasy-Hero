using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlashShooter : MonoBehaviour
{
    public Camera cam;
    public GameObject projectTile;
    public Transform firePoint;
    public float fireRate = 4f;

    private Vector3 destination;
    private float timeTofire;
    private GroundSlash groundSlashScript;
    private InputController inputActions;
    private Animator animator;

    void Awake()
    {
        inputActions = new InputController();
        animator = GetComponent<Animator>();
    }
   
    void Update()
    {
        if (inputActions.Player.Ground.triggered)
        {
            ShootProjecttile();
            animator.SetTrigger("Slash");
            Debug.Log("Press");
        }
    }

    private void ShootProjecttile()
    {
        Ray ray = cam.ViewportPointToRay(new(0.5f, 0.5f, 0));
        destination = ray.GetPoint(1000);
    }

    public void InstantiateProjectile()
    {
        var projecttileObj = Instantiate(projectTile, firePoint.position, Quaternion.identity) as GameObject;

        groundSlashScript = projecttileObj.GetComponent<GroundSlash>();
        RotationToDestination(projecttileObj, destination, true);
        projecttileObj.GetComponent<Rigidbody>().velocity = transform.forward * groundSlashScript.speed;
    }

    private void RotationToDestination(GameObject obj, Vector3 destination, bool onlyY)
    {
        var direction = destination - obj.transform.position;
        var rotation = Quaternion.LookRotation(direction);

        if (onlyY)
        {
            rotation.x = 0;
            rotation.z = 0;
        }

        obj.transform.localRotation = Quaternion.Lerp(obj.transform.rotation, rotation, 1);
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

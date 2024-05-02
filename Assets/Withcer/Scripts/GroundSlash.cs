using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSlash : MonoBehaviour
{
    public float speed = 30f;
    public float slowDownRate = 0.01f;
    public float detectingDistance = 0.1f;
    public float destroyDelay = 5f;

    private Rigidbody rigidBody;
    private bool stoped;

    void Start()
    {
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        if (GetComponent<Rigidbody>() != null)
        {
            rigidBody = GetComponent<Rigidbody>();
            StartCoroutine(SlowDownRate());
        }
        else
            Debug.Log("No RigidBody");

        Destroy(gameObject, destroyDelay);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 distance = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        if (Physics.Raycast(distance, transform.TransformDirection(-Vector3.up), out hit, detectingDistance))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        Debug.DrawRay(distance, transform.TransformDirection(-Vector3.up * detectingDistance), Color.red);
    }

    IEnumerator SlowDownRate()
    {
        float t = 1;
        while (t > 0)
        {
            rigidBody.velocity = Vector3.Lerp(Vector3.zero, rigidBody.velocity, t);
            t -= slowDownRate;
            yield return new WaitForSeconds(0.1f);
        }

        stoped = true;
    }
}

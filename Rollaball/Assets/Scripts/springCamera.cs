using UnityEngine;

public class springCamera : MonoBehaviour
{
    public Transform target;
    public float stiffness = 150f;

    public float damping = 10f;
    private Vector3 currentVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentVelocity, damping);
        }
    }
}
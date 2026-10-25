using UnityEngine;

public class Equilibrium : MonoBehaviour
{
    public GameObject ball;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 test = gameObject.transform.position;
        Vector3 ballTest = ball.transform.position;
        test.x = ballTest.x;
        test.y = 10f;
        test.z = ballTest.z - 10f;
        gameObject.transform.position = test;



    }
}

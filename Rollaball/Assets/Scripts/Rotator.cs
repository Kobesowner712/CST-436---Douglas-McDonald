using UnityEngine;

public class Rotator : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Time.deltaTime);
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);

    }
}

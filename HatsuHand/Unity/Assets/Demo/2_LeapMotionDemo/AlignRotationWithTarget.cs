using UnityEngine;

public class AlignRotationWithTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private void Start()
    {
    }

    void Update()
    {
        // Y²üčÉĢŻń]

        // X²ĘZ²Ģń]šÅč
        Vector3 currentRotation = transform.rotation.eulerAngles;
        //currentRotation.z = 0; // X²Ģń]šÅč
        currentRotation.y = target.rotation.eulerAngles.y; // Z²Ģń]šÅč

        // Åčµ½ń]šKp
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
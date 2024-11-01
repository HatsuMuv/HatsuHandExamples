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
        // Y������ɂ̂݉�]

        // X����Z���̉�]���Œ�
        Vector3 currentRotation = transform.rotation.eulerAngles;
        //currentRotation.z = 0; // X���̉�]���Œ�
        currentRotation.y = target.rotation.eulerAngles.y; // Z���̉�]���Œ�

        // �Œ肵����]��K�p
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
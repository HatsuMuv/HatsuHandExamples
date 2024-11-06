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
        // YŽ²Žü‚è‚É‚Ì‚Ý‰ñ“]

        // XŽ²‚ÆZŽ²‚Ì‰ñ“]‚ðŒÅ’è
        Vector3 currentRotation = transform.rotation.eulerAngles;
        //currentRotation.z = 0; // XŽ²‚Ì‰ñ“]‚ðŒÅ’è
        currentRotation.y = target.rotation.eulerAngles.y; // ZŽ²‚Ì‰ñ“]‚ðŒÅ’è

        // ŒÅ’è‚µ‚½‰ñ“]‚ð“K—p
        transform.rotation = Quaternion.Euler(currentRotation);
    }
}
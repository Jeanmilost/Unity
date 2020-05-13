using UnityEditor;
using UnityEngine;

/**
* Tool to snap objects on the ground
*@author Jean-Milost Reymond
*/
public class ES_SnapToGround : MonoBehaviour
{
    #region Public functions

    /**
    * Snaps all selected objects to the next hit ground
    *@note Objects will be aligned with their center point
    */
    [MenuItem("Misc./Snap To Ground %g")]
    public static void SnapToGround()
    {
        // iterate through selected objects
        foreach (var transform in Selection.transforms)
        {
            // get all vertical hit points until 10 units
            var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 10.0f);

            // iterate through hit points
            foreach (var hit in hits)
            {
                // found game object itself?
                if (hit.collider.gameObject == transform.gameObject)
                    continue;

                // set closest ground position
                transform.position = hit.point;
                break;
            }
        }
    }

    #endregion
}

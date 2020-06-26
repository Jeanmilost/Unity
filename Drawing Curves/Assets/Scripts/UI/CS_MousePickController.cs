using UnityEngine;

/**
* Provides a mouse pick controller
*@author Jean-Milost Reymond
*/
[RequireComponent(typeof(MeshCollider))]
public class CS_MousePickController : MonoBehaviour
{
    private Vector3 m_ScreenPoint;
    private Vector3 m_Offset;

    /**
    * Called when the left mouse button is clicked above the game object
    */
    void OnMouseDown()
    {
        // calculate the current mouse click position on the screen and the offset to apply to the game object
        m_ScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        m_Offset      = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                                                                        Input.mousePosition.y,
                                                                                        m_ScreenPoint.z));
    }

    /**
    * Called when the mouse is dragged above the scene
    */
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_ScreenPoint.z);
        Vector3 curPosition    = Camera.main.ScreenToWorldPoint(curScreenPoint) + m_Offset;
        transform.position     = curPosition;
    }
}

using UnityEngine;
using UnityEngine.UI;

/**
* Script allowing to change the dither transparency level on a cube from a slider
*@author Jean-Milost Reymond
*/
public class WS_DitherLevel : MonoBehaviour
{
    private GameObject m_Cube;
    private Slider     m_Slider;

    /**
    * Starts the script
    */
    void Start()
    {
        // get the cube
        m_Cube = GameObject.Find("Cube");
        Debug.Assert(m_Cube);

        // get the slider component
        m_Slider = GetComponent<Slider>();
        Debug.Assert(m_Slider);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        // get object renderers
        MeshRenderer[] renderers = m_Cube.GetComponentsInChildren<MeshRenderer>();

        // iterate through object renderers
        foreach (MeshRenderer renderer in renderers)
            // if dither shader, change its transparency
            renderer.material.SetFloat("_Transparency", m_Slider.value);
    }
}

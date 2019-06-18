using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Key : MonoBehaviour
{
    private Plane m_KeyHighLight;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_KeyHighLight = GetComponentInChildren<Plane>();
        //Debug.Assert(m_KeyHighLight != null);
//m_KeyHighLight.
    }
}

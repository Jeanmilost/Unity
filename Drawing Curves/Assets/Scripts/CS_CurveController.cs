using System;
using UnityEngine;

/**
* Provides a curve controller
*@author Jean-Milost Reymond
*/
public class CS_CurveController : MonoBehaviour
{
    /**
    * Point type to use to show the curve
    *@note These values means:
    *      - IE_PT_Line:   Each point composing the line are connected by segments, drawing thus a continuous line
    *      - IE_PT_Sphere: Each point composing the line is drawn with a sphere
    */
    public enum IEPointType
    {
        IE_PT_Line,
        IE_PT_Sphere
    }

    /**
    * Contains the sphere parameters, in case spheres are drawn
    */
    [Serializable]
    public class ISphere
    {
        // radius
        [Tooltip("Sphere radius")]
        public float m_Radius = 0.02f;

        // longitude division count
        [Tooltip("Sphere division count on the longitude")]
        public int m_LongitudeDivCount = 24;

        // latitude division count
        [Tooltip("Sphere division count on the latitude")]
        public int m_LatitudeDivCount = 16;

        /**
        * Create the sphere mesh
        *@return mesh containing the sphere
        */
        public Mesh CreateMesh()
        {
            return CS_Primitives.CreateSphere(m_Radius, m_LongitudeDivCount, m_LatitudeDivCount, true, false);
        }
    }

    public ISphere m_Sphere = new ISphere();

    // start from object
    [Tooltip("The object from which the curve should start")]
    public GameObject m_StartObject;

    // control point object
    [Tooltip("The object used as control handle for the curve")]
    public GameObject m_ControlObject;

    // end to object
    [Tooltip("The object at which the curve should end")]
    public GameObject m_EndObject;

    // point type
    [Tooltip("The curve item point type")]
    public IEPointType m_PointType = IEPointType.IE_PT_Sphere;

    // material
    [Tooltip("The material to use to draw the curve items")]
    public Material m_Material;

    // power color
    [Tooltip("The color for the line segment showing the accumulated power (similar to a progress bar)")]
    public Color m_PowerColor = new Color(0.05f, 0.6f, 0.15f);

    // point count
    [Tooltip("The point count (be careful, the more dots, the slower may be the drawing)")]
    public int m_Points = 20;

    // progress bar position
    [Tooltip("The progress bar position, between 0.0f and 1.0f")]
    public float m_ProgressBarPos;

    // fade out
    [Tooltip("If true, the curve will fade out. The more the distance is close to the end point, the more the points are transparent")]
    public bool m_FadeOut = true;

    // ignore first on fade
    [Tooltip("If true, the first point will not be drawn when the fade out option is enabled")]
    public bool m_IgnoreFirstOnFade = true;

    // fade extent factor
    [Tooltip("The fade extent factor, between 0.0f and 1.0f")]
    public float m_FadeExtentFactor = 0.02f;

    // receive shadows
    [Tooltip("If true, the curve may receive a shadow from the other objects")]
    public bool m_ReceiveShadows;

    // cast shadows
    [Tooltip("If true, the curve will cast shadows")]
    public bool m_CastShadows;

    // allows the hit
    [Tooltip("If true, the curve will stop when it hits an object in the scene")]
    public bool m_AllowHit;

    private GameObject   m_Curve;
    private GameObject   m_Line;
    private Mesh         m_SphereMesh;
    private LineRenderer m_LineRenderer;

    /**
    * Called to check if a hit between the curve and the hit object is allowed
    *@param sender - event sender
    *@param hitCollider - the curve hit collider
    *@return true if the hit should be ignored, otherwise false
    */
    public delegate bool OnDoIgnoreHitEvent(object sender, Collider hitCollider);

    /**
    * Gets or sets the curve start point
    */
    public Vector3 StartPoint { get; set; }

    /**
    * Gets or sets the curve control point
    */
    public Vector3 ControlPoint { get; set; }

    /**
    * Gets or sets the curve end point
    */
    public Vector3 EndPoint { get; set; }

    /**
    * Gets or sets the OnDoIgnoreHit event
    */
    public OnDoIgnoreHitEvent OnDoIgnoreHit { get; set; }

    /**
    * Starts the script
    */
    void Start()
    {
        m_SphereMesh = m_Sphere.CreateMesh();

        // get the main curve object
        m_Curve = GameObject.Find("Curve");
        Debug.Assert(m_Curve);

        // get the line object
        m_Line = GameObject.Find("Line");
        Debug.Assert(m_Line);

        // get the line renderer
        m_LineRenderer = m_Line.GetComponent<LineRenderer>();
        Debug.Assert(m_LineRenderer);
    }

    /**
    * Updates the scene (synchronous, once per frame)
    */
    void Update()
    {
        // if a start object was defined, use it
        if (m_StartObject)
            StartPoint = m_StartObject.transform.position;

        // if a control object was defined, use it
        if (m_ControlObject)
            ControlPoint = m_ControlObject.transform.position;

        // if an end object was defined, use it
        if (m_EndObject)
            EndPoint = m_EndObject.transform.position;

        Vector3[] positions      = new Vector3[m_Points];
        Vector3   hitPoint       = Vector3.zero;
        int       powerThreshold = (int)(m_Points * m_ProgressBarPos);
        int       posCount       = 0;

        // iterate through points
        for (int i = 0; i < m_Points; ++i)
        {
            // calculate line segment start and end positions on the line
            float startPos =  i      / (float)m_Points;
            float endPos   = (i + 1) / (float)m_Points;

            // calculate line segment start and end points
            Vector3 startPoint = CS_BezierCurve.GetQuadraticBezierPoint(StartPoint, EndPoint, ControlPoint, startPos);
            Vector3 endPoint   = CS_BezierCurve.GetQuadraticBezierPoint(StartPoint, EndPoint, ControlPoint, endPos);

            // ignore the first item if needed
            if (m_FadeOut && m_IgnoreFirstOnFade && i == 0)
                continue;

            // process the next line position
            switch (m_PointType)
            {
                case IEPointType.IE_PT_Sphere:
                {
                    Material mat;

                    // do apply a fade out on the line?
                    if (m_FadeOut)
                    {
                        // calculate next alpha value to apply. NOTE multiply by 50 because this was the value where
                        // the extent roughly filled the entire curve
                        float alpha = ((m_Points - (i * (m_FadeExtentFactor * 50))) * m_Material.color.a) / m_Points;

                        // is alpha value out of bounds?
                        if (alpha <= 0)
                            break;
                        else
                        if (alpha > 1.0f)
                            alpha = 1.0f;

                        Color itemColor =
                                (i >= powerThreshold ? new Color(m_Material.color.r, m_Material.color.g, m_Material.color.b, alpha)
                                                     : new Color(m_PowerColor.r,     m_PowerColor.g,     m_PowerColor.b,     alpha));

                        // create a new material to draw the next sphere
                        mat = new Material(m_Material)
                        {
                            color = itemColor
                        };

                        // change the emission color
                        mat.SetColor("_EmissionColor", itemColor);
                    }
                    else
                    {
                        Color itemColor =
                                (i >= powerThreshold ? new Color(m_Material.color.r, m_Material.color.g, m_Material.color.b, m_Material.color.a)
                                                     : new Color(m_PowerColor.r,     m_PowerColor.g,     m_PowerColor.b,     m_PowerColor.a));

                        mat = new Material(m_Material)
                        {
                            color = itemColor
                        };

                        // change the emission color
                        mat.SetColor("_EmissionColor", itemColor);
                    }

                    // draw the next sphere
                    Graphics.DrawMesh(m_SphereMesh,
                                      startPoint,
                                      Quaternion.identity,
                                      mat,
                                      m_Curve.layer,
                                      null,
                                      0,
                                      null,
                                      m_CastShadows,
                                      m_ReceiveShadows);

                    break;
                }

                case IEPointType.IE_PT_Line:
                {
                    // calculate the index to use in the destination array
                    int index = (m_FadeOut && m_IgnoreFirstOnFade) ? i - 1 : i;

                    // update the array position count and set the next line point
                    posCount  = index;
                    positions.SetValue(transform.InverseTransformPoint(startPoint), index);
                    break;
                }
            }

            // detect if line hit with something in the scene
            if (m_AllowHit && i < m_Points - 1)
                if (CheckHit(startPoint, endPoint, ref hitPoint))
                    break;
        }

        // render the line, otherwise disable it
        if (m_PointType == IEPointType.IE_PT_Line)
        {
            // copy the positions and resize the array in case it was truncated
            Vector3[] pos = new Vector3[posCount];
            Array.Copy(positions, pos, posCount);

            // set the start and end colors
            m_LineRenderer.startColor = m_Material.color;
            m_LineRenderer.endColor   = m_PowerColor;

            Gradient gradient = new Gradient();

            // configure the line color
            if (m_FadeOut)
                gradient.SetKeys(new GradientColorKey[]{new GradientColorKey(m_PowerColor, m_ProgressBarPos), new GradientColorKey(m_Material.color, m_ProgressBarPos + 0.001f)},
                                 new GradientAlphaKey[]{new GradientAlphaKey(1.0f,         0.0f),             new GradientAlphaKey(0.0f,             Mathf.Max(1.0f - m_FadeExtentFactor, 0.001f))});
            else
                gradient.SetKeys(new GradientColorKey[]{new GradientColorKey(m_PowerColor, m_ProgressBarPos), new GradientColorKey(m_Material.color, m_ProgressBarPos + 0.01f)},
                                 new GradientAlphaKey[]{new GradientAlphaKey(1.0f,         0.0f),             new GradientAlphaKey(1.0f,             1.0f)});

            m_LineRenderer.colorGradient = gradient;

            // configure the shadow
            m_LineRenderer.shadowCastingMode = m_CastShadows ? UnityEngine.Rendering.ShadowCastingMode.TwoSided : UnityEngine.Rendering.ShadowCastingMode.Off;
            m_LineRenderer.receiveShadows    = m_ReceiveShadows;

            // draw the line
            m_LineRenderer.positionCount = posCount;
            m_LineRenderer.SetPositions(pos);
            m_LineRenderer.gameObject.SetActive(true);
        }
        else
            // line isn't used, disable the renderer
            m_LineRenderer.gameObject.SetActive(false);
    }

    /**
    * Checks if a line segment hit something in the scene
    *@param start - line segment start position
    *@param end - line segment end position
    *@param[out] hitPoint - hit point, if line segment hit something
    *@return true if line segment hit something, otherwise false
    */
    bool CheckHit(Vector3 start, Vector3 end, ref Vector3 hitPoint)
    {
        // check if hit point was found
        if (!Physics.Raycast(start, (end - start).normalized, out RaycastHit hit, (end - start).magnitude))
            return false;

        // found it?
        if (!hit.collider)
            return false;

        // check if several scene objects should be exluded
        if (OnDoIgnoreHit != null && OnDoIgnoreHit(this, hit.collider))
            return false;

        // found hit, get its point
        hitPoint = hit.point;
        return true;
    }
}

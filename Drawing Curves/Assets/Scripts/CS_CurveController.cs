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
    *      - IE_PT_Surface: Each point composing the line is drawn with a surface
    *      - IE_PT_Sphere:  Each point composing the line is drawn with a sphere
    *      - IE_PT_Line:    Each point composing the line are connected by segments, drawing thus a continuous line (WARNING may be slow)
    */
    public enum IEPointType
    {
        IE_PT_Line,
        IE_PT_Surface,
        IE_PT_Sphere
    }

    /**
    * Contains the line parameters, in case line is drawn
    */
    [Serializable]
    public class Line
    {
        // width
        [Tooltip("Line width")]
        public float m_Width = 0.2f;
    }

    /**
    * Contains the surface parameters, in case surfaces are drawn
    */
    [Serializable]
    public class Surface
    {
        // width
        [Tooltip("Surface width")]
        public float m_Width = 0.02f;

        // height
        [Tooltip("Surface height")]
        public float m_Height = 0.02f;

        /**
        * Create the surface  mesh
        *@return mesh containing the surface
        */
        public Mesh CreateMesh()
        {
            return CS_Primitives.CreateSurface(m_Width, m_Height, true, true);
        }
    }

    /**
    * Contains the sphere parameters, in case spheres are drawn
    */
    [Serializable]
    public class Sphere
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
            return CS_Primitives.CreateSphere(m_Radius, m_LongitudeDivCount, m_LatitudeDivCount, true, true);
        }
    }

    public Line    m_Line    = new Line();
    public Surface m_Surface = new Surface();
    public Sphere  m_Sphere  = new Sphere();

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
    [Tooltip("The fade extent factor")]
    public float m_FadeExtentFactor = 1.5f;

    // receive shadows
    [Tooltip("If true, the curve may receive a shadow from the other objects")]
    public bool m_ReceiveShadows;

    // cast shadows
    [Tooltip("If true, the curve will cast shadows")]
    public bool m_CastShadows;

    // allows the hit
    [Tooltip("If true, the curve will stop when it hits an object in the scene")]
    public bool m_AllowHit;

    private GameObject m_Curve;
    private Mesh       m_SurfaceMesh;
    private Mesh       m_SphereMesh;

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
        m_SurfaceMesh = m_Surface.CreateMesh();
        m_SphereMesh  = m_Sphere.CreateMesh();

        // get the main curve object
        m_Curve = GameObject.Find("Curve");
        Debug.Assert(m_Curve);
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

        Vector3 hitPoint       = Vector3.zero;
        int     powerThreshold = (int)((float)m_Points * m_ProgressBarPos);

        // iterate through points
        for (int i = 0; i < m_Points; ++i)
        {
            // calculate line segment start and end positions on the line
            float startPos =  i      / (float)m_Points;
            float endPos   = (i + 1) / (float)m_Points;

            // calculate line segment start and end points
            Vector3 startPoint = CS_BezierCurve.GetQuadraticBezierPoint(StartPoint, EndPoint, ControlPoint, startPos);
            Vector3 endPoint   = CS_BezierCurve.GetQuadraticBezierPoint(StartPoint, EndPoint, ControlPoint, endPos);

            Material mat;

            // do apply a fade out on the line?
            if (m_FadeOut)
            {
                // ignore the first item if needed
                if (m_IgnoreFirstOnFade && i == 0)
                    continue;

                // calculate next alpha value to apply
                float alpha = ((m_Points - (i * m_FadeExtentFactor)) * m_Material.color.a) / m_Points;

                // is alpha value out of bounds?
                if (alpha <= 0)
                    break;
                else
                if (alpha > 1.0f)
                    alpha = 1.0f;

                Color itemColor = (i >= powerThreshold ? new Color(m_Material.color.r, m_Material.color.g, m_Material.color.b, alpha)
                                                       : new Color(m_PowerColor.r,     m_PowerColor.g,     m_PowerColor.b,     alpha));

                // create a new material to draw the next line segment
                mat = new Material(m_Material)
                {
                    color = itemColor
                };

                // change the emission color
                mat.SetColor("_EmissionColor", itemColor);
            }
            else
            {
                Color itemColor = (i >= powerThreshold ? new Color(m_Material.color.r, m_Material.color.g, m_Material.color.b, m_Material.color.a)
                                                       : new Color(m_PowerColor.r,     m_PowerColor.g,     m_PowerColor.b,     m_PowerColor.a));

                mat = new Material(m_Material)
                {
                    color = itemColor
                };

                // change the emission color
                mat.SetColor("_EmissionColor", itemColor);
            }

            // draw sight line point
            switch (m_PointType)
            {
                case IEPointType.IE_PT_Surface: Graphics.DrawMesh(m_SurfaceMesh, startPoint, Quaternion.identity, mat, m_Curve.layer, null, 0, null, m_CastShadows, m_ReceiveShadows); break;
                case IEPointType.IE_PT_Sphere:  Graphics.DrawMesh(m_SphereMesh,  startPoint, Quaternion.identity, mat, m_Curve.layer, null, 0, null, m_CastShadows, m_ReceiveShadows); break;
                case IEPointType.IE_PT_Line:
                {
                    // calculate and get the line segment (NOTE this way it's not optimized. Matrix should be used instead)
                    Mesh lineSegmentMesh = CreateLineSegment(startPoint, endPoint, m_Line.m_Width);

                    // draw the line segment
                    Graphics.DrawMesh(lineSegmentMesh, Matrix4x4.identity, mat, m_Curve.layer, null, 0, null, m_CastShadows, m_ReceiveShadows);
                    break;
                }
            }

            // do detect if line hit with something?
            if (m_AllowHit && i < m_Points - 1)
                if (CheckHit(startPoint, endPoint, ref hitPoint))
                    break;
        }
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
        if (!Physics.Raycast(start, end, out RaycastHit hit))
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

    /**
    * Create a mesh containing a line segment between 2 points
    *@param start - start point
    *@param end - end point
    *@param lineWidth - line width
    *@return mesh containing line segment
    */
    Mesh CreateLineSegment(Vector3 start, Vector3 end, float lineWidth)
    {
        Vector3[] vertices  = new Vector3[4];
        Vector3[] normals   = new Vector3[4];
        Vector2[] texCoords = new Vector2[4];
        int[]     indices   = new int[6];

        // calculate surface normal and side
        Vector3 normal = Vector3.Cross(start,  end);
        Vector3 side   = Vector3.Cross(normal, end - start);

        side.Normalize();

        // create vertices
        vertices[0] = start + side * (lineWidth /  2.0f);
        vertices[1] = start + side * (lineWidth / -2.0f);
        vertices[2] = end   + side * (lineWidth /  2.0f);
        vertices[3] = end   + side * (lineWidth / -2.0f);

        // populate texture coordinates
        texCoords[0] = new Vector2(0.0f, 0.0f);
        texCoords[1] = new Vector2(0.0f, 1.0f);
        texCoords[2] = new Vector2(1.0f, 0.0f);
        texCoords[3] = new Vector2(1.0f, 1.0f);

        // populate normals
        for (int i = 0; i < 4; ++i)
            normals[i] = normal;

        // populate indices
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 2;
        indices[3] = 3;
        indices[4] = 2;
        indices[5] = 1;

        // create the mesh
        Mesh mesh = new Mesh
        {
            vertices  = vertices,
            normals   = normals,
            uv        = texCoords,
            triangles = indices
        };

        mesh.RecalculateBounds();

        return mesh;
    }
}

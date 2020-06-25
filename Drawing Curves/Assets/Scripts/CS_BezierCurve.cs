using UnityEngine;

/**
* Provides functions to calculate Bezier curves
*@author Jean-Milost Reymond
*/
public class CS_BezierCurve
{
    /**
    * Calculates a point on a quadratic Bezier curve
    *@param start - Bezier curve start coordinate
    *@param end - Bezier curve end coordinate
    *@param control - Bezier curve control point coordinate
    *@param position - position of the point to find in percent (between 0.0f and 1.0f)
    *@return quadratic Bezier point coordinate matching with position
    */
    public static Vector2 GetQuadraticBezierPoint(Vector2 start, Vector2 end, Vector2 control, float position)
    {
        // point p0p1 is the point on the line formed by the start position and the control point, and
        // point p1p2 is the point on the line formed by the control point and the line end
        Vector2 p0p1 = GetPointOnLine(start, control, position);
        Vector2 p1p2 = GetPointOnLine(control, end, position);

        // the resulting point is the point found on the intermediate segment (p0p1 to p1p2)
        return GetPointOnLine(p0p1, p1p2, position);
    }

    /**
    * Calculates a point on a quadratic Bezier curve
    *@param start - Bezier curve start coordinate
    *@param end - Bezier curve end coordinate
    *@param control - Bezier curve control point coordinate
    *@param position - position of the point to find in percent (between 0.0f and 1.0f)
    *@return quadratic Bezier point coordinate matching with position
    */
    public static Vector3 GetQuadraticBezierPoint(Vector3 start, Vector3 end, Vector3 control, float position)
    {
        // point p0p1 is the point on the line formed by the start position and the control point, and
        // point p1p2 is the point on the line formed by the control point and the line end
        Vector3 p0p1 = GetPointOnLine(start, control, position);
        Vector3 p1p2 = GetPointOnLine(control, end, position);

        // the resulting point is the point found on the intermediate segment (p0p1 to p1p2)
        return GetPointOnLine(p0p1, p1p2, position);
    }

    /**
    * Calculates a point on a cubic Bezier curve
    *@param start - Bezier curve start coordinate
    *@param end - Bezier curve end coordinate
    *@param control1 - first Bezier curve control point coordinate
    *@param control2 - second Bezier curve control point coordinate
    *@param position - position of the point to find in percent (between 0.0f and 1.0f)
    *@return cubic Bezier point coordinate matching with position
    */
    public static Vector2 GetCubicBezierPoint(Vector2 start, Vector2 end, Vector2 control1, Vector2 control2, float position)
    {
        // point p0p1 is the point on the line formed by the start position and the first control point,
        // point p1p2 is the point on the line formed by the first control point and the second control
        // point, and point p2p3 is the point on the line formed by the second control point and the
        // line end
        Vector2 p0p1 = GetPointOnLine(start, control1, position);
        Vector2 p1p2 = GetPointOnLine(control1, control2, position);
        Vector2 p2p3 = GetPointOnLine(control2, end, position);

        // the resulting point is the quadratic bezier point found on the intermediate segments (p0p1 to
        // p1p2 and p1p2 to p2p3)
        return GetQuadraticBezierPoint(p0p1, p2p3, p1p2, position);
    }

    /**
    * Calculates a point on a cubic Bezier curve
    *@param start - Bezier curve start coordinate
    *@param end - Bezier curve end coordinate
    *@param control1 - first Bezier curve control point coordinate
    *@param control2 - second Bezier curve control point coordinate
    *@param position - position of the point to find in percent (between 0.0f and 1.0f)
    *@return cubic Bezier point coordinate matching with position
    */
    public static Vector3 GetCubicBezierPoint(Vector3 start, Vector3 end, Vector3 control1, Vector3 control2, float position)
    {
        // point p0p1 is the point on the line formed by the start position and the first control point,
        // point p1p2 is the point on the line formed by the first control point and the second control
        // point, and point p2p3 is the point on the line formed by the second control point and the
        // line end
        Vector3 p0p1 = GetPointOnLine(start, control1, position);
        Vector3 p1p2 = GetPointOnLine(control1, control2, position);
        Vector3 p2p3 = GetPointOnLine(control2, end, position);

        // the resulting point is the quadratic bezier point found on the intermediate segments (p0p1 to
        // p1p2 and p1p2 to p2p3)
        return GetQuadraticBezierPoint(p0p1, p2p3, p1p2, position);
    }

    /**
    * Calculates a point on a line
    *@param start - line start coordinate
    *@param end - line end coordinate
    *@param position - position of the point to find in percent (between 0.0f and 1.0f)
    *@return point coordinates on the line
    */
    static Vector2 GetPointOnLine(Vector2 start, Vector2 end, float position)
    {
        return (start + ((end - start) * position));
    }

    /**
    * Calculates a point on a line
    *@param start - line start coordinate
    *@param end - line end coordinate
    *@param position - position of the point to find in percent (between 0.0f and 1.0f)
    *@return point coordinates on the line
    */
    static Vector3 GetPointOnLine(Vector3 start, Vector3 end, float position)
    {
        return (start + ((end - start) * position));
    }
}

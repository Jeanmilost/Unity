using UnityEngine;

/**
* Provides functions to create simple primitives meshes like surface, sphere, ...
*@author Jean-Milost Reymond
*/
public class CS_Primitives
{
    /**
    * Creates a mesh containing a surface
    *@param width - surface width
    *@param height - surface height
    *@param hasNormals - if true, normals will be added to the mesh vertex buffer
    *@param hasTexCoords - if true, texture coordinates will be added to the mesh vertex buffer
    *@return mesh containing the surface
    */
    static public Mesh CreateSurface(float width, float height, bool hasNormals, bool hasTexCoords)
    {
        Vector3[] vertices  = new Vector3[4];
        Vector3[] normals   = new Vector3[4];
        Vector2[] texCoords = new Vector2[4];
        int[]     indices   = new int[6];

        // create a buffer template: 0 for negative values, 1 for positive
        int[] bufferTemplate =
        {
            0, 0,
            0, 1,
            1, 0,
            1, 1
        };

        // iterate through vertex to create
        for (uint i = 0; i < 4; ++i)
        {
            // calculate template buffer index
            uint index = i * 2;

            // populate vertex buffer
            if (bufferTemplate[index] != 0)
                vertices[i].x =  width / 2.0f;
            else
                vertices[i].x = -width / 2.0f;

            if (bufferTemplate[index + 1] != 0)
                vertices[i].y =  height / 2.0f;
            else
                vertices[i].y = -height / 2.0f;

            vertices[i].z = 0.0f;

            // vertex has a normal?
            if (hasNormals)
            {
                normals[i].x =  0.0f;
                normals[i].y =  0.0f;
                normals[i].z = -1.0f;
            }

            // vertex has UV texture coordinates?
            if (hasTexCoords)
            {
                // calculate texture u coordinate
                if (bufferTemplate[index] != 0)
                    texCoords[i].x = 1.0f;
                else
                    texCoords[i].x = 0.0f;

                // calculate texture v coordinate
                if (bufferTemplate[index + 1] != 0)
                    texCoords[i].y = 1.0f;
                else
                    texCoords[i].y = 0.0f;
            }
        }

        // populate faces
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
            triangles = indices
        };

        // add normals, if required
        if (hasNormals)
            mesh.normals = normals;

        // add texture coordinates, if required
        if (hasTexCoords)
            mesh.uv = texCoords;

        mesh.RecalculateBounds();

        return mesh;
    }

    /**
    * Creates a mesh containing a sphere
    *@param radius - sphere radius
    *@param lonDivCount - division count on the sphere longitude
    *@param latDivCount - division count on the sphere latitude
    *@param hasNormals - if true, normals will be added to the mesh vertex buffer
    *@param hasTexCoords - if true, texture coordinates will be added to the mesh vertex buffer
    *@return mesh containing the sphere
    */
    static public Mesh CreateSphere(float radius, int lonDivCount, int latDivCount, bool hasNormals, bool hasTexCoords)
    {
        // calculate vertex buffer length
        int bufLen = (lonDivCount + 1) * latDivCount + 2;

        Vector3[] vertices  = new Vector3[bufLen];
        Vector3[] normals   = new Vector3[bufLen];
        Vector2[] texCoords = new Vector2[bufLen];
        float     pi2       = Mathf.PI * 2f;

        // add first vertex
        vertices[0] = Vector3.up * radius;

        // add first texture coordinate (if required)
        if (hasTexCoords)
            texCoords[0] = Vector2.up;

        // iterate through latitude divisions to create
        for (int lat = 0; lat < latDivCount; ++lat)
        {
            // calculate next latitude division
            float a1   = Mathf.PI * ((lat + 1) / (float)(latDivCount + 1));
            float sin1 = Mathf.Sin(a1);
            float cos1 = Mathf.Cos(a1);

            // iterate through longitude divisions to create
            for (int lon = 0; lon <= lonDivCount; ++lon)
            {
                // calculate next longitude division
                float a2   = pi2 * (lon == lonDivCount ? 0 : lon) / lonDivCount;
                float sin2 = Mathf.Sin(a2);
                float cos2 = Mathf.Cos(a2);

                // calculate next index in vertex buffer
                int index = lon + lat * (lonDivCount + 1) + 1;

                // add vertex
                vertices[index] = new Vector3(sin1 * cos2, cos1, sin1 * sin2) * radius;

                // add normal (if required)
                if (hasNormals)
                    normals[index] = vertices[index].normalized;

                // add texture coordinate (if required)
                if (hasTexCoords)
                    texCoords[index] = new Vector2((float)lon / lonDivCount, 1f - (float)(lat + 1) / (latDivCount + 1));
            }
        }

        // add last vertex
        vertices[bufLen - 1] = Vector3.up * -radius;

        // Add first and last normals (if required)
        if (hasNormals)
        {
            normals[0]          = vertices[0].normalized;
            normals[bufLen - 1] = vertices[bufLen - 1].normalized;
        }

        // add last texture coordinate (if required)
        if (hasTexCoords)
            texCoords[bufLen - 1] = Vector2.zero;

        int   nbFaces     = bufLen;
        int   nbTriangles = nbFaces     * 2;
        int   nbIndexes   = nbTriangles * 3;
        int[] indices     = new int[nbIndexes];

        int i = 0;

        // calculate and add top vertex indices
        for (int lon = 0; lon < lonDivCount; ++lon)
        {
            indices[i] = lon + 2; ++i;
            indices[i] = lon + 1; ++i;
            indices[i] = 0;       ++i;
        }

        // calculate and add middle vertex indices
        for (int lat = 0; lat < latDivCount - 1; ++lat)
            for (int lon = 0; lon < lonDivCount; ++lon)
            {
                int current = lon + lat * (lonDivCount + 1) + 1;
                int next    = current + lonDivCount + 1;

                indices[i] = current;     ++i;
                indices[i] = current + 1; ++i;
                indices[i] = next + 1;    ++i;

                indices[i] = current;  ++i;
                indices[i] = next + 1; ++i;
                indices[i] = next;     ++i;
            }

        // calculate and add bottom vertex indices
        for (int lon = 0; lon < lonDivCount; ++lon)
        {
            indices[i] = bufLen - 1;             ++i;
            indices[i] = bufLen - (lon + 2) - 1; ++i;
            indices[i] = bufLen - (lon + 1) - 1; ++i;
        }

        // create the mesh
        Mesh mesh = new Mesh
        {
            vertices  = vertices,
            triangles = indices
        };

        // add normals, if required
        if (hasNormals)
            mesh.normals = normals;

        // add texture coordinates, if required
        if (hasTexCoords)
            mesh.uv = texCoords;

        mesh.RecalculateBounds();

        return mesh;
    }
}

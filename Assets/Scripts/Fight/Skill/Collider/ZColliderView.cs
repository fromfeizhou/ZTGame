using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZColliderView {

    public static GameObject CreateColliderView(CollBase collider)
    {
        #if UNITY_EDITOR
        GameObject prefab = UnityEditor.AssetDatabase.LoadMainAssetAtPath("Assets/ResourcesLib/Skill/Prefabs/collider.prefab") as GameObject;
        if (null != prefab)
        {
            GameObject colliderView = GameObject.Instantiate(prefab);


            MeshFilter filter = colliderView.AddComponent<MeshFilter>();
            if (filter != null)
            {
                switch (collider.ColliderType)
                {
                    case CollBase.ColType.RECTANGLE:
                        filter.sharedMesh = CreateRectangleMesh(collider);
                        break;
                    case CollBase.ColType.SECTOR:
                        filter.sharedMesh = CreateSectorMesh(collider);
                        break;
                    default:
                        filter.sharedMesh = CreateCircleMesh(collider);
                        break;
                }
            }

            MeshRenderer meshRender = colliderView.AddComponent<MeshRenderer>();
            Shader shader = Shader.Find("Diffuse");
            meshRender.sharedMaterial = new Material(shader);
            return colliderView;
        }
#endif
        return null;

    }

    private static Mesh CreateCircleMesh(CollBase collider)
    {
        int segment = 60;
        float radius = (collider as CollRadius).Radius;
        Mesh mesh = new Mesh();
        int vlen = 1 + segment;
        Vector3[] vertices = new Vector3[vlen];
        vertices[0] = Vector3.zero;

        float angleDegree = 360;
        float angle = Mathf.Deg2Rad * angleDegree;
        float currAngle = angle / 2;
        float deltaAngle = angle / segment;
        for (int i = 1; i < vlen; i++)
        {
            float cosA = Mathf.Cos(currAngle);
            float sinA = Mathf.Sin(currAngle);
            vertices[i] = new Vector3(cosA * radius, 0, sinA * radius);
            currAngle -= deltaAngle;
        }

        int tlen = segment * 3;
        int[] triangles = new int[tlen];
        for (int i = 0, vi = 1; i < tlen - 3; i += 3, vi++)
        {
            triangles[i] = 0;
            triangles[i + 1] = vi;
            triangles[i + 2] = vi + 1;
        }
        triangles[tlen - 3] = 0;
        triangles[tlen - 2] = vlen - 1;
        triangles[tlen - 1] = 1;  //链接第一个点

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }

    private static Mesh CreateRectangleMesh(CollBase collider)
    {
        CollRectange coll = collider as CollRectange;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-coll.width * 0.5f, 0, -coll.height * 0.5f);
        vertices[1] = new Vector3(coll.width * 0.5f, 0, -coll.height * 0.5f);
        vertices[2] = new Vector3(coll.width * 0.5f, 0, coll.height * 0.5f);
        vertices[3] = new Vector3(-coll.width * 0.5f, 0, coll.height * 0.5f);
        int[] triangles = new int[6] { 0, 3, 2, 2, 1, 0 };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }

    private static Mesh CreateSectorMesh(CollBase collider)
    {
        CollSector coll = collider as CollSector;
        float insideRaius = coll.inCircle;
        float radius = coll.outCircle;
        int segment = 60;
        float angleDegree = coll.angle;

        Mesh mesh = new Mesh();

        int vlen = segment * 2 + 2;
        Vector3[] vertices = new Vector3[vlen];

        float angle = Mathf.Deg2Rad * angleDegree;
        float currAngle = angle / 2;
        float deltaAngle = angle / segment;
        for (int i = 0; i < vlen; i += 2)
        {
            float cosA = Mathf.Cos(currAngle);
            float sinA = Mathf.Sin(currAngle);
            vertices[i] = new Vector3(cosA * insideRaius, 0, sinA * insideRaius);
            vertices[i + 1] = new Vector3(cosA * radius, 0, sinA * radius);
            currAngle -= deltaAngle;
        }

        int tlen = segment * 6;
        int[] triangles = new int[tlen];
        for (int i = 0, vi = 0; i < tlen; i += 6, vi += 2)
        {
            triangles[i] = vi;
            triangles[i + 1] = vi + 1;
            triangles[i + 2] = vi + 3;
            triangles[i + 3] = vi + 3;
            triangles[i + 4] = vi + 2;
            triangles[i + 5] = vi;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        return mesh;
    }
}

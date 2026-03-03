using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class VisionCone : MonoBehaviour
{
    public float viewRadius = 6f;
    [Range(0, 360)]
    public float viewAngle = 90f;

    public int rayCount = 50;
    public LayerMask obstacleMask;
    public LayerMask targetMask;

    [Header("Mesh Settings")]
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Visual Settings")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Color normalColor = new Color(1, 1, 1, 0.3f);
    [SerializeField] private Color detectedColor= new Color(1,0,0,0.3f);
    private Mesh mesh;

    private bool targetVisible;
    MaterialPropertyBlock mpb;
    void Start()
    {
        
        mesh = new Mesh();
        mpb = new MaterialPropertyBlock();

        if (meshRenderer) meshRenderer.material = normalMaterial;
        if (meshFilter)meshFilter.mesh = mesh;

        if (normalMaterial) ChangeColor(normalColor);


    }

    void LateUpdate()
    {
        DrawFieldOfView();
        if(IsTargetVisible())
        {
            if(targetVisible) return;
            if (normalMaterial) ChangeColor(detectedColor);
            targetVisible = true;
        }
        else
        {
            if(!targetVisible) return;
            if (normalMaterial) ChangeColor(normalColor);
            targetVisible = false;
        }
    }

    void DrawFieldOfView()
    {
        float angle = transform.eulerAngles.z + 90f - viewAngle / 2f;
        float angleIncrease = viewAngle / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 2];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = Vector3.zero; // Center of the cone

        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 dir = DirFromAngle(angle);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, viewRadius, obstacleMask);
            if (hit.collider != null)
                vertices[i+1] = (transform.InverseTransformPoint(hit.point));
            else
                vertices[i+1] = (transform.InverseTransformPoint(transform.position + dir * viewRadius));

            angle += angleIncrease;
        }

        for (int i = 0; i < rayCount; i++)
        {
            triangles[i*3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    Vector3 DirFromAngle(float angleInDegrees)
    {
        float rad = angleInDegrees * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    public bool IsTargetVisible()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        foreach(var col in cols)
        {
            Vector2 dirToTarget = (col.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(transform.up, dirToTarget);

            if (angle > viewAngle / 2f)
                return false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget, viewRadius, obstacleMask);

            if (hit.collider == null)
                return true;

        }
        //if (Vector2.Distance(transform.position, target.position) > viewRadius)
        //    return false;

        //Vector2 dirToTarget = (target.position - transform.position).normalized;

        return false;
    }
    void ChangeColor(Color col)
    {
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", col);
        meshRenderer.SetPropertyBlock(mpb);
    }
}
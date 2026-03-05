using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

[RequireComponent(typeof(Light2D))]
public class SmoothFreeformVision : MonoBehaviour
{
    [System.Serializable]
    public class VisionSettings
    {
        public float radius = 8f;
        [Range(0, 360)] public float angle = 90f;
        public Color color = Color.white;
        public float intensity = 1f;
        public float falloff = 0.4f;
    }

    [Header("Vision Settings")]
    [SerializeField] private VisionSettings vision = new VisionSettings();

    [Header("Quality")]
    [SerializeField][Range(8, 128)] private int radialSegments = 64;
    [SerializeField] private float updateRate = 0.05f;

    [Header("Detection")]
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask obstacleMask;

    private Light2D light2D;

    private float updateTimer;

    private List<Transform> visibleTargets = new List<Transform>();


    void Awake()
    {
        light2D = GetComponent<Light2D>();

        ConfigureLight();
    }

    void ConfigureLight()
    {
        light2D.lightType = Light2D.LightType.Freeform;

        light2D.color = vision.color;
        light2D.intensity = vision.intensity;
        light2D.falloffIntensity = vision.falloff;

        light2D.shadowIntensity = 1f;
        light2D.shadowVolumeIntensity = 1f;

        UpdateLightShape();
    }


    void Update()
    {
        //updateTimer += Time.deltaTime;

        //if (updateTimer >= updateRate)
        //{
        //    updateTimer = 0f;

        //    UpdateLightShape();
        //    //DetectTargets();
        //}
    }
    private void LateUpdate()
    {
        UpdateLightShape();

    }


    void UpdateLightShape()
    {
        List<Vector3> points = new List<Vector3>();

       

        float startAngle = -vision.angle / 2f;
        float step = vision.angle / (radialSegments - 1);

        for (int i = 0; i < radialSegments; i++)
        {
            float currentAngle = startAngle + step * i;

            Vector3 direction =
                Quaternion.Euler(0, 0, currentAngle) * transform.up;

            float distance = vision.radius;

            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                direction,
                vision.radius,
                obstacleMask
            );

            if (hit.collider != null)
            {
                distance = hit.distance;
            }

            Vector3 localDir =
                Quaternion.Euler(0, 0, currentAngle) * Vector3.up;

            Vector3 localPoint = localDir * distance;

            points.Add(localPoint);
        }
        points.Add(Vector3.zero);
        //Vector3 direction1 =
        //        Quaternion.Euler(0, 0, vision.angle/2f) * transform.up;
        //points.Add(direction1*0.1f);
        //if (points.Count > 2)
        //{
        //    points.Add(points[0]);
        //}

        //light2D.SetShapePath(new Vector3[0]);
        light2D.SetShapePath(points.ToArray());
    }



    //void DetectTargets()
    //{
    //    visibleTargets.Clear();

    //    Collider2D[] targets =
    //        Physics2D.OverlapCircleAll(
    //            transform.position,
    //            vision.radius,
    //            targetMask
    //        );

    //    foreach (Collider2D target in targets)
    //    {
    //        Transform t = target.transform;

    //        Vector2 dir =
    //            (t.position - transform.position).normalized;

    //        float distance =
    //            Vector2.Distance(transform.position, t.position);

    //        float angle =
    //            Vector2.Angle(transform.up, dir);

    //        if (angle <= vision.angle / 2f)
    //        {
    //            RaycastHit2D hit =
    //                Physics2D.Raycast(
    //                    transform.position,
    //                    dir,
    //                    distance,
    //                    obstacleMask
    //                );

    //            if (hit.collider == null ||
    //               hit.collider.transform == t)
    //            {
    //                visibleTargets.Add(t);

    //                Debug.DrawLine(
    //                    transform.position,
    //                    t.position,
    //                    Color.green,
    //                    updateRate
    //                );
    //            }
    //        }
    //    }
    //}


    public bool CanSee(GameObject target)
    {
        return visibleTargets.Contains(target.transform);
    }

    public List<Transform> GetVisibleTargets()
    {
        return visibleTargets;
    }


    public void SetVisionAngle(float newAngle)
    {
        vision.angle = Mathf.Clamp(newAngle, 0, 360);
        UpdateLightShape();
    }

    public void SetVisionRadius(float newRadius)
    {
        vision.radius = Mathf.Max(0, newRadius);
        UpdateLightShape();
    }



    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            vision.radius
        );
    }
}
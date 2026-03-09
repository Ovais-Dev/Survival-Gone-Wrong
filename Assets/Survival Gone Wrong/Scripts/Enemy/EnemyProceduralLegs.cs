using UnityEngine;
using Pathfinding;

public class EnemyProceduralLegs : MonoBehaviour
{
    public LineRenderer leftLeg;
    public LineRenderer rightLeg;

    AIPath aiPath;

    public float legLength = 0.35f;
    public float stepDistance = 0.25f;
    public float stepHeight = 0.15f;
    public float walkSpeed = 6f;
    public float legOffset = 2f;
    float walkCycle;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
    }

    void Update()
    {
        Vector2 velocity = aiPath.velocity;
        //Vector2 vel = aiPath.velocity;

        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        float speed = velocity.magnitude;

        if (speed > 0.05f)
        {
            walkCycle += Time.deltaTime * walkSpeed * speed;
        }

        AnimateLeg(leftLeg, walkCycle, 0);
        AnimateLeg(rightLeg, walkCycle, legOffset);
    }

    void AnimateLeg(LineRenderer leg, float cycle, float offset)
    {
        float phase = cycle + offset;

        float step = Mathf.Sin(phase);
        float lift = Mathf.Abs(Mathf.Cos(phase));

        Vector3 hip = Vector3.zero;

        Vector3 foot = new Vector3(
            step * stepDistance,
            -legLength + lift * stepHeight,
            0
        );

        leg.SetPosition(0, hip);
        leg.SetPosition(1, foot);
    }
}
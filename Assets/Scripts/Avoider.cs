using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Avoider : Wanderer
{
    private SteeringOutput steeringOutput = new SteeringOutput();
    private CollisionAvoidance collisionAvoidance = new CollisionAvoidance();

    public Static[] targets;
    public float radius;
    public bool avoid;
    // Start is called before the first frame update
    protected override void Start()
    {
        collisionAvoidance.character = this;
        collisionAvoidance.maxAcceleration = maxAcceleration;
        collisionAvoidance.radius = radius;
        collisionAvoidance.targets = targets;

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (avoid)
        {
            steeringOutput = collisionAvoidance.getSteering();
            if (steeringOutput != null)
                velocity -= steeringOutput.linear * Time.deltaTime;
        }
        base.Update();
    }
}

class CollisionAvoidance
{
    public Static character;
    public float maxAcceleration;
    public Static[] targets;
    public float radius;

    public SteeringOutput getSteering()
    {
        float shortestTime = Mathf.Infinity;
        Static firstTarget = null;
        float firstMinSeparation = 0;
        float firstDistance = 0;
        Vector3 firstRelativePos;
        Vector3 firstRelativeVel;
        float distance;
        Vector3 relativePos = Vector3.zero;
        Vector2 relativeVel;
        foreach (Static target in targets)
        {
            relativePos = target.transform.position - character.transform.position;
            relativeVel = target.velocity - character.velocity;
            var relativeSpeed = relativeVel.magnitude;
            var timeToCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

            distance = relativePos.magnitude;
            var minSeparation = distance - relativeSpeed * timeToCollision;
            if (minSeparation > 2 * radius) continue;

            if (timeToCollision > 0 && timeToCollision < shortestTime)
            {
                shortestTime = timeToCollision;
                firstTarget = target;
                firstMinSeparation = minSeparation;
                firstDistance = distance;
                firstRelativePos = relativePos;
                firstRelativeVel = relativeVel;
            }
        }

        if (firstTarget == null) return null;

        if (firstMinSeparation <= 0 || firstDistance < 2 * radius)
            relativePos = firstTarget.transform.position - character.transform.position;

        relativePos.Normalize();
        SteeringOutput result = new SteeringOutput();
        result.linear = relativePos * maxAcceleration;
        result.angular = 0;
        return result;
    }
}
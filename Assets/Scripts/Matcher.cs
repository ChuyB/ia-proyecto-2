using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Matcher : Static
{

    Align align = new Align();
    VelocityMatching velocityMatching = new VelocityMatching();

    SteeringOutput steeringOutput = new SteeringOutput();

    public Static target;
    public float maxAngularAcceleration;
    public float maxRotation;
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;
    public float maxAcceleration;

    public bool isAlign;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        align.character = this;
        align.target = target;
        align.maxAngularAcceleration = maxAngularAcceleration;
        align.targetRadius = targetRadius;
        align.maxRotation = maxRotation;
        align.targetRadius = targetRadius;
        align.timeToTarget = timeToTarget;

        velocityMatching.character = this;
        velocityMatching.target = target;
        velocityMatching.timeToTarget = timeToTarget;
        velocityMatching.maxAcceleration = maxAcceleration;

        if (isAlign)
        {
            steeringOutput = align.getSteering();
            transform.Rotate(0, 0, Mathf.Rad2Deg * rotation * Time.deltaTime);
            if (steeringOutput != null)
            {
                rotation += steeringOutput.angular * Time.deltaTime;
            } else
            {
                rotation = 0;
            }
        } else
        {
            steeringOutput = velocityMatching.getSteering();
            transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime);
            velocity += steeringOutput.linear * Time.deltaTime;
        }
    }
}

class Align
{
    public Static character;
    public Static target;
    public float maxAngularAcceleration;
    public float maxRotation;
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();
        float targetRotation;
        float rotation = Mathf.Deg2Rad * (target.transform.rotation.eulerAngles.z - character.transform.rotation.eulerAngles.z);
        rotation = mapToRange(rotation);
        float rotationSize = Mathf.Abs(rotation);

        if (rotationSize < targetRadius)
        {
            return null;
        }

        if (rotationSize > slowRadius)
        {
            targetRotation = maxRotation;
        } else
        {
            targetRotation = maxRotation * rotationSize / slowRadius;
        }

        targetRotation *= rotation / rotationSize;

        result.angular = targetRotation - character.rotation;
        
        result.angular /= timeToTarget;

        float angularAcceleration = Mathf.Abs(result.angular);
        if (angularAcceleration > maxAngularAcceleration)
        {
            result.angular /= angularAcceleration;
            result.angular *= maxAngularAcceleration;
        }

        result.linear = Vector2.zero;
        return result;
    }

    private float mapToRange(float angle)
    {
        return ((angle + Mathf.PI) % (2 * Mathf.PI)) - Mathf.PI;
    }
}

class VelocityMatching
{
    public Static character;
    public Static target;
    public float maxAcceleration;
    public float timeToTarget;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        result.linear = target.velocity - character.velocity;
        result.linear /= timeToTarget;

        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
        }

        result.angular = 0;
        return result;
    }
}
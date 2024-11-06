using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Seeker : Static
{

    private KinematicSeek kinematicSeek = new KinematicSeek();
    private KinematicArrive kinematicArrive = new KinematicArrive();
    private Seek seek = new Seek();
    private Arrive arrive = new Arrive();

    private KinematicSteeringOutput kinematicSteeringOutput = new KinematicSteeringOutput();
    private SteeringOutput seekSteeringOutput = new SteeringOutput();

    public Static target;
    public float maxSpeed;
    public float radius;
    public float timeToTarget;
    public float maxAcceleration;
    public float targetRadius;
    public float slowRadius;
    public float maxDistance;

    public bool isKinematicArrive;
    public bool isKinematic;
    public bool isFleeing;
    public bool isArrive;

    // Start is called before the first frame update
    void Start()
    {
        velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        kinematicSeek.target = target;
        kinematicSeek.character = this;
        kinematicSeek.maxSpeed = maxSpeed;

        kinematicArrive.target = target;
        kinematicArrive.character = this;
        kinematicArrive.maxSpeed = maxSpeed;
        kinematicArrive.timeToTarget = timeToTarget;
        kinematicArrive.radius = radius;

        seek.target = target;
        seek.character = this;
        seek.maxAcceleration = maxAcceleration;

        arrive.character = this;
        arrive.target = target;
        arrive.maxAcceleration = maxAcceleration;
        arrive.maxSpeed = maxSpeed;
        arrive.targetRadius = targetRadius;
        arrive.slowRadius = slowRadius;
        arrive.timeToTarget = timeToTarget;

        if (isKinematic)
        {
            if (isKinematicArrive)
            {
                kinematicSteeringOutput = kinematicArrive.getSteering();
                if (kinematicSteeringOutput == null)
                    return;
            }
            else
            {
                kinematicSteeringOutput = kinematicSeek.getSteering();
            }

            if (isFleeing)
            {
                if ((transform.position - target.transform.position).magnitude < maxDistance)
                {
                    transform.position -= new Vector3(kinematicSteeringOutput.velocity.x * Time.deltaTime, kinematicSteeringOutput.velocity.y * Time.deltaTime);
                }
            }
            else
            {
                transform.position += new Vector3(kinematicSteeringOutput.velocity.x * Time.deltaTime, kinematicSteeringOutput.velocity.y * Time.deltaTime);
            }
        } else
        {
            if (isArrive)
            {
                seekSteeringOutput = arrive.getSteering();
                if (seekSteeringOutput == null)
                    return;
            }
            else
            {
                seekSteeringOutput = seek.getSteering();
            }

            if (isFleeing && (transform.position - target.transform.position).magnitude >= maxDistance)
            {
                transform.position = transform.position;
            } else
            {
                transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
            }


            if (isFleeing)
            {
                if ((transform.position - target.transform.position).magnitude < maxDistance)
                {
                    velocity -= seekSteeringOutput.linear * Time.deltaTime;
                } else
                {
                    velocity = Vector2.zero;
                }
            } else
            {
                velocity += seekSteeringOutput.linear * Time.deltaTime;
            }

            if (velocity.magnitude > maxSpeed)
            {
                velocity.Normalize();
                velocity *= maxSpeed;
            }

        }


    }
}

class KinematicSeek
{
    public Static character;
    public Static target;
    public float maxSpeed;

    public KinematicSteeringOutput getSteering()
    {
        KinematicSteeringOutput result = new KinematicSteeringOutput();

        result.velocity = target.transform.position - character.transform.position;

        result.velocity.Normalize();
        result.velocity *= maxSpeed;

        character.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * character.newOrientation(character.transform.rotation.eulerAngles.y, result.velocity));

        result.rotation = 0;
        return result;
    }
}

class KinematicArrive
{
    public Static character;
    public Static target;
    public float maxSpeed;
    public float radius;
    public float timeToTarget;

    public KinematicSteeringOutput getSteering()
    {
        KinematicSteeringOutput result = new KinematicSteeringOutput();

        result.velocity = target.transform.position - character.transform.position;

        if (result.velocity.magnitude < radius)
            return null;

        result.velocity /= timeToTarget;

        if(result.velocity.magnitude > maxSpeed)
        {
            result.velocity.Normalize();
            result.velocity *= maxSpeed;
        }

        character.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * character.newOrientation(character.transform.rotation.eulerAngles.y, result.velocity));
        result.rotation = 0;
        return result;
    }
}

class KinematicSteeringOutput
{
    public Vector2 velocity;
    public float rotation;
}

class Seek
{
    public Static character;
    public Static target;
    public float maxAcceleration;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        result.linear = target.transform.position - character.transform.position;
        result.linear.Normalize();
        result.linear *= maxAcceleration;

        result.angular = 0;
        return result;
    }
}

class Arrive
{
    public Static character;
    public Static target;
    public float maxAcceleration;
    public float maxSpeed;
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();
        float targetSpeed;
        Vector2 targetVelocity;

        Vector3 direction = target.transform.position - character.transform.position;
        float distance = direction.magnitude;

        if (distance < targetRadius)
            return null;

        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        } else
        {
            targetSpeed = maxSpeed * distance / slowRadius;
        }

        targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        result.linear = targetVelocity - character.velocity;
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
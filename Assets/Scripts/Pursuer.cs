using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuer : Static
{
    private Pursue pursue = new Pursue();
    private LookWhereYoureGoing lookWhereYoureGoing = new LookWhereYoureGoing();
    private SteeringOutput steeringOutput;

    public Static target;
    public float maxPrediction;
    public float maxAcceleration;
    public float maxSpeed;
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;
    public float maxAngularAcceleration;
    public float maxRotation;
    public bool isEvade;
    public float maxDistance;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = new GameObject();
        Static invisibleTarget = obj.AddComponent<Static>();
        invisibleTarget.transform.position = target.transform.position;
        pursue.target = invisibleTarget;
        lookWhereYoureGoing.target = invisibleTarget;
    }

    // Update is called once per frame
    void Update()
    {
        pursue.character = this;
        pursue.proxTarget = target;
        pursue.maxAcceleration = maxAcceleration;
        pursue.maxSpeed = maxSpeed;
        pursue.targetRadius = targetRadius;
        pursue.slowRadius = slowRadius;
        pursue.timeToTarget = timeToTarget;
        pursue.maxPrediction = maxPrediction;
    
        lookWhereYoureGoing.character = this;
        lookWhereYoureGoing.maxAngularAcceleration = maxAngularAcceleration;
        lookWhereYoureGoing.maxRotation = maxRotation;
        lookWhereYoureGoing.slowRadius = slowRadius;
        lookWhereYoureGoing.timeToTarget = timeToTarget;

        steeringOutput = pursue.getPursueSteering();
        transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
        
        if (steeringOutput != null)
        {
            if (isEvade)
            {
                if ((transform.position - target.transform.position).magnitude < maxDistance)
                {
                    velocity -= steeringOutput.linear * Time.deltaTime;
                } else
                {
                    velocity = Vector2.zero;
                }
            }
            else
            {
                velocity += steeringOutput.linear * Time.deltaTime;
            }
        } else
        {
            velocity = Vector2.zero;
        }

        steeringOutput = lookWhereYoureGoing.getLookSteering();
        transform.Rotate(0, 0, Mathf.Rad2Deg * rotation * Time.deltaTime);

        if (steeringOutput != null)
        {
            rotation += steeringOutput.angular * Time.deltaTime;
        } else
        {
            rotation = 0;
        }
    }
}

class Pursue : Arrive
{
    public float maxPrediction;
    public Static proxTarget;

    public SteeringOutput getPursueSteering()
    {
        Vector2 direction = proxTarget.transform.position - character.transform.position;
        float distance = direction.magnitude;
        float speed = character.velocity.magnitude;
        float prediction;

        if (speed <= distance / maxPrediction)
        {
            prediction = maxPrediction;
        } else
        {
            prediction = distance / speed;
        }

        if (proxTarget.velocity.magnitude == 0)
        {
            target.transform.position = proxTarget.transform.position;
        }
        target.transform.position += new Vector3(proxTarget.velocity.x * prediction * Time.deltaTime, proxTarget.velocity.y * prediction * Time.deltaTime, 0);

        return getSteering();
    }
}

class LookWhereYoureGoing : Align
{
    public SteeringOutput getLookSteering()
    {
        Vector2 velocity = character.velocity;
        if (velocity.magnitude == 0) return null;

        target.transform.rotation = Quaternion.Euler(0, 0, (Mathf.Atan2(-velocity.x, velocity.y) + (Mathf.PI / 2)) * Mathf.Rad2Deg);

        return getSteering();
    }
}
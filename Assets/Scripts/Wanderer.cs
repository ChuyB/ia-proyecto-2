using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.GraphicsBuffer;

public class Wanderer : Static
{
    private KinematicWander kinematicWander = new KinematicWander();
    private KinematicSteeringOutput kinematicSteeringOutput = new KinematicSteeringOutput();
    private SteeringOutput steeringOutput = new SteeringOutput();
    private Wander wander = new Wander();

    public float maxSpeed;
    public float maxRotation;
    public bool isKinematic;
    public float maxAngularAcceleration;
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    public float wanderOrientation;
    public float maxAcceleration;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        GameObject obj = new GameObject();
        Static invisibleTarget = obj.AddComponent<Static>();
        invisibleTarget.transform.position = transform.position;
        wander.target = invisibleTarget;

        GameObject obj2 = new GameObject();
        Static invisibleTarget2 = obj2.AddComponent<Static>();
        invisibleTarget2.transform.position = transform.position;
        wander.faceTarget = invisibleTarget2;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isKinematic)
        {
            kinematicWander.character = this;
            kinematicWander.maxSpeed = maxSpeed;
            kinematicWander.maxRotation = maxRotation;

            kinematicSteeringOutput = kinematicWander.getSteering();
            transform.position += new Vector3(kinematicSteeringOutput.velocity.x, kinematicSteeringOutput.velocity.y);
            transform.Rotate(0, 0, kinematicSteeringOutput.rotation);            
        } else
        {
            wander.character = this;
            wander.maxAngularAcceleration = maxAngularAcceleration;
            wander.targetRadius = targetRadius;
            wander.maxRotation = maxRotation;
            wander.slowRadius = slowRadius;
            wander.timeToTarget = timeToTarget;
            wander.wanderOffset = wanderOffset;
            wander.wanderRadius = wanderRadius;
            wander.wanderRate = wanderRate;
            wander.wanderOrientation = wanderOrientation;
            wander.maxAcceleration = maxAcceleration;

            steeringOutput = wander.getWanderSteering();
            transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
            transform.Rotate(0, 0, rotation * Mathf.Rad2Deg * Time.deltaTime);

            if (steeringOutput != null)
            {
                velocity = steeringOutput.linear * Time.deltaTime;
                rotation = steeringOutput.angular * Time.deltaTime;
            }
        }


    }
}
class KinematicWander
{
    public Static character;
    public float maxSpeed;
    public float maxRotation;

    public KinematicSteeringOutput getSteering()
    {
        KinematicSteeringOutput result = new KinematicSteeringOutput();
        result.velocity = maxSpeed * character.transform.right;

        result.rotation = Random.Range(-1.0f, 1.0f) * maxRotation;

        return result;
    }
}

class Wander : Face
{
    public float wanderOffset;
    public float wanderRadius;
    public float wanderRate;
    public float wanderOrientation;
    public float maxAcceleration;

    public SteeringOutput getWanderSteering()
    {
        wanderOrientation += Random.Range(-1.0f, 1.0f) * wanderRate;

        var characterAngle = character.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float targetOrientation = wanderOrientation + characterAngle;
        Vector3 characterOrientation = new Vector3(Mathf.Cos(characterAngle), Mathf.Sin(characterAngle), 0);

        faceTarget.transform.position = character.transform.position + (wanderOffset * characterOrientation);

        faceTarget.transform.position += new Vector3(Mathf.Cos(targetOrientation), Mathf.Sin(targetOrientation), 0) * wanderRadius;

        SteeringOutput result = faceGetSteering();

        if (result != null)
        {
            result.linear = maxAcceleration * characterOrientation;
        }

        return result;
    }
}
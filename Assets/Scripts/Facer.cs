using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class Facer : Static
{
    private Face face = new Face();
    private SteeringOutput steeringOutput = new SteeringOutput();

    public Static target;
    public float maxAngularAcceleration;
    public float maxRotation;
    public float targetRadius;
    public float slowRadius;
    public float timeToTarget;

    // Start is called before the first frame update
    void Start()
    {
        GameObject obj = new GameObject();
        Static invisibleTarget = obj.AddComponent<Static>();
        invisibleTarget.transform.position = target.transform.position;
        face.target = invisibleTarget;
    }

    // Update is called once per frame
    void Update()
    {
        face.character = this;
        face.faceTarget = target;
        face.maxAngularAcceleration = maxAngularAcceleration;
        face.targetRadius = targetRadius;
        face.maxRotation = maxRotation;
        face.targetRadius = targetRadius;
        face.timeToTarget = timeToTarget;

        steeringOutput = face.faceGetSteering();
        transform.Rotate(0, 0, Mathf.Rad2Deg * rotation * Time.deltaTime);
        if (steeringOutput != null) {
            rotation += steeringOutput.angular * Time.deltaTime;
        } else
        {
            rotation = 0;
        }

        
    }
}

class Face : Align
{
    public Static faceTarget;

    public SteeringOutput faceGetSteering()
    {
        Vector2 direction = faceTarget.transform.position - character.transform.position;

        if (direction.magnitude == 0) return null;

        target.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * (Mathf.Atan2(-direction.x, direction.y) + (Mathf.PI / 2)));

        return getSteering();
    }
}
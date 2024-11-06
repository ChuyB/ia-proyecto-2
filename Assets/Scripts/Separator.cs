using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Separator : Matcher
{
    private SteeringOutput steeringOutput = new SteeringOutput();
    private Separation separation = new Separation();

    public Static[] targets;
    public float threshold;
    public float decayCoefficient;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void Update()
    {
        separation.character = this;
        separation.maxAcceleartion = maxAcceleration;
        separation.targets = targets;
        separation.threshold = threshold;
        separation.decayCoefficient = decayCoefficient;

        steeringOutput = separation.getSteering();
        velocity -= steeringOutput.linear * Time.deltaTime;
        base.Update();
    }
}

class Separation
{
    public Static character;
    public float maxAcceleartion;

    public Static[] targets;
    public float threshold;
    public float decayCoefficient;
    private float strength = 0;

    public SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        foreach (Static target in targets)
        {
            Vector3 direction = target.transform.position - character.transform.position;
            float distance = direction.magnitude;

            if (distance < threshold)
            {
                strength = Mathf.Min(decayCoefficient / (distance * distance), maxAcceleartion);
            }

            direction.Normalize();
            result.linear += strength * new Vector2(direction.x, direction.y);
        }

        return result;
    }
}
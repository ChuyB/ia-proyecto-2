using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static : MonoBehaviour
{
    public Vector2 velocity;
    public float rotation;
    public float speed;
    public bool isControlled;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isControlled)
        {
            float angle = MouseOrientation();

            Vector2 movementDirection = Movement();
            velocity = movementDirection * speed;
            rotation = angle * Mathf.Deg2Rad;
            if (rotation < 0) rotation = (2 * Mathf.PI) + rotation;

            transform.position += new Vector3(velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, 0);
            transform.rotation = Quaternion.AngleAxis(rotation * Mathf.Rad2Deg, Vector3.forward);
        }
    }

    public float newOrientation(float current, Vector2 velocity)
    {
        if (velocity.magnitude > 0)
        {
            return Mathf.Atan2(-velocity.x, velocity.y);
        }
        
        return current;
    }

    private float MouseOrientation()
    {
        Vector2 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }

    private Vector2 Movement()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        return input;
    }
}

class Kinematic
{
    public Vector2 position;
    public float orientation;
    public Vector2 velocity;
    public float rotation;

    public void update(SteeringOutput steering, float time)
    {
        position += velocity * time;
        orientation += rotation * time;

        velocity += steering.linear * time;
        rotation += steering.angular * time;
    }

    public void update(SteeringOutput steering, float maxSpeed, float time)
    {
        position += velocity * time;
        orientation += rotation * time;

        velocity += steering.linear * time;
        rotation += steering.angular * time;

        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
    }
}

class SteeringOutput
{
    public Vector2 linear = new Vector2(0, 0);
    public float angular = 0;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Rigidbody2D _rigidbody;

    public float maxSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float acceleration = 1f;

        Transform target = GameController.instance?.player?.transform;

        if (target != null)
        {
            Vector2 directionToTarget = target.position - transform.position;
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            _rigidbody.MoveRotation(angle);
        }

        _rigidbody.AddForce(transform.right * acceleration);

        _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }
}

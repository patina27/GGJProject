using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D _rigidbody;

    // Configs
    public KeyCode keyUp;
    public KeyCode keyDown;
    public KeyCode keyLeft;
    public KeyCode keyRight;
    public KeyCode keyDash;
    public float moveSpeed;
    public float dashSpeed;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float speed = moveSpeed;
        if (Input.GetKey(keyDash))
        {
            if (GameController.instance.dashEnabled)
            {
                speed = dashSpeed;
                GameController.instance.dashMode = true;
            }
        }
        if (Input.GetKey(keyUp))
        {
            _rigidbody.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        }
        if (Input.GetKey(keyDown))
        {
            _rigidbody.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
        }
        if (Input.GetKey(keyLeft))
        {
            _rigidbody.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
        }
        if (Input.GetKey(keyRight))
        {
            _rigidbody.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        }
        Vector2 dir = _rigidbody.velocity;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(-dir.x, dir.y) * Mathf.Rad2Deg);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Happened!");
        if (collision.gameObject.tag == "enemy")
        {
            if (GameController.instance.dashMode)
            {
                Destroy(collision.gameObject);
                if (GameController.instance.explosionGO != null)
                {
                    GameObject anExplosion = Instantiate(GameController.instance.explosionGO, transform.position, Quaternion.identity);
                    Destroy(anExplosion, 0.25f);
                }
                GameController.instance.dashCollision = true;
            }
            else if (!GameController.instance.protectedMode)
            {
                GameController.instance.oxygen = GameController.instance.oxygen - 0.1f;
                GameController.instance.protectedMode = true;
            }
        }
    }
}

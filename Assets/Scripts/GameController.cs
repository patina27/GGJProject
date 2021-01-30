using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    // Outlets
    public GameObject ObstacleGO;
    public Camera cam;
    public GameObject player;
    public GameObject explosionGO;
    public GameObject OxygenGO;
    public float dashTime;
    public float cdTime;
    public float protectTime;

    public bool dashEnabled = true;
    public bool dashMode = false;
    public bool dashCollision = false;

    public bool protectedMode = false;
    public float oxygen = 1f;

    private float _spawnOffset;
    private float _dashTimer;
    private float _cdTimer;
    private float _protectTimer;
    private float _lastFlash;

    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        _spawnOffset = 1.0f;
        StartCoroutine("SpawnObstacle");
        dashEnabled = true;
        dashMode = false;
        _dashTimer = -1f;
        _cdTimer = -1f;
        _protectTimer = -1f;
        _lastFlash = -1f;
        oxygen = 1f;
    }



    // Update is called once per frame
    void Update()
    {
        DashStateTracker();
        ProtectedModeTracker();
    }

    private IEnumerator SpawnObstacle()
    {
        while (true)
        {
            GameObject obstacle = Instantiate(ObstacleGO) as GameObject;
            Transform transform = obstacle.GetComponent<Transform>();
            float height = cam.orthographicSize;
            float width = cam.orthographicSize * cam.aspect;
            float sign = (Random.Range(-1, 1) >= 0) ? 1 : -1;
            if (Random.Range(-1, 1) >= 0)
            {
                transform.position = new Vector3(cam.transform.position.x + Random.Range(-width, width), cam.transform.position.y + sign * height, 0);

            }
            else
            {
                transform.position = new Vector3(cam.transform.position.x + sign * width, cam.transform.position.y + Random.Range(-height, height), 0);

            }
            yield return new WaitForSeconds(_spawnOffset);
            if (_spawnOffset > 0.3f)
            {
                _spawnOffset = _spawnOffset - 0.01f;
            }
        }
    }

    private void DashStateTracker()
    {
        if (dashMode)
        {
            if (_dashTimer == -1)
            {
                _dashTimer = Time.time;
                //MeshRenderer renderer = player.GetComponentInChildren<MeshRenderer>();
                //Debug.Log("Got renderer: " + renderer.ToString());
                //for (int i = 0; i < renderer.material.GetTexturePropertyNames().Length; i ++)
                //{
                //    Debug.Log(renderer.material.GetTexturePropertyNames()[i]);
                //}
                // Debug.Log(renderer.materials[0].shader.GetPropertyName(1));

                // Debug.Log("Got renderer: " + renderer.ToString());
                Transform oxygenPosition = player.transform;
                Instantiate(OxygenGO, oxygenPosition);
            }

            if (Time.time - _dashTimer > dashTime)
            {
                dashMode = false;
                dashCollision = false;
                dashEnabled = false;
                _dashTimer = -1;
                _cdTimer = Time.time;
            }
        }

        if (_cdTimer != -1 && Time.time - _cdTimer > cdTime)
        {
            _cdTimer = -1;
            dashEnabled = true;
        }

        if (dashMode)
        {

        }
    }

    private void ProtectedModeTracker()
    {
        if (protectedMode)
        {
            if (_protectTimer == -1)
            {
                _protectTimer = Time.time;
            }
            if (Time.time - _protectTimer > protectTime)
            {
                protectedMode = false;
                _protectTimer = -1;
                _lastFlash = -1;
                player.GetComponentInChildren<Renderer>().enabled = true;
            }
        }

        if (protectedMode)
        {
            float interval = 0.2f;
            if (_lastFlash == -1 || Time.time - _lastFlash > interval)
            {
                _lastFlash = Time.time;
                player.GetComponentInChildren<Renderer>().enabled = !player.GetComponentInChildren<Renderer>().enabled;
            }
        }
    }
}

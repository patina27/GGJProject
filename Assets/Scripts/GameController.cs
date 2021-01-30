using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    // Outlets
    public GameObject obstacleGO;
    public Camera cam;
    public GameObject player;
    public GameObject explosionGO;
    public GameObject oxygenGO;
    public GameObject healthBar;
    public GameObject gameOverCanvas;
    public GameObject gameplayCanvas;
    public GameObject textCombo;
    public KeyCode keyRestart;
    public int combo;

    public float dashTime;
    public float cdTime;
    public float protectTime;
    public float oxygenGain;
    public float oxygenLoss;
    public float oxygenLossOffset;
    public float oxygenRegularLoss;

    public bool dashEnabled = true;
    public bool dashMode = false;
    public bool dashCollision = false;

    public bool protectedMode = false;
    public float oxygen = 1f;

    private bool _isGameActive;
    private float _spawnOffset;
    private float _spawnOxygenOffset;
    private float _dashTimer;
    private float _cdTimer;
    private float _protectTimer;
    private float _lastFlash;
    private float _oxygenTimer;

    public void GainOxygen()
    {
        oxygen = (oxygen + oxygenGain <= 1f) ? oxygen + oxygenGain : 1f;
    }
    public void GainOxygen(float gain)
    {
        oxygen = (oxygen + gain <= 1f) ? oxygen + gain : 1f;
    }

    public void LoseOxygen(float loss)
    {
        oxygen = (oxygen - loss >= 0f) ? oxygen - loss : 0f;
    }

    public void LoseOxygen()
    {
        oxygen = (oxygen - oxygenLoss >= 0f) ? oxygen - oxygenLoss : 0f;
    }

    public void GetHit()
    {
        LoseOxygen();
        protectedMode = true;
        combo = 0;
    }
    void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        _spawnOffset = 1.0f;
        _spawnOxygenOffset = 2f;
        _isGameActive = true;
        gameOverCanvas.SetActive(false);
        StartCoroutine("SpawnObstacle");
        StartCoroutine("SpawnOxygen");
        dashEnabled = true;
        dashMode = false;
        _dashTimer = -1f;
        _cdTimer = -1f;
        _protectTimer = -1f;
        _lastFlash = -1f;
        _oxygenTimer = Time.time;
        oxygen = 1f;
        combo = 0;
    }



    // Update is called once per frame
    void Update()
    {
        DashStateTracker();
        ProtectedModeTracker();
        RegularDecreaseOxygen();
        UpdateUI();
        GameEndingTracker();
        ComboTracker();
    }

    private IEnumerator SpawnObstacle()
    {
        while (_isGameActive)
        {
            GameObject obstacle = Instantiate(obstacleGO) as GameObject;
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

    private IEnumerator SpawnOxygen()
    {
        while (_isGameActive)
        {
            GameObject oxygen = Instantiate(oxygenGO) as GameObject;
            Transform transform = oxygen.GetComponent<Transform>();
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
            yield return new WaitForSeconds(_spawnOxygenOffset);
            if (_spawnOxygenOffset > 0.3f)
            {
                _spawnOxygenOffset = _spawnOxygenOffset - 0.01f;
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
                Vector2 dir = player.GetComponent<Rigidbody2D>().velocity.normalized;
                Instantiate(oxygenGO, player.transform.position - new Vector3(dir.x, dir.y), Quaternion.identity);
            }

            if (Time.time - _dashTimer > dashTime)
            {
                dashMode = false;
                if (dashCollision)
                {
                    combo += 1;
                }
                else
                {
                    combo = 0;
                }
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

    private void UpdateUI()
    {
        Slider slider = healthBar.GetComponent<Slider>();
        slider.value = oxygen;
        textCombo.GetComponent<Text>().text = combo.ToString();
    }

    private void RegularDecreaseOxygen()
    {
        if (Time.time - _oxygenTimer > oxygenLossOffset)
        {
            _oxygenTimer = Time.time;
            LoseOxygen(oxygenRegularLoss);
        }
    }

    private void GameEndingTracker()
    {
        if (oxygen == 0f)
        {
            gameOverCanvas.SetActive(true);
            gameplayCanvas.SetActive(false);
            _isGameActive = false;
        }
        if (!_isGameActive)
        {
            if (Input.GetKey(keyRestart))
            {
                RestartGame();
            }

        }
    }

    private void RestartGame()
    {
        Debug.Log("restart game called!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void ComboTracker()
    {

    }
}


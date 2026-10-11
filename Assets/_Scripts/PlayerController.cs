using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float thrustForce = 1.0f;
    public float maxSpeed = 10.0f;
    public float scoreMultiplier = 10.0f;

    public GameObject boosterFlame;

    private float elapsedTime = 0f;
    private float score = 0f;
    private bool isDead = false;

    Rigidbody2D rb;

    [SerializeField] private PanelRenderer panel;

    [SerializeField] private VisualElementReference labelReference;
    [SerializeField] private VisualElementReference restartButtonReference;
    [SerializeField] private VisualElementReference highScoreReference;

    private Label scoreText;
    private Button restartButton;
    private Label highScoreText;
    private float highScore = 0f;
    private const string HighScoreKey = "HighScore";

    public GameObject explotionPrefab;


    private void Awake()
    {
        highScore = PlayerPrefs.GetFloat(HighScoreKey, 0f);
        panel.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panel, VisualElement root, int version)
    {
        labelReference.RegisterReferenceResolvedCallback(ve =>
        {
            if (ve is Label lbl) HandleLableReferenceCallBack(lbl);
        });

        restartButtonReference.RegisterReferenceResolvedCallback(ve =>
        {
            if (ve is Button btn)
            {
                if (restartButton != null) restartButton.clicked -= RestartGame;
                restartButton = btn;
                restartButton.clicked += RestartGame;
                restartButton.style.display = isDead ? DisplayStyle.Flex : DisplayStyle.None;
            }
        });

        highScoreReference.RegisterReferenceResolvedCallback(ve =>
        {
            if (ve is Label lbl)
            {
                highScoreText = lbl;
                highScoreText.text = $"High Score: {highScore}";
            }
        });
    }

    private void HandleLableReferenceCallBack(VisualElement element)
    {
        if (element is Label lbl)
            scoreText = lbl;
    }

    private void OnEnable()
    {
        Debug.Log("On Enable called");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        panel.UnregisterUIReloadCallback(OnUIReload);
        if (restartButton != null) restartButton.clicked -= RestartGame;
    }


    void Update()
    {
        if (isDead) return;

        elapsedTime += Time.deltaTime;
        score = Mathf.Floor(elapsedTime * scoreMultiplier);

        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = (mousePos - transform.position).normalized;
            transform.up = direction;
            rb.AddForce(direction * thrustForce);

            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            boosterFlame.SetActive(true);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            boosterFlame.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Game Over");
            GameOver();
        }
    }

    private void GameOver()
    {
        isDead = true;

        if (explotionPrefab != null)
        {
            Instantiate(explotionPrefab, transform.position, Quaternion.identity);
        }

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat(HighScoreKey, highScore);
            PlayerPrefs.Save();

            if (highScoreText != null)
                highScoreText.text = $"High Score: {highScore}";
        }


        boosterFlame.SetActive(false);

        foreach (var r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
            
        GetComponent<Collider2D>().enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        if (restartButton != null)
        {
            restartButton.style.display = DisplayStyle.Flex;
        }
            
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

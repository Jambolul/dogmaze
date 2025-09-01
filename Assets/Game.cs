using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Game : MonoBehaviour
{
    [Header("Scene refs")]
    public CinemachineVirtualCamera vcam;
    public Transform Player;
    public Transform Goal;
    public Transform Walls;
    public GameObject WallTemplate;
    public GameObject FloorTemplate;

    [Header("UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [Header("Audio")]
    public AudioSource ambience;
    public AudioSource sfx;
    public AudioClip ambienceClip;
    public AudioClip goalClip;
    [SerializeField] private AudioClip loseClip;

    [Header("Tuning")]
    [Min(2)] public int Width = 6;
    [Min(2)] public int Height = 6;
    [Range(0f, 1f)] public float HoleProbability = 0.10f;

    [Tooltip("Only used if you enable keyboard debug movement.")]
    [Min(0)] public float MovementSmoothing = 12f;
    public bool EnableKeyboardMovement = false;

    bool[,] HWalls, VWalls;
    int goalX, goalY;
    int playerX, playerY;

    bool transitioning = false;
    Collider2D playerCol;
    Rigidbody2D playerRb;

    void Awake()
    {
        if (!vcam) vcam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vcam && vcam.Follow == null && Player) vcam.Follow = Player;

        if (Player)
        {
            playerCol = Player.GetComponent<Collider2D>();
            playerRb = Player.GetComponent<Rigidbody2D>();
        }
    }

    void Start()
    {
        if (!ValidateSetup()) { enabled = false; return; }

        Time.timeScale = 1f;                                   
        if (winPanel) winPanel.SetActive(false);               
        if (losePanel) losePanel.SetActive(false);             

        if (ambience && ambienceClip)
        {
            ambience.clip = ambienceClip;
            ambience.loop = true;
            ambience.Play();
        }

        StartNext();
    }

    void Update()
    {
        if (!EnableKeyboardMovement || HWalls == null || VWalls == null) return;

        if (Input.GetKeyDown(KeyCode.A) && playerX > 0 && !HWalls[playerX, playerY]) playerX--;
        if (Input.GetKeyDown(KeyCode.D) && playerX < Width - 1 && !HWalls[playerX + 1, playerY]) playerX++;
        if (Input.GetKeyDown(KeyCode.W) && playerY < Height - 1 && !VWalls[playerX, playerY + 1]) playerY++;
        if (Input.GetKeyDown(KeyCode.S) && playerY > 0 && !VWalls[playerX, playerY]) playerY--;

        playerX = Mathf.Clamp(playerX, 0, Width - 1);
        playerY = Mathf.Clamp(playerY, 0, Height - 1);

        Vector3 target = new Vector3(playerX + 0.5f, playerY + 0.5f, Player.position.z);
        float t = 1f - Mathf.Exp(-Mathf.Max(0f, MovementSmoothing) * Time.deltaTime);
        Player.position = Vector3.Lerp(Player.position, target, t);

        if (Vector2.Distance(Player.position, new Vector2(goalX + 0.5f, goalY + 0.5f)) < 0.12f)
            LevelComplete();

        if (Input.GetKeyDown(KeyCode.G)) StartNext();
    }

    public void StartNext()
    {
        Width = Mathf.Max(2, Width);
        Height = Mathf.Max(2, Height);
        HoleProbability = Mathf.Clamp01(HoleProbability);

        if (Walls)
            for (int i = Walls.childCount - 1; i >= 0; i--)
                Destroy(Walls.GetChild(i).gameObject);

        (HWalls, VWalls) = GenerateLevel(Width, Height);

        playerX = Random.Range(0, Width);
        playerY = Random.Range(0, Height);

        float minDist = 0.5f * Mathf.Max(Width, Height);
        do
        {
            goalX = Random.Range(0, Width);
            goalY = Random.Range(0, Height);
        } while (Vector2.Distance(new Vector2(playerX, playerY), new Vector2(goalX, goalY)) < minDist);

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Instantiate(FloorTemplate, new Vector3(x + 0.5f, y + 0.5f, 0f),
                            Quaternion.identity, Walls);

        for (int x = 0; x < Width + 1; x++)
            for (int y = 0; y < Height; y++)
                if (HWalls[x, y])
                    Instantiate(WallTemplate, new Vector3(x, y + 0.5f, 0f),
                                Quaternion.identity, Walls);

        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height + 1; y++)
                if (VWalls[x, y])
                    Instantiate(WallTemplate, new Vector3(x + 0.5f, y, 0f),
                                Quaternion.Euler(0, 0, 90f), Walls);

        if (Player) Player.position = new Vector3(playerX + 0.5f, playerY + 0.5f, Player.position.z);
        if (Goal) Goal.position = new Vector3(goalX + 0.5f, goalY + 0.5f, Goal.position.z);

        if (vcam) vcam.m_Lens.OrthographicSize = Mathf.Pow(Mathf.Max(Width / 1.5f, Height), 0.70f) * 0.95f;
    }

    public void GameOver()
    {
        if (transitioning) return;
        transitioning = true;

        SetPlayerActive(false);
        if (sfx && loseClip) sfx.PlayOneShot(loseClip);

        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void LevelComplete()
    {
        if (transitioning) return;
        transitioning = true;

        SetPlayerActive(false);
        if (sfx && goalClip) sfx.PlayOneShot(goalClip);

        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void SetPlayerActive(bool on)
    {
        if (playerRb) playerRb.simulated = on;
        if (playerCol) playerCol.enabled = on;
    }

    (bool[,], bool[,]) GenerateLevel(int w, int h)
    {
        var hwalls = new bool[w + 1, h];
        var vwalls = new bool[w, h + 1];
        var visited = new bool[w, h];

        bool Dfs(int x, int y)
        {
            if (visited[x, y]) return false;
            visited[x, y] = true;

            var dirs = new (int nx, int ny, bool[,] wall, int wx, int wy)[]
            {
                (x - 1, y, hwalls, x,     y),
                (x + 1, y, hwalls, x + 1, y),
                (x, y - 1, vwalls, x,     y),
                (x, y + 1, vwalls, x,     y + 1),
            };

            foreach (var d in dirs.OrderBy(_ => Random.value))
            {
                bool inBounds = d.nx >= 0 && d.nx < w && d.ny >= 0 && d.ny < h;
                bool carve = inBounds && (Dfs(d.nx, d.ny) || Random.value < HoleProbability);
                d.wall[d.wx, d.wy] = !carve;
            }
            return true;
        }

        Dfs(0, 0);
        return (hwalls, vwalls);
    }

    bool ValidateSetup()
    {
        if (!Player || !Goal || !Walls || !WallTemplate || !FloorTemplate)
        {
            Debug.LogError("Assign Player, Goal, Walls (empty container), WallTemplate prefab and FloorTemplate prefab.", this);
            return false;
        }
        if (WallTemplate.scene.IsValid() || FloorTemplate.scene.IsValid())
            Debug.LogWarning("Templates should be prefab assets (drag from Project window).", this);
        return true;
    }
}

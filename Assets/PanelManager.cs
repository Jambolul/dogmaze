using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    private bool gameEnded;

    private void Awake()
    {
        Time.timeScale = 1f;
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        gameEnded = false;
    }

    public void Win()
    {
        if (gameEnded) return;
        gameEnded = true;
        if (sfxSource && winClip) sfxSource.PlayOneShot(winClip);
        if (winPanel) winPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Lose()
    {
        if (gameEnded) return;
        gameEnded = true;
        if (sfxSource && loseClip) sfxSource.PlayOneShot(loseClip);
        if (losePanel) losePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}

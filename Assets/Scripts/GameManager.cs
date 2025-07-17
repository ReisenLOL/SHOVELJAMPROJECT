using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject respawnCanvas;
    public GameObject gameCanvas;
    public TextMeshProUGUI reasonText;
    public TextMeshProUGUI wavesText;

    public void GameOver(bool isCore)
    {
        respawnCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        gameOverUI.SetActive(true);
        Time.timeScale = 0;
        wavesText.text = "Waves survived: " + FindFirstObjectByType<WaveManager>().currentWaveNumber;
        string textDisplay;
        if (isCore)
        {
            textDisplay = "CORE DESTROYED\n";
        }
        else
        {
            textDisplay = "CANNOT RESPAWN\n";
        }
        textDisplay += "MASSIVE SKILL ISSUE";
        reasonText.text = textDisplay;
    }
    public void Restart()
    {
        SceneManager.LoadScene("Prototype");
    }

    public void Exit()
    {
        
    }
}

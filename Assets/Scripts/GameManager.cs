using System;
using Bremsengine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject respawnCanvas;
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public TextMeshProUGUI reasonText;
    public TextMeshProUGUI wavesText;

    private void Start()
    {
        GeneralManager.SetPause(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverUI.activeSelf)
        {
            Pause();
        }
    }

    public void GameOver(bool isCore)
    {
        respawnCanvas.SetActive(false);
        gameCanvas.SetActive(false);
        gameOverUI.SetActive(true);
        pauseCanvas.SetActive(false);
        GeneralManager.SetPause(true);
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

    public void Pause()
    {
        GeneralManager.TogglePause();
        pauseCanvas.SetActive(!pauseCanvas.activeSelf);
    }
    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        SceneManager.LoadScene(0);
    }
}

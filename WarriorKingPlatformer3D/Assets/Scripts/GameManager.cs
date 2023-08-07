using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject GameOverUiScreen;
    [SerializeField] GameObject GameWinUiScreen;

    //coinSystem
    private int totalCoinCollected;
    [SerializeField] TextMeshProUGUI coinText;

    //levelSystem
    [SerializeField] private List<GameObject> levelList = new List<GameObject>();
    private int levelIndexNum;
    private int displayLevelIndexNum;
    [SerializeField] private TextMeshProUGUI leveltext;

    //Audio
    [SerializeField] AudioSource levelLostAudio;
    [SerializeField] AudioSource levelWinAudio;
    [SerializeField] AudioSource bgAudio;

    private void Start()
    {
        levelIndexNum = PlayerPrefs.GetInt("LevelIndex");

        if (levelList[levelIndexNum].activeSelf == false)
        {
            for (int i = 0; i < levelList.Count; i++)
            {
                levelList[i].SetActive(false);
            }
            levelList[levelIndexNum].SetActive(true);
        }

        displayLevelIndexNum = PlayerPrefs.GetInt("DisplayLevelIndex");
        leveltext.text = "LEVEL-" + (displayLevelIndexNum + 1);

        totalCoinCollected = PlayerPrefs.GetInt("CoinAmount");
        UpdateCoinText();
    }

    public void GameOver()
    {
        levelLostAudio.Play();
        bgAudio.Stop();
        GameOverUiScreen.SetActive(true);
        //Time.timeScale = 0;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void CoinCollected(int value)
    {
        totalCoinCollected += value;
        UpdateCoinText();
    }
    private void UpdateCoinText()
    {
        coinText.text = totalCoinCollected.ToString();
    }
    public void LevelWin()
    {
        GameWinUiScreen.SetActive(true);
        levelWinAudio.Play();
        bgAudio.Stop();
    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("CoinAmount", totalCoinCollected);
        if (PlayerPrefs.GetInt("LevelIndex") == (levelList.Count - 1))
        {
            PlayerPrefs.SetInt("LevelIndex", 0);
            PlayerPrefs.SetInt("DisplayLevelIndex", PlayerPrefs.GetInt("DisplayLevelIndex") + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            PlayerPrefs.SetInt("LevelIndex", PlayerPrefs.GetInt("LevelIndex") + 1);
            PlayerPrefs.SetInt("DisplayLevelIndex", PlayerPrefs.GetInt("DisplayLevelIndex") + 1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    GameObject levelIndicator;
    [SerializeField]
    TMP_Text currentLevelText;
    [SerializeField]
    Slider currentLevelProgress;
    [SerializeField]
    GameObject levelIndicatorPanel;
    [SerializeField]
    GameObject levelProgressPanel;
    [SerializeField]
    TMP_Text levelTotalScore;
    [SerializeField]
    GameObject gameOverPanel;
    [SerializeField]
    GameObject nextLevelPanel;
    [SerializeField]
    GameObject clickMePanel;

    //CALLED AFTER LEVEL IS GENERATED
    public void UpdateUI()
    {
        scoreText.text = PlayerPrefs.GetInt("score", 0).ToString();
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        currentLevelText.text = currentLevel.ToString();
        for (int i = 0; i < levelIndicator.transform.childCount; i++)
        {
            levelIndicator.transform.GetChild(i).GetComponentInChildren<TMP_Text>().text = (currentLevel + i).ToString();
        }
        levelIndicatorPanel.SetActive(true);
        clickMePanel.SetActive(true);

    }

    //CALLED AFTER GAME IS STARTE
    public void StartGame()
    {
        levelIndicatorPanel.SetActive(false);
        clickMePanel.SetActive(false);
        levelProgressPanel.SetActive(true);
    }

    //CALLED DURING
    public void UpdatePathProgress(float _percent)
    {
        currentLevelProgress.value = _percent;
    }

    //CALLED AFTER GAME LOST
    public void ShowGameOver()
    {
        levelProgressPanel.SetActive(false);

        gameOverPanel.SetActive(true);
    }

    //CALLED AFTER GAME WON
    public void NextLevel(int _score)
    {
        AudioManager.instance.Play("win");

        levelProgressPanel.SetActive(false);
        levelTotalScore.text = _score.ToString();
        PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score", 0) + _score);

        nextLevelPanel.SetActive(true);
    }

    public void HideScore()
    {
        scoreText.transform.parent.gameObject.SetActive(false);
    }
}

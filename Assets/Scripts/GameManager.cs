using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Fade mainMenuFade;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject cutScene;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    [SerializeField] private float cutSceneTime;
    private float cutSceneTimeLeft;

    [SerializeField] private AudioSource backgroundSource;
    [SerializeField] private AudioSource endgameSource;
    [SerializeField] private AudioClip winSound;
    
    public static Action OnStartGame = delegate { };

    private void Awake()
    {
        mainMenu.SetActive(true);
        cutScene.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        WaveManager.OnWinGame += HandleWinGame;
        MrFrog.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        WaveManager.OnWinGame -= HandleWinGame;
        MrFrog.OnPlayerDeath -= HandlePlayerDeath;
    }
    private void Update()
    {
       if (cutScene.activeSelf)
        {
            cutSceneTimeLeft -= Time.deltaTime;
            if (cutSceneTimeLeft <= 0)
            {
                StartGame();
            }
        }
    }

    public void OnClickStartGame()
    {
        mainMenu.SetActive(false);
        StartGame();
        backgroundSource.Play();
        //cutScene.SetActive(true);
        //cutSceneTimeLeft = cutSceneTime;
    }

    public void OnClickSkipCutScene()
    {
        StartGame();
    }

    public void OnClickRestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void StartGame()
    {
        mainMenu.SetActive(false);
        cutScene.SetActive(false);
        OnStartGame?.Invoke();
    }

    private void HandleWinGame()
    {
        winScreen.SetActive(true);
        backgroundSource.Stop();
        endgameSource.PlayOneShot(winSound);
    }

    private void HandlePlayerDeath()
    {
        backgroundSource.volume = 1;
        loseScreen.SetActive(true);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    menu,
    inTheGame,
    gameOver,
    pause
}

public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.menu;
    public static GameManager sharedInstance;
    public Canvas menuCanvas;
    public Canvas gameCanvas;
    public Canvas gameOverCanvas;
    public Canvas pauseCanvas;
    public int collectedCoins = 0;
    private int lvFinalAdded = 1;
    public LevelBlock levelHard;

    public GameObject starItem;
    public int starsAdded = 1;

    public Button buttonPause;
    public bool pause = false;

    public List<ParallaxEffect> listParallax;
    private void Awake()
    {
        sharedInstance = this;
    }

    private void Start()
    {
        currentGameState = GameState.menu;
        menuCanvas.enabled = true;
        gameCanvas.enabled = false; 
        gameOverCanvas.enabled = false;
        menuCanvas.GetComponent<AudioSource>().Play();
        gameCanvas.GetComponent<AudioSource>().Stop();
        gameOverCanvas.GetComponent<AudioSource>().Stop();

    }

    private void FixedUpdate()
    {
        //Debug.Log((int)PlayerController.sharedInstance.distanceTravelled);
        if ((int)PlayerController.sharedInstance.distanceTravelled == (lvFinalAdded * 100))
        {
            if (LevelGenerator.sharedInstance.allTheLevelBlocks.Count <= 20)
            {
                LevelGenerator.sharedInstance.allTheLevelBlocks.Add(levelHard);
                lvFinalAdded++;    
            }

            PlayerController.sharedInstance.increaseVelocityGravity();
        }

        if (((int) PlayerController.sharedInstance.distanceTravelled != 0)&&
            ((int)PlayerController.sharedInstance.distanceTravelled % (200*starsAdded) == 0))
        {
            //DESCOMENTAR , GENERADOR DE ESTRELLAS
            GameObject starItemAdd = Instantiate(starItem);
            starItemAdd.transform.position = new Vector2(
                PlayerController.sharedInstance.transform.position.x+10f
                ,PlayerController.sharedInstance.transform.position.y);
            starItemAdd.transform.parent = LevelGenerator.sharedInstance.currentLevelBlocks[LevelGenerator.sharedInstance.currentLevelBlocks.Count - 1].transform;
            starsAdded++;
            
        }
    }

    private void Update()
    {
        if ((Input.GetButtonDown("Submit") && (currentGameState != GameState.inTheGame)))
        {
            StartGame();
        }
        
    }
   
    public void StartGame()
    {
        PlayerController.sharedInstance.StartGame();
        LevelGenerator.sharedInstance.GenerateInitialBlocks();
        UpdateGameCanvas.sharedInstance.SetRecordPoints();
        collectedCoins = 0;
        UpdateGameCanvas.sharedInstance.SetCoinsNumber();
        ChangeGameState(GameState.inTheGame);
        
        foreach (var parallax in GameManager.sharedInstance.listParallax)
        {
            parallax.ResetPosition();
        }
    }
   
    public void GameOver()
    {
        ChangeGameState(GameState.gameOver);
        LevelGenerator.sharedInstance.RemoveAllBlocks();
        UpdateGameOverCanvas.sharedInstance.SetScorePointsAndCoins();
    }

    public void BackToMainMenu()
    {
        ChangeGameState(GameState.menu);
    }

    public void CollectCoin()
    {
        collectedCoins++;
        UpdateGameCanvas.sharedInstance.SetCoinsNumber();
    }
   
    private void ChangeGameState(GameState newGameState)
    {
        if (newGameState == GameState.menu)
        {
            menuCanvas.enabled = true;
            gameCanvas.enabled = false;
            gameOverCanvas.enabled = false;
            pauseCanvas.enabled = false;
            
            menuCanvas.GetComponent<AudioSource>().Play();
            gameCanvas.GetComponent<AudioSource>().Stop();
            gameOverCanvas.GetComponent<AudioSource>().Stop();
            
            
        }
        else if(newGameState == GameState.gameOver)
        {
            menuCanvas.enabled = false;
            gameCanvas.enabled = false;
            gameOverCanvas.enabled = true;
            pauseCanvas.enabled = false;
            currentGameState = GameState.gameOver;
            
            menuCanvas.GetComponent<AudioSource>().Stop();
            gameCanvas.GetComponent<AudioSource>().Stop();
            gameOverCanvas.GetComponent<AudioSource>().Play();
        }
        else if (newGameState == GameState.inTheGame)
        {  
            menuCanvas.enabled = false;
            gameOverCanvas.enabled = false;
            gameCanvas.enabled = true;
            pauseCanvas.enabled = false;
            currentGameState = GameState.inTheGame;
            
            menuCanvas.GetComponent<AudioSource>().Stop();
            gameCanvas.GetComponent<AudioSource>().Play();
            gameOverCanvas.GetComponent<AudioSource>().Stop();
        }
        else if (newGameState == GameState.pause)
        {  
            menuCanvas.enabled = false;
            gameOverCanvas.enabled = false;
            gameCanvas.enabled = false;
            pauseCanvas.enabled = true;
            currentGameState = GameState.pause;
            
            menuCanvas.GetComponent<AudioSource>().Stop();
            gameCanvas.GetComponent<AudioSource>().Pause();
            gameOverCanvas.GetComponent<AudioSource>().Stop();
        }
    }

    public void PauseRestart()
    {
        
        pause = !pause;
        if (pause)
        {
            Time.timeScale = 0;
            ChangeGameState(GameState.pause);
        }
        else
        {
            ChangeGameState(GameState.inTheGame);
            Time.timeScale = 1;
        }
        //Debug.Log(pause);
    } 
}
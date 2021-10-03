using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int connections = 0;
    public int Connections => connections;

    int impsSaved = 0;
    public int ImpsSaved => impsSaved;

 

    int totalImps = 0;
    public int TotalImps => totalImps;

    public Graph graph;

    public int ImpsNeededToWin = 10;

    public Action onLevelCompleted;
    public Action allImsSaved;
    public Action levelImpossible;


    public string nextSceneName;

    public bool finalLevel = false;

    public void CheckIfGraphIsValid()
    {
        bool valid = Graph.Instance.HasConnectableAnchors();
        if (!valid)
            levelImpossible?.Invoke();
    }

    private void Awake()
    {
        //PauseGame();
        ResumeGame();
    }
    internal void ImpDied()
    {
        totalImps--;
        if (totalImps < ImpsNeededToWin)
            levelImpossible?.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ConnectionMade()
    {
        connections++;
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ConnectionRemoved()
    {
        if(connections > 0)
            connections--;
    }

    public void AddTotalImps(int count = 1)
    {
        totalImps += count;
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private void Update()
    {
        CheckIfGraphIsValid();
    }

    public void ImpSaved()
    {
        impsSaved++;
        if (impsSaved == totalImps)
            allImsSaved?.Invoke();

        if (impsSaved == ImpsNeededToWin)
            onLevelCompleted?.Invoke();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIscript : MonoBehaviour
{
    public GameManager gameManager;

    public TMPro.TextMeshProUGUI txtImpsSaved;
    public TMPro.TextMeshProUGUI txtConnections;
    public TMPro.TextMeshProUGUI txtTotalImps;

    TMPro.TextMeshProUGUI btnText;

    public Button btnNextLevel;

    bool nextLevel = true;

    Animator anim;

    private void Start()
    {

        anim = GetComponent<Animator>();
        gameManager.onLevelCompleted += () =>
        {
            if (gameManager.finalLevel == false)
                btnNextLevel.gameObject.SetActive(true);
        };
        gameManager.allImsSaved += OnLevelCompleted;
        btnNextLevel.gameObject.SetActive(false);
        gameManager.levelImpossible += LevelImpossible;

        btnText = btnNextLevel.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        btnNextLevel.onClick.AddListener(OnNextlevelClicked);
    }


    void LevelImpossible()
    {
        nextLevel = false;
        btnText.text = "Retry";
        btnNextLevel.gameObject.SetActive(true);
    }

    void OnNextlevelClicked()
    {
        if (nextLevel)
            gameManager.LoadNextLevel();
        else
            gameManager.ReloadLevel();
    }

    void OnLevelCompleted()
    {
        if(gameManager.finalLevel == false)
            btnNextLevel.gameObject.SetActive(true);

        anim.SetTrigger("LevelCompleted");
        gameManager.PauseGame();
    }

    public void SaveMoreImps()
    {
        //anim.SetTrigger("HideLevelCompleted");
        //gameManager.ResumeGame();
    }

    public void NextLevel()
    {
        gameManager.LoadNextLevel();
    }

    private void Update()
    {
        txtImpsSaved.text = $"{gameManager.ImpsSaved}/{gameManager.ImpsNeededToWin}";
        txtConnections.text = $"{gameManager.Connections}";
        txtTotalImps.text = $"{gameManager.TotalImps}";
    }


}

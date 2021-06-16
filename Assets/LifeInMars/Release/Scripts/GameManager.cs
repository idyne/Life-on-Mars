﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using RocketGM;
using RocketFacebook;
public class GameManager : MonoBehaviour
{
    public delegate void Callback();

    private static int levelCount = 1;
    private bool isLevelCountSet = false;
    private bool isLocked = false;


    public GameState State = GameState.NOT_STARTED;
    private static GameManager instance;

    private UIStartText uiStartText = null;

    [SerializeField] private string[] successTexts = null;
    [SerializeField] private Color[] successTextColors = null;
    [Header("Prefabs")]
    [SerializeField] private GameObject uiPrefab = null;
    [SerializeField] private GameObject uiLoadingScreenPrefab = null;
    [SerializeField] private GameObject uiCompleteScreenPrefab = null;
    [SerializeField] private GameObject uiLevelTextPrefab = null;
    [SerializeField] private GameObject uiStartTextPrefab = null;
    [SerializeField] private GameObject confettiShowerPrefab = null;
    [SerializeField] private GameObject confettiBlastPrefab = null;
    [SerializeField] private GameObject successTextPrefab = null;
    [SerializeField] private GameObject instructionTextPrefab = null;
    [SerializeField] private GameObject[] emojiEffectPrefabs = null;




    public static GameManager Instance
    {
        get
        {
            GameManager instance = GameManager.instance;
            if (!instance)
            {
                instance = new GameObject("GameManager").AddComponent<GameManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        AvoidDuplication();
        SetLevelCount();
        AdjustCurrentLevel();
    }

    private void Update()
    {
        if (!isLocked)
            CheckInput();
        if (Input.GetKeyDown(KeyCode.S))
        {
            TakeScreenshot();
        }
    }

    private void TakeScreenshot()
    {
        string folderPath = Directory.GetCurrentDirectory() + "/Screenshots/";

        if (!System.IO.Directory.Exists(folderPath))
            System.IO.Directory.CreateDirectory(folderPath);

        var screenshotName =
                                "Screenshot_" +
                                System.DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss") +
                                ".png";
        ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName));
        Debug.Log(folderPath + screenshotName);
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.X) && State == GameState.STARTED)
        {
            FinishLevel(true);
        }
        else if (Input.GetKeyDown(KeyCode.C) && State == GameState.STARTED)
        {
            FinishLevel(false);
        }
        else if (Input.GetMouseButtonDown(0) && State == GameState.NOT_STARTED)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        print("GameManager level started");
        State = GameState.STARTED;
        if (uiStartText)
            uiStartText.Hide();
        RocGm.PlayerProgress.StartProgress(CurrentLevel);
        LevelManager.Instance.StartLevel();
    }

    public int CurrentLevel
    {
        get
        {
            return PlayerPrefs.GetInt("currentLevel");
        }
        set
        {
            PlayerPrefs.SetInt("currentLevel", value);
        }
    }

    private void AdjustCurrentLevel()
    {
        if (!isLevelCountSet)
            SetLevelCount();
        if (CurrentLevel == 0 || CurrentLevel > levelCount)
            CurrentLevel = 1;
        if (SceneManager.GetActiveScene().buildIndex == 0) // no level is loaded
        {
            LoadLevel(CurrentLevel);
        }

    }


    private void SetLevelCount()
    {
        // There will be only one scene (LevelLoader) in the build settings other than level scenes.
        levelCount = SceneManager.sceneCountInBuildSettings - 1;
        isLevelCountSet = true;
    }

    private void AvoidDuplication()
    {
        if (!instance)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else
            DestroyImmediate(gameObject);
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(CurrentLevel);
    }
    public void LoadLevel(int level)
    {
        StartCoroutine(LoadLevelAsynchronously(level));
    }

    private IEnumerator LoadLevelAsynchronously(int level)
    {
        isLocked = true;
        UILoadingScreen loadingScreen = CreateLoadingScreen();
        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }
        if (loadingScreen)
            loadingScreen.Hide();
        CreateUILevelText();
        print(LevelManager.Instance.Type);
        if (LevelManager.Instance.Type == LevelManager.LevelType.PLANT)
            CreateUIStartText(0.38f);
        else CreateUIStartText();
        isLocked = false;
        State = GameState.NOT_STARTED;
    }

    private void CreateUILevelText()
    {
        Transform parent = GetUICanvas();
        GameObject go = Instantiate(uiLevelTextPrefab, parent);
        Text levelText = go.GetComponent<Text>();
        levelText.text = "Day " + CurrentLevel;
    }

    private void CreateUIStartText()
    {
        Transform parent = GetUICanvas();
        GameObject go = Instantiate(uiStartTextPrefab, parent);
        Text levelText = go.GetComponent<Text>();
        levelText.text = "TAP TO PLAY";
        uiStartText = go.GetComponent<UIStartText>();
    }

    private void CreateUIStartText(float verticalAnchorPosition)
    {
        Transform parent = GetUICanvas();
        GameObject go = Instantiate(uiStartTextPrefab, parent);
        Text levelText = go.GetComponent<Text>();
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(rectTransform.anchorMin.x, verticalAnchorPosition);
        rectTransform.anchorMax = new Vector2(rectTransform.anchorMax.x, verticalAnchorPosition);
        rectTransform.anchoredPosition = Vector2.zero;
        levelText.text = "TAP TO PLAY";
        uiStartText = go.GetComponent<UIStartText>();
    }

    public void CreateUICompleteScreen(bool success)
    {
        Transform parent = GetUICanvas();
        GameObject go = Instantiate(uiCompleteScreenPrefab, parent);
        UICompleteScreen uiCompleteScreen = go.GetComponent<UICompleteScreen>();
        uiCompleteScreen.SetScreen(success, CurrentLevel);


    }

    private void InstantiateConfettiShower()
    {
        print("Instantiated confetti shower");
        Instantiate(confettiShowerPrefab, Camera.main.transform);

        //Instantiate(confettiShowerPrefab, GameObject.FindGameObjectWithTag("Second Camera").transform);
    }

    private void InstantiateConfettiBlast()
    {
        print("Instantiated confetti blast");
        Instantiate(confettiBlastPrefab, Camera.main.transform);
    }

    public void InstantiateEmojiEffect(int index)
    {
        if (index < emojiEffectPrefabs.Length)
        {
            Transform cam = Camera.main.transform;
            Instantiate(emojiEffectPrefabs[index], cam);
        }
    }

    public Transform GetUICanvas()
    {
        return GameObject.Find("UI").transform.Find("Canvas");
    }

    public void InstantiateSuccessText(int index)
    {
        Text successText = Instantiate(successTextPrefab, GetUICanvas()).GetComponent<Text>();
        successText.text = successTexts[index];
        successText.color = successTextColors[index];
        successText.transform.LeanMoveLocalY(successText.transform.position.y + 3, 2f);
        successText.transform.LeanScale(successText.transform.localScale * 1.2f, 0.8f).setEaseOutElastic();
        LeanTween.textAlpha(successText.GetComponent<RectTransform>(), 0, 1f).setEaseInCubic();
        Destroy(successText, 2);
    }

    public void InstantiateInstructionText(string text, float time)
    {
        Text instructionText = Instantiate(instructionTextPrefab, GetUICanvas()).GetComponent<Text>();
        instructionText.transform.localScale = Vector3.zero;
        instructionText.transform.LeanScale(Vector3.one, 0.2f);
        instructionText.text = text;
        LTDescr scaleLoop = null;
        if (instructionText)
        {
            LeanTween.delayedCall(0.2f, () => { scaleLoop = instructionText.transform.LeanScale(instructionText.transform.localScale * 1.1f, 0.8f).setEaseInQuart().setLoopPingPong(); });
            LeanTween.delayedCall(time - 0.25f, () => { scaleLoop.pause(); instructionText.transform.LeanScale(Vector3.zero, 0.2f); });
        }
    }

    private UILoadingScreen CreateLoadingScreen()
    {
        UILoadingScreen uiLoadingScreen = FindObjectOfType<UILoadingScreen>();
        if (!uiLoadingScreen)
        {
            Transform parent = GetUICanvas();
            GameObject go = Instantiate(uiLoadingScreenPrefab, parent);
            uiLoadingScreen = go.AddComponent<UILoadingScreen>();
        }
        return uiLoadingScreen;

    }

    public void FinishLevel(bool success)
    {
        State = GameState.FINISHED;
        if (CurrentLevel == 1 && PlayerPrefs.GetInt("firstLevelReported") == 0)
        {
            PlayerPrefs.SetInt("firstLevelReported", 1);
            RocFacebookController.ReportFirstLevelCompleted();
        }
        RocGm.PlayerProgress.StopProgress(CurrentLevel, success ? 1 : 0);
        if (success)
        {
            InstantiateConfettiBlast();
            InstantiateConfettiShower();
            LeanTween.delayedCall(1f, () =>
            {
                CreateUICompleteScreen(success);
                CurrentLevel += 1;
                AdjustCurrentLevel();
            });
        }
        else
        {
            CreateUICompleteScreen(success);
        }
    }

    public enum GameState { STARTED, NOT_STARTED, PAUSED, FINISHED }
}

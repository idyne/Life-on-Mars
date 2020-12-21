using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public delegate void Callback();

    private static int levelCount = 1;
    private bool isLevelCountSet = false;
    private bool isLocked = false;


    public GameState State = GameState.NOT_STARTED;
    private static GameManager instance;

    private UIStartText uiStartText = null;

    [SerializeField] private GameObject uiPrefab = null;
    [SerializeField] private GameObject uiLoadingScreenPrefab = null;
    [SerializeField] private GameObject uiCompleteScreenPrefab = null;
    [SerializeField] private GameObject uiLevelTextPrefab = null;
    [SerializeField] private GameObject uiStartTextPrefab = null;
    [SerializeField] private GameObject confettiShowerPrefab = null;
    [SerializeField] private GameObject confettiBlastPrefab = null;
    [SerializeField] private GameObject successTextPrefab = null;
    [SerializeField] private string[] successTexts = null;
    [SerializeField] private Color[] successTextColors = null;
    [SerializeField] private GameObject instructionTextPrefab = null;


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
            print("GameManager level started");
            State = GameState.STARTED;
            if (uiStartText)
                uiStartText.Hide();

            LevelManager.Instance.StartLevel();
        }
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
        CreateUIStartText();
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

    public void CreateUICompleteScreen(bool success)
    {
        Transform parent = GetUICanvas();
        GameObject go = Instantiate(uiCompleteScreenPrefab, parent);
        UICompleteScreen uiCompleteScreen = go.GetComponent<UICompleteScreen>();
        uiCompleteScreen.SetScreen(success, CurrentLevel);
        if (success)
        {
            InstantiateConfettiBlast();
            InstantiateConfettiShower();
            //StartCoroutine(DoDelayed(0.2f, InstantiateConfettiShower));
        }

    }

    private void InstantiateConfettiShower()
    {
        print("Instantiated confetti shower");
        Instantiate(confettiShowerPrefab, Camera.main.transform);
    }

    private void InstantiateConfettiBlast()
    {
        print("Instantiated confetti blast");
        Instantiate(confettiBlastPrefab, Camera.main.transform);
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
        LeanTween.delayedCall(0.2f, () => { scaleLoop = instructionText.transform.LeanScale(instructionText.transform.localScale * 1.1f, 0.8f).setEaseInQuart().setLoopPingPong(); });
        LeanTween.delayedCall(time - 0.2f, () => { scaleLoop.pause(); instructionText.transform.LeanScale(Vector3.zero, 0.2f); });
        Destroy(instructionText, time + 2);
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
        CreateUICompleteScreen(success);
        if (success)
        {
            CurrentLevel += 1;
            AdjustCurrentLevel();
        }
    }

    public enum GameState { STARTED, NOT_STARTED, PAUSED, FINISHED }
}

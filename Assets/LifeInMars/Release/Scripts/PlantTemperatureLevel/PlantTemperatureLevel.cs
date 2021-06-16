using FSG.MeshAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class PlantTemperatureLevel : LevelManager
{
    private int requiredTemperature = 34;
    [SerializeField] private Text requiredTemperatureText = null;
    [SerializeField] private Text enteredTemperatureText = null;
    [SerializeField] private Text headerText = null;
    [SerializeField] private PostProcessVolume postProcessVolume = null;
    [SerializeField] private Renderer[] heaterRends = null;
    [SerializeField] private AirConditioner airConditioner = null;
    [SerializeField] private Renderer greenhouseRend = null;
    [SerializeField] private Texture frozenGlassTexture = null;
    [SerializeField] private string[] headers = null;
    [SerializeField] private Image[] nodes = null;
    [Header("Prefabs")]
    [SerializeField] private GameObject[] airConditionerEffectPrefabs = null;
    [SerializeField] private GameObject coldAirConditionerEffectPrefab = null;

    private int headerIndex = 0;

    private int enteredTemperature = 0;
    private bool finish = false;
    private float targetBloomIntensity = 0;
    private WaterStick[] waterSticks = null;

    private int EnteredTemperature
    {
        get
        {
            return enteredTemperature;
        }
        set
        {
            enteredTemperature = value;
            enteredTemperatureText.text = enteredTemperature.ToString();
        }
    }
    private new void Awake()
    {
        base.Awake();
        waterSticks = FindObjectsOfType<WaterStick>();
        SetRequiredTemperature();
        SetHeaderText();
        type = LevelType.PLANT;
    }

    private void Update()
    {
        float bloomIntensity = postProcessVolume.profile.GetSetting<Bloom>().intensity.value;
        if (bloomIntensity != targetBloomIntensity)
        {
            bloomIntensity = Mathf.MoveTowards(bloomIntensity, targetBloomIntensity, Time.deltaTime);
            postProcessVolume.profile.GetSetting<Bloom>().intensity.Override(bloomIntensity);
        }

    }

    private void SetRequiredTemperature()
    {
        requiredTemperature = Random.Range(1, 99);
        if (requiredTemperatureText)
            requiredTemperatureText.text = requiredTemperature.ToString();
        EnteredTemperature = 0;
        if (enteredTemperatureText)
            enteredTemperatureText.text = "";
    }

    private void SetHeaderText()
    {
        headerText.text = headers[headerIndex];
    }
    public override void StartLevel()
    {
        return;
    }

    // called by Event Trigger component
    public bool PressNumberButton(int number)
    {
        bool result = EnteredTemperature <= 9;
        if (result)
            EnteredTemperature = EnteredTemperature * 10 + number;
        return result;
    }

    private void SetNodeColor(int index, bool success)
    {
        nodes[index].color = success ?
            Color.HSVToRGB(107f / 360f, 85f / 100f, 100f / 100f) :
            Color.HSVToRGB(11f / 360f, 100f / 100f, 100f / 100f);
    }

    public bool PressEnterButton()
    {
        if (EnteredTemperature != 0 && !finish)
        {
            bool success = EnteredTemperature == requiredTemperature;
            SetNodeColor(headerIndex, success);
            if (success)
            {
                GameManager.Instance.InstantiateSuccessText(headerIndex);
                int emojiIndex = 4;
                if (headerIndex == 0)
                    emojiIndex = 3;
                else if (headerIndex == 2)
                    emojiIndex = 5;
                GameManager.Instance.InstantiateEmojiEffect(emojiIndex);
            }

            if (success && headerIndex < headers.Length - 1)
            {
                StartCoroutine(PlantsReact(success));
                if (headerIndex == 0)
                {
                    Heat(success);
                }
                else if (headerIndex == 1)
                {
                    foreach (WaterStick waterStick in waterSticks)
                        waterStick.OpenWater(1);
                }
                SetRequiredTemperature();
                headerIndex++;
                SetHeaderText();
                return true;
            }
            else
            {
                FinishLevel(success);
                return true;
            }
        }
        return false;
    }

    public override void FinishLevel(bool success)
    {
        StartCoroutine(FinishLevelCoroutine(success));
    }

    private IEnumerator PlantsReact(bool success)
    {
        Plant[] plants = FindObjectsOfType<Plant>();

        if (success)
        {
            if (headerIndex == 0)
            {
                foreach (Plant plant in plants)
                {
                    yield return new WaitForSeconds(Random.Range(0, 0.1f));
                    plant.Success(3);
                }
            }
            else if (headerIndex == 1)
            {
                foreach (Plant plant in plants)
                {
                    yield return new WaitForSeconds(Random.Range(0, 0.1f));
                    plant.Success(6);
                }
            }
            else if (headerIndex == 2)
            {
                foreach (Plant plant in plants)
                {
                    yield return new WaitForSeconds(Random.Range(0, 0.1f));
                    plant.Success(9);
                }
            }
        }
        else
        {
            bool less = EnteredTemperature < requiredTemperature;
            if (headerIndex == 0)
            {
                if (less)
                {
                    FreezeDirt();
                    FreezeGlass();
                }

                foreach (Plant plant in plants)
                {
                    yield return new WaitForSeconds(Random.Range(0, 0.1f));
                    if (less)
                    {
                        plant.Freeze();
                        plant.Fail(2);
                    }
                    else
                        plant.Fail(1);

                }
            }
            else if (headerIndex == 1)
            {
                foreach (Plant plant in plants)
                {
                    yield return new WaitForSeconds(Random.Range(0, 0.1f));
                    plant.Fail(5);
                }
            }
            else if (headerIndex == 2)
            {
                foreach (Plant plant in plants)
                {
                    yield return new WaitForSeconds(Random.Range(0, 0.1f));
                    plant.Fail(8);
                }
            }


        }
    }

    private IEnumerator FinishLevelCoroutine(bool success)
    {
        finish = true;
        if (!success && headerIndex == 0 && EnteredTemperature < requiredTemperature)
        {
            airConditioner.SpeedUp();
            Instantiate(coldAirConditionerEffectPrefab);
            //GameManager.Instance.InstantiateEmojiEffect(6);
        }
        else if (headerIndex == 0)
        {
            Heat(success);
            //GameManager.Instance.InstantiateEmojiEffect(2);
        }
        else if (!success && headerIndex == 1)
        {
            //GameManager.Instance.InstantiateEmojiEffect(0);
            foreach (WaterStick waterStick in waterSticks)
                waterStick.OpenWater(EnteredTemperature < requiredTemperature ? 0 : 2);
        }
        else if (headerIndex == 2)
        {
            //GameManager.Instance.InstantiateEmojiEffect(EnteredTemperature < requiredTemperature ? 1 : (success ? 5 : 1));
            if (success)
                GameManager.Instance.InstantiateEmojiEffect(5);
            airConditioner.SpeedUp();
            Instantiate(airConditionerEffectPrefabs[EnteredTemperature < requiredTemperature ? 0 : (success ? 1 : 2)]);
        }

        yield return new WaitForSeconds(1);
        StartCoroutine(PlantsReact(success));
        yield return new WaitForSeconds(2f);
        GameManager.Instance.FinishLevel(success);
    }

    private void Heat(bool success)
    {
        targetBloomIntensity = success ? 0.1f : 0.3f;
        foreach (Renderer heaterRend in heaterRends)
        {
            heaterRend.materials[1].EnableKeyword("_EMISSION");
            Gradient gradient = ColorManager.CreateGradient(heaterRend.materials[1].color, ColorManager.SetSaturation(heaterRend.materials[1].color, success ? 0.5f : 1));
            ColorManager.DoColorTransition(heaterRend, 1, gradient, 1);
        }
    }

    private void FreezeDirt()
    {
        Gradient gradient = ColorManager.CreateGradient(greenhouseRend.materials[1].color, ColorManager.SetSaturation(greenhouseRend.materials[1].color, 0.3f));
        ColorManager.DoColorTransition(greenhouseRend, 1, gradient, 1);
    }

    private void FreezeGlass()
    {
        greenhouseRend.materials[2].SetTexture("_BaseMap", frozenGlassTexture);
        //LeanTween.delayedCall(0.5f, () => { greenhouseRend.materials[2].SetTexture("_BaseMap", frozenGlassTexture); });
        ColorManager.DoAlphaTransition(greenhouseRend, 2, 0.7f, 1);
    }
}

using FSG.MeshAnimator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantTemperatureLevel : LevelManager
{
    private int requiredTemperature = 34;
    [SerializeField] private Text requiredTemperatureText = null;
    [SerializeField] private Text enteredTemperatureText = null;
    [SerializeField] private Text headerText = null;
    [SerializeField] private GameObject airConditionerEffectPrefab = null;
    [SerializeField] private AirConditioner airConditioner = null;
    [SerializeField] private string[] headers = null;
    [SerializeField] private Image[] nodes = null;

    private int headerIndex = 0;

    private int enteredTemperature = 0;
    private bool finish = false;

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
        SetRequiredTemperature();
        SetHeaderText();

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

            }
            if (success && headerIndex < headers.Length - 1)
            {
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

    private IEnumerator FinishLevelCoroutine(bool success)
    {
        finish = true;
        Instantiate(airConditionerEffectPrefab);
        airConditioner.SpeedUp();
        yield return new WaitForSeconds(1);
        Plant[] plants = FindObjectsOfType<Plant>();

        if (success)
        {

            foreach (Plant plant in plants)
            {
                yield return new WaitForSeconds(Random.Range(0, 0.1f));
                plant.Success();
            }
            // TODO
            // Bitki animasyonu ve efektleri tetiklenecek
            // Bitki animasyonunun sonunda konfeti efekti tetiklenecek ve complete screen ekranı gösterilecek
        }
        else
        {
            foreach (Plant plant in plants)
            {
                yield return new WaitForSeconds(Random.Range(0, 0.1f));
                plant.Fail();
            }
        }
        yield return new WaitForSeconds(2f);
        GameManager.Instance.FinishLevel(success);
    }
}

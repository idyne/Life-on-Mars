using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineralSearchLevel : LevelManager
{
    [SerializeField] private float timeLimit = 45;
    [SerializeField] private Slider timeLeftSlider = null;
    [SerializeField] private GameObject orbEffectPrefab = null;
    [SerializeField] private GameObject directionArrowPrefab = null;
    //[SerializeField] private Transform directionCanvas = null;

    //private RectTransform[] directionArrows;
    private Transform[] directionArrows;
    private Camera cam;
    private List<Mineral> minerals;
    private Vehicle vehicle;
    private bool finished = false;
    private int collectedMineralCount = 0;
    private new void Awake()
    {
        base.Awake();
        vehicle = FindObjectOfType<Vehicle>();
        cam = Camera.main;
        minerals = new List<Mineral>(FindObjectsOfType<Mineral>());
        CreateDirectionArrows();
        timeLeftSlider.maxValue = timeLimit;
        timeLeftSlider.value = timeLimit;
    }

    private void Update()
    {
        ShowFloatingDirectionArrows();
        if (GameManager.Instance.State == GameManager.GameState.STARTED)
        {
            timeLeftSlider.value -= Time.deltaTime;
            if (timeLeftSlider.value <= 0)
                FinishLevel(false);
        }
    }

    private void CreateDirectionArrows()
    {
        directionArrows = new Transform[minerals.Count];
        for (int i = 0; i < minerals.Count; i++)
        {
            directionArrows[i] = Instantiate(directionArrowPrefab).transform;
            directionArrows[i].GetComponentInChildren<Renderer>().materials[1].color = minerals[i].glowColor;
        }

    }

    /*private void CreateDirectionArrows()
    {
        directionArrows = new RectTransform[minerals.Count];
        for (int i = 0; i < minerals.Count; i++)
        {
            directionArrows[i] = Instantiate(directionArrowPrefab, directionCanvas).GetComponent<RectTransform>();
        }
    }*/

    private void ShowFloatingDirectionArrows()
    {
        for (int i = 0; i < minerals.Count; i++)
        {
            Transform directionArrow = directionArrows[i];
            if (minerals[i])
            {
                float r = 3;
                directionArrow.gameObject.SetActive(true);
                Vector3 lookPos = minerals[i].transform.position;
                lookPos.y = directionArrow.position.y;
                Vector3 direction = minerals[i].transform.position - vehicle.transform.position;
                direction.Normalize();
                directionArrow.transform.position = vehicle.transform.position + direction * r;
                directionArrow.LookAt(lookPos);
            }
            else
            {
                directionArrow.gameObject.SetActive(false);
            }

        }
    }

    /*private void ShowFloatingDirectionArrows()
    {
        for (int i = 0; i < minerals.Count; i++)
        {
            RectTransform directionArrow = directionArrows[i];
            directionArrow.gameObject.SetActive(true);
            Vector2 pos = cam.WorldToScreenPoint(minerals[i].transform.position);
            if (pos.x < 0 || pos.x > Screen.width || pos.y < 0 || pos.y > Screen.height)
            {
                directionArrow.anchoredPosition = new Vector2(Mathf.Clamp(pos.x, directionArrow.sizeDelta.x / 2, Screen.width - directionArrow.sizeDelta.x / 2), Mathf.Clamp(pos.y, directionArrow.sizeDelta.y / 2, Screen.height - directionArrow.sizeDelta.x / 2));
                float a = Screen.width / 2;
                float b = Screen.height / 2;
                float x = (directionArrow.anchoredPosition.x - a);
                float y = (directionArrow.anchoredPosition.y - b);
                print("a: " + a + " b: " + b + " x: " + x + " y: " + y);
                float angle = Mathf.Atan((x / y)) * Mathf.Rad2Deg - 90;
                if (y < 0)
                    angle += 180;
                directionArrow.rotation = Quaternion.Euler(0, 0, -angle);
            }
            else
            {
                directionArrow.gameObject.SetActive(false);
            }
        }
        for (int i = minerals.Count; i < directionArrows.Length - minerals.Count; ++i)
        {
            directionArrows[i].gameObject.SetActive(false);
        }


    }*/
    public override void FinishLevel(bool success)
    {
        if (!finished)
        {
            finished = true;
            StartCoroutine(FinishLevelCoroutine(success));
        }
    }

    private IEnumerator FinishLevelCoroutine(bool success)
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.FinishLevel(success);
    }

    public override void StartLevel()
    {
        print("MineralSearchLevel started");
    }

    public IEnumerator CollectMineral(Mineral mineral)
    {
        if (!mineral.isCollected)
        {
            mineral.isCollected = true;
            minerals[minerals.IndexOf(mineral)] = null;
            collectedMineralCount++;
            //minerals.Remove(mineral);
            mineral.transform.LeanScale(Vector3.zero, 0.1f);
            Destroy(mineral.GetComponent<Collider>());
            //Instantiate(orbEffectPrefab, mineral.transform.position + Vector3.up * 0.3f, orbEffectPrefab.transform.rotation);
            yield return new WaitForSeconds(0.1f);
            Destroy(mineral.gameObject);
            if (collectedMineralCount == minerals.Count)
                FinishLevel(true);
        }

    }


}

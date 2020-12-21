using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableFixLevel : LevelManager
{
    [SerializeField] private Animator lidAnim = null;
    [SerializeField] private PathCreator pathCreator = null;
    [SerializeField] private float camSpeed = 2;
    public GameObject ElectricEffectPrefab = null;
    private Camera cam = null;
    private int screwCount = 4;
    private float cameraDistanceTravelled = 0;
    private bool started = false;
    public CableSystem CableSystem = null;
    public int ScrewCount
    {
        get
        {
            return screwCount;
        }
        set
        {
            screwCount = value;
            if (screwCount == 0)
                StartCoroutine(OpenLid());
        }
    }

    private new void Awake()
    {
        base.Awake();
        CableSystem = FindObjectOfType<CableSystem>();
        cam = Camera.main;
        cam.transform.position = pathCreator.path.GetPointAtDistance(cameraDistanceTravelled);
        Vector3 rot = pathCreator.path.GetRotationAtDistance(cameraDistanceTravelled).eulerAngles;
        rot.z = 0;
        cam.transform.rotation = Quaternion.Euler(rot);
    }

    private void Update()
    {
        if (started)
        {
            cameraDistanceTravelled = Mathf.Clamp(cameraDistanceTravelled + Time.deltaTime * camSpeed, 0, pathCreator.path.length);
            if (cameraDistanceTravelled == pathCreator.path.length)
            {
                return;
            }
                
            cam.transform.position = pathCreator.path.GetPointAtDistance(cameraDistanceTravelled);
            Vector3 rot = pathCreator.path.GetRotationAtDistance(cameraDistanceTravelled).eulerAngles;
            rot.z = 0;
            cam.transform.rotation = Quaternion.Euler(rot);
        }
    }
    public override void FinishLevel(bool success)
    {
        StartCoroutine(FinishLevelCoroutine(success));
    }

    private IEnumerator FinishLevelCoroutine(bool success)
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.FinishLevel(success);
    }

    public override void StartLevel()
    {
        started = true;
        LeanTween.delayedCall(pathCreator.path.length / camSpeed + 0.2f, () => { GameManager.Instance.InstantiateInstructionText("Remove the screws", 4); });
    }

    private IEnumerator OpenLid()
    {
        yield return new WaitForSeconds(2f);
        lidAnim.SetTrigger("Open");
        yield return new WaitForSeconds(1f);
        cam.transform.LeanMove(cam.transform.position+ cam.transform.forward * 0.25f, 0.6f);
        yield return new WaitForSeconds(0.6f);
        CableSystem.Unlock();
        GameManager.Instance.InstantiateInstructionText("Fix the cables", 4);
    }

}

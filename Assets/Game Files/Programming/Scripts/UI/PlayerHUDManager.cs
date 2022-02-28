using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHUDManager : MonoBehaviour {

    [Header("Main UI")]
    [SerializeField] private GameObject playerHealthBar;
    [SerializeField] private GameObject targetHealthBar;
    [SerializeField] private TextMeshProUGUI roundTimerText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Optional UI")]
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private bool displayTotalTime;
    [SerializeField] private TextMeshProUGUI targetNameText;
    [SerializeField] private bool displayTargetName;

    [Header("Object References")]
    public GameObject _player;
    private GameObject _target;
    [SerializeField] private GameObject reticle;

    [Header("Other")]
    [SerializeField] private RectTransform[] lineAnchors;
    private bool hasTarget = false;

    private float roundTimer;
    private bool roundActive;

    // ---

    public GameObject Target {
        private get {
            return _target;
        }
        set {
            _target = value;
            if(_target)
                OnTargetAcquired();
            else
                OnTargetLost();
        }
    }

    public Action OnRoundTimerEnd;

    // -----------------------------------------------------------------------------------------------------------

    private void Awake() {
        OnTargetLost();
    }

    private void OnEnable() {
        Application.onBeforeRender += SetLineRenderer;
        roundTimerText.text = "";
    }

    private void OnDisable() {
        Application.onBeforeRender -= SetLineRenderer;
    }

    private void LateUpdate() {
        // Round timer
        if(roundActive) {
            if(roundTimer > 0) {
                roundTimerText.text = $"{(int)roundTimer:00}";
                roundTimer -= Time.deltaTime;
            } else {
                roundTimer = 0;
                roundTimerText.text = "00";
                roundActive = false;
                OnRoundTimerEnd?.Invoke();
            }
        }

        // Total time
        totalTimeText.gameObject.SetActive(displayTotalTime);
        if(displayTotalTime)
            totalTimeText.text = FormatTime(Time.time);

        // Distance line
        if(reticle.activeInHierarchy != hasTarget) {
            hasTarget = reticle.activeInHierarchy;
            if(hasTarget)
                OnTargetAcquired();
            else
                OnTargetLost();
        }
        if(_player && _target)
            distanceText.text = $"{(int)Vector3.Distance(_player.transform.position, _target.transform.position)} m";
    }

    /*private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)) {
            StartRoundTimer(99);
        }
    }*/

    // -----------------------------------------------------------------------------------------------------------

    public void StartRoundTimer(float time) {
        roundTimer = time;
        roundActive = true;

        _player = PlayerManager.Instance.PlayerController.gameObject;
    }

    private void OnTargetAcquired() {
        if(!roundActive)
            return;

        targetHealthBar.SetActive(true);
        targetNameText.gameObject.SetActive(displayTargetName);
        // TODO - set name
        _target = TargetingManager.Instance.Target.gameObject;

        distanceText.gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(true);
    }

    private void OnTargetLost() {
        targetHealthBar.SetActive(false);
        targetNameText.gameObject.SetActive(false);
        _target = null;

        distanceText.gameObject.SetActive(false);
        lineRenderer.gameObject.SetActive(false);
    }

    private void SetLineRenderer() {
        if(lineRenderer.gameObject.activeInHierarchy) {
            lineRenderer.SetPosition(0, lineAnchors[0].position);
            lineRenderer.SetPosition(1, lineAnchors[1].position);
            lineRenderer.SetPosition(2, lineAnchors[2].position);
        }
    }

    private string FormatTime(float time) {
        int hr = (int)time / 3600;
        int min = (int)(time / 60) % 60;
        float sec = time - (hr * 3600) - (min * 60);
        return $"{(hr > 0 ? $"{hr}:" : "")}{min:00}:{sec:00.00}";
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerHUDManager : Singleton<PlayerHUDManager> {

    [Header("Main UI")]
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private HealthBar targetHealthBar;
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
    private bool displayUI = true;

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

    public override void Awake() {
        base.Awake();
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
                EndRound();
            }
        }

        // Total time
        totalTimeText.gameObject.SetActive(displayTotalTime && displayUI);
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

    // -----------------------------------------------------------------------------------------------------------

    public void StartRoundTimer(float time) {
        if(roundActive)
            return;

        roundTimer = time;
        roundActive = true;

        _player = PlayerManager.Instance.PlayerController.gameObject;
        playerHealthBar.SetSmartObject(PlayerManager.Instance.PlayerObject);
    }

    public void EndRound(bool invokeTimerEnd = false) {
        roundTimer = 0;
        roundTimerText.text = "00";
        roundActive = false;
        //reticle.SetActive(false);
        OnTargetLost();

        if(invokeTimerEnd)
            OnRoundTimerEnd?.Invoke();
    }

    public void SetUIVisible(bool active) {
        displayUI = active;
        playerHealthBar.gameObject.SetActive(active);
        roundTimerText.gameObject.SetActive(active);
    }

    private void OnTargetAcquired() {
        if(!roundActive)
            return;

        _target = TargetingManager.Instance.Target.gameObject;
        targetHealthBar.gameObject.SetActive(displayUI);
        targetHealthBar.SetSmartObject(_target.GetComponentInParent<SmartObject>());
        targetNameText.gameObject.SetActive(displayTargetName && displayUI);
        targetNameText.text = _target.transform.root.name;

        distanceText.gameObject.SetActive(displayUI);
        lineRenderer.gameObject.SetActive(displayUI);
    }

    private void OnTargetLost() {
        _target = null;
        targetHealthBar.gameObject.SetActive(false);
        targetHealthBar.SetSmartObject(null);
        targetNameText.gameObject.SetActive(false);

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

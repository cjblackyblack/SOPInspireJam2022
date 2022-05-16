using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;

public class PlayerHUDManager : Singleton<PlayerHUDManager> {

    [Title("Main UI")]
    [SerializeField] private HealthBar playerHealthBar;
    [SerializeField] private HealthBar targetHealthBar;
    [SerializeField] private TextMeshProUGUI roundTimerText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private LineRenderer lineRenderer;

    [Title("Optional UI")]
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private bool displayTotalTime;
    [SerializeField] private TextMeshProUGUI targetNameText;
    [SerializeField] private bool displayTargetName;

    [Title("Object References")]
    [SerializeField] private GameObject reticle;
    [ReadOnly] public GameObject _player;
    private GameObject _target;

    [Title("Multiplayer UI")]
    [SerializeField] private GameObject reticleP2;
    [ReadOnly] public GameObject _player2;
    private GameObject _targetP2;
    [SerializeField] private TextMeshProUGUI distanceTextP2;
    [SerializeField] private LineRenderer lineRendererP2;
    private bool hasTargetP2 = false;

    [Title("Other")]
    [SerializeField] private RectTransform[] lineAnchors;
    private bool hasTarget = false;
    public float RoundTime;
    public float RoundTimer;
    private bool roundActive;
    private bool displayUI = true;
    public float SpeedrunTime;

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

    public GameObject TargetP2 {
        private get {
            return _targetP2;
        }
        set {
            _targetP2 = value;
            if(_targetP2)
                OnTargetAcquiredP2();
            else
                OnTargetLostP2();
        }
    }

    public Action OnRoundTimerEnd;

    // -----------------------------------------------------------------------------------------------------------

    #region Unity Functions

    public override void Awake() {
        base.Awake();
        OnTargetLost();
    }

    private void OnEnable() {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
        {
            RenderPipelineManager.beginCameraRendering += SetLineRenderer;
            TargetingManager.Instance.OnSwitchTarget += UpdateTarget;
            if(GameManager.Instance.Multiplayer) {
                RenderPipelineManager.beginCameraRendering += SetLineRendererP2;
                TargetingManagerP2.Instance.OnSwitchTarget += UpdateTargetP2;
            } else {
                reticleP2.SetActive(false);
            }
            roundTimerText.text = "";
        }
    }

    private void OnDisable() {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0)
        {
            RenderPipelineManager.beginCameraRendering -= SetLineRenderer;
            TargetingManager.Instance.OnSwitchTarget -= UpdateTarget;
            if(GameManager.Instance.Multiplayer) {
                RenderPipelineManager.beginCameraRendering -= SetLineRendererP2;
                TargetingManagerP2.Instance.OnSwitchTarget -= UpdateTargetP2;
            }
        }
    }

    private void LateUpdate() {
        // Round timer
        if(roundActive) {
            if(RoundTimer > 0) {
                roundTimerText.text = $"{(RoundTimer/RoundTime)*100:00.00}";
                RoundTimer -= (RoundTimer/ RoundTime > 0.1f ? Time.deltaTime : (RoundTimer / RoundTime < 0.01f ? Time.deltaTime * 0.25f : Time.deltaTime * 0.65f));
            } else {
                EndRound();
            }
        }

        // Total time
        totalTimeText.gameObject.SetActive(displayTotalTime && displayUI);
        if(displayTotalTime && !GameManager.Instance.Multiplayer)
            totalTimeText.text = FormatTime(SpeedrunTime);

        // Distance line - P1
        if(reticle.activeInHierarchy != hasTarget) {
            hasTarget = reticle.activeInHierarchy;
            if(hasTarget)
                OnTargetAcquired();
            else
                OnTargetLost();
        }
        if(_player && _target)
            distanceText.text = $"{Mathf.Round((Vector3.Distance(_player.transform.position, _target.transform.position)) * 100f) / 100f} m";

        // Distance line - P2
        if(GameManager.Instance.Multiplayer) {
            if(reticleP2.activeInHierarchy != hasTargetP2) {
                hasTargetP2 = reticle.activeInHierarchy;
                if(hasTargetP2)
                    OnTargetAcquiredP2();
                else
                    OnTargetLostP2();
            }
            if(_player2 && _targetP2)
                distanceTextP2.text = $"{Mathf.Round((Vector3.Distance(_player2.transform.position, _targetP2.transform.position)) * 100f) / 100f} m";
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Round Control

    public void StartRoundTimer(float time) {
        if(roundActive)
            return;
        RoundTime = time;
        RoundTimer = time;
        roundActive = true;

        if(!GameManager.Instance.Multiplayer) {
            _player = PlayerManager.Instance.PlayerControllerP1.gameObject;
            playerHealthBar.SetSmartObject(PlayerManager.Instance.PlayerObjectP1);
        } else {
            _player = PlayerManager.Instance.PlayerControllerP1.gameObject;
            _player2 = PlayerManager.Instance.PlayerControllerP2.gameObject;

            targetHealthBar.SetSmartObject(PlayerManager.Instance.PlayerObjectP1);
            targetHealthBar.gameObject.SetActive(true);
            targetNameText.text = _player.name;
            playerHealthBar.SetSmartObject(PlayerManager.Instance.PlayerObjectP2);
            playerHealthBar.gameObject.SetActive(true);
            totalTimeText.text = _player2.name;
        }
    }

    public void EndRound(bool invokeTimerEnd = false) {
        RoundTimer = 0;
        //roundTimerText.text = "00";
        roundActive = false;
        //reticle.SetActive(false);
        OnTargetLost();

        if(invokeTimerEnd)
            OnRoundTimerEnd?.Invoke();
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Targeting

    private void UpdateTarget() {
        hasTarget = TargetingManager.Instance.Target != null;
        if(hasTarget)
            OnTargetAcquired();
        else
            OnTargetLost();
    }

    private void OnTargetAcquired() {
        if(!roundActive)
            return;

        _target = TargetingManager.Instance.Target.gameObject;
        if(!GameManager.Instance.Multiplayer) {
            targetHealthBar.gameObject.SetActive(displayUI);
            targetHealthBar.SetSmartObject(_target.GetComponentInParent<SmartObject>());
            targetNameText.gameObject.SetActive(displayTargetName && displayUI);
            targetNameText.text = _target.transform.root.name;
        }

        distanceText.gameObject.SetActive(displayUI);
        lineRenderer.gameObject.SetActive(displayUI);
    }

    private void OnTargetLost() {
        _target = null;
        if(!GameManager.Instance.Multiplayer) {
            targetHealthBar.gameObject.SetActive(false);
            targetHealthBar.SetSmartObject(null);
            targetNameText.gameObject.SetActive(false);
        }

        distanceText.gameObject.SetActive(false);
        lineRenderer.gameObject.SetActive(false);
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Multiplayer

    private void UpdateTargetP2() {
        hasTargetP2 = TargetingManagerP2.Instance.Target != null;
        if(hasTargetP2)
            OnTargetAcquiredP2();
        else
            OnTargetLostP2();
    }

    private void OnTargetAcquiredP2() {
        if(!roundActive)
            return;

        _targetP2 = TargetingManagerP2.Instance.Target.gameObject;
        distanceTextP2.gameObject.SetActive(displayUI);
        lineRendererP2.gameObject.SetActive(displayUI);
    }

    private void OnTargetLostP2() {
        _targetP2 = null;
        distanceTextP2.gameObject.SetActive(false);
        lineRendererP2.gameObject.SetActive(false);
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    #region Line Renderer

    private void SetLineRenderer(ScriptableRenderContext context, Camera camera) {
        SetLineRenderer();
    }

    private void SetLineRenderer() {
        if(lineRenderer.gameObject.activeInHierarchy) {
            lineRenderer.SetPosition(0, lineAnchors[0].position);
            lineRenderer.SetPosition(1, lineAnchors[1].position);
            lineRenderer.SetPosition(2, lineAnchors[2].position);
        }
    }

    // ---

    private void SetLineRendererP2(ScriptableRenderContext context, Camera camera) {
        SetLineRendererP2();
    }

    private void SetLineRendererP2() {
        if(lineRendererP2.gameObject.activeInHierarchy) {
            lineRendererP2.SetPosition(0, lineAnchors[0].position);
            lineRendererP2.SetPosition(1, lineAnchors[1].position);
            lineRendererP2.SetPosition(2, lineAnchors[2].position);
        }
    }

    #endregion

    // -----------------------------------------------------------------------------------------------------------

    public void SetUIVisible(bool active) {
        displayUI = active;
        playerHealthBar.gameObject.SetActive(active);
        roundTimerText.gameObject.SetActive(active);
    }

    private string FormatTime(float time) {
        int hr = (int)time / 3600;
        int min = (int)(time / 60) % 60;
        float sec = time - (hr * 3600) - (min * 60);
        return $"{(hr > 0 ? $"{hr}:" : "")}{min:00}:{sec:00.00}";
    }
}

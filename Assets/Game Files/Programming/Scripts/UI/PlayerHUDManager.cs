using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    // -----------------------------------------------------------------------------------------------------------

    private void Awake() {
        OnTargetLost();
    }

    private void OnEnable() {
        Application.onBeforeRender += SetLineRenderer;
    }

    private void OnDisable() {
        Application.onBeforeRender -= SetLineRenderer;
    }

    private void LateUpdate() {
        totalTimeText.gameObject.SetActive(displayTotalTime);
        if(displayTotalTime)
            totalTimeText.text = ((int)Time.time).ToString();

        if(reticle.activeInHierarchy != hasTarget) {
            hasTarget = reticle.activeInHierarchy;
            if(hasTarget)
                OnTargetAcquired();
            else
                OnTargetLost();
        }
        //distanceText.text = $"{(int)Vector3.Distance(_player.transform.position, _target.transform.position)} m";
    }

    // -----------------------------------------------------------------------------------------------------------

    private void OnTargetAcquired() {
        Debug.Log("Gained target");
        targetHealthBar.SetActive(true);
        targetNameText.gameObject.SetActive(displayTargetName);
        // TODO - set name

        distanceText.gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(true);
    }

    private void OnTargetLost() {
        Debug.Log("Lost target");
        targetHealthBar.SetActive(false);
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

}

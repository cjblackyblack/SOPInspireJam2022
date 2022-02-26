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

    [Header("Optional UI")]
    [SerializeField] private TextMeshProUGUI totalTimeText;
    [SerializeField] private bool displayTotalTime;
    [SerializeField] private TextMeshProUGUI targetNameText;
    [SerializeField] private bool displayTargetName;

    [Header("Object References")]
    public GameObject _player;
    private GameObject _target;

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

    private void LateUpdate() {
        totalTimeText.gameObject.SetActive(displayTotalTime);
        if(displayTotalTime)
            totalTimeText.text = ((int)Time.time).ToString();
    }

    // -----------------------------------------------------------------------------------------------------------

    private void OnTargetAcquired() {
        targetHealthBar.SetActive(true);
        targetNameText.gameObject.SetActive(displayTargetName);
        // TODO - set name

        distanceText.gameObject.SetActive(true);
        distanceText.text = $"{(int)Vector3.Distance(_player.transform.position, _target.transform.position)}m";
    }

    private void OnTargetLost() {
        targetHealthBar.SetActive(false);
        targetNameText.gameObject.SetActive(false);

        distanceText.gameObject.SetActive(false);
    }

}

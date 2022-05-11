using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using System;

public class TargetingManagerP2 : Singleton<TargetingManager>
{
	public LayerMask Targetable;

	public TargetingState TargetingState;
	public TargetingDistance CurrentDistance;

	public TargetableObject Target;
	private bool lockedOn;
	public float LerpSpeed;

	public float TargetUpdateTime;
	public float TargetSwitchAccuracy;
	private float targetUpdateTicker;

	public float SwitchCooldownTime;
	private bool switchCooldown;
	private float switchTicker;

	private bool targetSwitched;
	private bool tryTargetSwitch;

	public RectTransform TargetingRect;
	public RectTransform RenderingRect;
	public RectTransform RenderingRectImage;
	public float RotateSpeed;
	public Sprite AutoSprite;
	public Sprite LockSprite;
	public CameraLookTarget TargetGroup;

	public Action OnSwitchTarget;

	private void Update()
	{
		if (!PlayerManager.Instance.PlayerControllerP2)
			return;

		if (Target == null)
			TargetingState = TargetingState.None;

		if (TargetingState != TargetingState.None)
		{
			TargetingRect.position = new Vector2(CameraManagerP2.Instance.MainCamera.WorldToScreenPoint(Target.transform.position).x, CameraManagerP2.Instance.MainCamera.WorldToScreenPoint(Target.transform.position).y);
			RenderingRect.transform.localPosition = Vector3.Lerp(RenderingRect.transform.localPosition, TargetingRect.transform.localPosition, LerpSpeed);
			RenderingRectImage.transform.localEulerAngles += (Vector3.forward * RotateSpeed * Time.deltaTime);
			if (!RenderingRect.gameObject.activeSelf)
			{
				RenderingRect.gameObject.SetActive(true);
				RenderingRect.transform.localPosition = TargetingRect.transform.localPosition;//dumb hack because I need the canvas rendered in canvas space but the world to screen point only works on Screen Space
			}
		}
		else
		{
			RenderingRectImage.transform.localRotation = Quaternion.Euler(Vector3.zero);
			if (RenderingRect.gameObject.activeSelf)
				RenderingRect.gameObject.SetActive(false);
		}

		targetUpdateTicker += Time.deltaTime;
		if (targetUpdateTicker > TargetUpdateTime)
		{

			if (PlayerManager.Instance.PlayerObjectP2.ActionStateMachine.CurrentActionEnum != ActionStates.Attack)
			{
				targetUpdateTicker = 0;
				PlayerManager.Instance.PlayerObjectP2.Controller.PollForTargets(); //Calls AutoTarget if things are found
			}
		}

		if (switchCooldown)
		{
			switchTicker += Time.deltaTime;
			if(switchTicker > SwitchCooldownTime)
			{
				switchCooldown = false;
				switchTicker = 0;
			}
		}
		if (PlayerManager.Instance.PlayerControllerP2.lookInput == Vector2.zero)
			tryTargetSwitch = false;                                 

		//if (lockedOn && (((tryTargetSwitch && !targetSwitched) || !tryTargetSwitch) && PlayerManager.Instance.PlayerController.ButtonLockReleaseBuffer > 0) )
		//{
			//tryTargetSwitch = false;
			//ToggleLockOn();
		//}

	}

	public void AutoTarget(Collider[] targetableObjects)
	{
		if (TargetingState == TargetingState.Locked)
			return;

		TargetingState = TargetingState.None;
		Target = null;
		PlayerManager.Instance.PlayerObjectP2.Target = null;

		Collider shortestTarget = null;

		foreach (Collider targetableObject in targetableObjects)
			if (targetableObject != null)
			{
				bool forceContinue = false;
				for (int i = 0; i < PlayerManager.Instance.PlayerObjectP2.TargetPosiitions.Length; i++)
					if (targetableObject == PlayerManager.Instance.PlayerObjectP2.TargetPosiitions[i])
						forceContinue = true;

				if (forceContinue)
					continue;

				if (shortestTarget == null)
					shortestTarget = targetableObject;
				else if ((targetableObject.transform.position - PlayerManager.Instance.PlayerObjectP2.transform.position).sqrMagnitude * (1 / targetableObject.GetComponent<TargetableObject>().Weight) < (shortestTarget.transform.position - PlayerManager.Instance.PlayerObjectP2.transform.position).sqrMagnitude * (1 / shortestTarget.GetComponent<TargetableObject>().Weight))
					if((targetableObject.transform.position - PlayerManager.Instance.PlayerObjectP2.transform.position).magnitude <= targetableObject.GetComponent<TargetableObject>().Radius)
						shortestTarget = targetableObject;
			}

		if(shortestTarget != null)
			TargetObject(shortestTarget.GetComponent<TargetableObject>());

	}

	public void ToggleLockOn()
	{
		if (TargetingState == TargetingState.Locked)
		{
			TargetingState = TargetingState.None;
			targetUpdateTicker = TargetUpdateTime;
			lockedOn = false;
			CameraManagerP2.Instance.LockedOn = false;
			RenderingRect.GetComponent<Image>().sprite = AutoSprite;
			PlayerManager.Instance.PlayerObjectP2.TargetingCollider.transform.localScale = new Vector3(9f, 8f, 9f);
		}
		else if (TargetingState == TargetingState.Auto)
		{
			PlayerManager.Instance.PlayerControllerP2.ButtonLockReleaseBuffer = 0;
			TargetingState = TargetingState.Locked;
			lockedOn = true;
			CameraManagerP2.Instance.LockedOn = true;
			RenderingRect.GetComponent<Image>().sprite = LockSprite;
			PlayerManager.Instance.PlayerObjectP2.TargetingCollider.transform.localScale = new Vector3(90f, 80f, 90f);
		}

		if(lockedOn)
			CameraManagerP2.Instance.ResetCamera();
	}

	//Get Player Input
	//Convert All Player Possible Targets to Screenspace
	//Compare distance from current carget 
	//shortest distance + Dot product of screen pace dir and input 
	public void SwitchTarget(Collider[] targetableObjects)
	{
		if (TargetingState != TargetingState.Locked)
		{
			ToggleLockOn();
			return;
		}

		if (switchCooldown)
			return;

		if (PlayerManager.Instance.PlayerControllerP2.lookInput == Vector2.zero)
		{
			return;
		}

		tryTargetSwitch = true;
		targetSwitched = false;
		Collider shortestTarget = null;

		foreach (Collider targetableObject in targetableObjects)
			if (targetableObject != null)
			{
				bool forceContinue = false;
				for (int i = 0; i < PlayerManager.Instance.PlayerObjectP2.TargetPosiitions.Length; i++)
					if (targetableObject == PlayerManager.Instance.PlayerObjectP2.TargetPosiitions[i])
						forceContinue = true;

				if (forceContinue)
					continue;

				//Compare screenspace direction with supplied input
				if (Vector2.Dot((CameraManagerP2.Instance.MainCamera.WorldToScreenPoint(targetableObject.transform.position) - CameraManagerP2.Instance.MainCamera.WorldToScreenPoint(Target.transform.position)).normalized, PlayerManager.Instance.PlayerControllerP2.lookInput.normalized) < TargetSwitchAccuracy)				
					continue;

				if (shortestTarget == null)
					shortestTarget = targetableObject;
				else if ((targetableObject.transform.position - Target.transform.position).sqrMagnitude * (1 / targetableObject.GetComponent<TargetableObject>().Weight) < (shortestTarget.transform.position - Target.transform.position).sqrMagnitude * (1 / shortestTarget.GetComponent<TargetableObject>().Weight))
					if ((targetableObject.transform.position - PlayerManager.Instance.PlayerObjectP2.transform.position).magnitude <= targetableObject.GetComponent<TargetableObject>().Radius)
						shortestTarget = targetableObject;
			}

		if (shortestTarget != null)
		{
			TargetObject(shortestTarget.GetComponent<TargetableObject>());
			switchCooldown = true;
		}
	}

	public void TargetObject(TargetableObject targetableObject)
	{
		if (TargetingState == TargetingState.None)
			TargetingState = TargetingState.Auto;

		targetSwitched = true;
		Target = targetableObject;
		TargetGroup.target = targetableObject.transform;
		PlayerManager.Instance.PlayerObjectP2.Target = Target;
		OnSwitchTarget?.Invoke();
	}
}
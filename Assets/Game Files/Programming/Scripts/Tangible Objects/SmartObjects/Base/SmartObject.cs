using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using KinematicCharacterController;

public class SmartObject : PhysicalObject, ICharacterController
{
	public KinematicCharacterMotor Motor => GetComponent<KinematicCharacterMotor>();
	public BaseController Controller => GetComponent<BaseController>();
	public ActionStateMachine ActionStateMachine => GetComponent<ActionStateMachine>();
	public LocomotionStateMachine LocomotionStateMachine => GetComponent<LocomotionStateMachine>();
	public EffectMachine EffectMachine => GetComponent<EffectMachine>();
	public Animator ShadowAnimator;
	//public RagdollController Ragdoll => GetComponentInChildren<RagdollController>();

	public Action<ActionStates> OnActionChange;
	public Action<LocomotionStates> OnLocomotionChange;

	[PropertyOrder(-98)]
	public StatMods StatMods;


	public SmartObjectProperties SmartObjectProperties;
	[TitleGroup("Properties")]
	public BodyReferences BodyReferences;
	public Gun[] Guns;


	[FoldoutGroup("Variables/Time")]
	public float ActiveAirTime;
	[FoldoutGroup("Variables/Time")]
	public int CurrentAirTime;
	[FoldoutGroup("Variables/Time")]
	public int TrackingTime;



	[FoldoutGroup("Variables/Velocity")]
	public float GravityModifier;

	[FoldoutGroup("Variables/Input"), PropertyOrder(-10)]
	public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;
	[FoldoutGroup("Variables/Input")]
	public Vector3 InputVector;
	[FoldoutGroup("Variables/Input")]
	public Vector3 LookInputVector;
	[FoldoutGroup("Variables/Input")]
	public Vector3 MovementVector;
	[FoldoutGroup("Variables/Input")]
	public Vector3 StoredMovementVector;

	public Collider[] GrabbedLedgeColliders = new Collider[4];
	public Collider[] CollisionColliders = new Collider[8];
	public Collider[] PossibleTargets = new Collider[16];
	public Collider TargetingCollider;

	public LedgeData ClimbingInfo;
	public Vector3 CharacterCenter;
	public float CharacterHeight;
	public float CharacterRadius;

	public bool GuardSuccess;
	public int MaxAirJumps;
	public int AirJumps;
	public int Cooldown;

	public int HitStun;
	public Vector3 KnockbackDir;
	public override void Start()
	{
		base.Start();
		Motor.CharacterController = this;
		SetTimeScale(LocalTimeScale);
		//Ragdoll.DisableRagdoll();
	}

	public void SetInputDir(Vector2 input, bool UseTargetOverride = false) //ADD OVERRIDE OR SOMETHING TO NOT TAKE CAMERA FOR FORWARD DIRECTION, THIS IS FOR EZ LOCK ON ATTACKS AND FOR AI 
	{
		// Clamp input
		
		Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(input.x, 0f, input.y), 1f);

		// Calculate camera direction and rotation on the character plane
		Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(CameraManager.Instance.MainCamera.transform.rotation * Vector3.forward, Motor.CharacterUp).normalized;

		if(Target != null && UseTargetOverride)
			cameraPlanarDirection = Vector3.ProjectOnPlane((Target.transform.position - transform.position).normalized, Motor.CharacterUp).normalized;

		if (cameraPlanarDirection.sqrMagnitude == 0f)
		{
			cameraPlanarDirection = Vector3.ProjectOnPlane(CameraManager.Instance.MainCamera.transform.rotation * Vector3.up, Motor.CharacterUp).normalized;
		}
		Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

		switch (LocomotionStateMachine.CurrentLocomotionEnum) //bruh put this shit in the damn locomotion state machine
		{
			case LocomotionStates.Grounded:
				{
					// Move and look inputs
					InputVector = cameraPlanarRotation * moveInputVector;

					switch (OrientationMethod)
					{
						case OrientationMethod.TowardsCamera:
							LookInputVector = cameraPlanarDirection;
							break;
						case OrientationMethod.TowardsMovement:
							LookInputVector = InputVector.normalized;
							break;
					}

					break;
				}
			case LocomotionStates.GroundedShoot:
				{
					// Move and look inputs
					InputVector = cameraPlanarRotation * moveInputVector;

					switch (OrientationMethod)
					{
						case OrientationMethod.TowardsCamera:
							LookInputVector = cameraPlanarDirection;
							break;
						case OrientationMethod.TowardsMovement:
							LookInputVector = InputVector.normalized;
							break;
					}

					break;
				}
			case LocomotionStates.Aerial:
				{
					// Move and look inputs
					InputVector = cameraPlanarRotation * moveInputVector;

					switch (OrientationMethod)
					{
						case OrientationMethod.TowardsCamera:
							LookInputVector = cameraPlanarDirection;
							break;
						case OrientationMethod.TowardsMovement:
							LookInputVector = InputVector.normalized;
							break;
					}
					break;
				}
			case LocomotionStates.AerialShoot:
				{
					// Move and look inputs
					InputVector = cameraPlanarRotation * moveInputVector;

					switch (OrientationMethod)
					{
						case OrientationMethod.TowardsCamera:
							LookInputVector = cameraPlanarDirection;
							break;
						case OrientationMethod.TowardsMovement:
							LookInputVector = InputVector.normalized;
							break;
					}
					break;
				}
			case LocomotionStates.Climbing:
				{
					InputVector = cameraPlanarRotation * moveInputVector;

					switch (OrientationMethod)
					{
						case OrientationMethod.TowardsCamera:
							LookInputVector = cameraPlanarDirection;
							break;
						case OrientationMethod.TowardsMovement:
							LookInputVector = InputVector.normalized;
							break;
					}
					break;
				}
		}
		Animator.SetFloat("InputY", input.y);
		Animator.SetFloat("InputX", Mathf.Abs(input.y) > 0.5f ? 0: input.x);
		ShadowAnimator.SetFloat("InputY", input.y);
		ShadowAnimator.SetFloat("InputX", Mathf.Abs(input.y) > 0.5f ? 0 : input.x);


	}

	public override void TakeDamage(ref DamageInstance damageInstance)
	{
		transform.rotation = Quaternion.LookRotation((Vector3.ProjectOnPlane(damageInstance.origin.transform.position - transform.position, Motor.CharacterUp)), Motor.CharacterUp);

		KnockbackDir = (((-Motor.CharacterForward * damageInstance.knockbackDirection.z)
		 + (Motor.CharacterRight * damageInstance.knockbackDirection.x)
		 + (Motor.CharacterUp * damageInstance.knockbackDirection.y)) * damageInstance.knockbackStrength)
		 + (InputVector.normalized * 0.5f);
		if (damageInstance.knockbackDirection.y > 0)
		{
			LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
			Motor.ForceUnground(0.02f);
		}
		Motor.BaseVelocity *= 0;
		HitStun = damageInstance.hitStun;
		Stats.HP -= Mathf.RoundToInt(damageInstance.damage);
		GameManager.Instance.GlobalHitStop(damageInstance.hitStopTime);
		ActionStateMachine.ChangeActionState(ActionStates.Hurt);
	}

	public override bool HitConfirmReaction(PhysicalObjectTangibility hitTangibility, DamageInstance damageInstance)
	{
		return true;
	}



	#region Kinematic Machine Callbacks
	public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
	{

		LocomotionStateMachine.UpdateRotation(ref currentRotation, deltaTime );
		ActionStateMachine.UpdateRotation(ref currentRotation, deltaTime);
	}

	public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
	{

		ActionStateMachine.UpdateVelocity(ref currentVelocity, deltaTime);
		LocomotionStateMachine.UpdateVelocity(ref currentVelocity, deltaTime);
		//currentVelocity *= LocalTimeScale;
		Velocity = currentVelocity;

	}

	public void BeforeCharacterUpdate(float deltaTime)
	{
		if (MovementVector != Vector3.zero)
			StoredMovementVector = MovementVector;


		Controller.BeforeObjectUpdate();


		//Motor.BaseVelocity *= LocalTimeScale;
		ActionStateMachine.BeforeCharacterUpdate(deltaTime);
		LocomotionStateMachine.BeforeCharacterUpdate(deltaTime);
		if (Cooldown > 0)
			Cooldown--;
		/*
		if(Controller.Button1Buffer == 5)
		{
			Ragdoll.EnableRagdoll();
		}

		if (Controller.Button1ReleaseBuffer == 5)
		{
			Ragdoll.DisableRagdoll();
		}
		*/
	}

	public void PostGroundingUpdate(float deltaTime)
	{
		ActionStateMachine.PostGroundingUpdate(deltaTime);
		LocomotionStateMachine.PostGroundingUpdate(deltaTime);

	}

	public void AfterCharacterUpdate(float deltaTime)
	{


		CurrentTime += LocalTimeScale;
		if (CurrentTime - CurrentFrame >= 1)
		{
			CurrentFrame = (int)CurrentTime;
			//moved these to test something if slow down acts weird in the future this is why
			ActionStateMachine.AfterCharacterUpdate(deltaTime);
			LocomotionStateMachine.AfterCharacterUpdate(deltaTime);
		}
	}

	public bool IsColliderValidForCollisions(Collider coll)
	{
		if(LocomotionStateMachine.IsColliderValidForCollisions(coll) && ActionStateMachine.IsColliderValidForCollisions(coll))// && !Ragdoll.Colliders.Contains(coll))
			return true;
		return false;
	}

	public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		ActionStateMachine.OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
		LocomotionStateMachine.OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
	}

	public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{
		ActionStateMachine.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
		LocomotionStateMachine.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
	}

	public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{
		ActionStateMachine.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
		LocomotionStateMachine.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
	}

	public void OnDiscreteCollisionDetected(Collider hitCollider)
	{
		ActionStateMachine.OnDiscreteCollisionDetected(hitCollider);
		LocomotionStateMachine.OnDiscreteCollisionDetected(hitCollider);
	}
	#endregion

	public void GrabLedge(AutoLedge ledge, bool continueClimb, float prevNorm)
	{
		// Transition to ladder climbing state
		//if ((LocomotionStateMachine.CurrentLocomotionEnum == LocomotionStates.Aerial && AirTime > 1 )|| continueClimb)
		//{
			
			ClimbingInfo.ActiveLedge = ledge;
			LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Climbing);
			ActionStateMachine.ChangeActionState(ActionStates.Idle);
		if (prevNorm >= ClimbingInfo.AnchorClamp.y && continueClimb)
			ClimbingInfo.NormalizedPosition = ClimbingInfo.AnchorClamp.x + ClimbingInfo.AnchorReOffset;
		else if (prevNorm <= ClimbingInfo.AnchorClamp.x && continueClimb)
			ClimbingInfo.NormalizedPosition = ClimbingInfo.AnchorClamp.y - ClimbingInfo.AnchorReOffset;
		//}
		// Transition back to default movement state
		//else 
		if (ClimbingInfo.ClimbingState == ClimbingState.Climbing)
		{
			ClimbingInfo.ClimbingState = ClimbingState.DeAnchoring;
			ClimbingInfo.AnchorTime = 0f;
			ClimbingInfo.AnchorStartPos = Motor.TransientPosition;
			ClimbingInfo.AnchorStartRot = Motor.TransientRotation;
			ClimbingInfo.TargetLedgePos = Motor.TransientPosition;
			ClimbingInfo.TargetLadderRot = ClimbingInfo.RotationBeforeClimbing;
		}
	}

	public void PollLedge()
	{
		PhysicsExtensions.OverlapBoxNonAlloc(ClimbingInfo.LedgeGrabber, GrabbedLedgeColliders);
		for (int i = 0; i < GrabbedLedgeColliders.Length; i++)
			if (GrabbedLedgeColliders[i] != null)
				if(GrabbedLedgeColliders[i].TryGetComponent(out AutoLedge ledge))
					GrabLedge(ledge, false, ClimbingInfo.NormalizedPosition);
		for (int i = GrabbedLedgeColliders.Length-1; i >= 0; i--)
			if (GrabbedLedgeColliders[i] != null)
				GrabbedLedgeColliders[i] = null;
	}

	public override void SetTimeScale(float speed)
	{
		LocalTimeScale = speed;
		Motor.LocalTime = speed;
		Animator.SetFloat("Time", speed);
		ShadowAnimator.SetFloat("Time", speed);
	}



	public void ToggleGuns(bool toggle)
	{
		foreach (Gun gun in Guns)
			gun.Active = toggle;
	}

	public void ToggleGuns(bool toggle, int index)
	{
		Guns[index].Active = toggle;
	}

	public void ToggleBodyVFX(BodyVFX bodyVFX, bool toggle)
	{
		switch (bodyVFX)
		{
			case BodyVFX.Weapon1:
				{
					break;
				}
			case BodyVFX.Weapon2:
				{
					break;
				}
			case BodyVFX.Boost:
				{
					break;
				}
			case BodyVFX.Jump:
				{
					break;
				}
			case BodyVFX.Hover:
				{
					break;
				}
		}
	}
}
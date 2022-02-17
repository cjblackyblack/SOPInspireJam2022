using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartState : ScriptableObject
{
	public string AnimationState;
	public float AnimationTransitionTime;
	public float AnimationTransitionOffset;
	public int MaxTime;
	public virtual void OnEnter(SmartObject smartObject)
	{
		smartObject.CurrentTime = -1;
		smartObject.CurrentFrame = -1;
		if (AnimationTransitionTime != 0)
		{
			smartObject.Animator.CrossFadeInFixedTime(AnimationState, AnimationTransitionTime, 0, AnimationTransitionOffset);
			smartObject.ShadowAnimator.CrossFadeInFixedTime(AnimationState, AnimationTransitionTime, 0, AnimationTransitionOffset);
		}
		else
		{
			smartObject.Animator.Play(AnimationState, 0, 0);
			smartObject.ShadowAnimator.Play(AnimationState, 0, 0);
		}
	}

	public virtual void OnExit(SmartObject smartObject)
	{

	}

	public virtual void HandleState(SmartObject smartObject)
	{

	}

	public virtual void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{

	}

	public virtual void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{

	}

	public virtual void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{

	}

	public virtual void PostGroundingUpdate(SmartObject smartObject, float deltaTime)
	{

	}

	public virtual void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{

	}

	public virtual bool IsColliderValidForCollisions(SmartObject smartObject, Collider coll)
	{
		return true;
	}

	public virtual void OnGroundHit(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{

	}

	public virtual void OnMovementHit(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
	{

	}

	public virtual void ProcessHitStabilityReport(SmartObject smartObject, Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
	{

	}

	public virtual void OnDiscreteCollisionDetected(SmartObject smartObject, Collider hitCollider)
	{

	}
}
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/GroundedShoot/Move")]
public class MoveShootState : SmartState
{
	public float MaxStableMoveSpeed;
	public float StableMovementSharpness;

	public override void OnEnter(SmartObject smartObject)
	{
		if (smartObject.LocomotionStateMachine.PreviousLocomotionEnum == LocomotionStates.Grounded && smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Move)
		{

		}
		else
		{
			smartObject.CurrentTime = -1;
			smartObject.CurrentFrame = -1;
		}
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
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.GroundedShoot);
	}
	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		//if (smartObject.Controller.Button1Buffer > 0 && smartObject.Cooldown <= 0)
		//	smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

		if (smartObject.Controller.Button2Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Boost);

		if ((smartObject.Controller.Button3ReleaseBuffer > 0 || !smartObject.Controller.Button3Hold))
		{
			smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Move);
		}

		if (smartObject.Controller.Button4Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);

		if (smartObject.InputVector == Vector3.zero)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

		smartObject.MovementVector = smartObject.InputVector;
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
	}
}
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Idle")]
public class IdleState : SmartState
{
    public int frictionStrength;
	public bool ignorePreviousAttack; //spaghet fix for final boss
	public override void OnEnter(SmartObject smartObject)
	{
		if (smartObject.LocomotionStateMachine.PreviousLocomotionEnum == LocomotionStates.GroundedShoot && smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Idle)
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
		smartObject.MovementVector *= 0;
		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
	}
	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if (smartObject.Controller.Button1Buffer > 0 && smartObject.Cooldown <= 0 && !ignorePreviousAttack)
			if (smartObject.ActionStateMachine.PreviousActionEnum == ActionStates.Attack && smartObject.CurrentFrame < 6) 
			{ 
				if ((smartObject.LocomotionStateMachine.CurrentLocomotionState.SmartStates[(int)ActionStates.Attack] as AttackState).FollowUpState)
					smartObject.ActionStateMachine.ChangeActionState((smartObject.LocomotionStateMachine.CurrentLocomotionState.SmartStates[(int)ActionStates.Attack] as AttackState).FollowUpState); 
			}
			else
				smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

		if (smartObject.Controller.Button2Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Boost);

		if ((smartObject.Controller.Button3Buffer > 0 || smartObject.Controller.Button3Hold) && smartObject.Cooldown <= 0)
		{
			smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.GroundedShoot);
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
		}

		if (smartObject.Controller.Button4Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);

		if (smartObject.InputVector != Vector3.zero)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Move);

		if(smartObject.GetComponent<PlayerController>())
			smartObject.Animator.SetBool("Rotating", (Mathf.Abs(CameraManager.Instance.FreeLookCam.m_XAxis.m_InputAxisValue) > 0.1f));


	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
	}
}
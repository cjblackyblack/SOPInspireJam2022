using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/Hurt")]
public class AerialHurtState : SmartState
{
	public TangibilityFrames[] TangibilityFrames;
	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);

		smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial);
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
		//if(smartObject.Motor.GroundingStatus.IsStableOnGround)
		//{
		//	smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Grounded);
		//	smartObject.ActionStateMachine.ChangeActionState(ActionStates.Hurt);
		//}
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		currentVelocity = smartObject.KnockbackDir * smartObject.Friction;
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
	}

	public override void UpdateRotation(SmartObject smartObject, ref Quaternion currentRotation, float deltaTime)
	{
		Vector3 smoothedLookInputDirection = smartObject.Motor.CharacterForward;

		currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, smartObject.Motor.CharacterUp);

		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateCharacterUp(smartObject, ref currentRotation, deltaTime);
	}

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if (smartObject.CurrentFrame > smartObject.HitStun)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}
}

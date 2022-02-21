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

	public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.AfterCharacterUpdate(smartObject, deltaTime);
		if (smartObject.CurrentFrame > MaxTime)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
	}

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		base.BeforeCharacterUpdate(smartObject, deltaTime);
		CombatUtilities.CreateTangibilityFrames(smartObject, TangibilityFrames);
	}

}

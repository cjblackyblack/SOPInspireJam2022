using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Idle")]
public class IdleState : SmartState
{
    public int frictionStrength;
	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
        smartObject.MovementVector *= 0;

	}
	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if (smartObject.Controller.Button1Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

		if (smartObject.Controller.Button2Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Guard);

		if (smartObject.Controller.Button4Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Jump);

		if (smartObject.InputVector != Vector3.zero)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Move);

		
	}

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
		smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
	}
}
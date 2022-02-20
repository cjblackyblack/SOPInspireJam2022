using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Move")]
public class MoveState : SmartState
{
    public float MaxStableMoveSpeed;
    public float StableMovementSharpness;

	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
		if (smartObject.Controller.Button1Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

		if (smartObject.Controller.Button2Buffer > 0)
			smartObject.ActionStateMachine.ChangeActionState(ActionStates.Boost);

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
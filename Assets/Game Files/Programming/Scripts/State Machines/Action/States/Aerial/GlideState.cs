using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "CharacterState/ActionState/Aerial/Glide")]
public class GlideState : SmartState
{
    public float GravityMod;
    public override void OnEnter(SmartObject smartObject)
    {
        base.OnEnter(smartObject);

        smartObject.GravityModifier = GravityMod;
    }

    public override void OnExit(SmartObject smartObject)
    {
        base.OnExit(smartObject);
        smartObject.ClimbingInfo.CanGrab = false;
        smartObject.GravityModifier = 1;
    }
    public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
    {
        smartObject.MovementVector = smartObject.InputVector;
        smartObject.StoredMovementVector = smartObject.MovementVector;
    }

    public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        if(smartObject.CurrentFrame == 0)
		{
            currentVelocity = Vector3.ProjectOnPlane(currentVelocity, smartObject.Motor.CharacterUp);
        }
        // Add move input
        smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);

    }


    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
    {

        if (smartObject.Controller.Button1Buffer > 0)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);

        if (!smartObject.Controller.Button2Hold || smartObject.Controller.Button2ReleaseBuffer > 0)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);

    }
}
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterState/ActionState/Grounded/Jump")]
public class JumpState : SmartState 
{
	public int JumpFrame;
    public float JumpSquatFriction;
	public float JumpPower = 10;
	public float JumpScalableForwardSpeed = 1;
    public float jumpCapsuleResume;
    public float FallVelocity;
    public MotionCurve MotionCurve;

	public override void OnEnter(SmartObject smartObject)
	{
		base.OnEnter(smartObject);
        smartObject.Controller.Button4Buffer = 0;
        smartObject.MovementVector = Vector3.zero;
    }

	public override void OnExit(SmartObject smartObject)
	{
		base.OnExit(smartObject);
        smartObject.GravityModifier = 1;
        smartObject.Motor.SetGroundSolvingActivation(true);
        smartObject.ClimbingInfo.CanGrab = false;
    }
	public override void BeforeCharacterUpdate(SmartObject smartObject, float deltaTime)
	{
        if (smartObject.CurrentFrame > JumpFrame)// && smartObject.InputVector.sqrMagnitude > 0f)
            smartObject.MovementVector = smartObject.InputVector;
    }

	public override void UpdateVelocity(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
	{
        if (smartObject.CurrentFrame < JumpFrame)
        {

                smartObject.MovementVector *= JumpSquatFriction;
                smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
            
        }
        else if ((smartObject.CurrentFrame >= JumpFrame) && smartObject.CurrentAirTime == 0)
        {
            Jump(smartObject, ref currentVelocity, deltaTime);
        }
        //else if (smartObject.CurrentTime == jumpCapsuleResume)
        //{
            //smartObject.Motor.ForceUnground(0.02f);
            // smartObject.Motor.SetCapsuleDimensions(0.35f, 1.5f, 0.75f);
        //}
        else
        {
            smartObject.Motor.ForceUnground(0.02f);
            smartObject.LocomotionStateMachine.CurrentLocomotionState.CalculateStateVelocity(smartObject, ref currentVelocity, deltaTime);
            //Test Hold to Increase Height
            smartObject.GravityModifier = (smartObject.Controller.Button4Hold && smartObject.Controller.Button4Buffer == 0 && smartObject.Controller.Button4ReleaseBuffer == 0) ? MotionCurve.VerticalCurve.Evaluate(smartObject.CurrentFrame) : MotionCurve.GravityMod(smartObject) / smartObject.LocalTimeScale;
        }
    }

    public override void AfterCharacterUpdate(SmartObject smartObject, float deltaTime)
	{

        if (smartObject.Controller.Button1Buffer > 0 && smartObject.CurrentAirTime > 0)
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Attack);


        float yVel = Vector3.Project(smartObject.Motor.BaseVelocity, smartObject.Motor.CharacterUp).y;
        if ((yVel < FallVelocity && smartObject.CurrentFrame > JumpFrame) || smartObject.CurrentFrame > MaxTime)
        {
            smartObject.ActionStateMachine.ChangeActionState(ActionStates.Idle);
            smartObject.Motor.SetGroundSolvingActivation(true);
        }
    }

	public void Jump(SmartObject smartObject, ref Vector3 currentVelocity, float deltaTime)
    {
        smartObject.MovementVector = smartObject.InputVector;
        Vector3 jumpDirection = smartObject.Motor.CharacterUp;
		if (smartObject.Motor.GroundingStatus.FoundAnyGround && !smartObject.Motor.GroundingStatus.IsStableOnGround )//&& (Vector3.Dot(Vector3.down, smartObject.Gravity.normalized) > 0.99f))
		{
			jumpDirection = smartObject.Motor.GroundingStatus.GroundNormal;
		}
		smartObject.Motor.ForceUnground(0.02f);
        //smartObject.Motor.SetCapsuleDimensions(0.35f, 1f, 1f);
        smartObject.Motor.SetGroundSolvingActivation(false);
		currentVelocity += (((jumpDirection * (JumpPower))) - (Vector3.Project(currentVelocity, smartObject.Motor.CharacterUp))) ;
        currentVelocity += (smartObject.MovementVector * JumpScalableForwardSpeed);
        smartObject.LocomotionStateMachine.ChangeLocomotionState(LocomotionStates.Aerial); //this is here to catch Coyote Time edge cases where we just changed from ground to aerial
        smartObject.ClimbingInfo.CanGrab = true;
        smartObject.ActiveAirTime = 10;
        smartObject.CurrentAirTime = 10;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : BaseController
{
    public SmartObject SmartObject => GetComponent<SmartObject>();

    public AnimationCurve ForwardCurve;
    public AnimationCurve StrafeCurve;

    public AnimationCurve Button1Curve;
    public AnimationCurve Button2Curve;
    public AnimationCurve Button3Curve;
    public AnimationCurve Button4Curve;

    public float CurrentTime;
	private void FixedUpdate()
	{
        CurrentTime += 1;
	}
	public override void BeforeObjectUpdate()
	{
        input = new Vector2(ForwardCurve.Evaluate(Time.time), StrafeCurve.Evaluate(CurrentTime));
        SmartObject.SetInputDir(input, true);
	}

}

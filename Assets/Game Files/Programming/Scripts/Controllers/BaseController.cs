using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
	public Vector2 input;

	public int Button1Buffer;
	public int Button2Buffer;
	public int Button3Buffer;
	public int Button4Buffer;
	public int ButtonLockBuffer;
	public int ButtonRecenterBuffer;

	public int Button1ReleaseBuffer;
	public int Button2ReleaseBuffer;
	public int Button3ReleaseBuffer;
	public int Button4ReleaseBuffer;
	public int ButtonLockReleaseBuffer;
	public int ButtonRecenterReleaseBuffer;

	public bool Button1Hold;
	public bool Button2Hold;
	public bool Button3Hold;
	public bool Button4Hold;
	public bool ButtonLockHold;
	public bool ButtonRecenterHold;
}
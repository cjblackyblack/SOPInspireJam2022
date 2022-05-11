using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager> 
{
	public PlayerController PlayerControllerP1;
	public SmartObject PlayerObjectP1;

	public PlayerController PlayerControllerP2;
	public SmartObject PlayerObjectP2;

	public override void Start()
	{
		base.Start();
		Application.targetFrameRate = 60;
	}
}

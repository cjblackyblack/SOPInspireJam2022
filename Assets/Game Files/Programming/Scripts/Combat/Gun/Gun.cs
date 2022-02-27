using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
	public SmartObject SourceObject => transform.root.GetComponent<SmartObject>();

	public bool Active;
	public float CurrentTime;
	public int CurrentFrame;

	public GunData GunData;
	float inactiveTimer;
	private void Update()
	{
		if (!Active)
			inactiveTimer += Time.deltaTime;
		if(inactiveTimer > 3f)
		{
			inactiveTimer = 0;
			CurrentTime = 0;
			CurrentFrame = 0;
		}
	}
	private void FixedUpdate()
	{
		if (CurrentTime > GunData.MaxTime)
		{
			CurrentTime = 0;
			CurrentFrame = 0;
		}

		if (!GunData || !Active)
			return;

		CurrentTime += SourceObject.LocalTimeScale;
		if (CurrentTime - CurrentFrame >= 1)
		{
			CurrentFrame = (int)CurrentTime;

			foreach(BulletData bulletData in GunData.Bullets)
			{
				if (CurrentFrame == bulletData.Frame)
				{ 
					GameObject bullet = Instantiate(bulletData.Bullet, transform.position,Quaternion.identity);
					if (SourceObject.GetComponent<PlayerController>())
					{
						bullet.transform.rotation = CameraManager.Instance.MainCamera.transform.rotation * Quaternion.Euler(bulletData.Direction);
						if (SourceObject.Motor.GroundingStatus.IsStableOnGround && CameraManager.Instance.FreeLookCam.m_YAxis.Value > 0.5f)
							bullet.transform.rotation = SourceObject.transform.rotation;
					}
				}
			}
		}
	}
}
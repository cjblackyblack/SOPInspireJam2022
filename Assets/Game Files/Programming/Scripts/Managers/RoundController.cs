using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Cinemachine;

public class RoundController : MonoBehaviour
{
	public PlayableDirector Director => GetComponent<PlayableDirector>();
	public TimelineAsset StartTimeline;
	public TimelineAsset WinTimeline;
	public TimelineAsset LoseTimeline;
	public TimelineAsset MPTimeline;
	public CinemachineVirtualCamera RoundEndCamera;
	public CinemachineVirtualCamera RoundEndCamera2;
	public Transform[] SpawnPoints;
	public GameObject[] Opponents;
	public bool MirrorMatch;
	public bool ReverseMatch;
	public List<SmartObject> ActiveOpponents;
	public bool FinalRound;
	bool started = false;

	private void Start()
	{
		//StartIntroduction();
		//GameEventManager.Instance.playRoundStartTimeline += StartIntroduction;\
		//StartRound();
		GameManager.Instance.CurrentRoundStartController = this;

		//PlayerHUDManager.Instance.OnRoundTimerEnd += EndTimerTrigger;

	}

	private void OnDestroy()
	{
		//GameEventManager.Instance.playRoundStartTimeline -= StartIntroduction;
		//PlayerHUDManager.Instance.OnRoundTimerEnd -= EndTimerTrigger;
	}

	private void Update()
	{
		if (!started)
			return;

		PlayerHUDManager.Instance.SpeedrunTime += Time.deltaTime;


		if (!GameManager.Instance.Multiplayer)
		{

			for (int i = ActiveOpponents.Count - 1; i >= 0; i--)
				if (ActiveOpponents[i].Stats.HP <= 0)
					ActiveOpponents.RemoveAt(i);

			if (ActiveOpponents.Count == 0)
				FinishRound(true);

			if (PlayerManager.Instance.PlayerObjectP1.Stats.HP <= 0 || (PlayerHUDManager.Instance.RoundTimer < 0f && started))
				FinishRound(false);
		}
		else
		{
			if (PlayerManager.Instance.PlayerObjectP1.Stats.HP <= 0 || PlayerManager.Instance.PlayerObjectP2.Stats.HP <= 0 || (PlayerHUDManager.Instance.RoundTimer < 0f && started))
				FinishRound(false);
		}
	}

	public void StartRound()
	{
		PlaceEntities();
		StartIntroduction();
	}

	public void EndTimerTrigger()
	{
		
		PlayerManager.Instance.PlayerObjectP1.ActionStateMachine.ChangeActionState(PlayerManager.Instance.PlayerObjectP1.LocomotionStateMachine.DeadState);
		if (GameManager.Instance.Multiplayer)
		{
			PlayerManager.Instance.PlayerObjectP1.ActionStateMachine.ChangeActionState(PlayerManager.Instance.PlayerObjectP1.LocomotionStateMachine.DeadState);
		}
	}

	public void FinishRound(bool win)
	{
		if (!GameManager.Instance.Multiplayer)
		{
			if (win)
			{
				Director.playableAsset = WinTimeline;
				Director.SetGenericBinding(WinTimeline.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<CinemachineBrain>());
				Director.SetGenericBinding(WinTimeline.GetRootTrack(2), PlayerManager.Instance.PlayerObjectP1.Animator);
				GameManager.Instance.MusicSource.Stop();
				GameManager.Instance.MusicSource.loop = false;
				GameManager.Instance.MusicSource.clip = GameManager.Instance.WinTrack;
				GameManager.Instance.MusicSource.Play();

			}
			else
			{
				Director.playableAsset = LoseTimeline;
				Director.SetGenericBinding(LoseTimeline.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<CinemachineBrain>());
				Director.SetGenericBinding(LoseTimeline.GetRootTrack(2), PlayerManager.Instance.PlayerObjectP1.Animator);
				GameManager.Instance.GameOver();
				if (PlayerHUDManager.Instance.RoundTimer <= 0)
					EndTimerTrigger();
			}
		}
		else
		{
			Director.playableAsset = MPTimeline;
			Director.SetGenericBinding(MPTimeline.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<CinemachineBrain>());
			if(PlayerHUDManager.Instance.RoundTimer > 0f)
			Director.SetGenericBinding(MPTimeline.GetRootTrack(2), PlayerManager.Instance.PlayerObjectP1.Animator); //winner
			Director.SetGenericBinding(MPTimeline.GetRootTrack(3), PlayerManager.Instance.PlayerObjectP1.Animator); //loser
			//Director.SetGenericBinding(MPTimeline.GetRootTrack(4), PlayerManager.Instance.PlayerObjectP1.Animator); //second loser for time out
			GameManager.Instance.MusicSource.Stop();
			GameManager.Instance.MusicSource.loop = false;
			GameManager.Instance.MusicSource.clip = GameManager.Instance.WinTrack;
			GameManager.Instance.MusicSource.Play();
		}

		PlayerHUDManager.Instance.EndRound();
		Director.extrapolationMode = DirectorWrapMode.Hold;
		Director.Play();
		started = false;

	}

	public void NextRound() //called by timeline started in Finish Round
	{
		if (!FinalRound)
			GameManager.Instance.NextLevel();
		else
			GameManager.Instance.GameWin();
	}

	public void LeaveRound()
	{
		GameManager.Instance.ResetLevel();
	}

	public void StartCountdown()
	{
		UIManager.Instance.RoundStartCountdown();
	}

	public void PlaceEntities()
	{ 
		GameObject player = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.PlayerLancer : GameManager.Instance.PlayerSword, SpawnPoints[0].position, SpawnPoints[0].rotation);
		PlayerManager.Instance.PlayerControllerP1 = player.GetComponent<PlayerController>();
		PlayerManager.Instance.PlayerObjectP1 = player.GetComponent<SmartObject>();
		EntityManager.Instance.Entities.Add(PlayerManager.Instance.PlayerObjectP1);
		RoundEndCamera.Follow = PlayerManager.Instance.PlayerObjectP1.TargetPosiitions[0].transform;
		RoundEndCamera.LookAt = PlayerManager.Instance.PlayerObjectP1.TargetPosiitions[0].transform;

		if (!GameManager.Instance.Multiplayer)
		{
			if (MirrorMatch)
			{
				GameObject mirror = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.AILancer : GameManager.Instance.AISword, SpawnPoints[1].position, SpawnPoints[1].rotation);
				mirror.GetComponent<SmartObject>().Target = PlayerManager.Instance.PlayerObjectP1.TargetPosiitions[0].GetComponent<TargetableObject>();
				EntityManager.Instance.Entities.Add(mirror.GetComponent<SmartObject>());
				ActiveOpponents.Add(mirror.GetComponent<SmartObject>());


			}
			else if (ReverseMatch)
			{
				GameObject reverse = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.AISword : GameManager.Instance.AILancer, SpawnPoints[1].position, SpawnPoints[1].rotation);
				reverse.GetComponent<SmartObject>().Target = PlayerManager.Instance.PlayerObjectP1.TargetPosiitions[0].GetComponent<TargetableObject>();
				EntityManager.Instance.Entities.Add(reverse.GetComponent<SmartObject>());
				ActiveOpponents.Add(reverse.GetComponent<SmartObject>());
			}
			else
			{
				if (Opponents?.Length > 0)
					for (int i = 0; i < Opponents.Length; i++)
					{
						GameObject indexedOpponent = Instantiate(Opponents[i], SpawnPoints[i + 1].position, SpawnPoints[i + 1].rotation);
						indexedOpponent.GetComponent<SmartObject>().Target = PlayerManager.Instance.PlayerObjectP1.TargetPosiitions[0].GetComponent<TargetableObject>();
						EntityManager.Instance.Entities.Add(indexedOpponent.GetComponent<SmartObject>());
						ActiveOpponents.Add(indexedOpponent.GetComponent<SmartObject>());
					}
			}
		}
		else
		{
			GameObject player2 = Instantiate(GameManager.Instance.SelectedCharacter2 == PlayerCharacter.Lancer ? GameManager.Instance.PlayerLancerP2 : GameManager.Instance.PlayerSwordP2, SpawnPoints[1].position, SpawnPoints[1].rotation);
			PlayerManager.Instance.PlayerControllerP2 = player2.GetComponent<PlayerController>();
			PlayerManager.Instance.PlayerObjectP2 = player2.GetComponent<SmartObject>();
			EntityManager.Instance.Entities.Add(PlayerManager.Instance.PlayerObjectP2);
			RoundEndCamera2.Follow = PlayerManager.Instance.PlayerObjectP2.TargetPosiitions[0].transform;
			RoundEndCamera2.LookAt = PlayerManager.Instance.PlayerObjectP2.TargetPosiitions[0].transform;
		}
	}

	public void StartIntroduction()
	{
		//TimelineAsset timelineAsset = Director.playableAsset as TimelineAsset;
		Director.playableAsset = StartTimeline;
		Director.extrapolationMode = DirectorWrapMode.None;
		Director.SetGenericBinding(StartTimeline.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<CinemachineBrain>());
		Director.SetGenericBinding(StartTimeline.GetRootTrack(2), PlayerManager.Instance.PlayerObjectP1.Animator);
		if (!GameManager.Instance.Multiplayer)
		{
			Director.SetGenericBinding(StartTimeline.GetRootTrack(3), EntityManager.Instance.Entities[1].Animator);
			if (Opponents.Length > 1)
				for (int i = 0; i < Opponents.Length; i++)
					Director.SetGenericBinding(StartTimeline.GetRootTrack(i + 3), EntityManager.Instance.Entities[i + 1].Animator);
		}
		else
		{
			Director.SetGenericBinding(StartTimeline.GetRootTrack(3), PlayerManager.Instance.PlayerObjectP2.Animator);
		}
		Director.Play();
	}

	public void ToggleControls(bool enabled)
	{

		TargetingManager.Instance.RenderingRectImage.gameObject.SetActive(false);
		foreach (SmartObject smartObject in EntityManager.Instance.Entities)
		{
			smartObject.InputVector = Vector3.zero;
			smartObject.MovementVector = Vector3.zero;
			smartObject.StoredMovementVector = Vector3.zero;
			smartObject.Motor.BaseVelocity *= 0;
			smartObject.Controller.Button1Buffer = 0;
			smartObject.Controller.Button1Hold = false;
			smartObject.Controller.Button1ReleaseBuffer = 0;

			smartObject.Controller.Button2Buffer = 0;
			smartObject.Controller.Button2Hold = false;
			smartObject.Controller.Button2ReleaseBuffer = 0;

			smartObject.Controller.Button3Buffer = 0;
			smartObject.Controller.Button3Hold = false;
			smartObject.Controller.Button3ReleaseBuffer = 0;

			smartObject.Controller.Button4Buffer = 0;
			smartObject.Controller.Button4Hold = false;
			smartObject.Controller.Button4ReleaseBuffer = 0;

			smartObject.Controller.enabled = enabled;
		}


		PlayerManager.Instance.PlayerObjectP1.GetComponent<PlayerInput>().enabled = enabled;

		CameraManager.Instance.FreeLookCam.gameObject.SetActive(enabled);
		CameraManager.Instance.FreeLookCam.Priority = enabled ? 10 : 0;
		//if (enabled)
		//	CameraManager.Instance.ResetCamera();
		started = enabled;
		//GameManager.Instance.MusicSource.loop = false;
		
		if (enabled)
		{
			GameManager.Instance.MusicSource.Stop();
			GameManager.Instance.MusicSource.clip = GameManager.Instance.FightTrack;
			GameManager.Instance.MusicSource.loop = true;
			GameManager.Instance.MusicSource.Play();
		}


		if(enabled) 
		{
			PlayerHUDManager.Instance.StartRoundTimer(180);
			TargetingManager.Instance.RenderingRectImage.gameObject.SetActive(true);
		}

		if (GameManager.Instance.Multiplayer)
		{
			PlayerManager.Instance.PlayerObjectP2.GetComponent<PlayerInput>().enabled = enabled;
			CameraManagerP2.Instance.FreeLookCam.gameObject.SetActive(enabled);
			CameraManagerP2.Instance.FreeLookCam.Priority = enabled ? 10 : 0;
		}
	}
}
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
	public TimelineAsset FinishTimeline;
	public CinemachineVirtualCamera RoundEndCamera;
	public Transform[] SpawnPoints;
	public GameObject[] Opponents;
	public bool MirrorMatch;
	public bool ReverseMatch;
	public List<SmartObject> ActiveOpponents;
	public bool FinalRound;
	bool started;

	private void Start()
	{
		//StartIntroduction();
		//GameEventManager.Instance.playRoundStartTimeline += StartIntroduction;\
		//StartRound();
		GameManager.Instance.CurrentRoundStartController = this;

	}

	private void OnDestroy()
	{
		//GameEventManager.Instance.playRoundStartTimeline -= StartIntroduction;
	}

	private void Update()
	{
		if (!started)
			return;

			for (int i = ActiveOpponents.Count -1; i >= 0 ; i--)
				if (ActiveOpponents[i].Stats.HP <= 0)
					ActiveOpponents.RemoveAt(i);

		if (ActiveOpponents.Count == 0)
			FinishRound(true);

		if (PlayerManager.Instance.PlayerObject.Stats.HP <= 0)
			FinishRound(false);
	}

	public void StartRound()
	{
		PlaceEntities();
		StartIntroduction();
	}

	public void FinishRound(bool win)
	{
		Director.playableAsset = FinishTimeline;
		Director.extrapolationMode = DirectorWrapMode.Hold;
		Director.SetGenericBinding(FinishTimeline.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<CinemachineBrain>());
		Director.SetGenericBinding(FinishTimeline.GetRootTrack(2), PlayerManager.Instance.PlayerObject.Animator);
		Director.Play();
		if (win)
		{
			GameManager.Instance.MusicSource.Stop();
			GameManager.Instance.MusicSource.clip = GameManager.Instance.WinTrack;
			GameManager.Instance.MusicSource.Play();
			//GameManager.Instance.GameWin();
			Debug.Log("gamewin");
		}
		else
		{
			GameManager.Instance.GameOver();
			Debug.Log("gameover");
		}
		started = false;
	}

	public void NextRound() //called by timeline started in Finish Round
	{
		if (!FinalRound)
			GameManager.Instance.NextLevel();
		else
			GameManager.Instance.GameWin();
	}

	public void PlaceEntities()
	{

		GameObject player = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.PlayerLancer : GameManager.Instance.PlayerSword, SpawnPoints[0].position, SpawnPoints[0].rotation);
		PlayerManager.Instance.PlayerController = player.GetComponent<PlayerController>();
		PlayerManager.Instance.PlayerObject = player.GetComponent<SmartObject>();
		EntityManager.Instance.Entities.Add(PlayerManager.Instance.PlayerObject);
		RoundEndCamera.Follow = PlayerManager.Instance.PlayerObject.TargetPosiitions[0].transform;
		RoundEndCamera.LookAt = PlayerManager.Instance.PlayerObject.TargetPosiitions[0].transform;

		if (MirrorMatch)
		{
			GameObject mirror = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.AILancer : GameManager.Instance.AISword, SpawnPoints[1].position, SpawnPoints[1].rotation);
			mirror.GetComponent<SmartObject>().Target = PlayerManager.Instance.PlayerObject.TargetPosiitions[0].GetComponent<TargetableObject>();
			EntityManager.Instance.Entities.Add(mirror.GetComponent<SmartObject>());
			ActiveOpponents.Add(mirror.GetComponent<SmartObject>());


		}
		else if (ReverseMatch)
		{
			GameObject reverse = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.AISword : GameManager.Instance.AILancer, SpawnPoints[1].position, SpawnPoints[1].rotation);
			reverse.GetComponent<SmartObject>().Target = PlayerManager.Instance.PlayerObject.TargetPosiitions[0].GetComponent<TargetableObject>();
			EntityManager.Instance.Entities.Add(reverse.GetComponent<SmartObject>());
			ActiveOpponents.Add(reverse.GetComponent<SmartObject>());
		}
		else
		{
			if (Opponents?.Length > 0)
				for (int i = 0; i < Opponents.Length; i++)
				{
					GameObject indexedOpponent = Instantiate(Opponents[i], SpawnPoints[i+1].position, SpawnPoints[i+1].rotation);
					indexedOpponent.GetComponent<SmartObject>().Target = PlayerManager.Instance.PlayerObject.TargetPosiitions[0].GetComponent<TargetableObject>();
					EntityManager.Instance.Entities.Add(indexedOpponent.GetComponent<SmartObject>());
					ActiveOpponents.Add(indexedOpponent.GetComponent<SmartObject>());
				}
		}
	}

	public void StartIntroduction()
	{
		//TimelineAsset timelineAsset = Director.playableAsset as TimelineAsset;
		Director.playableAsset = StartTimeline;
		Director.extrapolationMode = DirectorWrapMode.None;
		Director.SetGenericBinding(StartTimeline.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<CinemachineBrain>());
		Director.SetGenericBinding(StartTimeline.GetRootTrack(2), PlayerManager.Instance.PlayerObject.Animator);
		Director.SetGenericBinding(StartTimeline.GetRootTrack(3), EntityManager.Instance.Entities[1].Animator);
		if(Opponents.Length > 1)
			for(int i = 0; i < Opponents.Length; i++)
			Director.SetGenericBinding(StartTimeline.GetRootTrack(i+3), EntityManager.Instance.Entities[i+1].Animator);
		Director.Play();
	}

	public void ToggleControls(bool enabled)
	{
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


		PlayerManager.Instance.PlayerObject.GetComponent<PlayerInput>().enabled = enabled;


		CameraManager.Instance.FreeLookCam.Priority = enabled ? 10 : 0;
		//if (enabled)
		//	CameraManager.Instance.ResetCamera();
		started = enabled;

		GameManager.Instance.MusicSource.Stop();
		GameManager.Instance.MusicSource.clip = GameManager.Instance.FightTrack;
		GameManager.Instance.MusicSource.Play();

		if(enabled) {
			PlayerHUDManager.Instance.StartRoundTimer(120);
        }
	}
}
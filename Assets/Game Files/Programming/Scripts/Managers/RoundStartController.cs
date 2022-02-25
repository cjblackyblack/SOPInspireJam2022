using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class RoundStartController : MonoBehaviour
{
	public PlayableDirector Director => GetComponent<PlayableDirector>();
	public Transform[] SpawnPoints;
	public GameObject[] Opponents;
	public bool MirrorMatch;
	public bool ReverseMatch;

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

	public void StartRound()
	{
		PlaceEntities();
		StartIntroduction();
	}

	public void PlaceEntities()
	{

		GameObject player = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.PlayerLancer : GameManager.Instance.PlayerSword, SpawnPoints[0].position, SpawnPoints[0].rotation);
		PlayerManager.Instance.PlayerController = player.GetComponent<PlayerController>();
		PlayerManager.Instance.PlayerObject = player.GetComponent<SmartObject>();
		EntityManager.Instance.Entities.Add(PlayerManager.Instance.PlayerObject);
		if (MirrorMatch)
		{
			GameObject mirror = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.AILancer : GameManager.Instance.AISword, SpawnPoints[1].position, SpawnPoints[1].rotation);
			EntityManager.Instance.Entities.Add(mirror.GetComponent<SmartObject>());
		}
		else if (ReverseMatch)
		{
			GameObject reverse = Instantiate(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer ? GameManager.Instance.AISword : GameManager.Instance.AILancer, SpawnPoints[1].position, SpawnPoints[1].rotation);
			EntityManager.Instance.Entities.Add(reverse.GetComponent<SmartObject>());
		}
		else
		{
			if (Opponents?.Length > 0)
				for (int i = 1; i < Opponents.Length; i++)
				{
					GameObject indexedOpponent = Instantiate(Opponents[i], SpawnPoints[i].position, SpawnPoints[i].rotation);
					EntityManager.Instance.Entities.Add(indexedOpponent.GetComponent<SmartObject>());
				}
		}
	}

	public void StartIntroduction()
	{
		TimelineAsset timelineAsset = Director.playableAsset as TimelineAsset;
		Director.SetGenericBinding(timelineAsset.GetRootTrack(1), CameraManager.Instance.MainCamera.GetComponent<Cinemachine.CinemachineBrain>());
		Director.SetGenericBinding(timelineAsset.GetRootTrack(2), PlayerManager.Instance.PlayerObject.Animator);
		Director.Play();
	}

	public void ToggleControls(bool enabled)
	{
		foreach (SmartObject smartObject in EntityManager.Instance.Entities)
			smartObject.Controller.enabled = enabled;

		PlayerManager.Instance.PlayerObject.GetComponent<PlayerInput>().enabled = enabled;

		CameraManager.Instance.FreeLookCam.Priority = enabled ? 10 : 0;
	}
}
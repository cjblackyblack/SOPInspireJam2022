using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class RoundStartController : MonoBehaviour
{
	public PlayableDirector Director => GetComponent<PlayableDirector>();

	private void Start()
	{
		//GameEventManager.Instance.playRoundStartTimeline += StartIntroduction;
		StartIntroduction();
	}

	private void OnDestroy()
	{
		//GameEventManager.Instance.playRoundStartTimeline -= StartIntroduction;
	}

	public void StartIntroduction()
	{
		TimelineAsset timelineAsset = Director.playableAsset as TimelineAsset;
		Director.SetGenericBinding(timelineAsset.GetRootTrack(2), PlayerManager.Instance.PlayerObject.Animator);
		Director.Play();
	}

	public void ToggleControls(bool enabled)
	{
		//loop through entities manager, disable all Controllers
		//disable PlayerControls inputactions specifically as well
	}
}
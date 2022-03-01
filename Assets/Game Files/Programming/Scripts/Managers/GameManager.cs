using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
	public PlayerCharacter SelectedCharacter;

	public GameObject PlayerLancer;
	public GameObject PlayerSword;
	public GameObject AILancer;
	public GameObject AISword;
	public GameObject AIBoss;

	public int BattleScene = 1;
	public RoundController CurrentRoundStartController;
	public AudioSource MusicSource;
	public AudioSource SFXSource;
	public AudioClip MainTrack;
	public AudioClip FightTrack;
	public AudioClip WinTrack;
	public AudioClip LoseTrack;

	public override void Start()
	{
		base.Start();
		Application.targetFrameRate = 60;



		StartCoroutine(WaitStart());
		IEnumerator WaitStart()
		{
			yield return new WaitForEndOfFrame();
			if (SceneManager.GetActiveScene().buildIndex == 0)
				UIManager.Instance.ChangeGameState(GameState.Start);
			else
				UIManager.Instance.ChangeGameState(GameState.Gameplay);
		}
	}

	public void SetCharacter(int index)
	{
		Mathf.Clamp(index, 0, 1);
		SelectedCharacter = (PlayerCharacter)index;
	}

	public void Pause()
	{
		Time.timeScale = 0;
	}

	public void UnPause()
	{
		Time.timeScale = 1;
	}

	public void StartGame()
	{
		//SHOW UI LOADING SCREEN
		LoadBattleSceneAsync(BattleScene);
		//HIDE UI LOADING SCREEN
	}

	public void NextLevel()
	{
		UIManager.Instance.ChangeGameState(GameState.Loading);
		UnLoadSceneAsync(BattleScene);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		BattleScene++;
		LoadBattleSceneAsync(BattleScene);
		//UIManager.Instance.ChangeGameState(GameState.Gameplay);

	}

	public void ResetLevel()
	{
		UIManager.Instance.ChangeGameState(GameState.Loading);
		UnLoadSceneAsync(BattleScene);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		BattleScene++;
		UIManager.Instance.ChangeGameState(GameState.Start);
	}

	public void GameWin()
	{

		UIManager.Instance.ChangeGameState(GameState.Credits);
	}

	public void GameOver()
	{
		MusicSource.Stop();
		MusicSource.clip = LoseTrack;
		MusicSource.Play();
		UIManager.Instance.ChangeGameState(GameState.GameOver);
	}

	public void LoadCreditsScene()
	{
		UIManager.Instance.ChangeGameState(GameState.Loading);
		UnLoadSceneAsync(BattleScene);
		SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
		BattleScene++;
		StartCoroutine(LoadSceneAsyncCoroutine(BattleScene));

		IEnumerator LoadSceneAsyncCoroutine(int index)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
		}
	}

	void LoadBattleSceneAsync(int index)
	{
		StartCoroutine(LoadSceneAsyncCoroutine(index));

		IEnumerator LoadSceneAsyncCoroutine(int index)
		{
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
			SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(index));
			CurrentRoundStartController = FindObjectOfType<RoundController>();
			UIManager.Instance.ChangeGameState(GameState.Gameplay);

		}
	}

	void UnLoadSceneAsync(int index)
	{
		StartCoroutine(UnLoadSceneAsyncCoroutine(index));

		IEnumerator UnLoadSceneAsyncCoroutine(int index)
		{
			AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(index);

			while (!asyncLoad.isDone)
			{
				yield return null;
			}
		}
	}

	public void GlobalHitStop(float length)
	{
		StartCoroutine(GlobalHitStopCoroutine(length));
		IEnumerator GlobalHitStopCoroutine(float length)
		{
			Time.timeScale = 0;
			yield return new WaitForSecondsRealtime(length);
			Time.timeScale = 1;
		}
	}

}
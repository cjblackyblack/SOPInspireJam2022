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
	public RoundStartController CurrentRoundStartController;

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
		UIManager.Instance.ChangeGameState(GameState.Gameplay);

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
			SceneManager.SetActiveScene(SceneManager.GetSceneAt(index));
			CurrentRoundStartController = FindObjectOfType<RoundStartController>();
			CurrentRoundStartController.StartRound();
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
}
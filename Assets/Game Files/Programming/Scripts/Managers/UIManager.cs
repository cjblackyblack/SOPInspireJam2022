using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UIManager : Singleton<UIManager>
{
	public GameState CurrentGameState;
	public GameObject MainMenuUI;
	public GameObject CharacterSelectUI;
	public GameObject LoadingUI;
	public GameObject PlayerUI;
	public GameObject AdditionalUI;
	public GameObject PausedUI;
	public GameObject StartCountdownUI;
	public GameObject GameOverUI;
	public GameObject CreditsUI;
	public GameObject ControlsUI;

	public TextMeshProUGUI StartCountdownText;
	public TextMeshProUGUI GameOverCountdownText;

	public override void Start()
	{
		base.Start();
	}

	public void ToggleMainMenuUI(bool enabled)
	{
		MainMenuUI.gameObject.SetActive(enabled);
	}

	public void ToggleCharacterSelectUI(bool enabled)
	{
		CharacterSelectUI.gameObject.SetActive(enabled);
	}

	public void ToggleLoadingUI(bool enabled)
	{
		LoadingUI.gameObject.SetActive(enabled);
	}

	public void TogglePlayerUI(bool enabled)
	{
		PlayerUI.gameObject.SetActive(enabled);
	}

	public void ToggleAdditionalUI(bool enabled)
	{
		AdditionalUI.gameObject.SetActive(enabled);
	}

	public void TogglePauseUI(bool enabled)
	{
		PausedUI.gameObject.SetActive(enabled);
	}

	public void ToggleStartCountdownUI(bool enabled)
	{
		StartCountdownUI.SetActive(enabled);
	}

	public void ToggleGameOverUI(bool enabled)
	{
		GameOverUI.SetActive(enabled);
	}

	public void ToggleCreditsUI(bool enabled)
	{
		CreditsUI.SetActive(enabled);
	}

	public void ToggleControlsUI(bool enabled)
	{
		ControlsUI.SetActive(enabled);
	}


	public void ChangeGameState(GameState newGameState)
	{
		ToggleMainMenuUI(false);
		ToggleCharacterSelectUI(false);
		ToggleLoadingUI(false);
		TogglePlayerUI(false);
		ToggleAdditionalUI(false);
		TogglePauseUI(false);
		ToggleStartCountdownUI(false);
		ToggleGameOverUI(false);
		ToggleCreditsUI(false);
		ToggleControlsUI(false);

		CurrentGameState = newGameState;
		switch (CurrentGameState)
		{
			case GameState.Start:
				{
					ToggleMainMenuUI(true);
					break;
				}
			case GameState.CharacterSelect:
				{
					ToggleCharacterSelectUI(true);
					break;
				}
			case GameState.Loading:
				{
					ToggleLoadingUI(true);
					break;
				}
			case GameState.Gameplay:
				{
					TogglePlayerUI(true);
					ToggleAdditionalUI(true);
					if(FindObjectOfType<RoundController>())
					FindObjectOfType<RoundController>().StartRound(); //yes i see the spaghetti here no i will not be fixing it
					CameraManager.Instance.SetTarget(PlayerManager.Instance.PlayerObject.TargetPosiitions[0].transform);

					break;
				}
			case GameState.Paused:
				{
					TogglePauseUI(true);
					break;
				}
			case GameState.GameOver:
				{
					ToggleGameOverUI(true);
					GameOverCountdown();
					//TOGGLE 10S Countdown to restart level
					break;
				}
			case GameState.Credits:
				{
					GameManager.Instance.LoadCreditsScene();
					break;
				}
			case GameState.Controls:
				{

					break;
				}
		}
	}

	public void OnStartButton()
	{
		GameManager.Instance.StartGame();
	}

	public void OnMainStart()
	{
		ChangeGameState(GameState.CharacterSelect);
	}

	public void OnCharacterSelectButton()
	{
		int index = (int)GameManager.Instance.SelectedCharacter;
		index++;
		if (index > 1)
			index = 0;
		GameManager.Instance.SelectedCharacter = (PlayerCharacter)index;
	}

	public void OnOptionsButton()
	{

	}

	public void OnQuitButton()
	{
		Application.Quit();
	}

	public void OnCharacterSelectBack()
	{
		ChangeGameState(GameState.Start);
	}

	public void RoundStartCountdown()
	{
		StartCoroutine(Countdown());
		IEnumerator Countdown()
		{
			StartCountdownText.gameObject.SetActive(true);
			StartCountdownText.text = "Ready";
			yield return new WaitForSecondsRealtime(1);
			StartCountdownText.text = "3";
			yield return new WaitForSecondsRealtime(1);
			StartCountdownText.text = "2";
			yield return new WaitForSecondsRealtime(1);
			StartCountdownText.text = "1";
			yield return new WaitForSecondsRealtime(1);
			StartCountdownText.text = "GO";
			yield return new WaitForSecondsRealtime(1);
			StartCountdownText.gameObject.SetActive(false);
		}
	}

	public void GameOverCountdown()
	{
		StartCoroutine(Countdown());
		IEnumerator Countdown()
		{
			GameOverCountdownText.gameObject.SetActive(true);
			GameOverCountdownText.text = "10";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "9";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "8";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "7";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "6";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "5";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "4";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "3";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "2";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "1";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.text = "0";
			yield return new WaitForSecondsRealtime(1);
			GameOverCountdownText.gameObject.SetActive(false);
		}
	}
}
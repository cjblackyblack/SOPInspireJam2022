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
	public GameObject StaticUI;
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
		//ChangeGameState(GameState.Start);
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
		FlashStatic();

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
					if (FindObjectOfType<RoundController>())
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
			ToggleStartCountdownUI(true);
			StartCountdownText.gameObject.SetActive(true);
			StartCountdownText.text = "READY";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Pogs[0]);
			yield return new WaitForSecondsRealtime(1.5f);
			StartCountdownText.text = "3";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[3]);
			yield return new WaitForSecondsRealtime(1f);
			StartCountdownText.text = "2";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[2]);
			yield return new WaitForSecondsRealtime(1f);
			StartCountdownText.text = "1";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[1]);
			yield return new WaitForSecondsRealtime(1f);
			StartCountdownText.text = "GO";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[0]);
			yield return new WaitForSecondsRealtime(1f);
			StartCountdownText.gameObject.SetActive(false);
			StartCountdownUI.SetActive(false);
			ToggleStartCountdownUI(false);
		}
	}

	public void GameOverCountdown()
	{
		StartCoroutine(Countdown());
		IEnumerator Countdown()
		{
			GameOverCountdownText.gameObject.SetActive(true);
			GameOverCountdownText.text = "10";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[10]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "9";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[9]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "8";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[8]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "7";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[7]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "6";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[6]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "5";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[5]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "4";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[4]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "3";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[3]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "2";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[2]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "1";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[1]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.text = "0";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.CountdownClips[0]);
			yield return new WaitForSecondsRealtime(1f);
			GameOverCountdownText.gameObject.SetActive(false);
		}
	}

	public void FlashStatic()
	{
		StartCoroutine(FlashStaticCoroutine());
		IEnumerator FlashStaticCoroutine()
		{
			StaticUI.gameObject.SetActive(true);
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Static);
			yield return new WaitForSecondsRealtime(0.15f);
			GameManager.Instance.SFXSource.Stop();
			StaticUI.gameObject.SetActive(false);
		}
	}
}
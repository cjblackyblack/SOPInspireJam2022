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

	public GameObject Title1;
	public GameObject Title2;
	public GameObject Title3;
	public GameObject Title4;

	public TextMeshProUGUI StartCountdownText;
	public TextMeshProUGUI GameOverCountdownText;

	public GameObject Region;
	public GameObject Opponent;
	public GameObject CSStatic;
	public GameObject CSSSword;
	public GameObject CSSLancer;

	public GameObject[] CreditsLines;

	public override void Start()
	{
		base.Start();
		//ChangeGameState(GameState.Start);
	}

	public void ToggleMainMenuUI(bool enabled)
	{
		MainMenuUI.gameObject.SetActive(enabled);
			if(enabled)ShowTitle();
	}

	public void ToggleCharacterSelectUI(bool enabled)
	{
		CharacterSelectUI.gameObject.SetActive(enabled);
	}

	public void ToggleLoadingUI(bool enabled)
	{
		LoadingUI.gameObject.SetActive(enabled);
		for (int i = 1; i < 5; i++)
		{
			Opponent.transform.GetChild(i - 1).gameObject.SetActive(false);
			Region.transform.GetChild(i - 1).gameObject.SetActive(false);
		}
		if(GameManager.Instance.BattleScene < 5)
			Region.transform.GetChild(GameManager.Instance.BattleScene - 1).gameObject.SetActive(true);

		switch (GameManager.Instance.BattleScene)
		{
			case 1:
				{
					if(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer)
					{
						Opponent.transform.GetChild(0).gameObject.SetActive(true);
					}
					else
					{
						Opponent.transform.GetChild(1).gameObject.SetActive(true);
					}
					break;
				}
			case 2:
				{
					if (GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer)
					{
						Opponent.transform.GetChild(1).gameObject.SetActive(true);
					}
					else
					{
						Opponent.transform.GetChild(0).gameObject.SetActive(true);
					}
					break;
				}
			case 3:
				{
					Opponent.transform.GetChild(2).gameObject.SetActive(true);
					break;
				}
			case 4:
				{
					Opponent.transform.GetChild(3).gameObject.SetActive(true);
					break;
				}
		}
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
		if (enabled)
			ExitCredits();
	}

	public void ToggleControlsUI(bool enabled)
	{
		ControlsUI.SetActive(enabled);
	}


	public void ChangeGameState(GameState newGameState)
	{
		StopAllCoroutines();
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
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
					ToggleMainMenuUI(true);
					break;
				}
			case GameState.CharacterSelect:
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
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
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					TogglePlayerUI(true);
					ToggleAdditionalUI(true);
					if (FindObjectOfType<RoundController>())
						FindObjectOfType<RoundController>().StartRound(); //yes i see the spaghetti here no i will not be fixing it
					CameraManager.Instance.SetTarget(PlayerManager.Instance.PlayerObject.TargetPosiitions[0].transform);

					break;
				}
			case GameState.Paused:
				{
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;
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
					ToggleControlsUI(true);
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
		FlashSmallStatic();
		int index = (int)GameManager.Instance.SelectedCharacter;
		index++;
		if (index > 1)
			index = 0;
		GameManager.Instance.SelectedCharacter = (PlayerCharacter)index;
		CSSSword.gameObject.SetActive(GameManager.Instance.SelectedCharacter == PlayerCharacter.Sword);
		CSSLancer.gameObject.SetActive(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer);
	}

	public void OnOptionsButton()
	{
		ChangeGameState(GameState.Controls);
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

			yield return new WaitForSecondsRealtime(1f);
			ToggleStartCountdownUI(true);
			StartCountdownText.gameObject.SetActive(true);
			StartCountdownText.text = "READY";
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Pogs[0]);
			yield return new WaitForSecondsRealtime(2f);
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

	public void FlashSmallStatic()
	{
		StartCoroutine(FlashStaticCoroutine());
		IEnumerator FlashStaticCoroutine()
		{
			CSStatic.gameObject.SetActive(true);
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Static);
			yield return new WaitForSecondsRealtime(0.1f);
			GameManager.Instance.SFXSource.Stop();
			CSStatic.gameObject.SetActive(false);
		}
	}

	public void ShowTitle()
	{
		StartCoroutine(ShowTitleCoroutine());

		IEnumerator ShowTitleCoroutine()
		{
			Title1.gameObject.SetActive(false);
			Title2.gameObject.SetActive(false);
			Title3.gameObject.SetActive(false);
			Title4.gameObject.SetActive(false);
			yield return new WaitForSecondsRealtime(0.25f);
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Bangs[2]);
			Title1.gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(0.25f);
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Bangs[3]);
			Title2.gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(0.5f);
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Bangs[1]);
			Title3.gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(0.25f);
			GameManager.Instance.SFXSource.PlayOneShot(GameManager.Instance.Bangs[0]);
			Title4.gameObject.SetActive(true);
		}
	}



	public void ExitCredits()
	{
		StartCoroutine(ShowTitleCoroutine());

		IEnumerator ShowTitleCoroutine()
		{
			Debug.Log("I see you seeing in that you see me seeing you");
			yield return new WaitForSecondsRealtime(6);
			CreditsLines[0].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			CreditsLines[1].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			CreditsLines[2].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			CreditsLines[3].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			CreditsLines[4].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			CreditsLines[5].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			CreditsLines[6].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);


			CreditsLines[7].gameObject.SetActive(true);
			yield return new WaitForSecondsRealtime(2);

			yield return new WaitForSecondsRealtime(90);
			ChangeGameState(GameState.Start);
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
	public GameState CurrentGameState;
	public GameObject MainMenuUI;
	public GameObject CharacterSelectUI;
	public GameObject LoadingUI;
	public GameObject PlayerUI;
	public GameObject AdditionalUI;
	public GameObject PausedUI;

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

	public void ChangeGameState(GameState newGameState)
	{
		ToggleMainMenuUI(false);
		ToggleCharacterSelectUI(false);
		ToggleLoadingUI(false);
		TogglePlayerUI(false);
		ToggleAdditionalUI(false);
		TogglePauseUI(false);

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
					FindObjectOfType<RoundStartController>().StartRound(); //yes i see the spaghetti here no i will not be fixing it
					CameraManager.Instance.SetTarget(PlayerManager.Instance.PlayerObject.TargetPosiitions[0].transform);

					break;
				}
			case GameState.Paused:
				{
					TogglePauseUI(true);
					break;
				}
		}
	}

	public void OnStartButton()
	{
		GameManager.Instance.StartGame();
	}

	public void OnOptionsButton()
	{

	}

	public void OnQuitButton()
	{

	}

	public void OnCharacterSelectButton()
	{

	}

	public void OnCharacterSelectBack()
	{

	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSwap : MonoBehaviour
{
	public GameObject Lancer;
	public GameObject Sword;

	private void Start()
	{
		Lancer.gameObject.SetActive(GameManager.Instance.SelectedCharacter == PlayerCharacter.Lancer);
		Sword.gameObject.SetActive(GameManager.Instance.SelectedCharacter == PlayerCharacter.Sword);
	}
}

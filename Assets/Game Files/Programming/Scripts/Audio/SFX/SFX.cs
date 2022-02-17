using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SFX")]
public class SFX : ScriptableObject
{
	public AudioClip[] PossibleClips;
	public Vector2 PitchRange;
}

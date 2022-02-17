using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : CombatBox
{
	//CAN EASILY PUT VARIABLES AND HURTBOX BEHAVIOURS HERE
	private void Start()
	{
		SourceObject.Hurtboxes[CombatBoxID] = GetComponent<Collider>();
	}

	public override PhysicalObjectTangibility ProcessHitAction(ref DamageInstance damageInstance)//GOT HIT AND NEED TO DECIDE TO TELL PARENT OBJ OR HAVE BEHAVIOUR BECAUSE THIS SUPPORTS THAT TOO
	{

		SourceObject.TakeDamage(ref damageInstance);//for now just always sending upward
		return CurrentBoxTangibility;
	}
}
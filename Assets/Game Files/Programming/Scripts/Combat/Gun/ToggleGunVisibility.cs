using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGunVisibility : MonoBehaviour
{
    LocomotionStateMachine ParentMachine => GetComponentInParent<LocomotionStateMachine>();
    MeshRenderer mesh => GetComponent<MeshRenderer>();
    public bool reverse;
    // Update is called once per frame
    void Update()
    {

        if (ParentMachine.CurrentLocomotionEnum == LocomotionStates.AerialShoot || ParentMachine.CurrentLocomotionEnum == LocomotionStates.GroundedShoot)
        {
            if (reverse)
            {

                mesh.gameObject.SetActive(false);

            }
            else
            {

                mesh.gameObject.SetActive(true);
            }
        }
    }
}
﻿//-----------------------------------------------------------------------------
// MagnetController.cs
//
// Controller class for the magnet
//
//
// Authors: Michael Stefan Holly
//-----------------------------------------------------------------------------
//

using System.Collections;
using UnityEngine;
using VRTK;

/// <summary>
/// Controller class for the magnet
/// </summary>
public class MagnetController : VRTK_InteractableObject
{
    private bool IsMoving = false;

    private GameObject UsingObject;

    private SimulationController simController;

    protected override void Start()
    {
        base.Start();

        GameObject simControllerObject = GameObject.Find("SimulationController");
        if (simControllerObject)
            simController = simControllerObject.GetComponent<SimulationController>();

    }

    public override void StartUsing(GameObject usingObject)
    {
        base.StartUsing(usingObject);

        IsMoving = true;
        UsingObject = usingObject;


        simController.SimulationRunning = true;

        StartCoroutine(Move());
    }

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);

        IsMoving = false;

        simController.SimulationRunning = false;

    }

    private IEnumerator Move()
    {
        while(IsMoving)
        {
            this.transform.position = new Vector3(UsingObject.transform.position.x, this.transform.position.y, this.transform.position.z);

            UsingObject.GetComponent<VRTK_ControllerActions>().TriggerHapticPulse((ushort)(GetComponent<Magnet>().getExternalForce().magnitude * 100));

            yield return new WaitForFixedUpdate();
        }
    }
}

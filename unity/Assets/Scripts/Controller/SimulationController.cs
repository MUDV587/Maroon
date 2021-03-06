﻿//-----------------------------------------------------------------------------
// SimulationController.cs
//
// Class to handle the simulation flow
//
//
// Authors: Michael Stefan Holly
//          Michael Schiller
//          Christopher Schinnerl
//-----------------------------------------------------------------------------
//

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Class to handle the simulation flow
/// </summary>
public class SimulationController : MonoBehaviour
{
    /// <summary>
    /// Indicates whether the simulation is running
    /// </summary>
    [SerializeField]
    private bool simulationRunning;

    [SerializeField]
    private float timeScale = 1;

    public UnityEvent onStartRunning;
    public UnityEvent onStopRunning;
    
    private bool _inWholeResetMode = false;

    public float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = value;
            Time.timeScale = value;
        }
    }

    /// <summary>
    /// Indicates whether a single step should be simulated
    /// </summary>
    private bool stepSimulation;

    /// <summary>
    /// Indicates whether the simulation should be reset
    /// </summary>
    private bool simulationReset;

    /// <summary>
    /// The objects which must be reset
    /// </summary>
    private List<IResetObject> resetObjects;

    public event EventHandler<EventArgs> OnStart;

    public event EventHandler<EventArgs> OnStop;

    public event EventHandler<EventArgs> OnReset;

    private static SimulationController _instance;

    public static SimulationController Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SimulationController>();

            if(_instance == null)
                throw  new NullReferenceException("There is no simulation controller.");

            return _instance;
        }
    }

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        resetObjects = new List<IResetObject>();
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        InitSimController();
    }

    /// <summary>
    /// Initialization function for the Sim Controller
    /// </summary>
    private void InitSimController()
    {
        stepSimulation = false;
        simulationReset = true;
        Time.timeScale = timeScale;

        resetObjects.Clear();
        foreach (var obj in GameObject.FindGameObjectsWithTag("ResetObject"))
        {
            var resetObj = obj.GetComponentInParent<IResetObject>();
            if (resetObj != null)
                resetObjects.Add(resetObj);

        }
    }

    /// <summary>
    /// Delegate to get notification when the scene has unloaded.
    /// Reset timeScale to 1.0f.
    /// </summary>
    /// <param name="scene">Unloaded scene</param>
    private void OnSceneUnloaded(Scene scene)
    {
        TimeScale = 1.0f;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    /// <summary>
    /// Adds a reset object
    /// </summary>
    /// <param name="resetObj">the object to be reset</param>
    public void AddNewResetObject(IResetObject resetObj)
    {
        if (resetObj != null)
            resetObjects.Add(resetObj);
    }

    /// <summary>
    /// Adds a reset object at the begin so that it is called first when reseting the objects
    /// </summary>
    /// <param name="resetObj">the object to be reset</param>
    public void AddNewResetObjectAtBegin(IResetObject resetObj)
    {
        if(resetObj != null)
            resetObjects.Insert(0, resetObj);
    }

    
    /// <summary>
    /// Removes a reset object
    /// </summary>
    /// <param name="resetObj">the reset object to remove</param>
    public void RemoveResetObject(IResetObject resetObj)
    {
        resetObjects.Remove(resetObj);
    }

    /// <summary>
    /// Simulates one frame
    /// </summary>
    /// <returns>enumerator</returns>
    private IEnumerator SimulateOneFrame()
    {
        yield return 0;
        StopSimulation();
        stepSimulation = false;
    }

    /// <summary>
    /// Property that defines whether the simulation is running
    /// </summary>
    public bool SimulationRunning
    {
        get => simulationRunning;
        set
        {
            if (simulationRunning == value) return;

            _inWholeResetMode = false;
            
            simulationRunning = value;
            if(simulationRunning) onStartRunning.Invoke();
            else onStopRunning.Invoke();
        }
    }

    /// <summary>
    /// Getter for the stepSimulation field
    /// </summary>
    public bool StepSimulation => stepSimulation;

    /// <summary>
    /// Getter for the simulationReset field
    /// </summary>
    public bool SimulationJustReset => simulationReset;

    /// <summary>
    /// Starts the simulation
    /// </summary>
    public void StartSimulation()
    {
        SimulationRunning = true;
        simulationReset = false;
        _inWholeResetMode = false;

        if(!stepSimulation)
            OnStart?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Simulates a single frame
    /// </summary>
    public void SimulateStep()
    {
        stepSimulation = true;
        simulationReset = false;
        _inWholeResetMode = false;
        StartSimulation();
        StartCoroutine(SimulateOneFrame());
    }

    /// <summary>
    /// Stops the simulation
    /// </summary>
    public void StopSimulation()
    {
        SimulationRunning = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Resets the simulation
    /// </summary>
    public void ResetSimulation()
    {
        foreach (var resetObject in resetObjects)
        {
            resetObject.ResetObject();
        }

        StopSimulation();
        simulationReset = true;

        OnReset?.Invoke(this, EventArgs.Empty);

        _inWholeResetMode = true;
//        Debug.Log("Reset");
    }

    public void ResetWholeSimulation()
    {
        ResetSimulation();
        foreach (var resetObject in resetObjects.ToList())
        {
            if(resetObject is IResetWholeObject)
                (resetObject as IResetWholeObject).ResetWholeObject();
        }
    }

    public void ResetRespResetWholeSimulation()
    {
        if (!_inWholeResetMode)
            SimulationController.Instance.ResetSimulation();
        else
            SimulationController.Instance.ResetWholeSimulation();
    }

    public void TraceOutput(string output){
        Debug.Log(output);
    }
}

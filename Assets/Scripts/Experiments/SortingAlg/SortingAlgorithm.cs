using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SortingAlgorithm
{
    public SortingMachine machine;
    
    public virtual void ExecuteNextState() {}
    public virtual void ExecutePreviousState() {}
}

public class InsertionSort : SortingAlgorithm
{
    
    public override void ExecuteNextState()
    {
        machine.Insert(0, 1);
    }

    public override void ExecutePreviousState()
    {
    }

    private void handleState()
    {
        // switch (currentState)
        // {
        //     
        // }
    }
}

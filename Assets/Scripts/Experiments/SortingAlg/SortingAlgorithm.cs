using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SortingAlgorithm
{
    public SortingLogic sortingLogic;
    abstract public List<string> pseudocode { get; }
    
    protected enum SortingState
    {
        SS_None,
        SS_Line1,
        SS_Line2,
        SS_Line3,
        SS_Line4,
        SS_Line5,
        SS_Line6,
        SS_Line7,
        SS_Line8,
        SS_Line9,
        SS_Line10,
        SS_Line11,
        SS_Line12,
        SS_Line13,
        SS_Line14,
        SS_Line15
    }
    
    protected Stack<SortingState> _executedStates = new Stack<SortingState>();
    protected SortingState _nextState;
    
    protected int _operations;
    protected int _swaps;

    public SortingAlgorithm(SortingLogic logic)
    {
        sortingLogic = logic;
        
        _operations = 0;
        _swaps = 0;
        
        _executedStates.Push(SortingState.SS_None); //To check if at beginning
        _nextState = SortingState.SS_Line1;
    }

    public void ExecuteNextState()
    {
        sortingLogic.setPseudocode((int)_nextState);
        if (_nextState != SortingState.SS_None)
        {
            _operations++;
            _executedStates.Push(_nextState);
        }
        handleState(_nextState);
        sortingLogic.setSwapsOperations(_swaps, _operations);
    }

    public void ExecutePreviousState()
    {
        if (_executedStates.Peek() == SortingState.SS_None)
        {
            _nextState = SortingState.SS_Line1;
            sortingLogic.MoveFinished();
            _operations = 0;
            _swaps = 0;
            return;
        }
        _nextState = _executedStates.Pop();
        sortingLogic.setPseudocode((int)_executedStates.Peek());
        _operations--;
        handleReverseState(_nextState);
        sortingLogic.setSwapsOperations(_swaps, _operations);
    }

    protected virtual void handleState(SortingState currentState) {}
    protected virtual void handleReverseState(SortingState currentState) {}
}
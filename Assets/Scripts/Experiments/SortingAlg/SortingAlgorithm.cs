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
        SS_Line5
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
        handleReverseState(_nextState);
        sortingLogic.setSwapsOperations(_swaps, _operations);
    }

    protected virtual void handleState(SortingState currentState) {}
    protected virtual void handleReverseState(SortingState currentState) {}
}

public class InsertionSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Insertion Sort:</style>",
            "<style=\"command\">for</style> i = <style=\"number\">1</style> .. <style=\"command\">len</style>(A)-<style=\"number\">1</style>:",
            "    j = i-<style=\"number\">1</style>",
            "    <style=\"command\">while</style> A[j]>A[j+1] <style=\"command\">and</style> j>=0:",
            "        <style=\"function\">swap</style>(A[j+1],A[j])",
            "        j = j-<style=\"number\">1</style>"
        };
    }
    
    private int _n;
    private int _j;
    private int _i;
    
    private Stack<int> _oldJ = new Stack<int>();

    public InsertionSort(SortingLogic logic, int n) : base(logic)
    {
        _i = 0;
        _j = 0;
        _n = n;
    }

    protected override void handleState(SortingState currentState)
    {
        switch (currentState)
        {
            case SortingState.SS_Line1: // for i = 1 .. len(A)-1:
                _operations++;
                _i++;
                //Debug.Log("for i = " + _i);
                if (_i < _n)
                {
                    _nextState = SortingState.SS_Line2;
                }
                else
                {
                    _nextState = SortingState.SS_None;
                }
                _executedStates.Push(SortingState.SS_Line1);
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // j = i-1
                _operations++;
                _oldJ.Push(_j); //store j values for reverse steps
                _j = _i - 1;
                //Debug.Log("j = " + (_i - 1));
                _nextState = SortingState.SS_Line3;
                _executedStates.Push(SortingState.SS_Line2);
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // while A[j]>A[j+1] and j>=0:
                _operations++;
                //Debug.Log("Compare " + _j + " and " + (_j+1));
                if (_j < 0)
                {
                    _nextState = SortingState.SS_Line1;
                    sortingLogic.MoveFinished();
                    break;
                }
                if (sortingLogic.CompareGreater(_j, _j + 1))
                {
                    _nextState = SortingState.SS_Line4;
                }
                else
                {
                    _nextState = SortingState.SS_Line1;
                }
                _executedStates.Push(SortingState.SS_Line3);
                break;
            case SortingState.SS_Line4: // swap A[j+1] and A[j]
                _operations++;
                _swaps++;
                //Debug.Log("Swap " + _j + " and " + (_j+1));
                sortingLogic.Swap(_j+1, _j);
                _nextState = SortingState.SS_Line5;
                _executedStates.Push(SortingState.SS_Line4);
                break;
            case SortingState.SS_Line5: // j = j-1
                _operations++;
                _j--;
                //Debug.Log("Decreased j =  " + _j);
                _nextState = SortingState.SS_Line3;
                _executedStates.Push(SortingState.SS_Line5);
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_None:
                sortingLogic.sortingFinished();
                sortingLogic.MoveFinished();
                break;
        }
    }

    protected override void handleReverseState(SortingState currentState)
    {
        switch (currentState)
        {
            case SortingState.SS_Line1: // for i = 1 .. len(A)-1:
                _operations--;
                _i--;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // j = i-1
                _operations--;
                _j = _oldJ.Pop();
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // while A[j]>A[j+1] and j>=0:
                _operations--;
                sortingLogic.CompareGreater(_j, _j + 1);
                break;
            case SortingState.SS_Line4: // swap A[j+1] and A[j]
                _operations--;
                _swaps--;
                sortingLogic.Swap(_j+1, _j);
                break;
            case SortingState.SS_Line5: // j = j-1
                _operations--;
                _j++;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_None:
                sortingLogic.sortingFinished();
                sortingLogic.MoveFinished();
                break;
        }
    }
}

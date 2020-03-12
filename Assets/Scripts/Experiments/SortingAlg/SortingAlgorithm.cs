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
        Debug.Log(_nextState);
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
        Debug.Log(_nextState);
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
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // j = i-1
                _oldJ.Push(_j); //store j values for reverse steps
                _j = _i - 1;
                //Debug.Log("j = " + (_i - 1));
                _nextState = SortingState.SS_Line3;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // while A[j]>A[j+1] and j>=0:
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
                break;
            case SortingState.SS_Line4: // swap A[j+1] and A[j]
                _swaps++;
                //Debug.Log("Swap " + _j + " and " + (_j+1));
                sortingLogic.Swap(_j+1, _j);
                _nextState = SortingState.SS_Line5;
                break;
            case SortingState.SS_Line5: // j = j-1
                _j--;
                //Debug.Log("Decreased j =  " + _j);
                _nextState = SortingState.SS_Line3;
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
                _i--;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // j = i-1
                _j = _oldJ.Pop();
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // while A[j]>A[j+1] and j>=0:
                sortingLogic.CompareGreater(_j, _j + 1);
                break;
            case SortingState.SS_Line4: // swap A[j+1] and A[j]
                _swaps--;
                sortingLogic.Swap(_j+1, _j);
                break;
            case SortingState.SS_Line5: // j = j-1
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

public class MergeSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Merge Sort:</style>",
            "<style=\"function\">mergeSort</style>(i, j):",
            "    <style=\"command\">if</style> i<j-<style=\"number\">1</style>:",
            "        k = (i+j)/<style=\"number\">2</style>",
            "        <style=\"function\">mergeSort</style>(i,k)",
            "        <style=\"function\">mergeSort</style>(k,j)",
            "        <style=\"function\">merge</style>(i,k,j)",
            "",
            "<style=\"function\">merge</style>(i, k, j):",
            "    r = k",
            "    l = i",
            "    <style=\"command\">while</style> l<r <style=\"command\">and</style> r<j:",
            "        <style=\"command\">if</style> A[r]<A[l]:",
            "            <style=\"function\">insert</style>(r,l)",
            "            r = r+<style=\"number\">1</style>",
            "        l = l+<style=\"number\">1</style>"
        };
    }

    private int _i;
    private int _j;
    private int _k;
    private int _l;
    private int _r;
    private int _n;
    
    private Stack<int> _continueI = new Stack<int>();
    private Stack<int> _continueJ = new Stack<int>();
    private Stack<int> _continueK = new Stack<int>();
    private Stack<SortingState> _continueLine = new Stack<SortingState>();
    
    public MergeSort(SortingLogic logic, int n) : base(logic)
    {
        _i = 0;
        _j = n;
        _k = 0;
        _l = 0;
        _r = 0;
        _n = n;
    }

    private void enterSubroutineWithExitState(SortingState state)
    {
        _continueI.Push(_i);
        _continueJ.Push(_j);
        _continueK.Push(_k);
        _continueLine.Push(state);
    }

    private void leaveSubroutine()
    {
        if (_continueLine.Count == 0)
        {
            _nextState = SortingState.SS_None;
            return;
        }
        _i = _continueI.Pop();
        _j = _continueJ.Pop();
        _k = _continueK.Pop();
        _nextState = _continueLine.Pop();
    }
    
    protected override void handleState(SortingState currentState)
    {
        switch (currentState)
        {
            case SortingState.SS_Line1: // mergeSort(A, i, j):
                _nextState = SortingState.SS_Line2;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // if i<j-1:
                if (_i < _j - 1)
                {
                    _nextState = SortingState.SS_Line3;
                }
                else
                {
                    leaveSubroutine();
                }
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // k = (i + j) // 2
                _k = (_i + _j) / 2;
                _nextState = SortingState.SS_Line4;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line4: // mergeSort(A,i,k)
                enterSubroutineWithExitState(SortingState.SS_Line5);
                _j = _k;
                _nextState = SortingState.SS_Line1;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line5: // mergeSort(A,k,j)
                enterSubroutineWithExitState(SortingState.SS_Line6);
                _i = _k;
                _nextState = SortingState.SS_Line1;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line6: // merge(A,i,k,j)
                _nextState = SortingState.SS_Line8;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line7: // 
                break;
            case SortingState.SS_Line8: // merge(A, i, k, j):
                _nextState = SortingState.SS_Line9;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line9: // r = k
                _r = _k;
                _nextState = SortingState.SS_Line10;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line10: // l = i
                _l = _i;
                _nextState = SortingState.SS_Line11;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line11: // while l < r and r < j:
                if (_l < _r && _r < _j)
                {
                    _nextState = SortingState.SS_Line12;
                }
                else
                {
                    leaveSubroutine();
                }
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line12: // if A[r] < A[l]:
                if (sortingLogic.CompareGreater(_l, _r))
                {
                    _nextState = SortingState.SS_Line13;
                }
                else
                {
                    _nextState = SortingState.SS_Line15;
                }
                break;
            case SortingState.SS_Line13: // insert(A,r,l)
                _swaps++;
                sortingLogic.Insert(_r,_l);
                _nextState = SortingState.SS_Line14;
                break;
            case SortingState.SS_Line14: // r += 1
                _r++;
                _nextState = SortingState.SS_Line15;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line15: // l += 1
                _l++;
                _nextState = SortingState.SS_Line11;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_None:
                sortingLogic.sortingFinished();
                sortingLogic.MoveFinished();
                break;
        }
    }
}
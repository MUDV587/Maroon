using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSort : SortingAlgorithm
{
    public override List<string> pseudocode
    {
        get => new List<string>()
        {
            "<style=\"header\">Quick Sort:</style>",
            "<style=\"function\">quickSort</style>(l, r):",
            "    <style=\"command\">if</style> l<r-<style=\"number\">1</style>:",
            "        k = <style=\"function\">partition</style>(l,r)",
            "        <style=\"function\">quickSort</style>(l,k)",
            "        <style=\"function\">quickSort</style>(k+<style=\"number\">1</style>,r)",
            "",
            "<style=\"function\">partition</style>(l, r):",
            "    p = A[r-<style=\"number\">1</style>] <style=\"comment\">//pivot element</style>",
            "    k = l",
            "    <style=\"command\">for</style> j = l .. r-<style=\"number\">1</style>:",
            "        <style=\"command\">if</style> A[j]<=p:",
            "            <style=\"function\">swap</style>(j,k)",
            "            k = k+<style=\"number\">1</style>",
            "    <style=\"function\">swap</style>(k,r-<style=\"number\">1</style>)",
            "    <style=\"command\">return</style> k"
        };
    }

    private int _j;
    private int _k;
    private int _l;
    private int _r;
    private int _pInd;
    
    //Stacks for subroutine calling
    private Stack<int> _continueL = new Stack<int>();
    private Stack<int> _continueR = new Stack<int>();
    private Stack<int> _continueK = new Stack<int>();
    private Stack<SortingState> _continueLine = new Stack<SortingState>();
    
    //Stacks for reversing
    private Stack<int> _oldK = new Stack<int>();
    private Stack<int> _oldPInd = new Stack<int>();
    
    public QuickSort(SortingLogic logic, int n) : base(logic)
    {
        _j = -1;
        _k = 0;
        _l = 0;
        _r = n;
        _pInd = 0;
    }

    private void enterSubroutineWithExitState(SortingState state)
    {
        _continueL.Push(_l);
        _continueR.Push(_r);
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

        _l = _continueL.Pop();
        _r = _continueR.Pop();
        _k = _continueK.Pop();
        _nextState = _continueLine.Pop();
        sortingLogic.markCurrentSubset(_l, _r);
    }
    
    protected override void handleState(SortingState currentState)
    {
        switch (currentState)
        {
            case SortingState.SS_Line1: // quickSort(l,r):
                _nextState = SortingState.SS_Line2;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // if(l<r-1):
                if (_l < _r - 1)
                {
                    _nextState = SortingState.SS_Line3;
                }
                else
                {
                    leaveSubroutine();
                }
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // k = partition(l,r)
                _oldK.Push(_k);
                _nextState = SortingState.SS_Line7;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line4: // quickSort(l,k)
                enterSubroutineWithExitState(SortingState.SS_Line5);
                _r = _k;
                _j = _l-1;
                _nextState = SortingState.SS_Line1;
                sortingLogic.markCurrentSubset(_l, _r);
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line5: // quickSort(k+1,r)
                enterSubroutineWithExitState(SortingState.SS_Line6);
                _l = _k + 1;
                _j = _l-1;
                _nextState = SortingState.SS_Line1;
                sortingLogic.markCurrentSubset(_l, _r);
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line6: // 
                //dummy for leaving subroutine after finishing Line 5
                leaveSubroutine();
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line7: // partition(l, r):
                _nextState = SortingState.SS_Line8;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line8: // p = A[r-1]
                //we use the index of p instead
                //TODO: Set visual for pivot
                _oldPInd.Push(_pInd);
                _pInd = _r-1;
                _nextState = SortingState.SS_Line9;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line9: // k = l
                _oldK.Push(_k);
                _k = _l;
                _nextState = SortingState.SS_Line10;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line10: // for j in range(l,r-1):
                _j++;
                if (_j < _r - 1)
                {
                    _nextState = SortingState.SS_Line11;
                }
                else
                {
                    _nextState = SortingState.SS_Line14;
                }
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line11: // if A[j] <= p:
                if (sortingLogic.CompareGreater(_j, _pInd))
                {
                    _nextState = SortingState.SS_Line10;
                }
                else
                {
                    _nextState = SortingState.SS_Line12;
                }
                break;
            case SortingState.SS_Line12: // swap(j,k)
                _nextState = SortingState.SS_Line13;
                if (_j != _k)
                {
                    _swaps++;
                    sortingLogic.Swap(_j, _k);
                }
                else
                {
                    sortingLogic.MoveFinished();
                }
                break;
            case SortingState.SS_Line13: // k = k+1
                _k++;
                _nextState = SortingState.SS_Line10;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line14: // swap(k,r-1)
                _nextState = SortingState.SS_Line15;
                if (_k != _r - 1)
                {
                    _swaps++;
                    sortingLogic.Swap(_k, _r - 1);
                }
                else
                {
                    sortingLogic.MoveFinished();
                }
                break;
            case SortingState.SS_Line15: // return k
                _nextState = SortingState.SS_Line4;
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
            case SortingState.SS_None:
                sortingLogic.sortingFinished();
                sortingLogic.MoveFinished();
                break;
        }
    }
}

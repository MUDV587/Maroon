using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    //Stacks for subroutine calling
    private bool _leftSubroutine;
    private Stack<int> _continueI = new Stack<int>();
    private Stack<int> _continueJ = new Stack<int>();
    private Stack<int> _continueK = new Stack<int>();
    private Stack<SortingState> _continueLine = new Stack<SortingState>();
    
    //Stacks for reversing
    private Stack<int> _oldK = new Stack<int>();
    private Stack<int> _oldR = new Stack<int>();
    private Stack<int> _oldL = new Stack<int>();
    
    public MergeSort(SortingLogic logic, int n) : base(logic)
    {
        _leftSubroutine = false;
        _i = 0;
        _j = n;
        _k = 0;
        _l = 0;
        _r = 0;
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
        _nextState = _continueLine.Pop();
        _leftSubroutine = true;
    }

    private void continueValuesAfterSubroutine()
    {
        _leftSubroutine = false;
        _i = _continueI.Pop();
        _j = _continueJ.Pop();
        _k = _continueK.Pop();
        sortingLogic.markCurrentSubset(_i, _j);
    }
    
    protected override void handleState(SortingState currentState)
    {
        if (_leftSubroutine)
        {
            continueValuesAfterSubroutine();
        }
        switch (currentState)
        {
            case SortingState.SS_Line1: // mergeSort(A, i, j):
                _nextState = SortingState.SS_Line2;
                sortingLogic.markCurrentSubset(_i, _j);
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
                _oldK.Push(_k);
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
                _oldR.Push(_r);
                _r = _k;
                _nextState = SortingState.SS_Line10;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line10: // l = i
                _oldL.Push(_l);
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
                sortingLogic.markCurrentSubset(0,0); //all to default
                sortingLogic.sortingFinished();
                sortingLogic.MoveFinished();
                break;
        }
    }
    
    protected override void handleReverseState(SortingState currentState)
    {
        switch (currentState)
        {
            case SortingState.SS_Line1: // mergeSort(A, i, j):
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line2: // if i<j-1:
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line3: // k = (i + j) // 2
                _k = _oldK.Pop();
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line4: // mergeSort(A,i,k)
                leaveSubroutine();
                _nextState = currentState;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line5: // mergeSort(A,k,j)
                leaveSubroutine();
                _nextState = currentState;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line6: // merge(A,i,k,j)
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line7: // 
                break;
            case SortingState.SS_Line8: // merge(A, i, k, j):
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line9: // r = k
                _r = _oldR.Pop();
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line10: // l = i
                _l = _oldL.Pop();
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line11: // while l < r and r < j:
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line12: // if A[r] < A[l]:
                sortingLogic.CompareGreater(_l, _r);
                break;
            case SortingState.SS_Line13: // insert(A,r,l)
                _swaps--;
                sortingLogic.Insert(_l,_r);
                break;
            case SortingState.SS_Line14: // r += 1
                _r--;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_Line15: // l += 1
                _l--;
                sortingLogic.MoveFinished();
                break;
            case SortingState.SS_None:
                sortingLogic.sortingFinished();
                sortingLogic.MoveFinished();
                break;
        }
    }
}

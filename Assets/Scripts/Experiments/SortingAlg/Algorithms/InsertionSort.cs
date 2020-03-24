using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    public InsertionSort(SortingLogic logic, int n) : base(logic)
    {
        _nextState = new InsertionSortingState(this, n);
    }

    private class InsertionSortingState : SortingState
    {
        public InsertionSortingState(InsertionSortingState old) : base(old) {}

        public InsertionSortingState(InsertionSort algorithm, int n): base(algorithm)
        {
            _variables.Add("n", n);
            _variables.Add("i", 0);
            _variables.Add("j", 0);
        }

        public override SortingState Next()
        {
            InsertionSortingState next = new InsertionSortingState(this);
            next._line = _nextLine;
            return next;
        }
        
        public override SortingState Copy()
        {
            InsertionSortingState copy = new InsertionSortingState(this);
            return copy;
        }

        public override void Execute()
        {
            int n = _variables["n"];
            int i = _variables["i"];
            int j = _variables["j"];
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 1 .. len(A)-1:
                    i++;
                    
                    if (i < n)
                    {
                        _nextLine = SortingStateLine.SS_Line2;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_None;
                    }
                    break;
                case SortingStateLine.SS_Line2: // j = i-1
                    j = i - 1;
                    
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_Line3: // while A[j]>A[j+1] and j>=0:
                    if (j < 0)
                    {
                        _nextLine = SortingStateLine.SS_Line1;
                        break;
                    }
                    _requireWait = true;
                    if (_algorithm.CompareGreater(j, j + 1))
                    {
                        _nextLine = SortingStateLine.SS_Line4;
                    }
                    else
                    {
                        _nextLine = SortingStateLine.SS_Line1;
                    }
                    break;
                case SortingStateLine.SS_Line4: // swap A[j+1] and A[j]
                    _requireWait = true;
                    _algorithm.Swap(j+1, j);
                    _nextLine = SortingStateLine.SS_Line5;
                    break;
                case SortingStateLine.SS_Line5: // j = j-1
                    j--;
                    _nextLine = SortingStateLine.SS_Line3;
                    break;
                case SortingStateLine.SS_None:
                    break;
            }

            _variables["n"] = n;
            _variables["i"] = i;
            _variables["j"] = j;
        }
        
        public override void Undo()
        {
            _requireWait = false;
            switch (_line)
            {
                case SortingStateLine.SS_Line1: // for i = 1 .. len(A)-1:
                    break;
                case SortingStateLine.SS_Line2: // j = i-1
                    break;
                case SortingStateLine.SS_Line3: // while A[j]>A[j+1] and j>=0:
                    break;
                case SortingStateLine.SS_Line4: // swap A[j+1] and A[j]
                    int j = _variables["j"];
                    _requireWait = true;
                    _algorithm.UndoSwap(j+1, j);
                    break;
                case SortingStateLine.SS_Line5: // j = j-1
                    break;
                case SortingStateLine.SS_None:
                    break;
            }
        }
    }
}

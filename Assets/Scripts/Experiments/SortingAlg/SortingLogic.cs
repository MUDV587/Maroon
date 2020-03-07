using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortingLogic : MonoBehaviour
{
    public enum SortingAlgorithmType
    {
        SA_None,
        SA_RadixSort,
        SA_BubbleSort
    }
    
    [Header("Sorting Machine Settings")] 
    public SortingMachine sortingMachine;

    public SortingAlgorithmType sortingAlgorithm;
    private SortingAlgorithm _algorithm;

    
    [Header("Array Settings")] 
    public ArrayPlace referencePlace;
    [Range(0,10)]
    public int arraySize = 10;

    [Header("Debugging Variables")]
    [Range(0, 9)] public int moveFrom = 0;
    [Range(0, 9)] public int moveTo = 0;
    public bool move = false;
    public bool swap = false;
    
    private List<ArrayPlace> _arrayPlaces = new List<ArrayPlace>();

    public List<ArrayPlace> ArrayPlaces
    {
        get => _arrayPlaces;
    }
    
    
    private int currentSize = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if (referencePlace != null)
            _arrayPlaces.Add(referencePlace);
        CreateArray(arraySize);
    }

    // Update is called once per frame
    void Update()
    {
        if(currentSize != arraySize)
            CreateArray(arraySize);

        if (move) {
            Insert(moveFrom, moveTo);
        }

        if (swap) {
            Swap(moveFrom, moveTo);
        }
    }

    public void CreateArray(int newSize)
    {
        // + is going to left, - is going to right -> z-axis!
        var currentPos = Vector3.zero;
        var neededWidth = newSize * referencePlace.width;
        currentPos.z += (neededWidth / 2f) - referencePlace.width / 2f;
        
        for (var i = 0; i < _arrayPlaces.Count; ++i)
        {
            var isActive = i < newSize;
            _arrayPlaces[i].gameObject.SetActive(isActive);
            if (!isActive) continue;
            
            //Set Position
            _arrayPlaces[i].gameObject.transform.localPosition = currentPos;
            _arrayPlaces[i].Index = i;
            currentPos.z -= referencePlace.width;
        }

        while (_arrayPlaces.Count < newSize)
        {
            var newPlace = Instantiate(referencePlace.gameObject, referencePlace.transform.parent);
            
            newPlace.SetActive(true);
            newPlace.transform.localPosition = currentPos;
            var place = newPlace.GetComponent<ArrayPlace>();
            place.Index = _arrayPlaces.Count;
            currentPos.z -= referencePlace.width;
            
            _arrayPlaces.Add(place);
        }

        currentSize = arraySize;
    }

    public void Insert(int fromIdx, int toIdx)
    {
        if (fromIdx < 0 || fromIdx >= _arrayPlaces.Count || !_arrayPlaces[fromIdx].isActiveAndEnabled ||
            toIdx < 0 || toIdx >= _arrayPlaces.Count || !_arrayPlaces[toIdx].isActiveAndEnabled)
            return;
        
        sortingMachine.Insert(fromIdx, toIdx);
        // move = false;
    }

    public void Swap(int idx1, int idx2)
    {
        if (idx1 < 0 || idx1 >= _arrayPlaces.Count || !_arrayPlaces[idx1].isActiveAndEnabled ||
            idx2 < 0 || idx2 >= _arrayPlaces.Count || !_arrayPlaces[idx2].isActiveAndEnabled)
            return;

        sortingMachine.Swap(idx1, idx2);
        swap = false;
    }
    
    public void MoveFinished()
    {
        //TODO Jannik: This function gets called once the sorting machine finished insert, swap, to Bucket resp. compare
    }

    public void RearrangeArrayElements(float speed)
    {
        for (var i = 0; i < ArrayPlaces.Count - 1; ++i)
        {
            if (ArrayPlaces[i].sortElement != null) continue;
            
            ArrayPlaces[i].SetSortElement(_arrayPlaces[i+1].sortElement, speed);
            _arrayPlaces[i + 1].sortElement = null;
        }
    }

    public void MakePlaceInArray(int index, float speed)
    {
        for (var i = ArrayPlaces.Count - 1; i > index; --i)
        {
            if(ArrayPlaces[i].sortElement != null) continue;
            
            ArrayPlaces[i].SetSortElement(_arrayPlaces[i-1].sortElement, speed);
            ArrayPlaces[i - 1].sortElement = null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SortingLogic : MonoBehaviour
{
    [Header("Sorting Machine Settings")] 
    public SortingMachine sortingMachine;

    
    [Header("Array Settings")] 
    public ArrayPlace referencePlace;
    [Range(0,10)]
    public int arraySize = 10;
    public float _arrayArrangeSpeed = 0.3f;

    public int moveFrom = 0;
    public int moveTo = 0;
    public bool move = false;
    
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
            MoveElement(moveFrom, moveTo);
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
            _arrayPlaces[i].Index = i + 1;
            currentPos.z -= referencePlace.width;
        }

        while (_arrayPlaces.Count < newSize)
        {
            var newPlace = Instantiate(referencePlace.gameObject, referencePlace.transform.parent);
            
            newPlace.SetActive(true);
            newPlace.transform.localPosition = currentPos;
            var place = newPlace.GetComponent<ArrayPlace>();
            place.Index = _arrayPlaces.Count + 1;
            currentPos.z -= referencePlace.width;
            
            _arrayPlaces.Add(place);
        }

        currentSize = arraySize;
    }

    public void MoveElement(int fromIdx, int toIdx)
    {
        sortingMachine.MoveElement(fromIdx, toIdx);
        move = false;
    }

    public void RearrangeArrayElements()
    {
        for (var i = 0; i < ArrayPlaces.Count - 1; ++i)
        {
            if (ArrayPlaces[i].sortElement != null) continue;
            
            ArrayPlaces[i].SetSortElement(_arrayPlaces[i+1].sortElement, _arrayArrangeSpeed);
            _arrayPlaces[i + 1].sortElement = null;
        }
    }

    public void MakePlaceInArray(int index)
    {
        for (var i = ArrayPlaces.Count - 1; i > index; --i)
        {
            if(ArrayPlaces[i].sortElement != null) continue;
            
            ArrayPlaces[i].SetSortElement(_arrayPlaces[i-1].sortElement, _arrayArrangeSpeed);
            ArrayPlaces[i - 1].sortElement = null;
        }
    }
}

using System;
using UnityEngine;

public class SortingMachine : MonoBehaviour
{
    public enum SortingMachineState
    {
        SMS_Pause,
        SMS_ToSource,
        SMS_SourceDown,
        SMS_GrabSource,
        SMS_SourceUp,
        SMS_ToDestination,
        SMS_DestinationDown,
        SMS_PlaceDestination,
        SMS_Up
    }
    
    public SortingMachineState sortingState = SortingMachineState.SMS_Pause;
    public SortingLogic sortingLogic;
    
    [Header("Speed Settings")]
    public float distancePerSecond = 0.5f;
    public float distancePerSecondVertical = 0.5f;

    [Header("Highlight Settings")] 
    public GameObject highlighter;
    
    [Header("Grapper Settings")] 
    public GameObject bigGrapper;
    public float bigGrapperStart = 0;
    public float bigGrapperEnd = -0.2f;
    public GameObject middleGrapper;
    public float middleGrapperStart = 0.65f;
    public float middleGrapperEnd = 0.5f;
    public GameObject smallGrapper;
    public float smallGrapperStart = 0f;
    public float smallGrapperEnd = -0.45f;
    public GameObject grapperPointer;

    
    
    private int _sourceIdx;
    private int _destinationIdx;
    private GameObject _movingElement;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (sortingState)
        {
            case SortingMachineState.SMS_Pause: //to nothing as we pause
                break;
            case SortingMachineState.SMS_ToSource:
            {
                if(moveHorizontal(sortingLogic.ArrayPlaces[_sourceIdx].transform.position.z))
                    sortingState = SortingMachineState.SMS_SourceDown;
            } break;
            case SortingMachineState.SMS_SourceDown:
            {
                if (GoDown(sortingLogic.ArrayPlaces[_sourceIdx].sortElement.transform.position.y))
                    sortingState = SortingMachineState.SMS_GrabSource;
            } break;
            case SortingMachineState.SMS_GrabSource:
                highlighter.SetActive(true);
                _movingElement = sortingLogic.ArrayPlaces[_sourceIdx].sortElement;
                sortingLogic.ArrayPlaces[_sourceIdx].sortElement = null;
                _movingElement.transform.parent = smallGrapper.transform;

                sortingLogic.RearrangeArrayElements();
                sortingState = SortingMachineState.SMS_SourceUp;
                break;
            case SortingMachineState.SMS_SourceUp:
                if (GoUp())
                    sortingState = SortingMachineState.SMS_ToDestination;
                break;
            case SortingMachineState.SMS_ToDestination:
                if (moveHorizontal(sortingLogic.ArrayPlaces[_destinationIdx].transform.position.z))
                {
                    sortingState = SortingMachineState.SMS_DestinationDown;
                    sortingLogic.MakePlaceInArray(_destinationIdx);
                }
                break;
            case SortingMachineState.SMS_DestinationDown:
                if (GoDown(sortingLogic.ArrayPlaces[_destinationIdx].elementPlace.transform.position.y + 0.1f)) //TODO: SortingElement with WIDTH
                    sortingState = SortingMachineState.SMS_PlaceDestination;
                break;
            case SortingMachineState.SMS_PlaceDestination:
                highlighter.SetActive(false);
                sortingLogic.ArrayPlaces[_destinationIdx].SetSortElement(_movingElement, 1f);
                _movingElement = null;
                sortingState = SortingMachineState.SMS_Up;
                break;
            case SortingMachineState.SMS_Up:
                if (GoUp())
                    sortingState = SortingMachineState.SMS_Pause;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public bool MoveElement(int fromIdx, int toIdx)
    {
        if (sortingState != SortingMachineState.SMS_Pause) return false;

        _sourceIdx = fromIdx;
        _destinationIdx = toIdx;
        sortingState = SortingMachineState.SMS_ToSource;
        return true;
    }

    private bool GoDown(float sourceY) //returns if the sourceY was reached
    {
        var distance = Mathf.Abs(sourceY - grapperPointer.transform.position.y);
        var retValue = false;

        if (distance > distancePerSecondVertical * Time.deltaTime)
            distance = distancePerSecondVertical * Time.deltaTime;
        else
        {
            //next thing will be grapping the source
            retValue = true;
        }

        //biggest grapper
        var pos = bigGrapper.transform.localPosition;
        var diffY = pos.y - bigGrapperEnd;
        if (diffY >= 0)
        {
            pos.y -= diffY > distance ? distance : diffY;
            distance -= diffY > distance ? distance : diffY;
            bigGrapper.transform.localPosition = pos;
        }
                
        //middle grapper
        pos = middleGrapper.transform.localPosition;
        diffY = pos.y - middleGrapperEnd;
        if (diffY >= 0)
        {
            pos.y -= diffY > distance ? distance : diffY;
            distance -= diffY > distance ? distance : diffY;
            middleGrapper.transform.localPosition = pos;
        }
                
        //small grapper
        pos = smallGrapper.transform.localPosition;
        diffY = pos.y - smallGrapperEnd;
        if (diffY >= 0)
        {
            pos.y -= diffY > distance ? distance : diffY;
            distance -= diffY > distance ? distance : diffY;
            smallGrapper.transform.localPosition = pos;
        }

        return retValue;
    }

    private bool GoUp()
    {
        var madeDistance = 0f;
        var allowedDistance = distancePerSecondVertical * Time.deltaTime;
        var retvalue = true;
        
        //small grapper
        var pos = smallGrapper.transform.localPosition;
        var diffY = smallGrapperStart - pos.y;

        if (diffY > allowedDistance)
        {
            diffY = allowedDistance;
            allowedDistance = -1f;
        }
        else
        {
            allowedDistance -= diffY;
        }

        if (diffY >= 0)
        {
            pos.y = pos.y + diffY > smallGrapperStart ? smallGrapperStart : pos.y + diffY;
            smallGrapper.transform.localPosition = pos;
        }

        if (allowedDistance < 0f) return false;
        
        //middleGrapper
        pos = middleGrapper.transform.localPosition;
        diffY = middleGrapperStart - pos.y;

        if (diffY > allowedDistance)
        {
            diffY = allowedDistance;
            allowedDistance = -1f;
        }

        if (diffY >= 0)
        {
            pos.y = pos.y + diffY > middleGrapperStart ? middleGrapperStart : pos.y + diffY;
            middleGrapper.transform.localPosition = pos;
        }

        if (allowedDistance < 0f) return false;
        
        //bigGrapper
        pos = bigGrapper.transform.localPosition;
        diffY = bigGrapperStart - pos.y;

        if (diffY > allowedDistance)
        {
            diffY = allowedDistance;
            allowedDistance = -1f;
        }

        if (diffY >= 0)
        {
            pos.y = pos.y + diffY > bigGrapperStart ? bigGrapperStart : pos.y + diffY;
            bigGrapper.transform.localPosition = pos;
        }

        if (allowedDistance < 0f) return false;

        return true;
    }

    private bool moveHorizontal(float sourceZ)
    {
        var newPos = transform.position;
        var retValue = false;
        
        if (newPos.z < sourceZ)
        {
            newPos.z += distancePerSecond * Time.deltaTime;
            if (newPos.z >= sourceZ)
            {
                newPos.z = sourceZ;
                retValue = true;
            }
        }
        else
        {
            newPos.z -= distancePerSecond * Time.deltaTime;
            if (newPos.z <= sourceZ)
            {
                newPos.z = sourceZ;
                retValue = true;
            }
        }
        transform.position = newPos;
        return retValue;
    }
    
}

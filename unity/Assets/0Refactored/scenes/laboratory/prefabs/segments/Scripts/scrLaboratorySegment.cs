using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrLaboratorySegment : MonoBehaviour
{
    // #################################################################################################################
    // Members

    // Settings
    [SerializeField] private int segmentLength;

    // Constants
    private const int segmentLengthSmall = 4;
    private const int segmentLengthLarge = 8;

    // Current values
    private int targetIndex = 0;
    private int targetForwardTranslation = 0;
    private bool movedToTargetForwardTranslation = false;
    private bool animationActive = false;

    // #################################################################################################################
    // Methods

    // Sets target index and re-calculates target forward translation
    public void setTargetIndexAndForwardTranslation(int targetIndex)
    {
        // If segment is first segment
        if (targetIndex <= 0)
        {
            // Set index and translation
            this.targetIndex = 0;
            this.targetForwardTranslation = 0;
        }

        // If segment is any other segment
        else
        {
            // Set index
            this.targetIndex = targetIndex;

            // Origin + first segment +  all previous segments
            this.targetForwardTranslation = 0 + scrLaboratorySegment.segmentLengthSmall +
                                            (targetIndex - 1) * scrLaboratorySegment.segmentLengthLarge;
        }

        // Segment is possibly not at desired location anymore
        this.movedToTargetForwardTranslation = false;
    }

    public void moveToTargetForwardTranslation(bool animate = false)
    {
        // If animation
        if(animate)
        {
            // TODO
        }

        // If instant jump to position
        else
        {
            this.gameObject.transform.position = Vector3.zero +
                                                 this.gameObject.transform.forward * this.targetForwardTranslation;
            this.movedToTargetForwardTranslation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
    // TODO 
    }
}

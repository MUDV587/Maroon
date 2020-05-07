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
    // Methods: Previews in segment

    public void AddPreview(GameObject prefabPreview, bool isRight = false)
    {
        // Instantiate new preview
        GameObject new_preview = Instantiate(prefabPreview, this.gameObject.transform.position, Quaternion.identity,
                                             this.gameObject.transform);

        // Remove editor only stuff
        new_preview.transform.GetChild(0).gameObject.SetActive(false);
        
        // Rotate preview for other side
        if(isRight)
        {
            new_preview.transform.position = this.gameObject.transform.position +
                                             new Vector3(0, 0, scrLaboratorySegment.segmentLengthLarge);
            new_preview.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }

    // #################################################################################################################
    // Methods: Index, position and movement

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
            this.animationActive = true;
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
        if(this.animationActive)
        {
            // Calculate target and distance
            Vector3 vec_target = Vector3.zero + this.gameObject.transform.forward * this.targetForwardTranslation;
            float distance = Vector3.Distance(vec_target, this.gameObject.transform.position);

            // Move closer by half the distance
            this.gameObject.transform.position -= this.gameObject.transform.forward * 
                                                  (float)(distance * 0.5F * Time.deltaTime * 2.0F);

            // Check if close enough and end animation
            if(distance < 0.08F)
            {
                this.gameObject.transform.position = vec_target;
                this.movedToTargetForwardTranslation = true;
                this.animationActive = false;
            }
        }
   }
}

using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class SortingElement : MonoBehaviour
{
    [Header("Sorting Criteria:")]
    [Range(0, 100)]
    public int number;

    public Color color;
    [Range(0f, 0.3f)] public float size;

    [Header("Components")]
    public TextMeshPro text;
    
    private float _lastSize;
    private int _lastNumber;
    private Color _lastColor;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        AdaptAppearance();
    }

    private void Update()
    {
        if (number != _lastNumber || color != _lastColor || Math.Abs(size - _lastSize) > 0.00001)
            AdaptAppearance();
    }


    public void AdaptAppearance()
    {
        if(text)
            text.text = number.ToString();

        var trans = transform;
        trans.localScale = new Vector3(size, size, size);
        var localPos = trans.localPosition;
        localPos.y = 0.2f + size / 2f;
        trans.localPosition = localPos;

        foreach (var mat in _meshRenderer.materials)
            mat.color = color;
        
        _lastColor = color;
        _lastNumber = number;
        _lastSize = size;
    }
}
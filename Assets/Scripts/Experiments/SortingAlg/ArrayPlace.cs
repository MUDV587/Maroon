using TMPro;
using UnityEngine;

public class ArrayPlace : MonoBehaviour
{
    public float width = 0.3f;
    public Transform elementPlace;
    public GameObject sortElement = null; //this will be the place where we can access the sorting element (eg. the colored cube etc)
    public GameObject highlighter = null;
    
    private int _index = 0;

    private bool _moveSortElement;
    private float _distancePerSecond;
    
    public int Index {
        get => _index;
        set
        {
            if (value != _index)
            {
                _index = value;
                UpdateIndex();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateIndex();
    }

    private void Update()
    {
        if (!_moveSortElement || sortElement == null) return;
        var sortPos = sortElement.transform.localPosition;
        var refPos = elementPlace.localPosition;

        sortPos.x = getNewPos(sortPos.x, refPos.x);
        sortPos.z = getNewPos(sortPos.z, refPos.z);
        sortElement.transform.localPosition = sortPos;

        if (Mathf.Abs(sortPos.x - refPos.x) < 0.00001 && Mathf.Abs(sortPos.z - refPos.z) < 0.00001)
        {
            _moveSortElement = false;
        }
    }

    private float getNewPos(float current, float reference)
    {
        if (current < reference)
        {
            current += _distancePerSecond * Time.deltaTime;
            current = current > reference ? reference : current;
        }
        else
        {
            current -= _distancePerSecond * Time.deltaTime;
            current = current < reference ? reference : current;
        }

        return current;
    }

    private void UpdateIndex()
    {
        var textField = GetComponentInChildren<TextMeshPro>();
        if (textField)
        {
            textField.text = _index.ToString("0");
        }
    }

    public void SetSortElement(GameObject sortElement, float speed)
    {
        this.sortElement = sortElement;
        this.sortElement.transform.parent = transform;

        var sortPos = sortElement.transform.localPosition;
        var refPos = elementPlace.localPosition;

        var max = Mathf.Max(Mathf.Abs(sortPos.x - refPos.x), Mathf.Abs(sortPos.z - refPos.z));
        
        _distancePerSecond = max / speed;
        _moveSortElement = true;
    }
}

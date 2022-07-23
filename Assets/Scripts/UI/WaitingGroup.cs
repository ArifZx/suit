using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingGroup : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _text;

    [SerializeField]
    private GameObject _loadingImage;

    [SerializeField]
    private float _loadingSpeed = 360;

    // Start is called before the first frame update
    void Start()
    {
                
    }

    private void Update()
    {
        if(_loadingImage)
        {
            _loadingImage.transform.Rotate(Vector3.back, _loadingSpeed * Time.deltaTime);
        }
    }

}

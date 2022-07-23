using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;

public class CountdownGroup : MonoBehaviour
{
    [SerializeField]
    TMP_Text _text;

    public void SetText(string text)
    {
        if (_text == null) return;

        _text.text = text;

        _text.transform.localScale.Set(0, 0, 0);

        DOTween.Play(_text.gameObject.transform.DOScale(Vector3.one, 1f));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightUI : MonoBehaviour
{
    [SerializeField]
    private StarsUI _stars;
    public StarsUI Stars { get { return _stars; } }

    [SerializeField]
    private Sprite[] _sprites;

    [SerializeField]
    private Image _image;

    public void ChooseRPS(string name)
    {
        switch (name)
        {
            case "rock":
                _image.sprite = _sprites[0];
                break;

            case "paper":
                _image.sprite = _sprites[2];
                break;

            case "scissor":
                _image.sprite = _sprites[1];
                break;

            default:
                _image.sprite = _sprites[3];
                break;
        }
    }
}

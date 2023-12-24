using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    private float _timer = 0f;
    private TMP_Text _text;
    
    private void Start()
    {
        _text = gameObject.GetComponent<TMP_Text>();
        
        var topEdge = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        var _screenWidth = topEdge.x * 2;
        
        gameObject.transform.localScale = new Vector3(_screenWidth * 0.5f , _screenWidth * 0.5f, 1); 

    }

    private void Update()
    {
        if (Board.IsSolved) return;
        _timer += Time.deltaTime;
        _text.text = $"{(int)_timer / 60}m {(int)(_timer % 60)}s";
    }
}

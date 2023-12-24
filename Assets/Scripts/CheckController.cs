using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckController : MonoBehaviour
{
    void Start()
    {
        var topEdge = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        var _screenWidth = topEdge.x * 2;
        
        gameObject.transform.localScale = new Vector3(_screenWidth * 0.5f , _screenWidth * 0.5f, 1);
    }
}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountDown : MonoBehaviour
{
    private Text _text;
    
    private void Start()
    {
        _text = GetComponent<Text>();
        _text.text = "";
    }

    public void StartCountDown(Action action, int total = 10)
    {
        StartCoroutine(StartCount(total, action));
    }

    private IEnumerator StartCount(int total, Action action)
    {
        for (int i = 0; i < total; i++)
        {
            _text.text = (total - i).ToString();
            yield return new WaitForSeconds(1);
        }
        _text.text = "";
        if (action != null) action.Invoke();
    }
}

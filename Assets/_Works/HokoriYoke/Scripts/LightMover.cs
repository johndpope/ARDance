using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LightMover : MonoBehaviour
{
    [SerializeField] private float _startPosZ;
    [SerializeField] private float _endPosZ;
    [SerializeField] private float _duration;
    [SerializeField] private float _interval;
    
    void Start()
    {
        StartCoroutine(Repeat());
    }

    private void Move()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, _startPosZ);
        transform.DOMoveZ(_endPosZ, _duration);
    }

    private IEnumerator Repeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(_interval);
            Move();
        }
    }
}

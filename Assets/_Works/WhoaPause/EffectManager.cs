using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private SplitManager _splitManager;
    [SerializeField] private ShowOffManager _showOffManager;
    [SerializeField] private CountDown _countDown;
    [SerializeField] private GameObject _canvas;
    
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void EffectStart()
    {
        StartCoroutine(Schedule());
    }

    private IEnumerator Schedule()
    {
        while (gameObject.activeSelf)
        {
            _countDown.StartCountDown(null, 7);
            yield return new WaitForSeconds(7f);
            _canvas.SetActive(false);
            _audioSource.Play();
            yield return new WaitForSeconds(2.55f);
            _splitManager.SetCross(0.1f);
            yield return new WaitForSeconds(0.85f);
            _splitManager.BackCross(0.1f);
            yield return new WaitForSeconds(0.38f);
            _splitManager.SetRight(0.1f);
            yield return new WaitForSeconds(0.45f);
            _splitManager.SetLeft(0.1f);
            yield return new WaitForSeconds(0.45f);
            _splitManager.SetUp(0.1f);
            yield return new WaitForSeconds(0.45f);
            _splitManager.SetDown(0.1f);
            yield return new WaitForSeconds(0.74f);
            _splitManager.BackAll(0.1f);
            yield return new WaitForSeconds(3.5f);
            _showOffManager.Show();
            yield return new WaitForSeconds(4f);
            _showOffManager.Stop();
        }
    }
}

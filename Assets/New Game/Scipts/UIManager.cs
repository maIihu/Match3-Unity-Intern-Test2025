using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panelLose;
    [SerializeField] private GameObject panelWin;
    [SerializeField] private GameObject panelHome;
    [SerializeField] private GameObject botWin;
    [SerializeField] private GameObject botLose;
    [SerializeField] private GameObject board;

    [SerializeField] private GameObject timerText;
    private float timeRemaining = 60f;

    void Start()
    {
        BoardManager.IsLose += Lose;
        BoardManager.IsWin += Win;
        StartCoroutine(CountdownTimer());
    }
    private IEnumerator CountdownTimer()
    {
        while (timeRemaining > 0)
        {
            UpdateTimerUI();
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        Lose();  
    }
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.GetComponent<TextMeshProUGUI>().text = "Time: " + Mathf.CeilToInt(timeRemaining);
        }
    }
    private void OnDestroy()
    {
        BoardManager.IsLose -= Lose;
        BoardManager.IsWin -= Win;
    }
    private void Lose() {

        Time.timeScale = 0;
        panelLose.SetActive(true);
    }
    private void Win()
    {
        Time.timeScale = 0;
        panelWin.SetActive(true);
    }

    public void AutoPlayMode() {
        panelHome.SetActive(false);
        board.SetActive(true);
        botWin.SetActive(true);        
    }

    public void AutoLoseMode() {
        panelHome.SetActive(false);
        board.SetActive(true);
        botLose.SetActive(true);
    }

    public void TimeAttackMode()
    {
        panelHome.SetActive(false);
        board.SetActive(true);
        BoardManager.Instance.timeAttackMode = true;
        timerText.SetActive(true);
    }
}

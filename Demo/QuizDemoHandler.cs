using System.Collections;
using UnityEngine;
using EIC.Quiz;

/// <summary>
/// Quiz demo class
/// </summary>

public class QuizDemoHandler : MonoBehaviour
{
    [SerializeField] private QuizManager quizManager;

    private void Start()
    {
        StartCoroutine(NextQuestion(0f));
    }
    
    private void OnEnable()
    {
        quizManager.OnChoose += OnChoose;
    }

    private void OnDisable()
    {
        quizManager.OnChoose -= OnChoose;
    }

    private void OnChoose(bool correct, QuizResult quizResult)
    {
        Debug.Log($"{(correct ? "Right" : "Wrong")} answer!");
        
        if (quizResult.complete)
        {
            Debug.Log("The end!");
            Debug.Log($"Right answers: {quizResult.rightAnswers} | Wrong answers: {quizResult.wrongAnswers}");
            return;
        }
        
        StartCoroutine(NextQuestion(1f));
    }

    private IEnumerator NextQuestion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        var qdi = quizManager.PopQuestion();
        Debug.Log($"Question: {qdi.question}");
    }

    public void RefreshQuestion()
    {
        quizManager.RefreshQuestion();
    }
}
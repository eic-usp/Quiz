using System.Collections;
using UnityEngine;
using EIC.Quiz;

/// <summary>
/// Quiz demo class
/// Subscribe to these events
/// </summary>

public class QuizDemoHandler : MonoBehaviour
{
    [SerializeField] private QuizManager quizManager;

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
        quizManager.PopQuestion();
    }
}
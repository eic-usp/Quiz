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

    private int _rightAnswerCount;
    private int _wrongAnswerCount;

    private void OnEnable()
    {
        quizManager.OnChooseRight += OnChooseRight;
        quizManager.OnChooseWrong += OnChooseWrong;
        quizManager.OnComplete += OnComplete;
    }

    private void OnDisable()
    {
        quizManager.OnChooseRight -= OnChooseRight;
        quizManager.OnChooseWrong -= OnChooseWrong;
        quizManager.OnComplete -= OnComplete;
    }
    
    private void OnChooseRight()
    {
        // Debug.Log("Right answer!");
        _rightAnswerCount++;
        StartCoroutine(NextQuestion(1f));
    }
    
    private void OnChooseWrong()
    {
        // Debug.Log("Wrong answer!");
        _wrongAnswerCount++;
        StartCoroutine(NextQuestion(1f));
    }

    private void OnComplete()
    {
        Debug.Log("The end!");
        Debug.Log($"Right answers: {_rightAnswerCount} | Wrong answers: {_wrongAnswerCount}");
    }

    private IEnumerator NextQuestion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        quizManager.PopQuestion();
    }
}
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
        Debug.Log("Right answer!");
        StartCoroutine(NextQuestion(1f));
    }
    
    private void OnChooseWrong()
    {
        Debug.Log("Wrong answer!");
    }

    private void OnComplete()
    {
        Debug.Log("The end!");
    }

    private IEnumerator NextQuestion(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        quizManager.PopQuestion();
    }
}

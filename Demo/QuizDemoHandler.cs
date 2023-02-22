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
    }

    private void OnDisable()
    {
        quizManager.OnChooseRight -= OnChooseRight;
        quizManager.OnChooseWrong -= OnChooseWrong;
    }
    
    private void OnChooseRight()
    {
        Debug.Log("Right answer!");   
    }
    
    private void OnChooseWrong()
    {
        Debug.Log("Wrong answer!");
    }
}

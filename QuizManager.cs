using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace EIC.Quiz
{
    [AddComponentMenu("EIC/Quiz/QuizManager")]
    [RequireComponent(typeof(CanvasGroup))]
    public class QuizManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private Color rightAnswerColor;
        [SerializeField] private Color wrongAnswerColor;
        
        [Tooltip("From Resources folder. Leave empty to setup manually")]
        [SerializeField] private string questionsFilePath;
        
        [Tooltip("Number of questions to be picked")]
        [Min(0)]
        [SerializeField] private int numberOfQuestions;
        
        [Tooltip("Allow multiple failed attempts for each question")]
        [field: SerializeField] public bool MultipleAttempts { get; set; }

        public event System.Action<bool, QuizResult> OnChoose;

        public int QuestionStackCount => _quizDataItems.Count;
        
        private CanvasGroup _canvasGroup;
        private QuizOption[] _options;
        private Stack<QuizDataItem> _quizDataItems;
        private QuizDataItem _currentQuizDataItem;
        private QuizResult _quizResult;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _options = GetComponentsInChildren<QuizOption>();
            if (string.IsNullOrEmpty(questionsFilePath)) return;
            Setup(questionsFilePath, numberOfQuestions);
        }

        public void Setup(string resourcePath, int nQuestions = 0)
        {
            var qd = LoadResourceFromJson(resourcePath);
            SetQuestion(qd, nQuestions);
            PopQuestion();
        }

        private static QuizDataItem[] LoadResourceFromJson(string resourcePath)
        {
            var json = Resources.Load<TextAsset>(resourcePath);
            return JsonUtility.FromJson<QuizData>(json.text).questions;
        }

        private void SetQuestion(IList<QuizDataItem> quizDataItems, int nQuestions = 0)
        {
            _quizDataItems = new Stack<QuizDataItem>();
            _quizResult = default;
            
            var rng = new System.Random();
            var n = quizDataItems.Count;
            
            while (n > 1) 
            {
                var k = rng.Next(n--);
                (quizDataItems[n], quizDataItems[k]) = (quizDataItems[k], quizDataItems[n]);
            }

            var nQ = nQuestions > 0 && nQuestions < quizDataItems.Count ? nQuestions : quizDataItems.Count; 
            
            for (var i = 0; i < nQ; i++)
            {
                _quizDataItems.Push(quizDataItems[i]);
            }
        }

        public void RefreshQuestion()
        {
            if (_quizDataItems == null)
            {
                Debug.LogError("Questions are not set");
                return;
            }

            if (_quizDataItems.Count == 0)
            {
                Debug.LogWarning("Question stack is empty");
                return;
            }

            var tempQdi = _currentQuizDataItem;
            var tempResult = _quizResult;
            
            SetQuestion(_quizDataItems.ToArray());
            _quizResult = tempResult;
            PopQuestion();
            _quizDataItems.Push(tempQdi);
        }

        public void PopQuestion()
        {
            if (_quizDataItems == null)
            {
                Debug.LogError("Questions are not set");
                return;
            }

            if (_quizDataItems.Count == 0)
            {
                Debug.LogWarning("Question stack is empty");
                return;
            }
            
            _currentQuizDataItem = _quizDataItems.Pop();

            if (_currentQuizDataItem.options.Length != _options.Length)
            {
                Debug.LogError("The number of quiz option buttons must be the same as the number of options in the resource file");
                return;
            }

            questionText.text = _currentQuizDataItem.question;

            for (var i = 0; i < _options.Length; i++)
            {
                _options[i].SetAnswer(_currentQuizDataItem.options[i]);
            }

            var correctOption = _currentQuizDataItem.correctOption;
            var nOptions = _currentQuizDataItem.options.Length;
            var nOptionButtons = _options.Length;

            if (correctOption > nOptions || correctOption > nOptionButtons)
            {
                Debug.LogError($"Invalid option. The correct option must be between 1 and {_currentQuizDataItem.options.Length}");
                return;
            }

            _options[correctOption-1].Correct = true;
            _canvasGroup.interactable = true;
        }

        public void Choose(QuizOption quizOption)
        {
            if (_quizDataItems == null)
            {
                Debug.LogError("Questions are not set");
                return;
            }

            _quizResult.complete = _quizDataItems.Count == 0;
            _canvasGroup.interactable = MultipleAttempts;
            
            if (quizOption.Correct)
            {
                quizOption.Image.color = rightAnswerColor;
                _quizResult.rightAnswers++;
                _canvasGroup.interactable = false;
            }
            else
            {
                quizOption.Image.color = wrongAnswerColor;
                _quizResult.wrongAnswers++;
            }
            
            OnChoose?.Invoke(quizOption.Correct, _quizResult);
        }
    }
}
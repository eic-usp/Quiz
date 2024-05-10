using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

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
        
        [field: Tooltip("Allow multiple failed attempts for each question")]
        [field: SerializeField] public bool MultipleAttempts { get; set; }

        /// <summary>
        /// Invoked when a quiz option is chosen.
        /// The bool value indicates whether the selected option is the correct one
        /// and the <see cref="QuizResult"/> struct contains partial or complete info about the quiz results.
        /// </summary>
        public event System.Action<bool, QuizResult> OnChoose;

        /// <summary>
        /// The number of questions contained in the quiz question stack. The current question is not included,
        /// as it has already been popped.
        /// </summary>
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
            SetUp(questionsFilePath, numberOfQuestions);
        }

        /// <summary>
        /// Load questions from a resource file and set up the quiz.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <param name="nQuestions">The number of questions to be used in the quiz.</param>
        
        public void SetUp(string resourcePath, int nQuestions = 0)
        {
            var qd = LoadResourceFromJson(resourcePath);
            SetQuestions(qd, nQuestions);
        }

        /// <summary>
        /// Load questions from a JSON file.
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns>The array of questions obtained from the resource file.</returns>
        
        private static QuizDataItem[] LoadResourceFromJson(string resourcePath)
        {
            var json = Resources.Load<TextAsset>(resourcePath);
            return JsonUtility.FromJson<QuizData>(json.text).questions;
        }

        /// <summary>
        /// Set up the quiz based on the provided list of questions. The questions are randomly selected.
        /// </summary>
        /// <param name="quizDataItems">A collection of questions (<see cref="QuizDataItem"/>).</param>
        /// <param name="nQuestions">The number of questions to be used in the quiz.</param>
        
        private void SetQuestions(IList<QuizDataItem> quizDataItems, int nQuestions = 0)
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

        /// <summary>
        /// Shuffle the question stack and pick a new random question, then return the current question to the stack
        /// </summary>
        /// <returns>The newly picked question</returns>
        
        public QuizDataItem RefreshQuestion()
        {
            if (_quizDataItems == null)
            {
                Debug.LogError("Questions are not set");
                return null;
            }

            if (_quizDataItems.Count == 0)
            {
                Debug.LogWarning("Question stack is empty");
                return null;
            }

            // Save current data
            
            var tempQdi = _currentQuizDataItem;
            var tempResult = _quizResult;
            
            SetQuestions(_quizDataItems.ToArray()); // Reset the questions
            _quizResult = tempResult; // Retrieve saved data
            var qdi = PopQuestion(); // Pop the new question
            _quizDataItems.Push(tempQdi); // Push the old question back into the stack
            
            return qdi;
        }
        
        /// <summary>
        /// Pop a question from the stack
        /// </summary>
        /// <returns>The question on top of the stack</returns>

        public QuizDataItem PopQuestion()
        {
            if (_quizDataItems == null)
            {
                Debug.LogError("Questions are not set");
                return null;
            }

            if (_quizDataItems.Count == 0)
            {
                Debug.LogWarning("Question stack is empty");
                return null;
            }
            
            _currentQuizDataItem = _quizDataItems.Pop();

            // Check if the sizes match
            
            if (_currentQuizDataItem.options.Length != _options.Length)
            {
                Debug.LogError("The number of quiz option buttons must be the same as the number of options in the resource file");
                return null;
            }
            
            LocalizationManager.SetStringReference(questionText.GetComponent<LocalizeStringEvent>(), LocalizationManager.Quiz, _currentQuizDataItem.question);

            // Set the text for each option
            
            for (var i = 0; i < _options.Length; i++)
            {
                _options[i].SetAnswer(_currentQuizDataItem.options[i]);
            }

            var correctOption = _currentQuizDataItem.correctOption;
            var nOptions = _currentQuizDataItem.options.Length;
            var nOptionButtons = _options.Length;

            // Check bounds
            
            if (correctOption > nOptions || correctOption > nOptionButtons)
            {
                Debug.LogError($"Invalid option. The correct option must be between 1 and {_currentQuizDataItem.options.Length}");
                return null;
            }

            _options[correctOption-1].Correct = true; // Set the correct option
            _canvasGroup.interactable = true; // Enable options

            return _currentQuizDataItem;
        }

        /// <summary>
        /// Select one of the available options. Triggers <see cref="OnChoose"/> event.
        /// </summary>
        /// <param name="quizOption">The selected option</param>
        
        public void Choose(QuizOption quizOption)
        {
            if (_quizDataItems == null)
            {
                Debug.LogError("Questions are not set");
                return;
            }

            _quizResult.complete = _quizDataItems.Count == 0; // Condition for complete state
            _canvasGroup.interactable = MultipleAttempts; // When multiple attempts option is checked, don't disable

            if (quizOption.Correct)
            {
                quizOption.Image.color = rightAnswerColor;
                _quizResult.rightAnswers++;
                _canvasGroup.interactable = false; // Disable so the player won't keep selecting options
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
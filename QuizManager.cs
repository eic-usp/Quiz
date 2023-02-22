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

        public event System.Action OnChooseRight;
        public event System.Action OnChooseWrong;
        
        private CanvasGroup _canvasGroup;
        private QuizOption[] _options;
        private QuizDataItem[] _quizDataItems;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (string.IsNullOrEmpty(questionsFilePath)) return;
            Setup(questionsFilePath);
        }

        public void Setup(string resourcePath)
        {
            var qd = LoadResourceFromJson(resourcePath);
            SetQuestion(qd);
        }

        public QuizDataItem[] LoadResourceFromJson(string resourcePath)
        {
            var json = Resources.Load<TextAsset>(resourcePath);
            return JsonUtility.FromJson<QuizData>(json.text).questions;
        }

        public void SetQuestion(QuizDataItem[] quizDataItems)
        {
            _quizDataItems = quizDataItems;
            RefreshQuestion();
        }

        public void RefreshQuestion()
        {
            var qdi = _quizDataItems[Random.Range(0, _quizDataItems.Length)];

            _options ??= GetComponentsInChildren<QuizOption>();
            
            if (qdi.options.Length != _options.Length)
            {
                Debug.LogError("The number of quiz option buttons must be the same as the number of options in the resource file");
                return;
            }

            questionText.text = qdi.question;

            for (var i = 0; i < _options.Length; i++)
            {
                _options[i].SetAnswer(qdi.options[i]);
            }

            if (qdi.correctOption > _options.Length || qdi.correctOption > qdi.options.Length)
            {
                Debug.LogError($"Invalid option. The correct option must be between 1 and {qdi.options.Length}");
                return;
            }

            _options[qdi.correctOption-1].Correct = true;
            _canvasGroup.interactable = true;
        }

        public Color Choose(bool correct)
        {
            if (correct)
            {
                _canvasGroup.interactable = false;
                OnChooseRight?.Invoke();
                return rightAnswerColor;
            }
            
            OnChooseWrong?.Invoke();
            return wrongAnswerColor;
        }
    }
}
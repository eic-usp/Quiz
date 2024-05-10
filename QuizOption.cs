using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace EIC.Quiz
{
    [AddComponentMenu("EIC/Quiz/QuizOption")]
    [RequireComponent(typeof(Button), typeof(Image))]
    public class QuizOption : MonoBehaviour
    {
        public bool Correct { get; set; }
        public string Answer { get; private set; }
        public Image Image { get; private set; }

        private TextMeshProUGUI _text;

        private Button _button;
        private Color _defaultColor;
        private QuizManager _quizManager;

        private void Awake()
        {
            _button = GetComponent<Button>();
            Image = GetComponent<Image>();
            _defaultColor = Image.color;
            _text = GetComponentInChildren<TextMeshProUGUI>();
            _quizManager = GetComponentInParent<QuizManager>();
            _button.onClick.AddListener(Choose);
        }

        /// <summary>
        /// Set option text
        /// </summary>
        /// <param name="answer"></param>
        
        public void SetAnswer(string answer)
        {
            if (!_text)
            {
                Debug.LogError("Quiz option requires a Text Mesh Pro component as a child");
                return;
            }

            Answer = answer;
            //_text.text = Answer;
            LocalizationManager.SetStringReference(_text.GetComponent<LocalizeStringEvent>(), LocalizationManager.Quiz, Answer);
            Reset();
        }

        /// <summary>
        /// Reset option default properties
        /// </summary>
        
        public void Reset()
        {
            Image.color = _defaultColor;
            _button.interactable = true;
            Correct = false;
        }
        
        /// <summary>
        /// Choose quiz option
        /// </summary>
        
        public void Choose() => _quizManager.Choose(this);
    }
}
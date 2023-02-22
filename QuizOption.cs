using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EIC.Quiz
{
    [AddComponentMenu("EIC/Quiz/QuizOption")]
    [RequireComponent(typeof(Button), typeof(Image))]
    public class QuizOption : MonoBehaviour
    {
        public bool Correct { get; set; }
        public string Answer { get; private set; }

        private TextMeshProUGUI _text;
        private Button _button;
        private Image _image;
        private QuizManager _quizManager;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
            _quizManager = GetComponentInParent<QuizManager>();
            _button.onClick.AddListener(Choose);
        }

        public void SetAnswer(string answer)
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();

            if (!_text)
            {
                Debug.LogError("Quiz option requires a Text Mesh Pro component");
                return;
            }

            Answer = answer;
            _text.text = Answer;
        }

        private void Choose()
        {
            _image.color = _quizManager.Choose(Correct);
            _button.interactable = false;
        }
    }
}
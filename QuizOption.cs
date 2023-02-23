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
        // public Image Image {
        //     get
        //     {
        //         _image ??= GetComponent<Image>();
        //         return _image;
        //     }
        //     private set => _image = value;
        // }
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
            _quizManager = GetComponentInParent<QuizManager>();
            _button.onClick.AddListener(Choose);
        }

        public void SetAnswer(string answer)
        {
            _text ??= GetComponentInChildren<TextMeshProUGUI>();

            if (!_text)
            {
                Debug.LogError("Quiz option requires a Text Mesh Pro component");
                return;
            }

            Answer = answer;
            _text.text = Answer;
            Reset();
        }

        public void Reset()
        {
            Image.color = _defaultColor;
            _button.interactable = true;
        }
        
        private void Choose() => _quizManager.Choose(this);
    }
}
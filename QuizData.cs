namespace EIC.Quiz
{
    [System.Serializable]
    public struct QuizData
    {
        public QuizDataItem[] questions;
    }

    [System.Serializable]
    public class QuizDataItem
    {
        public string question;
        public string[] options;
        public int correctOption;
    }

    [System.Serializable]
    public struct QuizResult
    {
        public int rightAnswers;
        public int wrongAnswers;
        public bool complete;
    }
}
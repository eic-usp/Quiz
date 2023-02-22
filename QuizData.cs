namespace EIC.Quiz
{
    [System.Serializable]
    public struct QuizData
    {
        public QuizDataItem[] questions;
    }

    [System.Serializable]
    public struct QuizDataItem
    {
        public string question;
        public string[] options;
        public int correctOption;
    }
}
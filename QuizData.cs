namespace EIC.Quiz
{
    /// <summary>
    /// Container for QuizDataItem objects
    /// </summary>
    
    [System.Serializable]
    public struct QuizData
    {
        public QuizDataItem[] questions;
    }

    /// <summary>
    /// Info about a question, including its options and which one is correct
    /// </summary>
    
    [System.Serializable]
    public class QuizDataItem
    {
        public string question;
        public string[] options;
        public int correctOption;
    }

    /// <summary>
    /// Info about the quiz result. The <see cref="QuizResult.complete"/> field indicates whether the result is partial or complete.
    /// Also includes counters for right and wrong answers.
    /// </summary>
    
    [System.Serializable]
    public struct QuizResult
    {
        public int rightAnswers;
        public int wrongAnswers;
        public bool complete;
    }
}
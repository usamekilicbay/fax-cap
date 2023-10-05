namespace FaxCap.Entity
{
    public class Question
    {
        public string QuestionText { get; }
        public bool Answer { get; }
        public Category Category { get; }

        public Question(string questionText, bool answer, Category category)
        {
            QuestionText = questionText;
            Answer = answer;
            Category = category;
        }
    }
}

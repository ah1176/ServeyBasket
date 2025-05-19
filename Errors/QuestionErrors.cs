
namespace Survey_Basket.Errors
{
    public static class QuestionErrors
    {
        public static readonly Error QuestionNotFound = new("Not Found", "Question With This Id Wasn't Found", StatusCodes.Status404NotFound);
        public static readonly Error QuestionsNotFound = new("Not Found", "No Items In Questions Table", StatusCodes.Status404NotFound);
        public static readonly Error DuplicateQuestionContent = new("Duplicate Content", "Cannot Insert Duplicate Content", StatusCodes.Status409Conflict);
    }
}

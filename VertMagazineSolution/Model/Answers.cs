using System.Collections.Generic;

namespace VertMagazineSolution.Model
{
    public class Answers
    {
        public Answers()
        {
            shouldBe = new List<string>();
        }
        public string totalTime { get; set; }
        public bool answerCorrect { get; set; }
        public List<string> shouldBe { get; set; }
    }

    public class AnswerRequest
    {
        public AnswerRequest()
        {
            subscribers = new List<string>();
        }
        public List<string> subscribers { get; set; }
    }
}

namespace Eraasoft_Task4.Net
{
    class Question
    {
        public string Qheader { get; set; }
        public string Qbody { get; set; }
        public int Qmarks { get; set; }

        public AnswerList Answers { get; set; } = new AnswerList();

        public Question(string qheader, string qbody, int qmarks)
        {
            Qheader = qheader;
            Qbody = qbody;
            Qmarks = qmarks;
        }

        public void Display()
        {
            Console.WriteLine($"{Qheader}\n{Qbody} (Marks: {Qmarks})");
            for (int i = 0; i < Answers.Count; i++)
            {
                Console.WriteLine($"- {Answers[i].Text}");
            }
        }
    }

    class TrueOrFalseQuestion : Question
    {
        public TrueOrFalseQuestion(string qheader, string qbody, int qmarks, bool isTrueCorrect)
            : base(qheader, qbody, qmarks)
        {
            Answers.Add("True", isTrueCorrect);
            Answers.Add("False", !isTrueCorrect);
        }

        public bool CheckAnswer(string input)
        {
            string trimmedInput = input.Trim();
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers[i].Text.Equals(trimmedInput, StringComparison.OrdinalIgnoreCase) &&
                    Answers[i].IsCorrect)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class ChooseOneOrAllQuestion : Question
    {
        public bool IsMultipleAnswerAllowed { get; set; }

        public ChooseOneOrAllQuestion(string qheader, string qbody, int qmarks, bool isMultipleAnswerAllowed)
            : base(qheader, qbody, qmarks)
        {
            IsMultipleAnswerAllowed = isMultipleAnswerAllowed;
        }

        public new void Display()
        {
            Console.WriteLine($"{Qheader}\n{Qbody} (Marks: {Qmarks})");
            Console.WriteLine(IsMultipleAnswerAllowed ? "Select ALL correct answers (comma-separated):" : "Select ONE correct answer:");
            for (int i = 0; i < Answers.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Answers[i].Text}");
            }
        }

        public List<Answer> GetCorrectAnswers()
        {
            List<Answer> correctAnswers = new List<Answer>();
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Answers[i].IsCorrect)
                {
                    correctAnswers.Add(Answers[i]);
                }
            }
            return correctAnswers;
        }

        public bool CheckAnswer(string input)
        {
            string[] separatedAnswers = input.Split(',');
            List<string> studentAnswers = new List<string>();
            for (int i = 0; i < separatedAnswers.Length; i++)
            {
                studentAnswers.Add(separatedAnswers[i].Trim());
            }

            List<Answer> correctAnswers = GetCorrectAnswers();

            if (studentAnswers.Count != correctAnswers.Count)
            {
                return false;
            }

            for (int i = 0; i < correctAnswers.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < studentAnswers.Count; j++)
                {
                    if (correctAnswers[i].Text.Equals(studentAnswers[j], StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
    }

    class Answer
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }

        public Answer(string text, bool isCorrect)
        {
            Text = text;
            IsCorrect = isCorrect;
        }
    }

    class AnswerList : List<Answer>
    {
        public void Add(string text, bool isCorrect)
        {
            base.Add(new Answer(text, isCorrect));
        }
    }

    class Exam
    {
        public int NumberOfQuestions { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();

        public void StartExam()
        {
            int totalScore = 0;

            for (int i = 0; i < Questions.Count; i++)
            {
                Question question = Questions[i];

                Console.WriteLine();
                if (question is ChooseOneOrAllQuestion)
                {
                    ((ChooseOneOrAllQuestion)question).Display();
                }
                else
                {
                    question.Display();
                }

                Console.Write("Your answer: ");
                string input = Console.ReadLine();

                bool isCorrect = false;

                if (question is TrueOrFalseQuestion)
                {
                    isCorrect = ((TrueOrFalseQuestion)question).CheckAnswer(input);
                }
                else if (question is ChooseOneOrAllQuestion)
                {
                    isCorrect = ((ChooseOneOrAllQuestion)question).CheckAnswer(input);
                }

                if (isCorrect)
                {
                    totalScore += question.Qmarks;
                }
            }

            Console.WriteLine("\nYour total score is: " + totalScore);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===== Teacher Mode: Enter Questions =====");

            Exam exam = new Exam();

            Console.Write("Enter number of questions: ");
            exam.NumberOfQuestions = int.Parse(Console.ReadLine());

            for (int i = 0; i < exam.NumberOfQuestions; i++)
            {
                Console.WriteLine($"\nQuestion {i + 1}:");
                Console.Write("Enter question type (1=True/False, 2=Choose One/All): ");
                int type = int.Parse(Console.ReadLine());

                Console.Write("Enter question header: ");
                string header = Console.ReadLine();

                Console.Write("Enter question body: ");
                string body = Console.ReadLine();

                Console.Write("Enter question marks: ");
                int marks = int.Parse(Console.ReadLine());

                if (type == 1)
                {
                    Console.Write("Is 'True' the correct answer? (y/n): ");
                    bool isTrueCorrect = Console.ReadLine().Trim().ToLower() == "y";

                    var q = new TrueOrFalseQuestion(header, body, marks, isTrueCorrect);
                    exam.Questions.Add(q);
                }
                else if (type == 2)
                {
                    Console.Write("Allow multiple correct answers? (y/n): ");
                    bool multiple = Console.ReadLine().Trim().ToLower() == "y";

                    var q = new ChooseOneOrAllQuestion(header, body, marks, multiple);

                    Console.Write("How many answers to add? ");
                    int answerCount = int.Parse(Console.ReadLine());

                    for (int j = 0; j < answerCount; j++)
                    {
                        Console.Write($"Enter text for answer {j + 1}: ");
                        string text = Console.ReadLine();
                        Console.Write("Is this answer correct? (y/n): ");
                        bool isCorrect = Console.ReadLine().Trim().ToLower() == "y";
                        q.Answers.Add(text, isCorrect);
                    }

                    exam.Questions.Add(q);
                }
            }

            Console.Clear();
            exam.StartExam();
        }
    }
}
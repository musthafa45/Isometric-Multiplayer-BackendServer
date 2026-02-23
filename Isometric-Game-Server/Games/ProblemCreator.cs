using System;
using System.Collections.Generic;

namespace Isometric_Game_Server.Games {
    public class ProblemCreator {
        private Random random = new Random();

        public Problem CreateProblem(Complexity complexity) {
            int min = 1;
            int max = 10;

            switch (complexity) {
                case Complexity.Low:
                    min = 1; max = 10;
                    break;
                case Complexity.Medium:
                    min = 5; max = 30;
                    break;
                case Complexity.Hard:
                    min = 10; max = 100;
                    break;
            }

            int a = random.Next(min, max);
            int b = random.Next(min, max);

            int correct = a + b;

            Problem problem = new Problem();
            problem.Question = $"{a} + {b}";
            problem.Complexity = complexity;

            problem.CorrectIndex = random.Next(0, 4);

            int wrong1 = correct + random.Next(1, 5);
            int wrong2 = correct - random.Next(1, 5);
            int wrong3 = correct + random.Next(6, 10);

            string[] answers = new string[4];

            answers[problem.CorrectIndex] = correct.ToString();

            List<string> wrongAnswers = new List<string> {
                wrong1.ToString(),
                wrong2.ToString(),
                wrong3.ToString()
            };

            int w = 0;
            for (int i = 0; i < 4; i++) {
                if (answers[i] == null) {
                    answers[i] = wrongAnswers[w];
                    w++;
                }
            }

            problem.AnswerA = answers[0];
            problem.AnswerB = answers[1];
            problem.AnswerC = answers[2];
            problem.AnswerD = answers[3];

            return problem;
        }
    }

    public class Problem {
        public int Id;
        public Complexity Complexity;

        public string Question;
        public string AnswerA;
        public string AnswerB;
        public string AnswerC;
        public string AnswerD;

        public int CorrectIndex; // 0=A,1=B,2=C,3=D
    }

    public enum Complexity {
        None,
        Low,
        Medium,
        Hard,
    }
}

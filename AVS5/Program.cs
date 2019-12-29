using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AVS5
{
    class Program
    {
        private const bool SHUFFLETHENTAKE = false;  // true - сначала тесты перемешиваются, потом из них берутся первые n штук. false - сначала из исходного списка берётся n тестов, потом они перемешиваются.
        private const string LOCATION = "avs_demo.txt";
        private static List<Question> questions = new List<Question>();

        //Проверка диапазона включает граничные значения
        static int EnterIntInRange(int min, int max)
        {
            int toReturn;
            while (!int.TryParse(Console.ReadLine(), out toReturn) || !(toReturn >= min && toReturn <= max))
            {
                Console.Write("Неверный ввод, повторите еще раз: ");
            }
            return toReturn;
        }

        public static bool Init()
        {
            Console.OutputEncoding = Encoding.UTF8;
            string[] lines;
            try
            {
                lines = File.ReadAllLines(LOCATION);
            }
            catch (Exception)
            {
                Console.WriteLine($"Файл по пути \"{LOCATION}\" не найден");
                return false;
            }
            for (int i = 0; i < lines.Length; i += 4)
            {
                try
                {
                    questions.Add(new Question { Text = lines[i], Variants = lines[i + 1], RightAnswer = int.Parse(lines[i + 2]) });
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error on question {(i / 4) + 1}, line {i + 1} - {i + 3}");
                    return false;
                }
            }
            if (SHUFFLETHENTAKE)
                questions.Shuffle();
            Console.WriteLine($"Тесты успешно загружены ({questions.Count} шт.)");
            return true;
        }

        static void TestSetup()
        {
            int amountOfTests = 0;
            Console.WriteLine("Нажмите на любую клавишу...");
            Console.ReadKey();
            Console.Clear();
            Console.Write($"Выберите количество вопросов (1-{questions.Count}): ");
            amountOfTests = EnterIntInRange(1, questions.Count);
            List<Question> questionsForTest = questions.Take(amountOfTests).ToList();
            if (!SHUFFLETHENTAKE)
                questionsForTest.Shuffle();
            BeginTest(questionsForTest);
        }

        static void BeginTest(List<Question> questionsForTest)
        {
            Console.WriteLine("Нажмите на любую клавишу для начала теста ...");
            Console.ReadKey();
            Console.Clear();
            for (int i = 0; i < questionsForTest.Count; i++)
            {
                Console.Clear();
                Console.WriteLine($"Вопрос {i + 1} из {questionsForTest.Count}\n");
                Console.WriteLine(questionsForTest[i]);
                Console.Write("Ваш ответ (1-5): ");
                questionsForTest[i].ChosenAnswer = EnterIntInRange(1, 5);
                questionsForTest[i].IsRight = questionsForTest[i].RightAnswer == questionsForTest[i].ChosenAnswer;
            }
            EndTest(questionsForTest);
        }

        static void EndTest(List<Question> questionsForTest)
        {
            Console.Clear();
            int amountOfRhightAnswers = questionsForTest.Where(q => q.IsRight == true).Count();
            Console.WriteLine($"Результат {amountOfRhightAnswers} из {questionsForTest.Count} ({(int)((double)amountOfRhightAnswers / questionsForTest.Count * 100)}%)\n");
            Console.WriteLine("Выберите опцию:");
            Console.WriteLine("1 - Просмотр всех отвеченных вопросов");
            Console.WriteLine("2 - Просмотр вопросов с неверным ответом");
            Console.WriteLine("3 - Выход");
            int option = EnterIntInRange(1, 3);
            switch (option)
            {
                case 1:
                    {
                        Console.Clear();
                        foreach (Question q in questionsForTest)
                        {
                            if (!q.IsRight)
                                Console.WriteLine("\n*************WRONG*************");
                            else
                                Console.WriteLine("\n*******************************");
                            Console.WriteLine(q);
                            Console.WriteLine($"\nПравльный ответ: {q.RightAnswer}\nВаш ответ: {q.ChosenAnswer}\n");
                        }
                        Console.WriteLine("Нажмите любую клавишу для выхода");
                        Console.ReadKey();
                        break;
                    }
                case 2:
                    {
                        Console.Clear();
                        if (questionsForTest.Where(qu => qu.IsRight == false).Count() == 0)
                        {
                            Console.WriteLine("Empty list");
                            break;
                        }
                        foreach (Question q in questionsForTest.Where(qu => qu.IsRight == false))
                        {
                            Console.WriteLine("\n*******************************");
                            Console.WriteLine(q);
                            Console.WriteLine($"Правльный ответ: {q.RightAnswer}\nВаш ответ: {q.ChosenAnswer}\n");
                        }
                        Console.WriteLine("Нажмите любую клавишу для выхода");
                        Console.ReadKey();
                        break;
                    }
                case 3:
                    {
                        Console.Clear();
                        break;
                    }
            }
        }


        static void Main(string[] args)
        {
            if (Init())
                TestSetup();
            else
            {
                Console.WriteLine("Нажмите любую клавишу для выхода");
                Console.ReadKey();
            }
        }
    }
}

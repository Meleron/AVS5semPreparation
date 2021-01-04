using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#pragma warning disable CS0162

namespace AVS5
{
    class Program
    {
        //######################
        //#                    #
        //#                    #
        //#     НАСТРОЙКИ      #
        //#                    #
        //#                    #
        //######################

        /*
        * Как работает SHUFFLETHENTAKE?
        * Пусть у нас есть последовательность чисел (номера вопросов) 123456789
        * Допустим мы хотим потренеровать 5 вопросов
        * Если SHUFFLETHENTAKE = true, то произойдет следующее: 123456789 ---(перемешиваем вопросы)---> 476592138 ---(берём 5 штук)---> 47659 - пул вопросов, которые будут появляться.
        * Если SHUFFLETHENTAKE = false, то произойдет следующее: 123456789 ---(берём 5 штук)---> 12345 ---(перемешиваем)---> 43125 - пул вопросов, которые будут появляться.
        * 
        * Если мы хотим, допустим, потренеровать с 20 по 80 вопрос, то делаем следующее:
        * Устанавливаем SHUFFLETHENTAKE в false, FIRSTQUESTION приравниваем числу 20. Затем, в программе, указываем, что нам нужно 60 вопросов (80-20).
        */

        private const bool SHUFFLETHENTAKE = true;  // true - сначала все вопросы перемешиваются, потом из них берутся первые n штук. false - сначала из исходного упорядоченного списка берётся n тестов, потом они перемешиваются.
        private const bool SHOWRESULINSTANT = true;  //  true - результат ответа показывается сразу, после его введения. false - показывается только итоговый результат в конце теста. 
        private const int FIRSTQUESTION = 0;  //  Номер вопроса с которого будет начинаться отбор тестов. Следует использовать, если хотите прорешать определённый вариант. Работает, если SHUFFLETHENTAKE установлен в false.
        private const string LOCATION = "avs_demo.txt";  //  Расположение файла с вопросами
        public const bool RANDOMIZEANSWERS = true;  //  true - варианты ответов распологаются в случайном порядке. false - варианты ответов стоят на одном месте



        private static List<Question> questions = new List<Question>();

        static void PrintSettings()
        {
            Console.WriteLine("\n***ТЕКУЩИЕ НАСТРОЙКИ***\n");
            Console.WriteLine($"SHUFFLETHENTAKE = {SHUFFLETHENTAKE} (перемешать все тесты перед тем, как выбрать n штук)");
            Console.WriteLine($"SHOWRESULINSTANT = {SHOWRESULINSTANT} (мгновенное отображать правильность ответа на вопрос)");
            Console.WriteLine($"RANDOMIZEANSWERS = {RANDOMIZEANSWERS} (рандомизация вариантов ответа)");
            if(!SHUFFLETHENTAKE)
                Console.WriteLine($"FIRSTQUESTION = {FIRSTQUESTION} (пропустить заданное количество вопросов)");
            Console.WriteLine($"\nНастроить данные параметры и прочитать более точное описание можно в начале программы\n");

        }

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
            PrintSettings();
            return true;
        }

        static void TestSetup()
        {
            int amountOfTests = 0;
            Console.WriteLine("Нажмите на любую клавишу...");
            Console.ReadKey();
            Console.Clear();
            if (!SHUFFLETHENTAKE)
                Console.Write($"Выберите количество вопросов (1-{questions.Count - FIRSTQUESTION}) (пропущено {FIRSTQUESTION} вопросов): ");
            else
                Console.Write($"Выберите количество вопросов (1-{questions.Count}): ");
            amountOfTests = EnterIntInRange(1, questions.Count);
            if(!SHUFFLETHENTAKE)
                Console.WriteLine($"\nБудут использованы вопросы №{FIRSTQUESTION + 1} - {FIRSTQUESTION + amountOfTests} из исходного списка\n");
            else
                Console.WriteLine($"\nВопросы будут выбраны из всего списка\n");
            List<Question> questionsForTest;
            if (SHUFFLETHENTAKE)
                questionsForTest = questions.Take(amountOfTests).ToList();
            else
                questionsForTest = questions.Skip(FIRSTQUESTION).Take(amountOfTests).ToList();
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
                if (SHOWRESULINSTANT)
                {
                    Console.WriteLine();
                    if (questionsForTest[i].IsRight)
                    {
                        Console.WriteLine("Правильно");
                    }
                    else
                    {
                        Console.WriteLine("Неправильно");
                        Console.WriteLine($"Правильный ответ: {questionsForTest[i].RightAnswer}");
                    }
                    Console.WriteLine("\nНажмите на любую клавишу...");
                    Console.ReadKey();
                }
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
                            Console.WriteLine($"\nПравильный ответ: {q.RightAnswer}\nВаш ответ: {q.ChosenAnswer}\n");
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
                            Console.WriteLine($"Правильный ответ: {q.RightAnswer}\nВаш ответ: {q.ChosenAnswer}\n");
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

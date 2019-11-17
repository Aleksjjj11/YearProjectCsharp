using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using SemestreProject.Snake;

namespace SemestreProject
{
    class Game
    {
        static string nameUser = "Anonymous";
        static string fileScore = "Score.txt";
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Write("Перед началом игры введите ваш ник: ");
            nameUser = Console.ReadLine();
            nameUser = nameUser.Replace("-", "");
            //For some reason need wrote Snake for correctly work.
            GameField gameField = new GameField();
            Snake.Snake snake = new Snake.Snake();
            Fruit fruit = new Fruit();
            Menu(snake, fruit, gameField);
        }

        static void Menu(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            Console.Clear();
            Console.WriteLine($"\n\t\t\t1) Новая игра\n" +
                              $"\t\t\t2) Продолжить игру\n" +
                              $"\t\t\t3) Просмотр всей таблицы рекордов\n" +
                              $"\t\t\t4) Просмотр своих рекордов\n" +
                              $"\t\t\t0) Выход\n");
            ConsoleKeyInfo key;
            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.D0:
                {
                    Console.CursorVisible = true;
                    Environment.Exit(0);
                    break;    
                }
                case ConsoleKey.D1:
                {
                    StartGame(snake, fruit, gameField);
                    Play(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D2:
                {
                    if (gameField.Upload(snake, fruit))
                    {
                        gameField.Render();
                        Play(snake, fruit, gameField);    
                    }
                    else
                    {
                        Console.WriteLine("Для продолжения нажмите любую клавишу");
                        Console.ReadKey();
                        Menu(snake, fruit, gameField);
                    }
                    break;
                }
                case ConsoleKey.D3:
                {
                    ShowScoreTable(false);
                    Console.WriteLine("Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    Menu(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D4:
                {
                    ShowScoreTable(true);
                    Console.WriteLine("Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    Menu(snake, fruit, gameField);
                    break;
                }
                default:
                {
                    StartGame(snake, fruit, gameField);
                    break;
                }
            };
        }
        static void Play(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            bool isLose = false;
            while (!gameField.IsWin() && !isLose)
            {
                gameField.ReadAction(snake, fruit);
                if (snake.moveStatus is MoveStatus.Moving)
                {
                    isLose = gameField.Tick(snake, fruit);
                }
            }
            SaveScore(gameField);
            if (isLose) 
                Console.WriteLine("\nGame over!\nYou lose.");
            else
                Console.WriteLine("Game finished! You won!! Incredible!!");
            Console.WriteLine("Для продолжения нажмите любую клавишу + Enter");
            Console.ReadLine();
            Menu(snake, fruit, gameField);
        }
        static void ShowScoreTable(bool isPerson = false)
        {
            List<string> list = LoadRecords(isPerson);
            if (list is null)
            {
                Console.WriteLine("В данный момент список рекордов пуст.");
                return;
            }
            foreach (string record in list)
            {
                Console.WriteLine(record);
            }
        }

        static void SaveScore(GameField gameField)
        {
            StreamWriter fileWriter = new StreamWriter(fileScore, true);
            fileWriter.WriteLine(nameUser + "-" + gameField.GetScore());
            fileWriter.Close();
        }

        static void EnterSizeField(GameField gameField)
        {
            int height, width;
            try
            {
                Console.WriteLine("Введите высоту поля");
                height = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Введите ширину поля");
                width = Convert.ToInt32(Console.ReadLine());
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошбика: {e.Message}\n" +
                                  "Некорректное значение высоты/ширины.\n" +
                                  "Были выставлены стандартные значения 10х15.");
                height = 10;
                width = 15;
                Console.WriteLine("Для продолжения нажмите любую клавишу + Enter");
                Console.ReadLine();
            }
            gameField.SetHeight(height);
            gameField.SetWidth(width);
        }

        static void StartGame(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            snake = new Snake.Snake();
            fruit = new Fruit();
            gameField = new GameField();
            EnterSizeField(gameField);
            fruit.NewPosition(gameField);
            gameField.AddSnake(snake);
            gameField.AddGameObject(fruit);
            
            gameField.Render();
            Play(snake, fruit, gameField);
        }
        static List<string> LoadRecords(bool isPerson = false)
        {
            List<string> result = new List<string>();
            StreamReader fileReader = new StreamReader(fileScore);
            int numLine = 0;
            string line;
            while ((line = fileReader.ReadLine()) != null)
            {
                if (isPerson)
                {
                    if (line.Split("-").Length != 2) return null;
                    if (line.Split("-")[0] == nameUser)
                    {
                        result.Add(line);
                        numLine++;
                    }    
                }
                else
                {
                    if (line.Split("-").Length != 2) return null;
                    result.Add(line);
                    numLine++;
                }
            }
            if (numLine is 0) return null;
            fileReader.Close();
            return result;
        }
    }
}
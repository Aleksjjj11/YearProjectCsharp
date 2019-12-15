using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Timers;
using SemestreProject.Snake;
using Timer = System.Timers.Timer;

namespace SemestreProject
{
    class Game
    {
        private static string _nameUser = "Anonymous";
        private const string Filescore = "Score.txt";
        private static Timer _tickTimer;
        private static GameField _gameField;
        private static Snake.Snake _snake;
        private static Fruit _fruit;
        private static bool _isLose;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Write("Перед началом игры введите ваш ник: ");
            _nameUser = Console.ReadLine();
            _nameUser = _nameUser?.Replace("-", "");
            //For some reason need wrote Snake for correctly work.
            _gameField = new GameField();
            _snake = new Snake.Snake();
            _fruit = new Fruit();
            Menu(_snake, _fruit, _gameField);
        }

        static void SetTimer(double interval = 200)
        {
            _tickTimer = new Timer(interval);
            _tickTimer.Elapsed += TickTimerOnElapsed;
            _tickTimer.AutoReset = true;
            _tickTimer.Enabled = true;
        }

        private static void TickTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_snake.moveStatus is MoveStatus.Moving)
            {
                _isLose = _gameField.Tick(_snake, _fruit);
            }
            ((Timer) sender).Interval = _snake.speed;
        }

        static public void Menu(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine($"\n\n\t\t\t1) Новая игра\n" +
                                  $"\n\n\t\t\t2) Просмотр всей таблицы рекордов\n" +
                                  $"\n\n\t\t\t3) Просмотр своих рекордов\n" +
                                  $"\n\n\t\t\t0) Выход\n");
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
                        StartGame();
                        Play();
                        break;
                    }
                    case ConsoleKey.D2:
                    {
                        ShowScoreTable(false);
                        Console.WriteLine("Для продолжения нажмите любую клавишу");
                        Console.ReadKey();
                        break;
                    }
                    case ConsoleKey.D3:
                    {
                        ShowScoreTable(true);
                        Console.WriteLine("Для продолжения нажмите любую клавишу");
                        Console.ReadKey();
                        break;
                    }
                }
            } while (key.Key != ConsoleKey.Backspace);
        }

        static void TestThread()
        {
            while (true)
            {
                _gameField.ReadAction(_snake, _fruit);
            }
        }
        static void Play()
        {
            _isLose = false;
            SetTimer(_snake.speed);
            _tickTimer.Enabled = true;
            Thread actionThread = new Thread(TestThread);
            actionThread.IsBackground = true;
            actionThread.Start();
            while (!_gameField.IsWin() && !_isLose)
            {
                
            }
            _tickTimer.Stop();
            _tickTimer.Enabled = false;
            SaveScore();
            if (_isLose) 
                Console.WriteLine("\nGame over!\nYou lose.");
            else
                Console.WriteLine("Game finished! You won!! Incredible!!");
            Console.WriteLine("Для продолжения нажмите любую клавишу + Enter");
            Console.ReadLine();
            Menu(_snake, _fruit, _gameField);
        }
        static void ShowScoreTable(bool isPerson = false)
        {
            List<string> list = LoadRecords(isPerson);
            SortScore(list);
            if (list is null)
            {
                Console.WriteLine("В данный момент список рекордов пуст.");
                return;
            }

            int num = 1;
            foreach (string record in list)
            {
                Console.WriteLine($"\t{num}. " + record);
                num++;
            }
        }

        static void SaveScore()
        {
            StreamWriter fileWriter = new StreamWriter(Filescore, true);
            fileWriter.WriteLine(_nameUser + "-" + _gameField.GetScore());
            fileWriter.Close();
        }

        static void SortScore(List<string> records)
        {
            try
            {
                for (int i = 0; i < records.Count; i++)
                for (int j = 0; j < records.Count - 1; j++)
                {
                    int scoreFirst = Convert.ToInt32(records[j].Split("-")[1]);
                    int scoreSecond = Convert.ToInt32(records[j+1].Split("-")[1]);
                    if (scoreFirst < scoreSecond)
                    {
                        string med = records[j];
                        records[j] = records[j + 1];
                        records[j + 1] = med;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
        static void EnterSizeField()
        {
            int height, width;
            try
            {
                Console.WriteLine("Введите высоту и ширину поля");
                string values = Console.ReadLine();
                if (values?.Split(" ").Length > 1)
                {
                    height = Convert.ToInt32(values.Split(" ")[0]);
                    width = Convert.ToInt32(values.Split(" ")[1]);
                }
                else
                {
                    height = Convert.ToInt32(values);    
                    width = Convert.ToInt32(Console.ReadLine());
                }
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
            _gameField.SetHeight(height);
            _gameField.SetWidth(width);
        }

        static void StartGame()
        {
            _snake = new Snake.Snake();
            _fruit = new Fruit();
            _gameField = new GameField();
            EnterSizeField();
            _fruit.NewPosition(_gameField);
            _gameField.AddSnake(_snake);
            _gameField.AddGameObject(_fruit);
            
            _gameField.Render();
            Play();
        }
        static List<string> LoadRecords(bool isPerson = false)
        {
            List<string> result = new List<string>();
            StreamReader fileReader = new StreamReader(Filescore);
            int numLine = 0;
            string line;
            while ((line = fileReader.ReadLine()) != null)
            {
                if (isPerson)
                {
                    if (line.Split("-").Length != 2) return null;
                    if (line.Split("-")[0] == _nameUser)
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
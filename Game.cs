using System;
using System.IO;
using SemestreProject.Snake;

namespace SemestreProject
{
    class Game
    {
        static private string nameUser = "Anonymous";

        static void Main(string[] args)
        {
            Console.Write("Перед началом игры введите ваш ник: ");
            nameUser = Console.ReadLine();
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
                    Environment.Exit(0);
                    break;    
                }
                case ConsoleKey.D1:
                {
                    StartGame(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D2:
                {
                    gameField.Upload(snake, fruit);
                    gameField.Render();
                    Play(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D3:
                {
                    ShowAllScoreTable();
                    Console.WriteLine("Для продолжения нажмите любую клавишу");
                    Console.ReadKey();
                    Menu(snake, fruit, gameField);
                    break;
                }
                case ConsoleKey.D4:
                {
                    ShowUserScoreTable();
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
        static public void Play(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            bool isLose = false;
            while (!gameField.IsWin() && !isLose)
            {
                //reading action
                gameField.ReadAction(snake, fruit);
                if (snake.moveStatus is MoveStatus.Moving)
                {
                    gameField.Render();
                    isLose = gameField.Tick(snake, fruit);
                    gameField.Render();
                }
            }
            SaveScore(gameField);
            if (isLose) 
                Console.WriteLine("Game over!\nYou lose.");
            else
                Console.WriteLine("Game finished! You won!! Incredible!!");
            Menu(snake, fruit, gameField);
        }
        static void ShowAllScoreTable()
        {
            //Отображение таблицы рекордов
            string fileName = "Score.txt";
            StreamReader fileReader = new StreamReader(fileName);
            int numLine = 0;
            Console.WriteLine("Таблица рекордов");
            string line;
            while ((line = fileReader.ReadLine()) != null)
            {
                Console.WriteLine($"{numLine+1}. {line}");
                numLine++;
            }
            if (numLine is 0) Console.WriteLine("Ещё нет рекордов");
            fileReader.Close();
        }

        static void ShowUserScoreTable()
        {
            string fileName = "Score.txt";
            StreamReader fileReader = new StreamReader(fileName);
            int numLine = 0;
            Console.WriteLine("Ваши рекорды:");
            string line;
            while ((line = fileReader.ReadLine()) != null)
            {
                if (line.Split("-")[0] == nameUser)
                {
                    Console.WriteLine($"{numLine+1}. {line}");
                    numLine++;
                }
            }
            if (numLine is 0) Console.WriteLine("У вас ещё нет рекордов");
            fileReader.Close();
        }

        static void SaveScore(GameField gameField)
        {
            string fileName = "Score.txt";
            StreamWriter fileWriter = new StreamWriter(fileName, true);
            fileWriter.WriteLine(nameUser + "-" + gameField.GetScore());
            fileWriter.Close();
        }

        static void EnterSizeField(GameField gameField)
        {
            Console.WriteLine("Введите высоту поля");
            int height, width;
            height = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Введите ширину поля");
            width = Convert.ToInt32(Console.ReadLine());
            gameField.SetHeight(height);
            gameField.SetWidth(width);
        }

        static void StartGame(Snake.Snake snake, Fruit fruit, GameField gameField)
        {
            gameField = new GameField();
            EnterSizeField(gameField);
            snake = new Snake.Snake();
            fruit = new Fruit();
            fruit.NewPosition(gameField.GetHeight(), gameField.GetWidth());
            gameField.AddSnake(snake);
            gameField.AddGameObject(fruit);
            
            gameField.Render();
            Play(snake, fruit, gameField);
        }
    }
}
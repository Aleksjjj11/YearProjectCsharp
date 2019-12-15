using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace SemestreProject.Snake
{
    public class GameField
    {
        private int _height;
        private int _width;
        private int _score;
        private Cell[,] _fields;
        public GameField(int heightInput = 10, int widthInput = 15)
        {
            _score = 0;
            _height = heightInput < 10 ? 10 : heightInput;
            _width = widthInput < 15 ? 15 : widthInput;
            _fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    _fields[i,j] = new Cell(i,j);
        }

        public Cell GetCell(int x, int y)
        {
            return _fields[y, x];
        }
        public void SetHeight(int value)
        {
            _height = value < 10 ? 10 : value;
            _fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    _fields[i,j] = new Cell(i,j);
        }
        public void SetWidth(int value)
        {
            _width = value < 15 ? 15 : value;
            _fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
                for (int j = 0; j < _width; j++)
                    _fields[i,j] = new Cell(i,j);
        }

        public int GetScore()
        {
            return _score;
        }
        public int GetHeight()
        {
            return _height;
        }

        public int GetWidth()
        {
            return _width;
        }

        public void Render()
        {
            String oldBuffer = " ";
            String newBuffer = "";
            Console.Clear();
            newBuffer += $"\nYour score {_score}\n";
            for (int i = 0; i < _width + 1; i++)
            {
                newBuffer += "#";
            }
            newBuffer += "\t\tEsp - Pause\n";
            for (int i = 0; i < _height; i++)
            {
                newBuffer += "#"; 
                for (int j = 0; j < _width; j++)
                {
                    if (_fields[i, j].obj == null)
                    {
                        newBuffer += " ";
                        continue;
                        
                    }
                    newBuffer += _fields[i, j].obj.GetSymbol();
                }
                newBuffer += "#\n";
            }

            for (int i = 0; i < _width + 1; i++)
            {
                newBuffer += "#";
            }

            oldBuffer = oldBuffer.Replace(oldBuffer, newBuffer);
            Console.Write(oldBuffer);
        }

        public bool IsWin()
        {
            return _score >= (_height * _width - 1) * 10;
        }

        public void AddGameObject(GameObject gameObject)
        {
            Console.WriteLine(gameObject.GetType().Name);
            if (gameObject.GetCell() is null)
            {
                gameObject.SetCell(_fields[0,0]);
                _fields[0, 0].obj = gameObject;
            }
            
            _fields[gameObject.GetCell().Y, gameObject.GetCell().X].obj = gameObject;
        }

        public void AddSnake(Snake snake)
        {
            AddGameObject(snake.GetHead());
            foreach (Tail tail in snake.GetTail())
            {
                AddGameObject(tail);
            }
        }

        public void ReadAction(Snake snake, Fruit fruit)
        {
            ConsoleKey key;
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey(false).Key;

                switch (key)
                {
                    case ConsoleKey.A:
                    {
                        snake.SwapVector(TypeVector.Left);
                        snake.moveStatus = MoveStatus.Moving;
                        break;
                    }
                    case ConsoleKey.W:
                    {
                        snake.SwapVector(TypeVector.Up);
                        snake.moveStatus = MoveStatus.Moving;
                        break;
                    }
                    case ConsoleKey.D:
                    {
                        snake.SwapVector(TypeVector.Right);
                        snake.moveStatus = MoveStatus.Moving;
                        break;
                    }
                    case ConsoleKey.S:
                    {
                        snake.SwapVector(TypeVector.Down);
                        snake.moveStatus = MoveStatus.Moving;
                        break;
                    }
                    case ConsoleKey.Escape:
                    {
                        snake.moveStatus = MoveStatus.Stopping;
                        break;
                    }
                    case ConsoleKey.R:
                    {
                        Reset(snake, fruit);
                        AddGameObject(fruit);
                        AddSnake(snake);
                        Render();
                        break;
                    }
                    case ConsoleKey.M:
                    {
                        Game.Menu(snake, fruit, this);
                        break;   
                    }
                }
            }
        }

        private void Reset(Snake snake, Fruit fruit)
        {
            snake.Reset();
            _fields = new Cell[_height, _width];
            for (int i = 0; i < _height; i++)
            for (int j = 0; j < _width; j++)
                _fields[i,j] = new Cell(i,j);
            fruit.NewPosition(this);
            _score = 0;
        }
        public bool Tick(Snake snake, Fruit fruit)
        {
            if (snake.moveStatus is MoveStatus.Stopping) return false;
            TypeVector needSave = TypeVector.Down;
            bool need = false;
            while (snake.StackVectors.Count > 0)
            {
                TypeVector vector = snake.StackVectors.Pop();
                if (!snake != vector)
                {
                    snake.currentVector = vector;
                    break;
                }
                need = true;
                needSave = vector;
            }
            snake.StackVectors.Clear();

            if (need)
                snake.StackVectors.Push(needSave);
            

            if (snake.IsObstecle(this))
            {
                return true;
            }
            
            snake.Move(this);
            AddSnake(snake);
            if (snake.IsFruit(fruit))
            {
                _score += 10;
                if (snake.speed > 100) snake.UpSpeed();
                snake.GetTail().Add(snake.GetTail().Count is 0
                    ? new Tail(snake.GetHead().GetCell())
                    : new Tail(snake.GetTail().Last().GetCell()));
                do
                {
                    fruit.NewPosition(this);
                } while (fruit.GetCell().obj != null);
                AddGameObject(fruit);
            }
            Render();
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace SemestreProject.Snake
{
    public class Snake : BaseObject
    {
        private Head head;
        private List<Tail> tails;
        public TypeVector currentVector { get; set; }
        public Stack<TypeVector> StackVectors = new Stack<TypeVector>();
        public int speed { get; set; }
        public MoveStatus moveStatus { get; set; }

        public Snake(Cell value = null)
        {
            speed = 200;
            head = new Head(value);
            tails = new List<Tail>();
            currentVector = TypeVector.Right;
        }

        public static TypeVector operator !(Snake obj)
        {
            if (obj.currentVector == TypeVector.Down) return TypeVector.Up;
            if (obj.currentVector == TypeVector.Left) return TypeVector.Right;
            if (obj.currentVector == TypeVector.Right) return TypeVector.Left;
            if (obj.currentVector == TypeVector.Up) return TypeVector.Down;
            return TypeVector.Down;
        }
        public Head GetHead()
        {
            return head;
        }

        public List<Tail> GetTail()
        {
            return tails;
        } 
        //Проверяет наткнётся ли змейка на препятствие, следующим своим перемещением 
        public bool IsObstecle(GameField gameField)
        {
            switch (currentVector)
            {
                case TypeVector.Right:
                {
                    if (head.GetCell().X + 1 == gameField.GetWidth()) return true;
                    if (gameField.GetCell(head.GetCell().X + 1, head.GetCell().Y).obj != null
                        && gameField.GetCell(head.GetCell().X + 1, head.GetCell().Y).obj.GetType().Name ==
                        "Tail")
                        return true;
                    break;   
                }
                case TypeVector.Left:
                {
                    if (head.GetCell().X == 0) return true;
                    if (gameField.GetCell(head.GetCell().X - 1, head.GetCell().Y).obj != null
                        && gameField.GetCell(head.GetCell().X - 1, head.GetCell().Y).obj.GetType().Name ==
                        "Tail")
                        return true;
                    break;
                }
                case TypeVector.Up:
                {
                    if (head.GetCell().Y == 0) return true;
                    if (gameField.GetCell(head.GetCell().X, head.GetCell().Y - 1).obj != null
                        && gameField.GetCell(head.GetCell().X, head.GetCell().Y - 1).obj.GetType().Name ==
                        "Tail")
                        return true;
                    break;
                }
                case TypeVector.Down:
                {
                    if (head.GetCell().Y + 1 == gameField.GetHeight()) return true;
                    if (gameField.GetCell(head.GetCell().X, head.GetCell().Y + 1).obj != null
                        && gameField.GetCell(head.GetCell().X, head.GetCell().Y + 1).obj.GetType().Name ==
                        "Tail")
                        return true;
                    break;
                }
            }
            return false;
        }

        public bool IsFruit(Fruit fruit)
        {
            if (head.GetCell() == fruit.GetCell())
                return true;
            return false;
        }

        public void SwapVector(TypeVector typeVector)
        {
            //newVector = typeVector;
            StackVectors.Push(typeVector);
        }

        public void Move(GameField gameField)
        {
            this.GetHead().GetCell().obj = null;
            Cell prevCell = head.GetCell();
            switch (currentVector)
            {
                case TypeVector.Down:
                {
                    Cell oldCell = head.GetCell();
                    head.SetCell(gameField.GetCell(oldCell.X, oldCell.Y + 1));
                    oldCell.obj = null;
                    break;
                }
                case TypeVector.Left:
                {
                    Cell oldCell = head.GetCell();
                    head.SetCell(gameField.GetCell(oldCell.X - 1, oldCell.Y));
                    oldCell.obj = null;
                    break;
                }
                case TypeVector.Right:
                {
                    Cell oldCell = head.GetCell();
                    head.SetCell(gameField.GetCell(oldCell.X + 1, oldCell.Y));
                    oldCell.obj = null;
                    break;
                }
                case TypeVector.Up:
                {
                    Cell oldCell = head.GetCell();
                    head.SetCell(gameField.GetCell(oldCell.X, oldCell.Y - 1));
                    oldCell.obj = null;
                    break;
                }
            }
            if (this.GetTail().Count != 0)
            {
                this.GetTail().Last().GetCell().obj = null;
                for (int i = 0; i < tails.Count; i++)
                {
                    Cell prev2Cell = tails[i].GetCell();
                    tails[i].SetCell(prevCell);
                    prevCell = prev2Cell;
                }
            }
        }

        public void Reset()
        {
            head = new Head();
            tails = new List<Tail>();
            speed = 200;
            currentVector = TypeVector.Right;
            moveStatus = MoveStatus.Stopping;
        }
        public void UpSpeed()
        {
            speed = speed > 65 ? speed - 5 : 65;
        }
    }
}
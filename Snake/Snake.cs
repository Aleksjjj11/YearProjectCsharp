using System.Collections.Generic;
using System.Linq;

namespace SemestreProject.Snake
{
    public class Snake : BaseObject
    {
        private Head head;
        private List<Tail> tails;
        public TypeVector vector { get; set; }
        public int speed { get; set; }
        public MoveStatus moveStatus { get; set; }

        public Snake()
        {
            speed = 200;
            head = new Head();
            tails = new List<Tail>();
            vector = TypeVector.Right;
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
            switch (vector)
            {
                case TypeVector.Right:
                {
                    if (this.GetHead().GetPositionX() + 1 == gameField.GetWidth()) return true;
                    if (gameField.GetObjectsInTail(this.GetHead().GetPositionX() + 1, this.GetHead().GetPositionY()) !=
                        null &&
                        gameField.GetObjectsInTail(this.GetHead().GetPositionX() + 1, this.GetHead().GetPositionY())
                            .GetType().Name == "Tail")
                        return true;
                    break;   
                }
                case TypeVector.Left:
                {
                    if (this.GetHead().GetPositionX() == 0) return true;
                    if (gameField.GetObjectsInTail(this.GetHead().GetPositionX() - 1, this.GetHead().GetPositionY()) !=
                        null &&
                        gameField.GetObjectsInTail(this.GetHead().GetPositionX() - 1, this.GetHead().GetPositionY())
                            .GetType().Name == "Tail")
                        return true;
                    break;
                }
                case TypeVector.Up:
                {
                    if (this.GetHead().GetPositionY() == 0) return true;
                    if (gameField.GetObjectsInTail(this.GetHead().GetPositionX(), this.GetHead().GetPositionY() - 1) !=
                        null &&
                        gameField.GetObjectsInTail(this.GetHead().GetPositionX(), this.GetHead().GetPositionY() - 1)
                            .GetType().Name == "Tail")
                        return true;
                    break;
                }
                case TypeVector.Down:
                {
                    if (this.GetHead().GetPositionY() + 1 == gameField.GetHeight()) return true;
                    if (gameField.GetObjectsInTail(this.GetHead().GetPositionX(), this.GetHead().GetPositionY() + 1) !=
                        null &&
                        gameField.GetObjectsInTail(this.GetHead().GetPositionX(), this.GetHead().GetPositionY() + 1)
                            .GetType().Name == "Tail")
                        return true;
                    break;
                }
            }
            return false;
        }

        public bool IsFruit(Fruit fruit)
        {
            if (this.GetHead().GetPositionX() == fruit.GetPositionX() && this.GetHead().GetPositionY() == fruit.GetPositionY())
                return true;
            return false;
        }

        public void SwapVector(TypeVector typeVector)
        {
            vector = typeVector;
        }

        public void Move()
        {
            if (moveStatus is MoveStatus.Stopping) return;
            
            int prevX = head.GetPositionX(), prevY = head.GetPositionY();
            for (int i = 0; i < tails.Count; i++)
            {
                int prev2X = tails[i].GetPositionX(), prev2Y = tails[i].GetPositionY();
                
                tails[i].SetPositionX(prevX);
                tails[i].SetPositionY(prevY);
                prevX = prev2X;
                prevY = prev2Y;
            }
            switch (vector)
            {
                case TypeVector.Down:
                {
                    this.head.SetPositionY(this.head.GetPositionY()+1);
                    break;
                }
                case TypeVector.Left:
                {
                    this.head.SetPositionX(this.head.GetPositionX()-1);
                    break;
                }
                case TypeVector.Right:
                {
                    this.head.SetPositionX(this.head.GetPositionX()+1);
                    break;
                }
                case TypeVector.Up:
                {
                    this.head.SetPositionY(this.head.GetPositionY()-1);
                    break;
                }
            }
            
        }

        public void UpSpeed()
        {
            speed = speed > 50 ? speed - 10 : 50;
        }
    }
}
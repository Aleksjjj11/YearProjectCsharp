namespace SemestreProject.Snake
{
    public class Tail : GameObject
    {
        public Tail()
        {
            cell = null;
        }
        public Tail(Cell value)
        {
            cell = value;
        }
        public override char GetSymbol()
        {
            return '*';
        }
    }
}
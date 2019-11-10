namespace SemestreProject.Snake
{
    public class Cell : BaseObject
    {
        public int X;
        public int Y;
        public GameObject obj;

        public Cell(int y, int x)
        {
            X = x;
            Y = y;
            obj = null;
        }
    }
}
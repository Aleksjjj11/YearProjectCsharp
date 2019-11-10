namespace SemestreProject.Snake
{
    public class Head : GameObject
    {
        public Head()
        {
            cell = null;
        }

        public Head(Cell value)
        {
            cell = value;
        }
    }
}
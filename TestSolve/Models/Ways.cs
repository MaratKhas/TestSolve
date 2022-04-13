namespace TestSolve.Models
{
    public class Ways
    {
        public int[,] way { get; set; }

        public Ways(int countStudent)
        {
            way = new int[countStudent, countStudent];     
        }
    }
}

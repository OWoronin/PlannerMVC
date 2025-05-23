namespace Pz_Proj_11_12.Models
{
    public class Planner
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Day> Days { get; set; } = new List<Day>();

    }
}

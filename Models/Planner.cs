namespace Pz_Proj_11_12.Models
{
    public class Planner
    {
        public int Id { get; set; }
        public string Name { get; set; }

        //add createdDate field? ?? 
        public ICollection<Day> Days { get; set; } = new List<Day>();
        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}

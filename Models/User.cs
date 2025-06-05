using System.ComponentModel.DataAnnotations;

namespace Pz_Proj_11_12.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; } //need to split: password + salt
        public ICollection<Planner> Planners { get; set; } = new List<Planner>();
    }
}

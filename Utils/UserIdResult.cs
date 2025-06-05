using Microsoft.AspNetCore.Mvc;

namespace Pz_Proj_11_12.Utils
{
    public class UserIdResult
    {
        public int? UserId { get; set; }

        public IActionResult Redirect { get; set; } = null;

        public bool IsRedirect => Redirect != null;
    }
}

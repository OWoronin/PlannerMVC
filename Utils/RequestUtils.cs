namespace Pz_Proj_11_12.Utils
{
    public static class RequestUtils
    {
        public static bool IsFromHomeController(string referrer)
        {
            if (!string.IsNullOrEmpty(referrer))
            {
                Uri uri = new(referrer);
                string previousPath = uri.AbsolutePath.ToLower();

                if (previousPath.EndsWith("/"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

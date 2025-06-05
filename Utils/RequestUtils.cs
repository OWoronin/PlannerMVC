using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Pz_Proj_11_12.Utils
{
    public static class RequestUtils
    {

        public static void GenerateBackData(string referrerUrl, ITempDataDictionary tempData)
        {
            if (string.IsNullOrWhiteSpace(referrerUrl))
                return;

            var array = referrerUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (array.Length < 3)
                return;

            tempData["Controller"] = array[^3];
            tempData["Action"] = array[^2];
            tempData["Id"] = array[^1];
        }

        public static void SetSessionFromReferrer(HttpContext context)
        {
            string referrerUrl = context.Request.Headers.Referer.ToString();

            if (string.IsNullOrWhiteSpace(referrerUrl))
                return;

            var array = referrerUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (array.Length < 3)
                return;

            var controller = array[^3];
            var action = array[^2];
            var idSegment = array[^1];

            if (!int.TryParse(idSegment, out int id))
                return;

            context.Session.SetString("Ref_Controller", controller);
            context.Session.SetString("Ref_Action", action);
            context.Session.SetInt32("Ref_Id", id);
        }

        public static (string? Controller, string? Action, int? Id) GetSessionValues(HttpContext context)
        {
            var controller = context.Session.GetString("Ref_Controller");
            var action = context.Session.GetString("Ref_Action");
            var id = context.Session.GetInt32("Ref_Id");

            return (controller, action, id);
        }
    }
}

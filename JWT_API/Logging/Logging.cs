using Newtonsoft.Json;

namespace JWT_API.Logging
{
    public class Logging:LoggingInterface

    {
        public string Success(string message, int statuscode, object? data)
        {
            var response = new JsonResponse
            {
                status="success",
                message = message,
                data = data,
                statuscode=statuscode,
            };
            return JsonConvert.SerializeObject(response);

        }
        public string Failure(string message, int statuscode, object? data)
        {
            var response = new JsonResponse
            {
                status="failure",
                message = message,
                data = data,
                statuscode=statuscode,
            };
            return JsonConvert.SerializeObject(response);
        }
    }
    public class JsonResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public int statuscode { get; set; }
    }
}

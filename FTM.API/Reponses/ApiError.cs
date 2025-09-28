using System.Net;

namespace FTM.API.Reponses
{
    public class ApiError : ApiResponse
    {
        public ApiError()
            : base(null, true, HttpStatusCode.BadRequest, "Error")
        {
        }

        public ApiError(object data)
            : base(data, true, HttpStatusCode.BadRequest, "Error")
        {
        }

        public ApiError(string message, object data)
            : base(data, true, HttpStatusCode.BadRequest, message)
        {
        }
    }
}

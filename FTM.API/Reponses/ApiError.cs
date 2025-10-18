using System.Net;

namespace FTM.API.Reponses
{
    public class ApiError : ApiResponse
    {
        public ApiError()
            : base(null, true, HttpStatusCode.BadRequest, "Error", true)
        {
        }

        public ApiError(object data)
            : base(data, true, HttpStatusCode.BadRequest, "Error", true)
        {
        }

        public ApiError(string message, HttpStatusCode httpStatus)
            : base(null, true, httpStatus, message, true)
        {
        }

        public ApiError(string message)
            : base(null, true, HttpStatusCode.BadRequest, message, true)
        {
        }

        public ApiError(string message, object data)
            : base(data, true, HttpStatusCode.BadRequest, message, true)
        {
        }


    }
}

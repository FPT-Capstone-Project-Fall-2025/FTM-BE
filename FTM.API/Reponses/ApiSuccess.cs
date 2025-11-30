using System.Net;

namespace FTM.API.Reponses
{
    public class ApiSuccess : ApiResponse
    {
        public ApiSuccess()
            : base(null, true, HttpStatusCode.OK, "Success")
        {
        }

        public ApiSuccess(object data)
            : base(data, true, HttpStatusCode.OK, "Success")
        {
        }

        public ApiSuccess(string message, object data)
            : base(data, true, HttpStatusCode.OK, message)
        {
        }
    }
}

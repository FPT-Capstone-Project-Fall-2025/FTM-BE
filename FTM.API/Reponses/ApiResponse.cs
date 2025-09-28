﻿using System.Net;

namespace FTM.API.Reponses
{
    public class ApiResponse
    {
        protected ApiResponse(object data, bool status, HttpStatusCode statusCode, string message)
        {
            Data = status ? data : null;
            Errors = status ? null : data;
            Status = status;
            StatusCode = (int)statusCode;
            Message = message ?? GetDefaultMessageForStatusCode((int)statusCode);
        }

        public int StatusCode { get; set; }
        public string Message { get; set; }
        public bool Status { get; set; } = true;
        public object Data { get; set; }
        public object Errors { get; set; }
        public bool HasError { get; set; } = false;

        private static string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have made",
                401 => "Authorized, you are not",
                404 => "Resource found, it was not",
                500 => "Errors are the path to the dark side.  Errors lead to anger.   Anger leads to hate.  Hate leads to career change.",
                409 => "Conflict",
                _ => null
            };
        }
    }
}

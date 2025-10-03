using System;
using System.Collections.Generic;

namespace FTM.Domain.DTOs.Authen
{
    public class ProvinceListResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameWithType { get; set; }
        public string Slug { get; set; }
    }

    public class WardListResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameWithType { get; set; }
        public string Path { get; set; }
        public string PathWithType { get; set; }
        public string Slug { get; set; }
    }
}
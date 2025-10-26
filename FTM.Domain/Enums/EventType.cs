using System.ComponentModel;

namespace FTM.Domain.Enums
{
    public enum EventType
    {
        [Description("Ma chay, giỗ")]
        Memorial = 1,

        [Description("Cưới hỏi")]
        Wedding = 2,

        [Description("Sinh nhật")]
        Birthday = 3,

        [Description("Khác")]
        Other = 4
    }
}

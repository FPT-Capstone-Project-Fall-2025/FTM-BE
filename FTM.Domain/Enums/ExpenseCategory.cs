namespace FTM.Domain.Enums
{
    /// <summary>
    /// Categories for expenses
    /// </summary>
    public enum ExpenseCategory
    {
        /// <summary>
        /// Education-related expenses (học phí, sách vở, etc.)
        /// </summary>
        Education = 0,

        /// <summary>
        /// Healthcare and medical expenses
        /// </summary>
        Healthcare = 1,

        /// <summary>
        /// Emergency support
        /// </summary>
        Emergency = 2,

        /// <summary>
        /// Family events (weddings, funerals, etc.)
        /// </summary>
        FamilyEvents = 3,

        /// <summary>
        /// Religious/spiritual activities
        /// </summary>
        Religious = 4,

        /// <summary>
        /// Community service and charity
        /// </summary>
        Community = 5,

        /// <summary>
        /// Administrative expenses
        /// </summary>
        Administrative = 6,

        /// <summary>
        /// Infrastructure and maintenance
        /// </summary>
        Infrastructure = 7,

        /// <summary>
        /// Other miscellaneous expenses
        /// </summary>
        Other = 99
    }
}
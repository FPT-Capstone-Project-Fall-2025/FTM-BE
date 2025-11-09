namespace FTM.Domain.Enums
{
    /// <summary>
    /// Status of fund transactions (especially for withdrawals requiring approval)
    /// </summary>
    public enum TransactionStatus
    {
        /// <summary>
        /// Transaction is pending approval
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Transaction has been approved
        /// </summary>
        Approved = 2,

        /// <summary>
        /// Transaction has been rejected
        /// </summary>
        Rejected = 3
    }
}

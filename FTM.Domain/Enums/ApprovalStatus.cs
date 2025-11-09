namespace FTM.Domain.Enums
{
    /// <summary>
    /// Approval status for expenses and other financial transactions
    /// </summary>
    public enum ApprovalStatus
    {
        /// <summary>
        /// Waiting for approval
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Approved and authorized
        /// </summary>
        Approved = 1,

        /// <summary>
        /// Rejected with reason
        /// </summary>
        Rejected = 2,

        /// <summary>
        /// Needs revision/modification
        /// </summary>
        NeedsRevision = 3
    }
}
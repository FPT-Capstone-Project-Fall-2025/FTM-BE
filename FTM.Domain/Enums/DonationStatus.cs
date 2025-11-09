namespace FTM.Domain.Enums
{
    /// <summary>
    /// Donation confirmation status
    /// </summary>
    public enum DonationStatus
    {
        /// <summary>
        /// Donation submitted but not yet confirmed (for cash donations)
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Donation confirmed and money received
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Donation rejected or cancelled
        /// </summary>
        Rejected = 3,

        /// <summary>
        /// Online payment completed automatically
        /// </summary>
        Completed = 4
    }
}
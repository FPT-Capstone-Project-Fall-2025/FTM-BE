namespace FTM.Domain.Enums
{
    /// <summary>
    /// Status of fundraising campaigns
    /// </summary>
    public enum CampaignStatus
    {
        /// <summary>
        /// Campaign is planned but not yet started
        /// </summary>
        Upcoming = 1,

        /// <summary>
        /// Campaign is currently active and accepting donations
        /// </summary>
        Active = 2,

        /// <summary>
        /// Campaign has completed successfully
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Campaign has been canceled
        /// </summary>
        Canceled = 4
    }
}

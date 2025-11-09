namespace FTM.Domain.Enums
{
    /// <summary>
    /// Payment methods for donations
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Cash donation - requires manual confirmation
        /// </summary>
        Cash = 1,

        /// <summary>
        /// Online bank transfer via PayOS
        /// </summary>
        BankTransfer = 2,

        /// <summary>
        /// Other payment methods
        /// </summary>
        Other = 3
    }
}
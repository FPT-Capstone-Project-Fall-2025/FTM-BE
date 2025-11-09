namespace FTM.Domain.Enums
{
    /// <summary>
    /// Types of fund transactions
    /// </summary>
    public enum MoneyType
    {
        /// <summary>
        /// Income/Contribution - THU (nộp tiền vào quỹ)
        /// </summary>
        Contribute = 1,

        /// <summary>
        /// Expense - CHI (chi tiêu từ quỹ)
        /// </summary>
        Expense = 2,

        /// <summary>
        /// Withdrawal - RÚT (rút tiền từ quỹ)
        /// </summary>
        Withdrawal = 3
    }
}

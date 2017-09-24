using PX.Common;

namespace BatchPaymentSFTPProvider
{
    [PXLocalizable("BatchPaymentSFTPProvider")]
    public static class Messages
    {
        public const string Transfer = "Transfer";
        public const string FailedToCreateDirectDepositProvider = "Failed to Create the Direct Deposit Provider. Please check that Plug-In(Type) is valid Type Name. {0}";
        public const string ACHTransferFailMoreThanOneAttachment = "More than one file is attached to the current document.";
        public const string ACHTransferFailNoAttachment = "No file is attached to the current document.";
        public const string ACHTransferFailProviderIsNotForDirectDeposit = "The processing center type is not compatible with Direct Deposit.";
        public const string ACHTransferFailNoProcessingCenterSelected = "No processing center has been selected.";
    }
}

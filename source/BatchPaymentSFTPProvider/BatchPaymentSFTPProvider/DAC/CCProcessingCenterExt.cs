using PX.CCProcessingBase;
using PX.Data;
using PX.DDProcessing;
using PX.Objects.CA;
using System;

namespace BatchPaymentSFTPProvider
{
    public class CCProcessingCenterExt : PXCacheExtension<CCProcessingCenter>
    {
        [PXDBString(255)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXMultipleProviderTypeSelector(typeof(ICCPaymentProcessing), typeof(IDDPaymentProcessing))]
        [PXUIField(DisplayName = "Payment Plug-In (Type)")]
        public virtual String ProcessingTypeName { get; set; }
    }
}

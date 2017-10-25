using PX.Data;
using PX.Objects.CA;

namespace BatchPaymentSFTPProviderAP
{
    public class PaymentMethodPXExt : PXCacheExtension<PaymentMethod>
    {
        public abstract class integratedProcessing : PX.Data.IBqlField { }
        [PXBool]
        [PXUIField(DisplayName = "Integrated Processing")]
        public virtual bool IntegratedProcessing { get; set; }
    }
}

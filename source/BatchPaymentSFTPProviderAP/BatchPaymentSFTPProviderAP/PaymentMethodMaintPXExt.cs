using PX.Data;
using PX.Objects.CA;

namespace BatchPaymentSFTPProviderAP
{
    public class PaymentMethodMaintPXExt : PXGraphExtension<PaymentMethodMaint>
    {
        protected virtual void PaymentMethod_IntegratedProcessing_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var row = (PaymentMethod)e.Row;
            if (row == null)
            {
                return;
            }
            var rowExt = sender.GetExtension<PaymentMethodPXExt>(row);

            sender.SetValue<PaymentMethod.aRIsProcessingRequired>(row, rowExt.IntegratedProcessing);
        }
    }
}

﻿using BatchPaymentSFTPProvider;
using PX.Data;
using PX.Objects.ACH;

namespace BatchPaymentSFTPProviderAR
{
    public class ARBatchEntryPXExt : PXGraphExtension<ARBatchEntry>
    {
        public PXBatchPaymentTransfer Transfer;

        #region CABatch Events

        protected virtual void CABatch_RowSelected(PXCache sender, PXRowSelectedEventArgs e, PXRowSelected del)
        {
            if (del != null)
            {
                del(sender, e);
            }

            Transfer.CABatch_RowSelected(sender, e);
        }

        protected virtual void CABatch_ProcessingCenterID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e, PXFieldDefaulting del)
        {
            if (del != null)
            {
                del(sender, e);
            }

            Transfer.CABatch_ProcessingCenterID_FieldDefaulting(sender, e);
        }

        #endregion
    }
}

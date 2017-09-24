using PX.Data;
using PX.Objects.CA;
using PX.Objects.CS;
using System;

namespace BatchPaymentSFTPProvider
{
    [PXTable(typeof(CABatch.batchNbr), IsOptional = true)]
    public class CABatchExt : PXCacheExtension<CABatch>
    {
        #region BatchModule
        public abstract class batchModule : PX.Data.IBqlField
        {
        }

        [PXDBString(2, IsFixed = true)]
        [PXDefault(PX.Objects.GL.BatchModule.AP)]
        [PXStringList(new string[] { PX.Objects.GL.BatchModule.AP, PX.Objects.GL.BatchModule.AR }, new string[] { PX.Objects.GL.BatchModule.AP, PX.Objects.GL.BatchModule.AR })]
        [PXUIField(DisplayName = "Module", Enabled = false)]
        public string BatchModule { get; set; }
        #endregion

        #region ProcessingCenterID
        public abstract class processingCenterID : PX.Data.IBqlField
        {
        }
        [PXDBString(10, IsUnicode = true)]
        [PXSelector(typeof(Search2<CCProcessingCenterPmntMethod.processingCenterID,
            InnerJoin<CCProcessingCenter, 
                On<CCProcessingCenterPmntMethod.processingCenterID, 
                    Equal<CCProcessingCenter.processingCenterID>>>,
            Where<CCProcessingCenter.cashAccountID, 
                Equal<Current<CABatch.cashAccountID>>,
            And<CCProcessingCenterPmntMethod.paymentMethodID, 
                Equal<Current<CABatch.paymentMethodID>>, 
            And<CCProcessingCenterPmntMethod.isActive, 
                Equal<boolTrue>>>>>))]
        [PXUIField(DisplayName = "Proc. Center ID")]
        [PXFormula(typeof(Default<CABatch.paymentMethodID>))]
        public virtual string ProcessingCenterID { get; set; }
        #endregion

        #region TransferTime
        public abstract class fileTransferTime : PX.Data.IBqlField
        {
        }

        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = "File Transfer Time", Enabled = false)]
        public virtual DateTime? FileTransferTime { get; set; }
        #endregion
    }
}

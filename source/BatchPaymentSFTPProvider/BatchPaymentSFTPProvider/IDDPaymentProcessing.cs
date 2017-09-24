using PX.CCProcessingBase;
using System.Collections.Generic;
using System.ComponentModel;

namespace PX.DDProcessing
{
    public abstract class IDDPaymentProcessing : ICCPaymentProcessing
    {
        #region New methods

        abstract public bool DoTransaction(string fileName, byte[] file, out string message);
        abstract public void Initialize(IProcessingCenterSettingsStorage aSettingsReader);

        #endregion

        #region Method to Hide/Route

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool DoTransaction(CCTranType aType, ProcessingInput aInputData, ProcessingResult aResult)
        {
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ExportSettings(IList<ISettingsDetail> aSettings, CCProcessingSettingsType settingsType)
        {
            ExportSettings(aSettings);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool IsSupported(CCTranType aType)
        {
            return false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override CCErrors ValidateSettings(ISettingsDetail setting)
        {
            return new CCErrors() { ErrorMessage = "", source = CCErrors.CCErrorSource.None };
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Initialize(IProcessingCenterSettingsStorage aSettingsReader, ICreditCardDataReader aCardDataReader, ICustomerDataReader aCustomerDataReader)
        {
            Initialize(aSettingsReader);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void Initialize(IProcessingCenterSettingsStorage aSettingsReader, ICreditCardDataReader aCardDataReader, ICustomerDataReader aCustomerDataReader, IDocDetailsDataReader aDocDetailsReader)
        {
            Initialize(aSettingsReader);
        }

        #endregion

        #region Methods To Keep
        
        //public override void TestCredentials(APIResponse apiResponse)
        //public override void ExportSettings(IList<ISettingsDetail> aSettings)
        
        #endregion

    }
}

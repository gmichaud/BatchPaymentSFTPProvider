using PX.CCProcessingBase;
using PX.Common;
using PX.Data;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;

namespace PX.DDProcessing
{
    public class SFTPDirectDepositProcessing : IDDPaymentProcessing
    {
        #region CTOR

        public SFTPDirectDepositProcessing()
        {
            this.Settings = new Dictionary<string, FTPDirectDepositDetail>();
        }

        #endregion

        #region Settings

        private static class SettingsKeys
        {
            //Not more Then 10 chars
            public static class Key
            {
                public const string URLConnection = "URLCONNECT";
                public const string LoginID = "LOGINID";
                public const string TranKey = "TRANKEY";
                public const string BankName = "BANKNAME";
                public const string Path = "PATH";
            }
            
            public static class Default
            {
                public const string URLConnection = "";
                public const string LoginID = "";
                public const string TranKey = "";
                public const string BankName = "";
                public const string Path = "";
            }
            
            [PXLocalizable]
            public static class Descr
            {
                public const string URLConnection = "URL for connecting to the SFTP Site";
                public const string LoginID = "Your Login";
                public const string TranKey = "Your Password";
                public const string BankName = "Bank Name";
                public const string Path = "Folder Path (blank for root)";
            }

        }

        private class FTPDirectDepositDetail : ISettingsDetail
        {
            public string DetailID { get; set; }
            public string Descr { get; set; }
            public string Value { get; set; }
            public int? ControlType { get; set; }
            public bool? IsEncryptionRequired { get; set; }
            private IList<KeyValuePair<string, string>> comboValues;

            public FTPDirectDepositDetail(string detailID, string descr, string value) : this(detailID, descr, value, 1) { }
            public FTPDirectDepositDetail(string detailID, string descr, string value, int? controlType)
            {
                this.DetailID = detailID;
                this.Descr = descr;
                this.Value = value;
                this.ControlType = controlType;
                this.IsEncryptionRequired = controlType == 4;
                comboValues = new List<KeyValuePair<string, string>>();
            }

            public IList<KeyValuePair<string, string>> GetComboValues()
            {
                return comboValues;
            }

            public void SetComboValues(IList<KeyValuePair<string, string>> list)
            {
                comboValues = list;
            }
        }

        private Dictionary<string, FTPDirectDepositDetail> Settings;
        private IProcessingCenterSettingsStorage settingsStorage;

        #endregion
        protected void LoadSettings()
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            this.settingsStorage.ReadSettings(settings);
            this.MergeSettingWithDefaults(settings);
        }
        protected void MergeSettingWithDefaults(Dictionary<string, string> aSettings)
        {
            if (!this.Settings.ContainsKey(SettingsKeys.Key.BankName))
                this.Settings[SettingsKeys.Key.BankName] = new FTPDirectDepositDetail(SettingsKeys.Key.BankName, PXLocalizer.Localize(SettingsKeys.Descr.BankName), SettingsKeys.Default.BankName);
            if (!this.Settings.ContainsKey(SettingsKeys.Key.URLConnection))
                this.Settings[SettingsKeys.Key.URLConnection] = new FTPDirectDepositDetail(SettingsKeys.Key.URLConnection, PXLocalizer.Localize(SettingsKeys.Descr.URLConnection), SettingsKeys.Default.URLConnection);
            if (!this.Settings.ContainsKey(SettingsKeys.Key.LoginID))
                this.Settings[SettingsKeys.Key.LoginID] = new FTPDirectDepositDetail(SettingsKeys.Key.LoginID, PXLocalizer.Localize(SettingsKeys.Descr.LoginID), SettingsKeys.Default.LoginID);
            if (!this.Settings.ContainsKey(SettingsKeys.Key.TranKey))
                this.Settings[SettingsKeys.Key.TranKey] = new FTPDirectDepositDetail(SettingsKeys.Key.TranKey, PXLocalizer.Localize(SettingsKeys.Descr.TranKey), SettingsKeys.Default.TranKey, 4);
            if (!this.Settings.ContainsKey(SettingsKeys.Key.Path))
                this.Settings[SettingsKeys.Key.Path] = new FTPDirectDepositDetail(SettingsKeys.Key.Path, PXLocalizer.Localize(SettingsKeys.Descr.Path), SettingsKeys.Default.Path);

            if (aSettings != null)
            {
                foreach (KeyValuePair<string, string> it in aSettings)
                {
                    if (!this.Settings.ContainsKey(it.Key))
                    {
                        this.Settings[it.Key] = new FTPDirectDepositDetail(it.Key, "Imported Option", it.Value);
                    }
                    else
                    {
                        this.Settings[it.Key].Value = it.Value;
                    }
                }
            }
        }


        #region IImplementation

        public override void Initialize(IProcessingCenterSettingsStorage aSettingsReader)
        {
            this.settingsStorage = aSettingsReader;
            this.LoadSettings();
        }

        public override void ExportSettings(IList<ISettingsDetail> aSettings)
        {
            if (this.settingsStorage != null)
            {
                this.MergeSettingWithDefaults(null);
                foreach (KeyValuePair<string, FTPDirectDepositDetail> it in this.Settings)
                {
                    aSettings.Add(it.Value);
                }
            }
        }

        public override void TestCredentials(APIResponse apiResponse)
        {
            var url = this.Settings[SettingsKeys.Key.URLConnection].Value;
            var username = this.Settings[SettingsKeys.Key.LoginID].Value;
            var password = this.Settings[SettingsKeys.Key.TranKey].Value;

            try
            {
                var sftpClient = new SftpClient(url, username, password);
                sftpClient.Connect();
                apiResponse.ErrorSource = CCErrors.CCErrorSource.None;
                apiResponse.isSucess = true;
            }
            catch (Exception ex)
            {
                apiResponse.ErrorSource = CCErrors.CCErrorSource.ProcessingCenter;
                apiResponse.isSucess = false;
                apiResponse.Messages["Exception"] = ex.Message;
            }
        }

        #endregion

        #region DO

        public override bool DoTransaction(string fileName, byte[] file, out string message)
        {
            message = string.Empty;
            var url = this.Settings[SettingsKeys.Key.URLConnection].Value;
            var username = this.Settings[SettingsKeys.Key.LoginID].Value;
            var password = this.Settings[SettingsKeys.Key.TranKey].Value;
            var path = this.Settings[SettingsKeys.Key.Path].Value;
            
            try
            {
                var sftpClient = new SftpClient(url, username, password);
                sftpClient.Connect();
                sftpClient.WriteAllBytes(Path.Combine(path,fileName).Replace("\\", "/"), file);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
        
        #endregion
    }
}

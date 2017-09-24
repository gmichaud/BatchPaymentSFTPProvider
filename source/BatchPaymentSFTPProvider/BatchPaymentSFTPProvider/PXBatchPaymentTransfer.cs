using PX.Data;
using PX.DDProcessing;
using PX.Objects.CA;
using PX.SM;
using System;
using System.Collections;
using System.Web.Compilation;

namespace BatchPaymentSFTPProvider
{
    public class PXBatchPaymentTransfer : PXAction<CABatch>
    {
        public PXBatchPaymentTransfer(PXGraph graph)
            : base(graph)
        {
        }

        public PXBatchPaymentTransfer(PXGraph graph, Delegate handler)
            : base(graph, handler)
        {
        }

        public PXBatchPaymentTransfer(PXGraph graph, string name)
            : base(graph, name)
        {
        }

        [PXUIField(DisplayName = Messages.Transfer, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Update)]
        [PXProcessButton]
        protected override IEnumerable Handler(PXAdapter adapter)
        {
            if (PXLongOperation.Exists(_Graph.UID))
            {
                throw new ApplicationException(PX.Objects.GL.Messages.PrevOperationNotCompleteYet);
            }

            _Graph.Actions.PressSave();
            CABatch doc = (CABatch)_Graph.Views[_Graph.PrimaryView].Cache.Current;
            CABatchExt docExt = PXCache<CABatch>.GetExtension<CABatchExt>(doc);

            if (doc.Released == true && doc.ExportTime.HasValue && !String.IsNullOrEmpty(docExt.ProcessingCenterID) && !docExt.FileTransferTime.HasValue)
            {
                PXLongOperation.StartOperation(_Graph, delegate () { TransferDocProcessingCenter(_Graph, doc, docExt); });
            }
            return adapter.Get();
        }

        public static void TransferDocProcessingCenter(PXGraph graph, CABatch doc, CABatchExt docExt)
        {
            var pcGraph = PXGraph.CreateInstance<CCProcessingCenterMaint>();
            pcGraph.ProcessingCenter.Current = pcGraph.
                                               ProcessingCenter.
                                               Search<CCProcessingCenter.processingCenterID>
                                               (docExt.ProcessingCenterID);

            var processingCenter = pcGraph.ProcessingCenter.Current;

            if (processingCenter != null)
            {
                var providerIsDirectDeposit = PXMultipleProviderTypeSelectorAttribute.IsProvider<CCProcessingCenter.processingTypeName, IDDPaymentProcessing>(pcGraph.ProcessingCenter.Cache, processingCenter);
                if (providerIsDirectDeposit)
                {
                    IDDPaymentProcessing provider = null;
                    try
                    {
                        Type providerType = PXBuildManager.GetType(processingCenter.ProcessingTypeName, true);
                        provider = (IDDPaymentProcessing)Activator.CreateInstance(providerType);
                        provider.Initialize(pcGraph);
                    }
                    catch (Exception ex)
                    {
                        throw new PXException(Messages.FailedToCreateDirectDepositProvider, ex.Message);
                    }
                    var fileNotes = PXNoteAttribute.GetFileNotes(graph.Views[graph.PrimaryView].Cache, doc);
                    if (fileNotes.Length == 1)
                    {
                        var fileNote = fileNotes[0];
                        UploadFileMaintenance upload = PXGraph.CreateInstance<UploadFileMaintenance>();
                        var file = upload.GetFile(fileNote);
                        string message;
                        if (!provider.DoTransaction(file.FullName, file.BinData, out message))
                        {
                            throw new PXException(message);
                        }
                        else
                        {
                            docExt.FileTransferTime = DateTime.Now;
                            graph.Views[graph.PrimaryView].Cache.Update(doc);
                            graph.Actions.PressSave();
                        }
                    }
                    else
                    {
                        if (fileNotes.Length > 1)
                        {
                            throw new Exception(Messages.ACHTransferFailMoreThanOneAttachment);
                        }
                        else
                        {
                            throw new PXException(Messages.ACHTransferFailNoAttachment);
                        }
                    }
                }
                else
                {
                    throw new PXException(Messages.ACHTransferFailProviderIsNotForDirectDeposit);
                }
            }
            else
            {
                throw new PXException(Messages.ACHTransferFailNoProcessingCenterSelected);
            }
        }

        public void CABatch_ProcessingCenterID_FieldDefaulting(PXCache sender, PXFieldDefaultingEventArgs e)
        {
            var ret = string.Empty;
            foreach (PXResult<CCProcessingCenterPmntMethod,
                              CCProcessingCenter> proc in
                              PXSelectorAttribute.SelectAll<CABatchExt.processingCenterID>(sender, e.Row))
            {
                var pmtMethod = (CCProcessingCenterPmntMethod)proc;
                ret = pmtMethod.ProcessingCenterID;
                if (pmtMethod.IsDefault == true)
                    break;
            }

            e.NewValue = ret;
        }

        public void CABatch_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            CABatch row = e.Row as CABatch;
            if (row == null) return;
            CABatchExt rowExt = PXCache<CABatch>.GetExtension<CABatchExt>(row);
            bool isReleased = (row.Released == true);
            bool isTransferable = isReleased &&
                                  row.ExportTime.HasValue &&
                                  !String.IsNullOrEmpty(rowExt.ProcessingCenterID) &&
                                  !rowExt.FileTransferTime.HasValue;
            if (!isReleased)
            {
                PXUIFieldAttribute.SetEnabled<CABatchExt.processingCenterID>(sender, row, true);
            }

            this.SetEnabled(isTransferable);
        }
    }
}

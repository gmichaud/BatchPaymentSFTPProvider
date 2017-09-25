# BatchPaymentSFTPProvider
BatchPaymentSFTPProviderAR.zip and BatchPaymentSFTPProviderAP.zip both can be used independently but they both need to be published along BatchPaymentSFTPProvider.zip

To compile the solution for AP Batch payment, follow these steps : 
                
1.	Publish BatchPaymentSFTPProvider.zip and BatchPaymentSFTPProviderAP.zip to your website.
2.	Run " source\BatchPaymentSFTPProviderAP\BatchPaymentSFTPProviderAP.sln"
3.	Replace PX.CCProcessingBase, PX.Common, PX.Data, PX.Objects missing references for both project with the corresponding dlls found in the bin folder of your website.
4.	Install the missing nugget package Renci.SshNet on BatchPaymentSFTPProvider project.
5.	Delete BatchPaymentSFTPProvider.dll and BatchPaymentSFTPProviderAP.dll from your website bin folder
6.	Include your website in the solution and add the 2 projects as reference to the web site bin folder.
7.	Build the solution and reload your website. You should now have the latest compiled code deployed on your website.

More details to come...

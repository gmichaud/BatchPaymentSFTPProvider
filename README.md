# BatchPaymentSFTPProvider
Allows the secure transfer over SFTP of ACH documents for AP/AR payments.

### Prerequisites
* Acumatica 5.3 or higher

# Quick Start
### Installation
Publish BatchPaymentSFTPProviderAR.zip and/or BatchPaymentSFTPProviderAP.zip along 
BatchPaymentSFTPProvider.zip

# Development
### Structure
* BatchPaymentSFTPProvider
	- Project containing common logic used by BatchPaymentSFTPProviderAR and BatchPaymentSFTPProviderAP.
* BatchPaymentSFTPProviderAR
	- Project pertaining to the Account Receivable module. Requires BatchPaymentSFTPProvider project.
* BatchPaymentSFTPProviderAP
	- Project pertaining to the Account Payable module. Requires BatchPaymentSFTPProvider project.

### Set-up
To compile the solution for AP Batch payment, follow these steps :

*Note : The same steps can be applied by replacing BatchPaymentSFTPProviderAP by BatchPaymentSFTPProviderAR*
1. Publish BatchPaymentSFTPProvider.zip and BatchPaymentSFTPProviderAP.zip to your website.
2. Run "source\BatchPaymentSFTPProviderAP\BatchPaymentSFTPProviderAP.sln"
3. Replace PX.CCProcessingBase, PX.Common, PX.Data, PX.Objects missing references for both project with the corresponding dlls found in the bin folder of your website.
4. Install the missing nugget package Renci.SshNet on BatchPaymentSFTPProvider project.
5. Delete BatchPaymentSFTPProvider.dll and BatchPaymentSFTPProviderAP.dll from your website bin folder
6. Include your website in the solution and add the 2 projects as reference to the web site bin folder.
7. Build the solution and reload your website. You should now have the latest compiled code deployed on your website.


## Copyright and License

Copyright © `2017` `Acumatica`

This component is licensed under the MIT License, a copy of which is available online at https://github.com/Acumatica/BatchPaymentSFTPProvider/blob/master/LICENSE

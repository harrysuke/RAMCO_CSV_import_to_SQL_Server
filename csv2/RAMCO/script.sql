SET NOCOUNT ON;

SELECT 'CompanyCode', 'InvoiceNumber', 'InvoiceDate' , 'SalesType', 'BillToCustomerCode', 'Currency', 'PayTerm', 'SalesPersonCode', 'ExchangeRate', 'TotalInvoiceAmount', 'CustomerAccountCode', 'Line', 'ItemDesc','UOM', 'Qty', 'UnitPrice', 'Remarks', 'UsageID','CostCenter', 'OwnTaxRegion', 'TaxAmount', 'TaxCode', 'TRAN_TYPE', 'CUST_VOYAGENO', 'PTPVOYAGENO', 'VESSEL_CODE', 'VESSEL_NAME', 'GT', 'LOA', 'ATA', 'ATD', 'ATB', 'ATUB', 'LOCATION', 'REFERENCE', 'DocumentType', 'CargoType', 'Operater', 'THROUGHPUT', 'Remarks1', 'Remarks2', 'Dimension1', 'Dimension2', 'Dimension3', 'Dimension4', 'Dimension5', 'Dimension6', 'Dimension7', 'Dimension8';
SELECT * FROM [JPB_intdb].[dbo].[viewRamcoCSV];

SET NOCOUNT OFF;

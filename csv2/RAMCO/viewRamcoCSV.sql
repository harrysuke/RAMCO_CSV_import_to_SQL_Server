--CREATE VIEW FOR viewRamcoCSV
convert(varchar, cast(convert(varchar, ar1.DocumentDate) as datetime), 103) AS InvoiceDate, 
convert(varchar, cast(convert(varchar, ar1.DocumentDate) as datetime), 103) AS InvoiceDate, 
ar1.TransactionType AS SalesType, ar1.BPCode AS BillToCustomerCode, 
ar1.CurrencyCode AS Currency, ar1.CreditTerm AS PayTerm, '' AS SalesPersonCode,'' AS ExchangeRate, ar2.Amount AS TotalInvoiceAmount, '2021001' AS CustomerAccountCode, 
ar1.Line, ar1.ProductDescription AS ItemDesc, '' AS UOM, '' AS Qty, ar1.Amount AS UnitPrice, '' AS Remarks, ar1.LedgerAccount AS UsageID, ar1.Dimension2 AS CostCenter, 
'MY' AS OwnTaxRegion, '' AS TaxAmount, '' AS TaxCode, '' AS TRAN_TYPE, 'NA' AS CUST_VOYAGENO, '' AS PTPVOYAGENO, 'NA' AS VESSEL_CODE, 'NA' AS VESSEL_NAME, '0' AS GT, '0' AS LOA, '' AS ATA, 
'' AS ATD, '' AS ATB, '' AS ATUB, '' AS LOCATION, '' AS REFERENCE, '' AS DocumentType, '' AS CargoType, '' AS Operater, '' AS THROUGHPUT, '' AS Remarks1, '' AS Remarks2, 
'' AS Dimension1, '' AS Dimension2, '' AS Dimension3, '' AS Dimension4, '' AS Dimension5, '' AS Dimension6, '' AS Dimension7, '' AS Dimension8 
FROM [JPB_intdb].[dbo].[ARTransactions] AS ar1 JOIN [JPB_intdb].[dbo].[ARTransactions] AS ar2 ON ar2.DocumentNumber=ar1.DocumentNumber AND ar2.Line=0 
WHERE ar1.TransactionType IN ('CEX', 'CIM', 'CVS', 'CFS') AND ar1.Line > 0
and ar1.FiscalPeriod=7
and ar1.FiscalYear=2023
--update the FetchByLN status to True
update [JPB_intdb].[dbo].[ARTransactionsRamco] 
set FetchByLN='True'
where FetchByLN='False'
and TransactionType IN ('JPR', 'JPS', 'JPZ', 'JPW', 'DGC', 'DGF')
--Update Ramco company code
sqlcmd -S amantea101\inforln -U baan -P P@ssw0rd -i companyCodeJPB.sql


--Generate CSV output filename ddmmyyyyhhmm
sqlcmd -S AMANTEA101\INFORLN -U baan -P  P@ssw0rd -i script.sql -s"," -h-1 -W -o "T:\RAMCO\CSV\COM_INV_%date:~0,2%%date:~3,2%%date:~-4%%time:~0,2%%time:~3,2%.csv"




--Update FetchByLN
sqlcmd -S amantea101\inforln -U baan -P P@ssw0rd -i fetchByLn.sql





cmd /k
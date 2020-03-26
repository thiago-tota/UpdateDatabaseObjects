:: WARNING
:: Find below examples of how to call the application via command line
:: This file is not used by application, is only for local tests purpose. Using this file is possible execute the EXE informing the arguments
:: Information related to database connection and local path must be updated before execution

rem generate views only (filter 3)
Swift.Database.exe "GET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "" "" "3"

rem generate from cart schema only
Swift.Database.exe "GET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "" "cart"

rem generate specific object only
Swift.Database.exe "GET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "spA_CheckPayments"

rem generate specific object only
Swift.Database.exe "GET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "spA_Check_Payment_PO_Box"

rem generate full content
Swift.Database.exe "GET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\"
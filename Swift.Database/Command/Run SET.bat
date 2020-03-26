:: WARNING
:: Find below examples of how to call the application via command line
:: This file is not used by application, is only for local tests purpose. Using this file is possible execute the EXE informing the arguments
:: Information related to database connection and local path must be updated before execution

rem update cart schema only
Swift.Database.exe "SET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "" "cart"

rem update specific object only
Swift.Database.exe "SET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "spA_CheckPayments"

rem update specific object only
Swift.Database.exe "SET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\" "spA_Check_Payment_PO_Box"

rem update full content
Swift.Database.exe "SET" "Server=VM-ASC-VBDEV4;Database=SwiftBMDH;Trusted_Connection=True;" "Output\\"
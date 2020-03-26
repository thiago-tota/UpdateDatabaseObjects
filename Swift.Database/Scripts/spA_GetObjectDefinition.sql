-- =============================================
-- Author:		TOTA
-- Create date: 19-FEB-2019
-- Description:	Return the object definition to be saved

--1.1	26-MAR-2020	TOTA: Included functions (O.type IN ('FN','AF','FS','FT')) on the return list
-- =============================================
SELECT O.object_id, S.principal_id, O.type, O.type_desc, O.name, S.name AS [schema], ISNULL(M.definition, SM.definition) AS Definition 
FROM sys.all_objects O
	INNER JOIN sys.schemas S ON O.schema_id = S.schema_id
	LEFT JOIN sys.sql_modules M ON O.object_id = M.object_id
	LEFT JOIN sys.system_sql_modules SM ON O.object_id = SM.object_id	
WHERE O.name = ISNULL(@ObjectName, O.name) AND O.type = ISNULL(@ObjectType, O.type) AND S.name = ISNULL(@SchemaName, s.name)
	AND LOWER(S.name) NOT IN('sys', 'information_schema') 
	AND (M.definition IS NOT NULL OR SM.definition IS NOT NULL OR O.type IN ('FN','AF','FS','FT', 'IF', 'TF', 'TR', 'V', 'P'))
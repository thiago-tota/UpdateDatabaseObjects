-- =============================================
-- Author:		TOTA
-- Create date: 22-FEB-2019
-- Description:	Return the index definition from View to be saved
--		Clustered index must be the first to be created.
-- =============================================
SELECT 'CREATE ' + 
	CASE WHEN I.is_unique = 1 THEN 'UNIQUE ' ELSE '' END +
	I.type_desc COLLATE DATABASE_DEFAULT +' INDEX ' +
	I.name  + ' ON '  +  
	Schema_name(V.Schema_id)+'.'+V.name + ' (' + 
	KeyColumns + ')' + 
	ISNULL(' INCLUDE('+IncludedColumns+')','') + 
	ISNULL(' WHERE  '+I.Filter_definition,'') AS [definition]
FROM sys.indexes I   
	JOIN sys.views V ON V.Object_id = I.Object_id AND V.name = ISNULL(@ObjectName, V.name)
	JOIN sys.sysindexes SI ON I.Object_id = SI.id AND I.index_id = SI.indid   
	JOIN (SELECT * FROM (  
	SELECT IC2.object_id , IC2.index_id ,  
		STUFF((SELECT ', ' + C.name + CASE WHEN MAX(CONVERT(INT,IC1.is_descending_key)) = 1 THEN ' DESC' ELSE ' ASC' END 
	FROM sys.index_columns IC1  
	JOIN Sys.columns C   
		ON C.object_id = IC1.object_id   
		AND C.column_id = IC1.column_id   
		AND IC1.is_included_column = 0  
	WHERE IC1.object_id = IC2.object_id   
		AND IC1.index_id = IC2.index_id   
	GROUP BY IC1.object_id,C.name,index_id  
	ORDER BY MAX(IC1.key_ordinal)  
		FOR XML PATH('')), 1, 2, '') KeyColumns   
	FROM sys.index_columns IC2   
	GROUP BY IC2.object_id ,IC2.index_id) tmp3 )tmp4   
	ON I.object_id = tmp4.object_id AND I.Index_id = tmp4.index_id  
	JOIN sys.stats ST ON ST.object_id = I.object_id AND ST.stats_id = I.index_id   
	JOIN sys.data_spaces DS ON I.data_space_id=DS.data_space_id   
	JOIN sys.filegroups FG ON I.data_space_id=FG.data_space_id   
	LEFT JOIN (SELECT * FROM (   
	SELECT IC2.object_id , IC2.index_id ,   
		STUFF((SELECT ', ' + C.name  
	FROM sys.index_columns IC1   
	JOIN Sys.columns C    
		ON C.object_id = IC1.object_id    
		AND C.column_id = IC1.column_id    
		AND IC1.is_included_column = 1   
	WHERE IC1.object_id = IC2.object_id    
		AND IC1.index_id = IC2.index_id    
	GROUP BY IC1.object_id,C.name,index_id   
		FOR XML PATH('')), 1, 2, '') IncludedColumns    
	FROM sys.index_columns IC2    
	GROUP BY IC2.object_id ,IC2.index_id) tmp1   
	WHERE IncludedColumns IS NOT NULL ) tmp2    
ON tmp2.object_id = I.object_id AND tmp2.index_id = I.index_id   
WHERE I.is_primary_key = 0 AND I.is_unique_constraint = 0
ORDER BY V.name, I.type_desc
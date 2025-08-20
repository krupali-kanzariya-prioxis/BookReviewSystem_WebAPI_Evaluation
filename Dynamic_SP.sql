CREATE OR ALTER PROC [dbo].[sp_DynamicGetAllReviews]
(
    @InputXML XML
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @SearchText NVARCHAR(200) = NULL,
        @PageNo INT = 1,
        @PageSize INT = 10,
        @SortColumn SYSNAME = 'ReviewDate',
        @SortOrder VARCHAR(5) = 'DESC',
        @Offset INT = 0,
        @Filter NVARCHAR(MAX) = NULL,
        @SQL NVARCHAR(MAX);

    -- Extract values from XML
    SELECT 
        @SearchText   = NULLIF(LTRIM(RTRIM(tbl.col.value('(SearchText/text())[1]', 'NVARCHAR(200)'))), ''),
        @PageNo       = ISNULL(NULLIF(tbl.col.value('(Page/text())[1]', 'INT'), 0), 1),
        @PageSize     = ISNULL(NULLIF(tbl.col.value('(PageSize/text())[1]', 'INT'), 10), 10),
        @SortColumn   = ISNULL(NULLIF(tbl.col.value('(SortColumn/text())[1]', 'SYSNAME'), ''), 'ReviewDate'),
        @SortOrder    = ISNULL(NULLIF(tbl.col.value('(SortOrder/text())[1]', 'VARCHAR(5)'), ''), 'DESC'),
        @Filter       = NULLIF(LTRIM(RTRIM(tbl.col.value('(Filters/text())[1]', 'NVARCHAR(MAX)'))), '')
    FROM @InputXML.nodes('/Search') AS tbl(col);

SET @Filter = LTRIM(RTRIM(@Filter));

IF @SortColumn = 'BookId'        SET @SortColumn = REPLACE(@SortColumn, 'BookId', 'book_id');
IF @SortColumn = 'ReviewSID'     SET @SortColumn = REPLACE(@SortColumn, 'ReviewSID', 'review_sid');
IF @SortColumn = 'ReviewerName'  SET @SortColumn = REPLACE(@SortColumn, 'ReviewerName', 'reviewer_name');
IF @SortColumn = 'Rating'        SET @SortColumn = REPLACE(@SortColumn, 'Rating', 'rating');
IF @SortColumn = 'Comment'       SET @SortColumn = REPLACE(@SortColumn, 'Comment', 'comment');
IF @SortColumn = 'ReviewDate'    SET @SortColumn = REPLACE(@SortColumn, 'ReviewDate', 'review_date');
IF @SortColumn = 'Status'        SET @SortColumn = REPLACE(@SortColumn, 'Status', 'status');
IF @SortColumn = 'CreatedAt'     SET @SortColumn = REPLACE(@SortColumn, 'CreatedAt', 'created_at');
IF @SortColumn = 'LastModifiedAt' SET @SortColumn = REPLACE(@SortColumn,'LastModifiedAt','last_modified_at');

-- If it ends with AND, remove it
IF RIGHT(@Filter, 3) = 'AND'
    SET @Filter = LEFT(@Filter, LEN(@Filter) - 3);


    -- Pagination
    SET @Offset = (@PageNo - 1) * @PageSize;

    -- Default filter
    IF (@Filter IS NULL) SET @Filter = '1=1';

    BEGIN TRY
        DECLARE @ResultTable TABLE
        (
            ErrorMessage NVARCHAR(MAX),
            Result NVARCHAR(MAX),
            PageFrom INT NULL,
            PageSize INT NULL,
            TotalCount INT  
        );

        -- Build dynamic SQL
SET @SQL = N'
;WITH RESULT AS (
    SELECT 
        BookId         AS book_id,
        ReviewSID      AS review_sid,
        ReviewerName   AS reviewer_name,
        Rating         AS rating,
        Comment        AS comment,
        ReviewDate     AS review_date,
        Status         AS status,
        CreatedAt      AS created_at,
        LastModifiedAt AS last_modified_at
    FROM Reviews
    WHERE (' + @Filter + N')
      AND (Status <> 3)
      AND (@SearchText IS NULL 
           OR ReviewerName LIKE ''%'' + @SearchText + ''%'' 
           OR Comment LIKE ''%'' + @SearchText + ''%'')
)
SELECT 
    (SELECT * 
     FROM RESULT 
     ORDER BY ' + QUOTENAME(@SortColumn) + ' ' + @SortOrder + N'
     OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
     FOR JSON PATH, INCLUDE_NULL_VALUES) AS Result,
    @PageNo AS PageFrom,
    @PageSize AS PageSize,
    (SELECT COUNT(*) FROM RESULT) AS TotalCount;';



    PRINT @SQL;

        -- Execute dynamic SQL
        INSERT INTO @ResultTable (Result, PageFrom, PageSize, TotalCount)
        EXEC sp_executesql
            @SQL,
            N'@SearchText NVARCHAR(200), @Offset INT, @PageSize INT, @PageNo INT',
            @SearchText = @SearchText,
            @Offset = @Offset,
            @PageSize = @PageSize,
            @PageNo = @PageNo;

        SELECT * FROM @ResultTable;
    END TRY
    BEGIN CATCH
        DECLARE @ErrMsg NVARCHAR(MAX) = ERROR_MESSAGE();
        SELECT @ErrMsg AS ErrorMessage, NULL AS Result, NULL AS PageFrom, NULL AS PageSize, NULL AS TotalCount;
    END CATCH
END
GO



DECLARE @xml XML = 
N'<Search>
    <SortColumn>ReviewDate</SortColumn>
    <SortOrder>DESC</SortOrder>
    <SearchText>Krupali</SearchText>
    <Filters></Filters>
    <Page>1</Page>
    <PageSize>10</PageSize>
</Search>';

EXEC sp_DynamicGetAllReviews @InputXML = @xml;



select * from Reviews


ALTER PROCEDURE [dbo].[sp_getReviewBySid]
(
    @ReviewSID NVARCHAR(100)
)
AS
BEGIN
 BEGIN TRY
    SET NOCOUNT ON;

    DECLARE @ResultTable AS TABLE
    (
        ErrorMessage NVARCHAR(MAX),
        Result NVARCHAR(MAX)
    );

    INSERT INTO @ResultTable (Result)
    SELECT 
    (
        SELECT 
            r.BookId             AS book_id,
            r.ReviewSID          AS review_sid,
            r.ReviewerName       AS reviewer_name,
            r.Rating             AS rating,
            r.Comment            AS comment,
            r.ReviewDate         AS review_date,
            r.Status             AS status,
            r.CreatedAt          AS created_at,
            r.LastModifiedAt     AS last_modified_at
        FROM Reviews r
        WHERE r.ReviewSID = @ReviewSID 
          AND r.Status <> 3
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER, INCLUDE_NULL_VALUES
    );

    SELECT * FROM @ResultTable;
 END TRY
 BEGIN CATCH
    INSERT INTO @ResultTable (ErrorMessage, Result) 
    VALUES (ERROR_MESSAGE(), NULL);

    SELECT * FROM @ResultTable;
 END CATCH
END
GO

[sp_getReviewBySid] 'REVFE2FFF7D-AAD3-40FC-8B96-3F761548A78D'


ALTER PROCEDURE [dbo].[sp_getReviewsByRating]
(
    @Rating INT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ResultTable TABLE
    (
        ErrorMessage NVARCHAR(MAX),
        Result NVARCHAR(MAX)
    );

    BEGIN TRY
        INSERT INTO @ResultTable (Result)
        SELECT (
            SELECT 
                r.BookId         AS book_id,
                r.ReviewSID      AS review_sid,
                r.ReviewerName   AS reviewer_name,
                r.Rating         AS rating,
                r.Comment        AS comment,
                r.ReviewDate     AS review_date,
                r.Status         AS status,
                r.CreatedAt      AS created_at,
                r.LastModifiedAt AS last_modified_at
            FROM Reviews r
            WHERE r.Rating = @Rating
            FOR JSON PATH, INCLUDE_NULL_VALUES
        );

        SELECT * FROM @ResultTable;
    END TRY
    BEGIN CATCH
        INSERT INTO @ResultTable (ErrorMessage, Result)
        VALUES (ERROR_MESSAGE(), NULL);

        SELECT * FROM @ResultTable;
    END CATCH
END
GO

[sp_getReviewsByRating] 1
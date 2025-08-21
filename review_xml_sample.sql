/*DECLARE @xml XML = 
N'<Search><SortColumn>LastModifiedAt</SortColumn><SortOrder>DESC</SortOrder><SearchText>%</SearchText><Filters>ReviewDate = ''2025-01-15''  AND</Filters><Page>1</Page><PageSize>10</PageSize></Search>';

EXEC sp_DynamicGetAllReviews @InputXML = @xml;*/

/*DECLARE @xml XML = 
N'<Search><SortColumn>LastModifiedAt</SortColumn><SortOrder>DESC</SortOrder><SearchText>Naisargi</SearchText><Filters>1 = 1 AND</Filters><Page>1</Page><PageSize>10</PageSize></Search>';

EXEC sp_DynamicGetAllReviews @InputXML = @xml;
*/
DECLARE @xml XML = 
N'<Search><SortColumn>LastModifiedAt</SortColumn><SortOrder>DESC</SortOrder><SearchText>%</SearchText><Filters>( ReviewDate &lt; &apos;2025-08-20&apos; ) AND</Filters><Page>1</Page><PageSize>10</PageSize></Search>'
EXEC sp_DynamicGetAllReviews @InputXML = @xml;


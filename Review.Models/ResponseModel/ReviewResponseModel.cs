using Newtonsoft.Json;

public class ReviewResponseModel
{
    [JsonProperty("book_id")]
    public int BookId { get; set; }

    [JsonProperty("review_sid")]
    public string ReviewSID { get; set; }

    [JsonProperty("reviewer_name")]
    public string ReviewerName { get; set; }

    [JsonProperty("rating")]
    public int? Rating { get; set; }

    [JsonProperty("comment")]
    public string Comment { get; set; }

    [JsonProperty("review_date")]
    public DateTime ReviewDate { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("last_modified_at")]
    public DateTime? LastModifiedAt { get; set; }
}

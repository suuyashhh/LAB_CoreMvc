namespace Models.DairyFarm
{
    public class AnimalDto
    {
        public int? AnimalId { get; set; }      // animal_id
        public int UserId { get; set; }         // user_id
        public string AnimalName { get; set; }  // animal_name
        public string? Date { get; set; }       // optional date string (yyyyMMdd) if needed
    }

    public class FeedDto
    {
        public int? FeedId { get; set; }       // feed_id
        public int UserId { get; set; }        // user_id
        public string FeedName { get; set; }   // feed_name
    }
}

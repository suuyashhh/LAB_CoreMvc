namespace Models.DairyFarm
{
    public class DTONotification
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; }
        public DateTime LastBreedingDate { get; set; }
        public int MonthNo { get; set; }
        public string Message { get; set; }
    }


}

namespace MyBoards.Entities
{
    public class Task : WorkItem
    {
        public string Activity { get; set; }
        public decimal RemainingWork { get; set; }
    }
}

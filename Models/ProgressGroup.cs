namespace UmatoMusume.Models
{
    public class ProgressGroup
    {
        public ProgressGroup() { }

        public ProgressGroup(int _current, int _total, string _message)
        {
            Current = _current;
            Total = _total;
            Message = _message;
        }

        public int Current { get; set; }

        public int Total { get; set; }

        public string Message { get; set; } = string.Empty;

        public (int, int, string) Deconstruct()
        {
            return (Current, Total, Message);
        }
    }
}
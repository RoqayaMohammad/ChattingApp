namespace ChattingApp.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;
        public int _PageSize = 10;
        public int PageSize
        {
            get => _PageSize; 
            set => _PageSize = (value > MaxPageSize)? MaxPageSize: value;
        }

        public string CurrentUsername { get; set; }

        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 90;

        public string OrderBy { get; set; } = "lastActive";
    }
}

namespace OrganizationApi.Models
{
    public class Path
    {
        public Path(int start, int end)
        {
            Start = start;
            End = end;
        }

        internal int Start { get; }
        internal int End { get; }

        public static implicit operator Path((int start, int end) path)
        {
            var (start, end) = path;
            return new Path(start, end);
        }
    }
}
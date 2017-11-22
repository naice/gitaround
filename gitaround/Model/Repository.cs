namespace gitaround.Model
{
    internal class Repository
    {
        public string Local { get; set; } = "localPath";
        public string CloneUrl { get; set; } = "";
        public string Remote { get; set; } = "origin";
        public string User { get; set; } = "";
    }
}
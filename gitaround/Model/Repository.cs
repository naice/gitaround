namespace gitaround.Model
{
    public class Repository
    {
        public string Local { get; set; } = "local path to project.";
        public string CloneUrl { get; set; } = "bitbucket, clone url of project.";
        public string Remote { get; set; } = "origin";
        public string User { get; set; } = "bitbucket, user name.";
    }
}
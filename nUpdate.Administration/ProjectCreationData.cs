namespace nUpdate.Administration
{
    public class ProjectCreationData
    {
        public string PrivateKey { get; set; }
        public UpdateProject Project { get; set; } = new UpdateProject();
    }
}

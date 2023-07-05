namespace VBoxManageUI
{
    public class VBoxHdd
    {
        public string UUID { get; set; }
        public string ParentUUID { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string Format { get; set; }
        public string Capacity { get; set; }
        public string Encryption { get; set; }
    }
}
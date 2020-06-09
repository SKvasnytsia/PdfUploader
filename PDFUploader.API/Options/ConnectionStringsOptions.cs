namespace PDFUploader.API.Options
{
    public enum ConnectionStringMode
    {
        Emulator,
        Azure
    }

    public class ConnectionStringsOptions
    {
        public ConnectionStringMode Mode { get; set; }
        public ConnectionStringOptions Azure { get; set; }
        public ConnectionStringOptions Emulator { get; set; }

        public ConnectionStringOptions ActiveConnectionStringOptions =>
            Mode == ConnectionStringMode.Azure ? Azure : Emulator;
    }
}
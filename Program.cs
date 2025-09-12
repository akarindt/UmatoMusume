namespace UmatoMusume
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Environment.SetEnvironmentVariable("SE_MANAGER", "0");
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain());
        }
    }
}
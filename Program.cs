using CSVImportExportApp.Data;
using System.Windows.Forms;

namespace CSVImportExportApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var context = new AppDbContext())
            {
                context.Database.EnsureCreated();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
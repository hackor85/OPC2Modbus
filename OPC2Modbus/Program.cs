using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using log4net.Config;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace OPC2Modbus
{
    static class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XmlConfigurator.Configure();

            Log.Info("Aplicación iniciada");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Opc2Modbus());
            Log.Info("Aplicación finalizada");
        }
    }
}

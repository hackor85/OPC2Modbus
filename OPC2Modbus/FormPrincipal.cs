using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using OPCAutomation;
using EasyModbus;

namespace OPC2Modbus
{

    public partial class Opc2Modbus : Form
    {
        private OPCServer ObjOPCServer;
        private Dictionary<int, OPCItem> Items = new Dictionary<int, OPCItem>();
        private OPCGroups ObjOPCGroups;
        private OPCGroup ObjOPCGroup;
        private ModbusServer modbusServer = new ModbusServer();
        private Boolean bExit = false;
        private Boolean bFatal = false;

        public Opc2Modbus()
        {
            InitializeComponent();
        }

        private bool ReadItemsCSV()
        {
            try
            {
                StreamReader reader = new StreamReader(File.OpenRead(@"..\..\etc\OPC2Modbus.csv"));
                reader.ReadLine();  // Ignore the first line.
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    String[] values = line.Split(',');

                    Items.Add(int.Parse(values[0]), new OPCItem { ItemID = values[1], Compressor = "C01", Description = values[2] });
                }
                return true;
            }
            catch (Exception ex)
            {
                OPC2Modbus.Program.Log.Fatal("Error al leer el archivo OPC2Modbus.csv. No existe o no tiene un formato correcto.", ex);
                MessageBox.Show("Error al leer el archivo OPC2Modbus.csv. No existe o no tiene un formato correcto.\n La aplicación va ha cerrarse.", "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bExit = true;
                bFatal = true;
                Application.Exit();
                return false;
            }
        }

        private void InitializeDictionary2()
        {
            
            int Comp = 0;
            Items.Add(Comp +  1, new OPCItem { ItemID = "M0032", Compressor = "C01", Description = "Suction gas temp" });
            Items.Add(Comp +  2, new OPCItem { ItemID = "M0033", Compressor = "C01", Description = "Discharge gas temp." });
            Items.Add(Comp +  3, new OPCItem { ItemID = "M0034", Compressor = "C01", Description = "Oil temp." });
            Items.Add(Comp +  4, new OPCItem { ItemID = "M0035", Compressor = "C01", Description = "Suction pressure" });
            Items.Add(Comp +  5, new OPCItem { ItemID = "M0036", Compressor = "C01", Description = "Discharge pressure" });
            Items.Add(Comp +  6, new OPCItem { ItemID = "M0037", Compressor = "C01", Description = "Suction pressure" });
            Items.Add(Comp +  7, new OPCItem { ItemID = "M0038", Compressor = "C01", Description = "Discharge pressure" });
            Items.Add(Comp +  8, new OPCItem { ItemID = "M0039", Compressor = "C01", Description = "Capacity position" });
            Items.Add(Comp +  9, new OPCItem { ItemID = "M0040", Compressor = "C01", Description = "Volume slide position (s)" });
            Items.Add(Comp + 10, new OPCItem { ItemID = "M0041", Compressor = "C01", Description = "Motor current" });
            Items.Add(Comp + 11, new OPCItem { ItemID = "M0042", Compressor = "C01", Description = "External input" });
            Items.Add(Comp + 12, new OPCItem { ItemID = "M0043", Compressor = "C01", Description = "Oil filter diff. pressure (s) / Intermediate pressure (r)" });
            Items.Add(Comp + 13, new OPCItem { ItemID = "M0044", Compressor = "C01", Description = "Suction gas superheat" });
            Items.Add(Comp + 14, new OPCItem { ItemID = "M0045", Compressor = "C01", Description = "Brine / intermediate temperature" });
            Items.Add(Comp + 15, new OPCItem { ItemID = "M0046", Compressor = "C01", Description = "Running hours" });
            Items.Add(Comp + 16, new OPCItem { ItemID = "M0047", Compressor = "C01", Description = "Oil pressure" });
            Items.Add(Comp + 17, new OPCItem { ItemID = "M0062", Compressor = "C01", Description = "Compressor mode" });
            Items.Add(Comp + 18, new OPCItem { ItemID = "M0063", Compressor = "C01", Description = "Compressor control mode" });
            Items.Add(Comp + 19, new OPCItem { ItemID = "M0064", Compressor = "C01", Description = "Sequence start number" });
            Items.Add(Comp + 20, new OPCItem { ItemID = "M0065", Compressor = "C01", Description = "System number" });
            Items.Add(Comp + 21, new OPCItem { ItemID = "M0066", Compressor = "C01", Description = "Ctrl. system (aggregate type)" });
            Items.Add(Comp + 22, new OPCItem { ItemID = "M0067", Compressor = "C01", Description = "Multisab State" });
            Items.Add(Comp + 23, new OPCItem { ItemID = "M0068", Compressor = "C01", Description = "Preceding compressor" });
            Items.Add(Comp + 24, new OPCItem { ItemID = "M0069", Compressor = "C01", Description = "Next compressor" });
            Items.Add(Comp + 25, new OPCItem { ItemID = "M0070", Compressor = "C01", Description = "Compressor to follow the next" });
            Items.Add(Comp + 26, new OPCItem { ItemID = "M0071", Compressor = "C01", Description = "Selected sys regulator" });
            Items.Add(Comp + 27, new OPCItem { ItemID = "M0073", Compressor = "C01", Description = "Liquid temperature T3" });
            Items.Add(Comp + 28, new OPCItem { ItemID = "M0080", Compressor = "C01", Description = "Alarms (Reciprocating / Screw)" });
            Items.Add(Comp + 29, new OPCItem { ItemID = "M0081", Compressor = "C01", Description = "Alarms (Reciprocating / Screw)" });
            Items.Add(Comp + 30, new OPCItem { ItemID = "M0082", Compressor = "C01", Description = "Alarms (Reciprocating / Screw)" });
            Items.Add(Comp + 31, new OPCItem { ItemID = "M0083", Compressor = "C01", Description = "Warnings (Reciprocating / Screw)" });
            Items.Add(Comp + 32, new OPCItem { ItemID = "M0084", Compressor = "C01", Description = "Warnings (Reciprocating / Screw)" });
            Items.Add(Comp + 33, new OPCItem { ItemID = "M0085", Compressor = "C01", Description = "Switch reg. SP1/ SP2" });
            Items.Add(Comp + 34, new OPCItem { ItemID = "M0086", Compressor = "C01", Description = "Set point actual, suction pressure" });
            Items.Add(Comp + 35, new OPCItem { ItemID = "M0170", Compressor = "C01", Description = "Compressor type" });
            Items.Add(Comp + 36, new OPCItem { ItemID = "M0171", Compressor = "C01", Description = "Refrigerant type" });
            Items.Add(Comp + 37, new OPCItem { ItemID = "M0172", Compressor = "C01", Description = "Regulation mode" });
        }
        
        private void Opc2Modbus_Load(object sender, EventArgs e)
        {
            if (ReadItemsCSV())
            {
                // Visualize only in nofification bar.
                this.Hide();
                this.WindowState = FormWindowState.Minimized;

                //Subscrib to OPC Server
                try
                {
                    ObjOPCServer = new OPCServer();

                    //Conect to the server
                    ObjOPCServer.Connect("POPCS.DAServer.1");

                    //Create a group
                    ObjOPCGroups = ObjOPCServer.OPCGroups;
                    ObjOPCGroup = ObjOPCGroups.Add("C01");

                    //Define event DataChange
                    ObjOPCGroup.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(OPCGroup_DataChange);

                    //Define DefaultAccessPath (necessary for POPCS.DAServer.1)
                    ObjOPCGroup.OPCItems.DefaultAccessPath = "C01";

                    //Define Items.
                    for (int i = 1; i <= 37; i++ )
                        ObjOPCGroup.OPCItems.AddItem(Items[i].ItemID, i);
                    
                    //Group properties
                    ObjOPCGroup.UpdateRate = 1000;
                    ObjOPCGroup.IsActive = true;
                    ObjOPCGroup.IsSubscribed = true;
                }
                catch (Exception ex)
                {
                    OPC2Modbus.Program.Log.Fatal("Error al suscribirse al servidor OPC", ex);
                    MessageBox.Show("Error al suscribirse al servidor OPC.\n La aplicación va ha cerrarse.", "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bExit = true;
                    bFatal = true;
                    Application.Exit();
                }

                // Start modbus server.
                try
                {
                    modbusServer.Listen();
                }
                catch (Exception ex)
                {
                    OPC2Modbus.Program.Log.Fatal("Error al iniciar el servidor Modbus TCP", ex);
                    MessageBox.Show("Error al iniciar el servidor Modbus TCP.\n La aplicación va ha cerrarse.", "Error fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bExit = true;
                    bFatal = true;
                    Application.Exit();
                }
            }
        }

        private void Opc2Modbus_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState) this.Hide();
        }

        private void Opc2Modbus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bFatal)
            {
                if (bExit)
                {
                    if (MessageBox.Show("¿Seguro que quiere dejar de ejecutar OPC2Modbus?", "Salir de la aplicación", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        bExit = false;
                    }
                }
                else
                {
                    MessageBox.Show("OPC2Modbus va seguir ejecutándose en la barra de tareas.", "Aviso de funcionamiento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Hide();
                    this.WindowState = FormWindowState.Minimized;
                    e.Cancel = true;
                }
            }
        }

        private void OPCGroup_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                try
                {
                    modbusServer.holdingRegisters[(int)ClientHandles.GetValue(i)] = (short)ItemValues.GetValue(i);
                }
                catch (Exception ex)
                {
                    OPC2Modbus.Program.Log.Error("Parámetro [" + Items[i].ItemID + ": " + Items[i].Compressor + " - " + Items[i].Description + "]; Valor [" + ItemValues.GetValue(i) + "]", ex);
                }
            }   
        }

        private void configToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bExit = true;
            Application.Exit();
        }

    }
}

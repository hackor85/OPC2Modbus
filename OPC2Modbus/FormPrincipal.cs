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
        public OPCServer ObjOPCServer;
        public OPCGroups ObjOPCGroups;
        public OPCGroup[] ObjOPCGroup;
        public ModbusServer modbusServer = new ModbusServer();
        public Dictionary<int, OPCItem> Items = new Dictionary<int, OPCItem>();
        public Boolean bExit = false;
        public Boolean bFatal = false;
        public Boolean bDebug = false;
        public int Compressors = 9;
        public int numItems = 100;
        public int numItemsCSV = 0;

        public Opc2Modbus()
        {
            InitializeComponent();

        }

        private bool ReadItemsCSV()
        {
            try
            {
                //Read the items in the config file.
                StreamReader reader = new StreamReader(File.OpenRead(@"..\..\etc\OPC2Modbus.csv"));
                reader.ReadLine();  // Ignore the first line.
                while (!reader.EndOfStream)
                {
                    String line = reader.ReadLine();
                    String[] values = line.Split(',');

                    Items.Add(int.Parse(values[0]), new OPCItem { ItemID = values[1], Compressor = "C01", Description = values[2] });
                    numItemsCSV++;
                }
                reader.Close();
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

            //Initialize the rest of compressors
            for (int c = 1; c < Compressors; c++)
            {
                for (int i = 1; i <= numItemsCSV; i++)
                {
                    Items.Add(c * numItems + i, new OPCItem { ItemID = Items[i].ItemID, Compressor = "C" + (c + 1).ToString("00"), Description = Items[i].Description });
                }
            }
            return true;
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
                    ObjOPCGroup = new OPCGroup[Compressors];
                    String GroupName;

                    //Conect to the server
                    ObjOPCServer.Connect("POPCS.DAServer.1");

                    //Create a group
                    ObjOPCGroups = ObjOPCServer.OPCGroups;

                    for (int c = 0; c < Compressors; c++)
                    {
                        GroupName = "C" + (c + 1).ToString("00");
                        ObjOPCGroup[c] = ObjOPCGroups.Add(GroupName);

                        //Define event DataChange
                        ObjOPCGroup[c].DataChange += new DIOPCGroupEvent_DataChangeEventHandler(OPCGroup_DataChange);

                        //Define DefaultAccessPath (necessary for POPCS.DAServer.1)
                        ObjOPCGroup[c].OPCItems.DefaultAccessPath = GroupName;

                        //Define Items.
                        for (int i = 1; i <= numItemsCSV; i++)
                            ObjOPCGroup[c].OPCItems.AddItem(Items[(c * numItems) + i].ItemID, (c * numItems) + i);

                        //Group properties
                        ObjOPCGroup[c].UpdateRate = 1000;
                        ObjOPCGroup[c].IsActive = true;
                        ObjOPCGroup[c].IsSubscribed = true;
                    }
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
                    modbusServer.holdingRegisters[(int)ClientHandles.GetValue(i)] = short.Parse(ItemValues.GetValue(i).ToString());
                    if (bDebug)
                        OPC2Modbus.Program.Log.Debug("Parámetro [" + Items[i].ItemID + ": " + Items[i].Compressor + " - " + Items[i].Description + "]; Valor [" + ItemValues.GetValue(i) + "]");
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

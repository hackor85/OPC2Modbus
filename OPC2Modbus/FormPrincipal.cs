using System;
using System.Windows.Forms;
using OPCAutomation;
using EasyModbus;

namespace OPC2Modbus
{
    public partial class Opc2Modbus : Form
    {
        String OPCServerName;
        OPCServer ObjOPCServer;
        OPCGroups ObjOPCGroups;
        OPCGroup ObjOPCGroupC01;
        OPCGroup ObjOPCGroupC02;
        OPCGroup ObjOPCGroupC03;
        OPCGroup ObjOPCGroupC04;
        ModbusServer modbusServer = new ModbusServer();
        private Boolean bExit = false;

        public Opc2Modbus()
        {
            InitializeComponent();
        }

        private void Opc2Modbus_Load(object sender, EventArgs e)
        {
            this.Hide();
            this.WindowState = FormWindowState.Minimized;
            //OPCServerName = "POPCS.DAServer.1";
            OPCServerName = "Matrikon.OPC.Simulation.1";
            ObjOPCServer = new OPCServer();
            try
            {
                ObjOPCServer.Connect(OPCServerName);
                ObjOPCGroups = ObjOPCServer.OPCGroups;
                ObjOPCGroupC01 = ObjOPCGroups.Add("C01");
                //ObjOPCGroupC02 = ObjOPCGroups.Add("C02");
                //ObjOPCGroupC03 = ObjOPCGroups.Add("C03");
                //ObjOPCGroupC04 = ObjOPCGroups.Add("C04");

                ObjOPCGroupC01.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(ObjOPCGroupC01_DataChange);
                //ObjOPCGroupC02.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(ObjOPCGroupC02_DataChange);
                //ObjOPCGroupC03.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(ObjOPCGroupC03_DataChange);
                //ObjOPCGroupC04.DataChange += new DIOPCGroupEvent_DataChangeEventHandler(ObjOPCGroupC04_DataChange);

                ObjOPCGroupC01.OPCItems.AddItem("Bucket Brigade.Int1", 1);
                ObjOPCGroupC01.OPCItems.AddItem("Bucket Brigade.Int2", 2);
                ObjOPCGroupC01.OPCItems.AddItem("Bucket Brigade.Int4", 3);
                ObjOPCGroupC01.OPCItems.AddItem("Bucket Brigade.Money", 4);
                /*ObjOPCGroupC01.OPCItems.DefaultAccessPath = "C01";
                ObjOPCGroupC01.OPCItems.AddItem("M0032", 1);
                ObjOPCGroupC01.OPCItems.AddItem("M0033", 2);
                ObjOPCGroupC01.OPCItems.AddItem("M0034", 3);
                ObjOPCGroupC01.OPCItems.AddItem("M0035", 4);
                ObjOPCGroupC01.OPCItems.AddItem("M0036", 5);
                ObjOPCGroupC01.OPCItems.AddItem("M0037", 6);
                ObjOPCGroupC01.OPCItems.AddItem("M0038", 7);
                ObjOPCGroupC01.OPCItems.AddItem("M0039", 8);
                ObjOPCGroupC01.OPCItems.AddItem("M0040", 9);
                ObjOPCGroupC01.OPCItems.AddItem("M0041", 10);
                ObjOPCGroupC01.OPCItems.AddItem("M0042", 11);
                ObjOPCGroupC01.OPCItems.AddItem("M0043", 12);
                ObjOPCGroupC01.OPCItems.AddItem("M0044", 13);
                ObjOPCGroupC01.OPCItems.AddItem("M0045", 14);
                ObjOPCGroupC01.OPCItems.AddItem("M0046", 15);
                ObjOPCGroupC01.OPCItems.AddItem("M0047", 16);
                ObjOPCGroupC01.OPCItems.AddItem("M0062", 17);
                ObjOPCGroupC01.OPCItems.AddItem("M0063", 18);
                ObjOPCGroupC01.OPCItems.AddItem("M0064", 19);
                ObjOPCGroupC01.OPCItems.AddItem("M0065", 20);
                ObjOPCGroupC01.OPCItems.AddItem("M0066", 21);
                ObjOPCGroupC01.OPCItems.AddItem("M0067", 22);
                ObjOPCGroupC01.OPCItems.AddItem("M0068", 23);
                ObjOPCGroupC01.OPCItems.AddItem("M0069", 24);
                ObjOPCGroupC01.OPCItems.AddItem("M0070", 25);
                ObjOPCGroupC01.OPCItems.AddItem("M0071", 26);
                ObjOPCGroupC01.OPCItems.AddItem("M0073", 27);
                ObjOPCGroupC01.OPCItems.AddItem("M0080", 28);
                ObjOPCGroupC01.OPCItems.AddItem("M0081", 29);
                ObjOPCGroupC01.OPCItems.AddItem("M0082", 30);
                ObjOPCGroupC01.OPCItems.AddItem("M0083", 31);
                ObjOPCGroupC01.OPCItems.AddItem("M0084", 32);
                ObjOPCGroupC01.OPCItems.AddItem("M0085", 33);
                ObjOPCGroupC01.OPCItems.AddItem("M0086", 34);
                ObjOPCGroupC01.OPCItems.AddItem("M0170", 35);
                ObjOPCGroupC01.OPCItems.AddItem("M0171", 36);
                ObjOPCGroupC01.OPCItems.AddItem("M0172", 37);*/

                ObjOPCGroupC01.UpdateRate = 1000;
                ObjOPCGroupC01.IsActive = true;
                ObjOPCGroupC01.IsSubscribed = true;
                /*ObjOPCGroupC02.UpdateRate = 1000;
                ObjOPCGroupC02.IsActive = true;
                ObjOPCGroupC02.IsSubscribed = true;
                ObjOPCGroupC03.UpdateRate = 1000;
                ObjOPCGroupC03.IsActive = true;
                ObjOPCGroupC03.IsSubscribed = true;
                ObjOPCGroupC04.UpdateRate = 1000;
                ObjOPCGroupC04.IsActive = true;
                ObjOPCGroupC04.IsSubscribed = true;*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
            modbusServer.Listen();
        }

        private void ObjOPCGroupC01_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                try
                {
                    //MessageBox.Show(ClientHandles.GetValue(i).ToString() + " -> " + ItemValues.GetValue(i).ToString());
                    modbusServer.holdingRegisters[(int)ClientHandles.GetValue(i)] = (short)ItemValues.GetValue(i);
                    //MessageBox.Show(modbusServer.holdingRegisters[(int)ClientHandles.GetValue(i)].ToString());
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.ToString());
                }
            }   
        }

        private void ObjOPCGroupC02_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                if ((Convert.ToInt32(ClientHandles.GetValue(1)) == 1))
                {
                    modbusServer.holdingRegisters[3] = (short)ItemValues.GetValue(1);
                }
                if ((Convert.ToInt32(ClientHandles.GetValue(2)) == 1))
                {
                    modbusServer.holdingRegisters[4] = (short)ItemValues.GetValue(2);
                }
            }
        }

        private void ObjOPCGroupC03_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                if ((Convert.ToInt32(ClientHandles.GetValue(5)) == 1))
                {
                    modbusServer.holdingRegisters[3] = (short)ItemValues.GetValue(1);
                }
                if ((Convert.ToInt32(ClientHandles.GetValue(6)) == 1))
                {
                    modbusServer.holdingRegisters[4] = (short)ItemValues.GetValue(2);
                }
            }
        }

        private void ObjOPCGroupC04_DataChange(int TransactionID, int NumItems, ref Array ClientHandles, ref Array ItemValues, ref Array Qualities, ref Array TimeStamps)
        {
            for (int i = 1; i <= NumItems; i++)
            {
                if ((Convert.ToInt32(ClientHandles.GetValue(7)) == 1))
                {
                    modbusServer.holdingRegisters[3] = (short)ItemValues.GetValue(1);
                }
                if ((Convert.ToInt32(ClientHandles.GetValue(8)) == 1))
                {
                    modbusServer.holdingRegisters[4] = (short)ItemValues.GetValue(2);
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

        private void Opc2Modbus_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState) this.Hide();
        }

        private void Opc2Modbus_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bExit)
            {
                MessageBox.Show("OPC2Modbus va seguir ejecutándose en la barra de tareas.", "Aviso de funcionamiento", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Hide();
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }
            else
            {
                if (MessageBox.Show("¿Seguro que quiere dejar de ejecutar OPC2Modbus?", "Salir de la aplicación", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    bExit = false;
                }
            }
        }

    }
}

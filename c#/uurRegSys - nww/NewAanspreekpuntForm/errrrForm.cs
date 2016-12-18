﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using NewCrossFunctions;

namespace NewAanspreekpuntForm {
    public partial class ErrrrForm : Form {
        public ErrrrForm(string _password, string _username, string _apiadrres) {
            InitializeComponent();
            _Password=_password;
            _Username=_username;
            _ApiAddres=_apiadrres;
        }

        private delegate void updateOverzichtDelegate();
        List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry> _LastRecivedOverzight = new List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry>();
        DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry _CurrentlySelectedUser = new DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry();
        bool _NOODMODUSENABLED = false;
        bool _IsInUpdateDontClearEditScreen = false;
        Timer _TimerReloadOverzicht = new Timer();

        string _Password = "";
        string _Username = "";
        string _ApiAddres = "";

        void enableNoodModus() {
            _NOODMODUSENABLED=true;
            buttonDisableNoodMode.Visible=true;
            panel2.Visible=false;
            this.BackColor=Color.Red;
            _TimerReloadOverzicht.Stop();
        }

        void disableNoodModus() {
            _NOODMODUSENABLED=false;
            buttonDisableNoodMode.Visible=false;
            panel2.Visible=true;
            this.BackColor=Color.Yellow; //times are outdated
            _TimerReloadOverzicht.Start();
            ReloadOverzight();
        }

        void disableEditControls() {
            buttonTekenIn.Enabled=false;
            buttonTekenUit.Enabled=false;
            buttonSave.Enabled=false;
            dateTimePickerTijdIn.Enabled=false;
            dateTimePickerTimeUit.Enabled=false;
            comboBoxRedenAfwezig.Enabled=false;
            textBoxOpmerking.Text="";
            textBoxOpmerking.Enabled=false;
            checkBoxHeefAfwezigReden.Checked=false;
            dateTimePickerVerwachteTijdVanAankomst.Enabled=false;
        }

        private void ErrrrForm_Load(object sender, EventArgs e) {
            _TimerReloadOverzicht.Interval=42000;
            _TimerReloadOverzicht.Tick+=new EventHandler(ReloadOverzight_Event);

            dateTimePickerTijdIn.ShowUpDown=true;
            dateTimePickerTijdIn.CustomFormat="HH:mm:ss";
            dateTimePickerTijdIn.Format=System.Windows.Forms.DateTimePickerFormat.Custom;

            dateTimePickerTimeUit.ShowUpDown=true;
            dateTimePickerTimeUit.CustomFormat="HH:mm:ss";
            dateTimePickerTimeUit.Format=System.Windows.Forms.DateTimePickerFormat.Custom;

            dateTimePickerVerwachteTijdVanAankomst.ShowUpDown=true;
            dateTimePickerVerwachteTijdVanAankomst.CustomFormat="HH:mm:ss";
            dateTimePickerVerwachteTijdVanAankomst.Format=System.Windows.Forms.DateTimePickerFormat.Custom;

            ReloadOverzight();
        }

        private NetComunicationTypesAndFunctions.ServerResponse webbbbrrrrrry(object request) {
            return NetComunicationTypesAndFunctions.WebRequest(request, _Username, _Password, _ApiAddres);
        }

        void ReloadOverzight_Event(object ja, object nee) {
            _IsInUpdateDontClearEditScreen=true;
            BeginInvoke(new updateOverzichtDelegate(ReloadOverzight));
            _IsInUpdateDontClearEditScreen=false;
        }

        void ReloadOverzight() {
            ReloadOverzight(true);
        }

        void ReloadOverzight(bool reloadFromServer) {
            if (!_NOODMODUSENABLED) {
                try {
                    _TimerReloadOverzicht.Stop();
                    NetComunicationTypesAndFunctions.ServerRequestOverzightFromOneDate request = new NetComunicationTypesAndFunctions.ServerRequestOverzightFromOneDate();
                    if (checkBoxSeUserTodayAsDate.Checked) {
                        request.useToday=true;
                    } else {
                        request.useToday=false;
                        request.dateToGetOverzightFrom=dateTimePickerSeDateToListTo.Value;
                    }
                    NetComunicationTypesAndFunctions.ServerResponse response;
                    try {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        response=webbbbrrrrrry(request);
                        label2.Text=sw.ElapsedMilliseconds.ToString();
                    } catch { // als server down is (als school in brand staat...)
                        if (MessageBox.Show("Kan Niet Verbinden Met Server", "Ga Naar Alarm Modus?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)==DialogResult.Yes) {
                            enableNoodModus();
                        } else {
                            this.BackColor=Color.Yellow;//times are outdated
                        }
                        _TimerReloadOverzicht.Start();
                        return;
                    }
                    this.BackColor=SystemColors.Control;
                    NetComunicationTypesAndFunctions.ServerResponseOverzightFromOneDate returnedValue;
                    if (response.IsErrorOcurred) {
                        throw new Exception(response.ErrorInfo.ErrorMessage);
                    } else {
                        //--
                        int _SelectedRowNummber = 0;
                        if (dataGridView1.CurrentCell!=null) {
                            _SelectedRowNummber=dataGridView1.CurrentCell.RowIndex;
                        }
                        ListSortDirection _oldSortOrder;
                        DataGridViewColumn _oldSortCol;
                        _oldSortOrder=dataGridView1.SortOrder==SortOrder.Ascending ?
                         ListSortDirection.Ascending : ListSortDirection.Descending;
                        _oldSortCol=dataGridView1.SortedColumn;
                        ///--
                        if (reloadFromServer) {
                            returnedValue=JsonConvert.DeserializeObject<NetComunicationTypesAndFunctions.ServerResponseOverzightFromOneDate>(JsonConvert.SerializeObject(response.Response));
                            _LastRecivedOverzight=returnedValue.EtList;
                        } else {
                            returnedValue=new NetComunicationTypesAndFunctions.ServerResponseOverzightFromOneDate();
                            returnedValue.EtList=_LastRecivedOverzight;
                        }
                        if (textBoxZoekOp.Text.Trim()=="") {
                            dataGridView1.DataSource=ForFormHelperFunctions.UserInfoListToDataTableForDataGridDisplay(returnedValue.EtList, returnedValue.SQlDateTime);
                            dataGridView1.Refresh();
                        } else { //zoek op
                            List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry> sortedList = new List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry>();
                            foreach (DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry perso in returnedValue.EtList) {
                                if (perso.userN.VoorNaam.Contains(textBoxZoekOp.Text.Trim())||perso.userN.AchterNaam.Contains(textBoxZoekOp.Text.Trim())) {
                                    sortedList.Add(perso);
                                }
                            }
                            dataGridView1.DataSource=ForFormHelperFunctions.UserInfoListToDataTableForDataGridDisplay(sortedList, returnedValue.SQlDateTime);
                            dataGridView1.Refresh();
                        }
                        //--
                        if (_oldSortCol!=null) {
                            DataGridViewColumn newCol = dataGridView1.Columns[_oldSortCol.Name];
                            dataGridView1.Sort(newCol, _oldSortOrder);
                        }
                        try {// voor als row[x] er niet (meer) is
                            if (dataGridView1.CurrentCell!=null) {
                                dataGridView1.CurrentCell=dataGridView1[1, _SelectedRowNummber];
                            }
                        } catch {
                            dataGridView1.ClearSelection();
                        }
                        ///--
                        dataGridView1.Columns[0].Width=130;
                        dataGridView1.Columns[1].Width=130;
                        dataGridView1.Columns[4].Width=dataGridView1.Width-dataGridView1.Columns[0].Width-dataGridView1.Columns[1].Width-dataGridView1.Columns[2].Width-dataGridView1.Columns[3].Width-3-20;
                        _TimerReloadOverzicht.Start();
                    }
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                    _TimerReloadOverzicht.Start();
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e) {
            if (!_IsInUpdateDontClearEditScreen) { // reload overzicht timer
                disableEditControls();
                if (dataGridView1.SelectedRows.Count==0) { return; }
                DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry selectedUserData = new DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry();
                string _voorNaam = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                string _achterNaam = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                bool found = false;
                foreach (DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry entry in _LastRecivedOverzight) {
                    if (entry.userN.VoorNaam==_voorNaam&&entry.userN.AchterNaam==_achterNaam) { selectedUserData=entry; found=true; break; }
                }
                if (!found) { MessageBox.Show("Cant Find Selected User"); return; } //bleh            
                _CurrentlySelectedUser=selectedUserData;
                labelVoorNaam.Text=selectedUserData.userN.VoorNaam;
                labelAchterNaam.Text=selectedUserData.userN.AchterNaam;

                buttonSave.Enabled=true;
                textBoxOpmerking.Enabled=true;
                //enable buttons / set values
               

                if (selectedUserData.hasTodayRegEntry) {
                    buttonSave.Enabled=true;

                    //select item in dropdown
                    bool erIsEenAfwezigNotatie = true;
                    if (selectedUserData.regE.IsZiek) {
                        comboBoxRedenAfwezig.SelectedItem=comboBoxRedenAfwezig.Items[2];
                    } else
                    if (selectedUserData.regE.IsFlexiebelverlof) {
                        comboBoxRedenAfwezig.SelectedItem=comboBoxRedenAfwezig.Items[4];
                    } else
                    if (selectedUserData.regE.IsStudieverlof) {
                        comboBoxRedenAfwezig.SelectedItem=comboBoxRedenAfwezig.Items[3];
                    } else
                    if (selectedUserData.regE.IsExcurtie) {
                        comboBoxRedenAfwezig.SelectedItem=comboBoxRedenAfwezig.Items[5];
                    } else
                    if (selectedUserData.regE.IsLaat) {
                        comboBoxRedenAfwezig.SelectedItem=comboBoxRedenAfwezig.Items[1];
                        dateTimePickerVerwachteTijdVanAankomst.Value=Convert.ToDateTime(selectedUserData.regE.Verwachtetijdvanaanwezighijd.ToString("hh\\:mm\\:ss"));
                    } else
                    if (selectedUserData.regE.IsAndereReden) {
                        comboBoxRedenAfwezig.SelectedItem=comboBoxRedenAfwezig.Items[6];
                        textBoxOpmerking.Text=selectedUserData.regE.Opmerking;
                    } else {
                        erIsEenAfwezigNotatie=false;
                    }

                    if (erIsEenAfwezigNotatie) {
                        checkBoxHeefAfwezigReden.Checked=true;
                        textBoxOpmerking.Enabled=true;
                    }

                    if (selectedUserData.regE.HeeftIngetekend) {
                        dateTimePickerTijdIn.Enabled=true;
                        dateTimePickerTijdIn.Value=Convert.ToDateTime(selectedUserData.regE.TimeInteken.ToString("hh\\:mm\\:ss"));
                        if (selectedUserData.regE.IsAanwezig) {
                            buttonTekenUit.Enabled=true;
                        } else {
                            dateTimePickerTimeUit.Enabled=true;
                            dateTimePickerTimeUit.Value=Convert.ToDateTime(selectedUserData.regE.TimeUitteken.ToString("hh\\:mm\\:ss"));
                            buttonTekenIn.Enabled=true; // anulleer uitteken
                        }
                    }
                } else {
                    buttonTekenIn.Enabled=true;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            ReloadOverzight();
        }

        private void textBoxZoekOp_TextChanged(object sender, EventArgs e) {
            ReloadOverzight(false);
        }

        private void checkBoxSeUserTodayAsDate_CheckedChanged(object sender, EventArgs e) {
            if (checkBoxSeUserTodayAsDate.Checked) {
                dateTimePickerSeDateToListTo.Enabled=false;
            } else {
                dateTimePickerSeDateToListTo.Enabled=true;
            }
        }

        private void tekenInOfUit(object sender, EventArgs e) {
            NetComunicationTypesAndFunctions.ServerRequestTekenInOfUit request = new NetComunicationTypesAndFunctions.ServerRequestTekenInOfUit();
            request.NFCCode=_CurrentlySelectedUser.userN.NFCID;
            NetComunicationTypesAndFunctions.ServerResponse response = webbbbrrrrrry(request);
            if (response.IsErrorOcurred) {
                MessageBox.Show(response.ErrorInfo.ErrorMessage);
            } else {
                ReloadOverzight();
            }
        }

        private void checkBoxHeefAfwezigReden_CheckedChanged(object sender, EventArgs e) {
            comboBoxRedenAfwezig.Enabled=true;
        }

        private void comboBoxRedenAfwezig_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBoxRedenAfwezig.SelectedItem.ToString()=="Laat"&&comboBoxRedenAfwezig.Enabled==true) {
                dateTimePickerVerwachteTijdVanAankomst.Enabled=true;
            } else {
                dateTimePickerVerwachteTijdVanAankomst.Enabled=false;
            }

        }
    }
}

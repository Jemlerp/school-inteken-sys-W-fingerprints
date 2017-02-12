﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NewCrossFunctions;

namespace NewAdmin {
    public partial class IrrrrForm : Form {

        public IrrrrForm(DateTime _dateToday, string _username, string _password, string _address) {
            InitializeComponent();
            _UserName = _username;
            _Password = _password;
            _UserName = _username;
            _DateToday = _dateToday;
            _Address = _address;
        }

        DateTime _DateToday;
        string _Password = "";
        string _Address = "";
        string _UserName = "";

        //uuren - edit users
        List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry> _ResievedUserListWithRegEntrys = new List<DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry>();
        DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry _CurrentlySelectedUurensUser = new DatabaseTypesAndFunctions.CombineerUserEntryRegEntryAndAfwezigEntry();


        //edit acounts
        /*
         * list<databaseypes.acoujnts> _resivedAcountList
         * 
         * databaseypes.acoujnts currentleyselectedacount = new databaseypes();
         */

        private NetComunicationTypesAndFunctions.ServerResponse webbbbrrrrrry(object request) {
            return NetComunicationTypesAndFunctions.WebRequest(request, _UserName, _Password, _Address);
        }

        public DateTime GetDateTimeFromServer() {
            NetComunicationTypesAndFunctions.ServerResponse response = webbbbrrrrrry(new NetComunicationTypesAndFunctions.ServerRequestSqlDateTime());
            if (response.IsErrorOccurred) {
                throw new Exception(response.ErrorInfo.ErrorMessage);
            } else {
                return JsonConvert.DeserializeObject<DateTime>(JsonConvert.SerializeObject(response.Response));
            }
        } //unsafe

        private void IrrrrForm_Resize(object sender, EventArgs e) {
            // resize all datagrids
        }

        private void IrrrrForm_Load(object sender, EventArgs e) {
            MakeUurenTabUseable();
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e) {
            switch (tabControl1.SelectedIndex) {
                case 1: // uuren
                    MakeUurenTabUseable();
                    break;
                case 2: // edit users
                    MakeEditUserTabUseable();
                    break;
                case 3: // edit acounts

                    break;
            }
        }

        #region uuren
        void MakeUurenTabUseable() {
            updateUurenDataGrid();
        }

        void updateUurenDataGrid() {
            NetComunicationTypesAndFunctions.ServerRequestOverzightFromOneDate request = new NetComunicationTypesAndFunctions.ServerRequestOverzightFromOneDate();
            request.dateToGetOverzightFrom = dateTimePickerUurenDatumVList.Value;
            request.useToday = false;
            request.alsoReturnExUsers = checkBoxUurenShowExUsers.Checked;

            NetComunicationTypesAndFunctions.ServerResponseOverzightFromOneDate overzight = new NetComunicationTypesAndFunctions.ServerResponseOverzightFromOneDate();

            try {
                NetComunicationTypesAndFunctions.ServerResponse serverResponse = webbbbrrrrrry(request);
                if (serverResponse.IsErrorOccurred) {
                    throw new Exception(serverResponse.ErrorInfo.ErrorMessage);
                } else {
                    overzight = JsonConvert.DeserializeObject<NetComunicationTypesAndFunctions.ServerResponseOverzightFromOneDate>(JsonConvert.SerializeObject(serverResponse.Response));
                    _ResievedUserListWithRegEntrys = overzight.EtList;
                }
            } catch (Exception ex) {
                if (MessageBox.Show(ex.Message, "Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry) {
                    updateUurenDataGrid();
                    return;
                }
            }

            dataGridViewUuren.DataSource = ForFormHelperFunctions.UserInfoListToDataTableForDataGridDisplay(overzight.EtList, GetDateTimeFromServer());
            dataGridViewUuren.Refresh();
        }

        private void dataGridViewUuren_SelectionChanged(object sender, EventArgs e) {
            if (dataGridViewUuren.SelectedRows.Count == 0) { return; }

            //get idfiers
            string voorNaam = dataGridViewUuren.SelectedRows[0].Cells[0].Value.ToString();
            string achterNaam = dataGridViewUuren.SelectedRows[0].Cells[1].Value.ToString();

            //zoek in resieveduserlisetwithregentrys en set currently selected user
            foreach (var i in _ResievedUserListWithRegEntrys) {
                if (i.UsE.VoorNaam == voorNaam && i.UsE.AchterNaam == achterNaam) {
                    _CurrentlySelectedUurensUser = i;
                    break;
                }
            }

            //clear inputs
            checkBoxUurenVermeldAfwezig.Checked = false;
            checkBoxUurenHeeftIngetekend.Checked = false;
            checkBoxUurenIsAanwezig.Checked = false;
            labelUurenRID.Text = "";
            labelUurenUID.Text = "";
            textBoxUurenOpmerking.Text = "";

            //output to inputs
            if (_CurrentlySelectedUurensUser.hasTodayRegEntry) {
                try {
                    checkBoxUurenHeeftIngetekend.Checked = _CurrentlySelectedUurensUser.RegE.HeeftIngetekend;
                } catch { }
                try {
                    checkBoxUurenIsAanwezig.Checked = _CurrentlySelectedUurensUser.RegE.IsAanwezig;
                } catch { }
                try {
                    dateTimePickerUurenTijdIn.Value = (new DateTime()) + _CurrentlySelectedUurensUser.RegE.TimeInteken;
                } catch { }
                try {
                    dateTimePickerUurenTijdUit.Value = (new DateTime()) + _CurrentlySelectedUurensUser.RegE.TimeUitteken;
                } catch { }
                try {
                    textBoxUurenOpmerking.Text = _CurrentlySelectedUurensUser.RegE.Opmerking;
                } catch { }
                try {
                    labelUurenRID.Text = _CurrentlySelectedUurensUser.RegE.ID.ToString();
                } catch { }
                try {
                    labelUurenUID.Text = _CurrentlySelectedUurensUser.UsE.ID.ToString();
                } catch { }

                if (_CurrentlySelectedUurensUser.RegE.IsZiek) {
                    comboBoxUurenAfwezighijdreden.SelectedItem = comboBoxUurenAfwezighijdreden.Items[0];
                    checkBoxUurenVermeldAfwezig.Checked = true;
                }
                if (_CurrentlySelectedUurensUser.RegE.IsStudieverlof) {
                    comboBoxUurenAfwezighijdreden.SelectedItem = comboBoxUurenAfwezighijdreden.Items[1];
                    checkBoxUurenVermeldAfwezig.Checked = true;
                }
                if (_CurrentlySelectedUurensUser.RegE.IsFlexiebelverlof) {
                    comboBoxUurenAfwezighijdreden.SelectedItem = comboBoxUurenAfwezighijdreden.Items[2];
                    checkBoxUurenVermeldAfwezig.Checked = true;
                }
                if (_CurrentlySelectedUurensUser.RegE.IsExcurtie) {
                    comboBoxUurenAfwezighijdreden.SelectedItem = comboBoxUurenAfwezighijdreden.Items[3];
                    checkBoxUurenVermeldAfwezig.Checked = true;
                }
            }


        }
        #endregion

        // edit user
        void MakeEditUserTabUseable() {

        }


    }
}

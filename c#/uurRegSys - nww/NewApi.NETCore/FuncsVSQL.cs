﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using NewCrossFunctions.NETCore;
using System.Data;
using Newtonsoft.Json;

namespace NewApi.NETCore {
    public static class FuncsVSQL {

        public static string _ConnectionString = "Server=DESKTOP-RAR7FQP\\SQLEXPRESS; Database=IntekenSysteem; User Id=sa; password=kanker;";
        //public static string _ConnectionString = "Server=SHISHIDOU-PC\\SQLEXPRESS; Database=IntekenSysteem; User Id=sa; password=kanker;";

        public static int SQLNonQuery(string _command) {
            SqlCommand command = new SqlCommand();
            command.CommandText = _command;
            return SQLNonQuery(command);
        }

        public static int SQLNonQuery(SqlCommand _command) {
            using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString)) {
                try {
                    _command.Connection = sqlConnection;
                    sqlConnection.Open();
                    return _command.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception($"ERROR @ SQL Command: {_command.CommandText} | Message: {ex.Message}");
                }
            }
        }

        /* illegal
        public static SqlDataReader SQLQuery(string _command)
        {
            SqlCommand command = new SqlCommand();
            command.CommandText = _command;
            return SQLQuery(command);
        }

        public static SqlDataReader SQLQuery(SqlCommand _command)
        {
            SqlConnection sqlConnection = new SqlConnection(_ConnectionString);
            try
            {
                _command.Connection = sqlConnection;
                sqlConnection.Open();
                return _command.ExecuteReader();
                //close? dispose?
            }
            catch (Exception ex)
            {
                sqlConnection.Dispose();
                throw new Exception($"ERROR @ SQL Command: {_command.CommandText} | Message: {ex.Message}");
            }
        }

    */

        //datatable afwezig in .netcore......

        private static T ReadFromReader<T>(IDataRecord _record, string _readProp) {
            if (_record.IsDBNull(_record.GetOrdinal(_readProp))) { return default(T); }
            return (T)_record.GetValue(_record.GetOrdinal(_readProp));
        }

        private static DateTime ReadDateTimeFromReader(IDataRecord _record, string _readProp) {
            if (_record.IsDBNull(_record.GetOrdinal(_readProp))) { return new DateTime(); }
            return DateTime.Parse(_record.GetValue(_record.GetOrdinal(_readProp)).ToString());
        }


        public static DateTime GetDateTimeFromSQLServer() {
            try {
                using (SqlConnection sqlConnection = new SqlConnection(_ConnectionString)) {
                    SqlCommand command = new SqlCommand();
                    command.CommandText = "select getdate() jikan";
                    command.Connection = sqlConnection;
                    sqlConnection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read()) {
                        return ((IDataRecord)reader).GetDateTime(reader.GetOrdinal("jikan"));
                    }
                }
            } catch (Exception ex) {
                throw new Exception($"ERROR @ SQL Command: select getdate() jikan | Message: {ex.Message}");
            }
            throw new Exception("dit kan niet~~ FuncdVSQSL.GetDateTimeFromSQlServer()");
        }


        public static List<DatabaseObjects.AcountTableEntry> GetListATFromReader(string _command) {
            SqlCommand command = new SqlCommand();
            command.CommandText = _command;
            return GetListATFromReader(command);
        }

        public static List<DatabaseObjects.AcountTableEntry> GetListATFromReader(SqlCommand _command) {
            List<DatabaseObjects.AcountTableEntry> toReturn = new List<DatabaseObjects.AcountTableEntry>();
            try {
                using (SqlConnection connection = new SqlConnection(_ConnectionString)) {
                    _command.Connection = connection;
                    connection.Open();
                    SqlDataReader _reader = _command.ExecuteReader();

                    List<string> fields = new List<string>();
                    for (int i = 0; i < _reader.FieldCount; i++) {
                        fields.Add(_reader.GetName(i));
                    }
                    while (_reader.Read()) {
                        DatabaseObjects.AcountTableEntry entry = new DatabaseObjects.AcountTableEntry();
                        if (fields.Contains(DatabaseObjects.AcountsTableNames.ID)) { entry.ID = ReadFromReader<Int32>((IDataRecord)_reader, DatabaseObjects.AcountsTableNames.ID); }
                        if (fields.Contains(DatabaseObjects.AcountsTableNames.Naam)) { entry.Naam = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.AcountsTableNames.Naam); }
                        if (fields.Contains(DatabaseObjects.AcountsTableNames.InlogNaam)) { entry.InlogNaam = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.AcountsTableNames.InlogNaam); }
                        if (fields.Contains(DatabaseObjects.AcountsTableNames.InlogWachtwoord)) { entry.InlogWachtwoord = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.AcountsTableNames.InlogWachtwoord); }
                        if (fields.Contains(DatabaseObjects.AcountsTableNames.AanspreekpuntBevoegthijdLvl)) { entry.AanspreekpuntBevoegdhijd = ReadFromReader<Int32>((IDataRecord)_reader, DatabaseObjects.AcountsTableNames.AanspreekpuntBevoegthijdLvl); }
                        if (fields.Contains(DatabaseObjects.AcountsTableNames.AdminBevoegdhijd)) { entry.AdminBevoegdhijd = ReadFromReader<Int32>((IDataRecord)_reader, DatabaseObjects.AcountsTableNames.AdminBevoegdhijd); }
                        toReturn.Add(entry);
                    }
                }
            } catch (Exception ex) {
                throw new Exception($"(ATFromReader)ERROR @ SQL Command: {_command.CommandText} | Message: {ex.Message}");
            }
            return toReturn;
        }


        public static List<DatabaseObjects.UserTableTableEntry> GetListUTFromReader(string _command) {
            SqlCommand command = new SqlCommand();
            command.CommandText = _command;
            return GetListUTFromReader(command);
        }

        public static List<DatabaseObjects.UserTableTableEntry> GetListUTFromReader(SqlCommand _command) {
            List<DatabaseObjects.UserTableTableEntry> toReturn = new List<DatabaseObjects.UserTableTableEntry>();

            try {
                using (SqlConnection connection = new SqlConnection(_ConnectionString)) {
                    _command.Connection = connection;
                    connection.Open();
                    SqlDataReader _reader = _command.ExecuteReader();

                    List<string> fields = new List<string>();
                    for (int i = 0; i < _reader.FieldCount; i++) {
                        fields.Add(_reader.GetName(i));
                    }
                    while (_reader.Read()) {
                        DatabaseObjects.UserTableTableEntry entry = new DatabaseObjects.UserTableTableEntry();
                        if (fields.Contains(DatabaseObjects.UserTableNames.ID)) { entry.ID = ReadFromReader<int>((IDataRecord)_reader, DatabaseObjects.UserTableNames.ID); }
                        if (fields.Contains(DatabaseObjects.UserTableNames.VoorNaam)) { entry.VoorNaam = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.UserTableNames.VoorNaam); }
                        if (fields.Contains(DatabaseObjects.UserTableNames.AchterNaam)) { entry.AchterNaam = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.UserTableNames.AchterNaam); }
                        if (fields.Contains(DatabaseObjects.UserTableNames.NFCID)) { entry.NFCID = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.UserTableNames.NFCID); }
                        if (fields.Contains(DatabaseObjects.UserTableNames.DateJoined)) { entry.DateJoined = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.UserTableNames.DateJoined); }
                        if (fields.Contains(DatabaseObjects.UserTableNames.DateLeft)) { entry.DateLeft = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.UserTableNames.DateLeft); }
                        if (fields.Contains(DatabaseObjects.UserTableNames.IsActiveUser)) { entry.IsActiveUser = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.UserTableNames.IsActiveUser); }
                        toReturn.Add(entry);
                    }
                }
            } catch (Exception ex) {
                throw new Exception($"(UTFromReader)ERROR @ SQL Command: {_command.CommandText} | Message: {ex.Message}");
            }

            return toReturn;
        }


        public static List<DatabaseObjects.RegistratieTableTableEntry> GetListRTFromReader(string _command) {
            SqlCommand command = new SqlCommand();
            command.CommandText = _command;
            return GetListRTFromReader(command);
        }

        public static List<DatabaseObjects.RegistratieTableTableEntry> GetListRTFromReader(SqlCommand _command) {
            List<DatabaseObjects.RegistratieTableTableEntry> toReturn = new List<DatabaseObjects.RegistratieTableTableEntry>();

            try {
                using (SqlConnection connection = new SqlConnection(_ConnectionString)) {
                    _command.Connection = connection;
                    connection.Open();
                    SqlDataReader _reader = _command.ExecuteReader();

                    List<string> fields = new List<string>();
                    for (int i = 0; i < _reader.FieldCount; i++) {
                        fields.Add(_reader.GetName(i));
                    }
                    while (_reader.Read()) {
                        DatabaseObjects.RegistratieTableTableEntry entry = new DatabaseObjects.RegistratieTableTableEntry();

                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.ID)) {
                            entry.ID = ReadFromReader<int>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.ID);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IDOfUserRelated)) {
                            entry.IDOfUserRelated = ReadFromReader<int>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IDOfUserRelated);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.Date)) {
                            entry.Date = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.Date);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.TimeInteken)) {
                            entry.TimeInteken = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.TimeInteken).TimeOfDay;
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.TimeUitteken)) {
                            entry.TimeUitteken = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.TimeUitteken).TimeOfDay;
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.HeeftIngetekend)) {
                            entry.HeeftIngetekend = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.HeeftIngetekend);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsAanwezig)) {
                            entry.IsAanwezig = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsAanwezig);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsZiek)) {
                            entry.IsZiek = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsZiek);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsFlexibelverlof)) {
                            entry.IsFlexiebelverlof = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsFlexibelverlof);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsStudieverlof)) {
                            entry.IsStudieverlof = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsStudieverlof);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsExcursie)) {
                            entry.IsExcurtie = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsExcursie);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsLaat)) {
                            entry.IsLaat = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsLaat);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.IsToegestaanAfwezig)) {
                            entry.IsToegestaalAfwezig = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.IsToegestaanAfwezig);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.Opmerking)) {
                            entry.Opmerking = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.Opmerking);
                        }
                        if (fields.Contains(DatabaseObjects.RegistratieTableNames.Verwachtetijdvanaanwezighijd)) {
                            entry.Verwachtetijdvanaanwezighijd = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.RegistratieTableNames.Verwachtetijdvanaanwezighijd).TimeOfDay;
                        }
                        toReturn.Add(entry);
                    }
                }
            } catch (Exception ex) {
                throw new Exception($"(RTFromReader)ERROR @ SQL Command: {_command.CommandText} | Message: {ex.Message}");
            }

            return toReturn;
        }

        public static List<DatabaseObjects.ModifierTableEntry> GetListMTFromReader(string _command) {
            SqlCommand command = new SqlCommand();
            command.CommandText = _command;
            return GetListMTFromReader(command);
        }

        //if (fields.Contains(DatabaseObjects)) { entry = ReadFromReader<>((IDataRecord)_reader, DatabaseObjects); }

        public static List<DatabaseObjects.ModifierTableEntry> GetListMTFromReader(SqlCommand _command) {
            List<DatabaseObjects.ModifierTableEntry> toReturn = new List<DatabaseObjects.ModifierTableEntry>();
            try {
                using (SqlConnection connection = new SqlConnection(_ConnectionString)) {
                    _command.Connection = connection;
                    connection.Open();
                    SqlDataReader _reader = _command.ExecuteReader();

                    List<string> fields = new List<string>();
                    for (int i = 0; i < _reader.FieldCount; i++) {
                        fields.Add(_reader.GetName(i));
                    }
                    while (_reader.Read()) {
                        DatabaseObjects.ModifierTableEntry entry = new DatabaseObjects.ModifierTableEntry();
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.ID)) { entry.ID = ReadFromReader<Int32>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.ID); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.DateTotEnMet)) { entry.DateTotEnMet = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.DateTotEnMet); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.DateVanafEnMet)) { entry.DateVanafEnMet = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.DateVanafEnMet); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.UserIDs)) { entry.UserIDs = JsonConvert.DeserializeObject<List<int>>(ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.UserIDs)); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.DaysOfEffect)) { entry.DaysOfEffect = JsonConvert.DeserializeObject<bool[]>(ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.DaysOfEffect)); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.HoursToAdd)) { entry.HoursToAdd = ReadDateTimeFromReader((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.HoursToAdd).TimeOfDay; }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.Omschrijving)) { entry.omschrijveing = ReadFromReader<string>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.Omschrijving); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.isStudiever)) { entry.isStudieVerlof = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.isStudiever); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.isExur)) { entry.isExurtie = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.isExur); }
                        if (fields.Contains(DatabaseObjects.ModifierTableNames.isflexy)) { entry.isFlexibelverlofoeorfsjklcghiur = ReadFromReader<bool>((IDataRecord)_reader, DatabaseObjects.ModifierTableNames.isflexy); }
                        toReturn.Add(entry);
                    }
                }
            } catch (Exception ex) {
                throw new Exception($"(MTFromReader)ERROR @ SQL Command: {_command.CommandText} | Message: {ex.Message}");
            }
            return toReturn;
        }

    }
}

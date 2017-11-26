//
// CTA Ridership analysis.
//
// Kandyce Burks
// U. of Illinois, Chicago
// CS341, Fall 2017
// Project #07
//

using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyFirstDBApp
{
    public partial class Form1 : Form
    {
        string filename, version, connectionInfo, input, sql, msg;
        SqlConnection db;
        SqlCommand cmd;
        object result;
        bool marked;

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            marked = !marked;
            if (marked)
            {
                this.comboBox1.Items.Clear();
                this.comboBox2.Items.Clear();
                this.listBox1.Items.Clear();
                this.listBox2.Items.Clear();
                this.listBox3.Items.Clear();
                this.listBox4.Items.Clear();
                this.listBox5.Items.Clear();
                this.listBox6.Items.Clear();
                this.listBox7.Items.Clear();
                this.listBox8.Items.Clear();
                this.listBox9.Items.Clear();
                this.listBox10.Items.Clear();

                db.Open();

                string sql, msg;

                sql = @"
             SELECT TOP 10 Stations.Name,SUM(Riderships.DailyTotal) AS Total
             FROM Stations
             INNER JOIN Riderships ON Riderships.StationID = Stations.StationID
             GROUP BY Stations.Name
             ORDER BY Total DESC
             ;";

                SqlDataAdapter adapter = new SqlDataAdapter(sql, db);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                foreach (DataRow row in ds.Tables["TABLE"].Rows)
                {
                    msg = String.Format("{0}", Convert.ToString(row["Name"]));
                    this.comboBox1.Items.Add(msg);
                }
                db.Close();
            }
            else
            {
                populateComboBox1(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            populateComboBox1(sender, e);
        }

        public Form1()
        {
            InitializeComponent();

            version = "MSSQLLocalDB";
            filename = "CTA.mdf";

            connectionInfo = String.Format(@"
Data Source=(LocalDB)\{0};AttachDbFilename=|DataDirectory|\{1};Integrated Security=True;
", version, filename);

            db = new SqlConnection(connectionInfo);

        }
        
        private void populateComboBox1(object sender, EventArgs e)
        {
            db.Open();

            string sql, msg;

            sql = @"
             SELECT Name
             FROM Stations
             ORDER BY Name ASC;";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, db);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                msg = String.Format("{0}", Convert.ToString(row["Name"]));
                this.comboBox1.Items.Add(msg);
            }
            db.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            db.Open();

            //Empty Fields
            this.comboBox2.Items.Clear();
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();
            this.listBox3.Items.Clear();
            this.listBox4.Items.Clear();
            this.listBox5.Items.Clear();
            this.listBox6.Items.Clear();
            this.listBox7.Items.Clear();

            //Get Selected Station
            string selected = Convert.ToString(this.comboBox1.SelectedItem);
            selected = selected.Replace("'", "''");
            cmd = new SqlCommand();
            cmd.Connection = db;

            /*------------------------------------Ridership Data: Begin------------------------------------*/

            //Total Ridership
            sql = @"
             SELECT SUM(Riderships.DailyTotal)
             FROM Riderships
             INNER JOIN Stations ON Riderships.StationID=Stations.StationID
             WHERE Stations.Name= '" + selected +
             "';";
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            msg = String.Format("{0:#,##0}", result);
            this.listBox1.Items.Add(msg);

            //Average Ridership
            sql = @"
             SELECT Count(*)
             FROM Riderships
             INNER JOIN Stations ON Riderships.StationID=Stations.StationID
             WHERE Stations.Name= '" + selected +
            "';";
            cmd.CommandText = sql;
            int total = Convert.ToInt32(result);            
            result = total / Convert.ToInt32(cmd.ExecuteScalar());
            msg = String.Format("{0:#,##0}/day", result);
            this.listBox2.Items.Add(msg);

            //Percentage of Ridership
            sql = @"
             SELECT SUM(convert(bigint, Riderships.DailyTotal))
             FROM Riderships
             ;";
            cmd.CommandText = sql;
            result = (float)((float)total / (float)Convert.ToInt64(cmd.ExecuteScalar()))*100;
            msg = String.Format("{0: ##.00}%", result);
            this.listBox3.Items.Add(msg);

            //Weekdays
            sql = @"
             SELECT SUM(Riderships.DailyTotal)
             FROM Riderships
             INNER JOIN Stations ON Riderships.StationID=Stations.StationID
             WHERE Stations.Name= '" + selected +
             @"' AND Riderships.TypeOfDay= 'W'
             ;";
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            msg = String.Format("{0:#,##0}", result);
            this.listBox5.Items.Add(msg);

            //Saturdays
            sql = @"
             SELECT SUM(Riderships.DailyTotal)
             FROM Riderships
             INNER JOIN Stations ON Riderships.StationID=Stations.StationID
             WHERE Stations.Name= '" + selected +
             @"' AND Riderships.TypeOfDay= 'A'
             ;";
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            msg = String.Format("{0:#,##0}", result);
            this.listBox6.Items.Add(msg);

            //Sundays
            sql = @"
             SELECT SUM(Riderships.DailyTotal)
             FROM Riderships
             INNER JOIN Stations ON Riderships.StationID=Stations.StationID
             WHERE Stations.Name= '" + selected +
             @"' AND Riderships.TypeOfDay= 'U'
             ;";
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            msg = String.Format("{0:#,##0}", result);
            this.listBox7.Items.Add(msg);
            /*------------------------------------Ridership Data: End------------------------------------*/

            //Get Stops Associated with Station
            sql = @"
             SELECT Stops.Name
             FROM Stops
             INNER JOIN Stations ON Stops.StationID=Stations.StationID
             WHERE Stations.Name= '" + selected +
             "' ORDER BY Stops.Name Asc;";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, db);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                msg = String.Format("{0}", Convert.ToString(row["Name"]));
                this.comboBox2.Items.Add(msg);
            }

            db.Close();

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            db.Open();

            //Empty Fields
            this.listBox4.Items.Clear();
            this.listBox8.Items.Clear();
            this.listBox9.Items.Clear();
            this.listBox10.Items.Clear();
            
            //Get Selected Stop
            string selected = Convert.ToString(this.comboBox2.SelectedItem);
            selected = selected.Replace("'", "''");

            cmd = new SqlCommand();
            cmd.Connection = db;

            /*------------------------------------Stops Data: Begin------------------------------------*/
            //Lines
            sql = @"
             SELECT Lines.Color
             FROM Lines, StopDetails, Stops
             WHERE Stops.Name = '" + selected + @"'
             AND Lines.LineID=StopDetails.LineID
             AND Stops.StopID=StopDetails.StopID
             ;";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, db);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            foreach (DataRow row in ds.Tables["TABLE"].Rows)
            {
                msg = String.Format("{0}", Convert.ToString(row["Color"]));
                this.listBox4.Items.Add(msg);
            }

            //Accessibility
            sql = @"
             SELECT Stops.ADA
             FROM Stops
             WHERE Stops.Name= '" + selected +
             "';";
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            msg = String.Format("{0}", Convert.ToString(result) == "True" ? "Yes" : "No");
            this.listBox9.Items.Add(msg);

            //Direction of Travel
            sql = @"
             SELECT Stops.Direction
             FROM Stops
             WHERE Stops.Name= '" + selected +
             "';";
            cmd.CommandText = sql;
            result = cmd.ExecuteScalar();
            msg = String.Format("{0}", result);
            this.listBox10.Items.Add(msg);

            //Location
            object latitude, longitude;

            sql = @"
             SELECT Stops.Latitude
             FROM Stops
             WHERE Stops.Name= '" + selected +
             "';";
            cmd.CommandText = sql;
            latitude = cmd.ExecuteScalar();

            sql = @"
             SELECT Stops.Longitude
             FROM Stops
             WHERE Stops.Name= '" + selected +
             "';";
            cmd.CommandText = sql;
            longitude = cmd.ExecuteScalar();
            msg = String.Format("({0: ##.0000}, ", latitude);
            msg += String.Format("{0: ##.0000})", longitude);
            this.listBox8.Items.Add(msg);
            /*------------------------------------Stops Data: End------------------------------------*/

            db.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlConnection db = null;
            try
            {
                string connectionInfo = "...";
                db = new SqlConnection(connectionInfo);
                db.Open();
            }
            catch
            { }
            finally
            {
                if (db != null && db.State == ConnectionState.Open)
                    db.Close();
            }
            marked = false;

        }
    }
}

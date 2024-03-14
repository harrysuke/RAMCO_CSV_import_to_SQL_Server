using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace csv2
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection con = new MySqlConnection("datasource=192.168.5.40;database=jplims;port=3306;username=blink2;password=sc0rpene@747"))
            {
                try
                {
                    con.Open();

                    if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text))
                    {
                        string hashedPassword = CalculateMD5Hash(textBox2.Text);
                        string query = "SELECT * FROM penguna WHERE User_Id = @userId AND katalaluan = @password";
                        using (MySqlCommand cmd = new MySqlCommand(query, con))
                        {
                            cmd.Parameters.AddWithValue("@userId", textBox1.Text);
                            cmd.Parameters.AddWithValue("@password", hashedPassword);

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string errorMessage = $"[SUCCESS] {textBox1.Text} login successfully at {DateTime.Now}";
                                    Updatelog(errorMessage);

                                    Form1 frm = new Form1();
                                    frm.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Username and Password not match", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    string errorMessage = $"[ERROR] Username and Password not match for {textBox1.Text} at {DateTime.Now}";
                                    Updatelog(errorMessage);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Username and Password cannot be empty", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        string errorMessage = $"[ERROR] Username and Password cannot be empty for {textBox1.Text} at {DateTime.Now}";
                        Updatelog(errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    string errorMessage = $"[ERROR] {ex.Message} at {DateTime.Now}";
                    Updatelog(errorMessage);
                }
                finally
                {
                    textBox1.Text = string.Empty;
                    textBox2.Text = string.Empty;
                }
            }
        }

        private string CalculateMD5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to a hexadecimal string.
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static void Updatelog(string logs)
        {
            string logFilePath = Path.Combine(Environment.CurrentDirectory, "log.txt");
            try
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(logs);
                    Console.WriteLine(logs);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Console.WriteLine(errorMessage);

                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(errorMessage);
                }
            }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string browser = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
            string register = "http://apps2.johorport.com.my/register";
            System.Diagnostics.Process.Start(browser, register);
        }
    }
}

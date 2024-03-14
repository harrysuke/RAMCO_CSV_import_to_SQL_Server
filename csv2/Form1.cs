using Microsoft.VisualBasic.Logging;
using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Net.Mail;
using System.Windows.Forms;

namespace csv2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Dock = DockStyle.Fill;

            int statusBarHeight = statusStrip1.Height;
            dataGridView1.Height -= statusBarHeight;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCSVFilesIntoDataGridView();

            toolStripStatusLabel1.Text = "Loading CSV to grid view...";
            statusStrip1.Update();
            toolStripProgressBar1.Value = 0;

            browseToolStripMenuItem.ToolTipText = "Manual browse";
            uploadToolStripMenuItem.ToolTipText = "Manual upload";
        }

        private void LoadCSVFilesIntoDataGridView()
        {
            try
            {
                //string directoryPath = @"C:\Users\kairi\Desktop\csv\";
                string directoryPath = LoadDirFromIni("CsvFolder");
                toolStripStatusLabel1.Text = directoryPath;
                if (Directory.Exists(directoryPath))
                {
                    toolStripStatusLabel1.Text = "Directory found";
                }
                else
                {
                    return;
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                FileInfo[] files = directoryInfo.GetFiles("*.csv");

                foreach (FileInfo file in files)
                {
                    BindData(file.FullName);
                }

                if (dataGridView1.NewRowIndex > 0 && dataGridView1.NewRowIndex < dataGridView1.Rows.Count)
                {
                    UploadToSQlDatabase();
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Updatelog(errorMessage);
            }

        }

        private static string LoadDirFromIni(string folder)
        {
            try
            {
                string iniFilePath = Path.Combine(Environment.CurrentDirectory, "csv.ini");

                if (!File.Exists(iniFilePath))
                {
                    string errorMessage = $"[ERR0R] csv.ini file not found at {DateTime.Now}";
                    Updatelog(errorMessage);
                    return null;
                }
                using (StreamReader sr = new StreamReader(iniFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] parts = line.Split("=");

                        if (parts.Length == 2)
                        {
                            if (folder == "CsvFolder" && parts[0].Trim().Equals("CsvFolder"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "BackupFolder" && parts[0].Trim().Equals("BackupFolder"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "Server" && parts[0].Trim().Equals("Server")) {
                                return parts[1].Trim();
                            }
                            if (folder == "Database" && parts[0].Trim().Equals("Database")) {
                                return parts[1].Trim();
                            }
                            if (folder == "UserId" && parts[0].Trim().Equals("UserId")) {
                                return parts[1].Trim();
                            }
                            if (folder == "Password" && parts[0].Trim().Equals("Password")) {
                                return parts[1].Trim();
                            }
                            if (folder == "SmtpClient" && parts[0].Trim().Equals("SmtpClient"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "SmtpPort" && parts[0].Trim().Equals("SmtpPort"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "EmailFrom" && parts[0].Trim().Equals("EmailFrom"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "EmailFromUsername" && parts[0].Trim().Equals("EmailFromUsername"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "EmailFromPassword" && parts[0].Trim().Equals("EmailFromPassword"))
                            {
                                return parts[1].Trim();
                            }
                            if (folder == "EmailTo" && parts[0].Trim().Equals("EmailTo"))
                            {
                                return parts[1].Trim();
                            }
                        }
                    }
                    string errorMessage = $"[ERR0R] Folder path not found ini csv.ini file at {DateTime.Now}";
                    Updatelog(errorMessage);
                    return null;
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Updatelog(errorMessage);
                return "";
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
            }
        }

        private void UploadToSQlDatabase()
        {
            string server = LoadDirFromIni("Server");
            string database = LoadDirFromIni("Database");
            string userid = LoadDirFromIni("UserId");
            string password = LoadDirFromIni("Password");

            try
            {
                //using (SqlConnection connection = new SqlConnection("Server=192.168.5.38\\INFORLN; Database=JPL_intdb;User Id=sa;Password=P@ssw0rd"))
                using (SqlConnection connection = new SqlConnection("Server="+server+"; Database="+database+";User Id="+userid+";Password="+password))
                {
                    connection.Open();
                    string tableName = "ARTransactions";
                    string insertQuery = "INSERT INTO " + tableName + "(";

                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        insertQuery += column.HeaderText + ",";
                    }

                    insertQuery = insertQuery.TrimEnd(',') + ") VALUES (";

                    foreach (DataGridViewColumn column in dataGridView1.Columns)
                    {
                        insertQuery += "@" + column.HeaderText + ",";
                    }

                    insertQuery = insertQuery.TrimEnd(',') + ")";

                    using (SqlCommand sqlCommand = new SqlCommand(insertQuery, connection))
                    {
                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            sqlCommand.Parameters.AddWithValue("@" + column.HeaderText, null);
                        }

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            for (int i = 0; i < dataGridView1.Columns.Count; i++)
                            {
                                sqlCommand.Parameters["@" + dataGridView1.Columns[i].HeaderText].Value = row.Cells[i].Value;
                            }
                            sqlCommand.ExecuteNonQuery();
                        }
                    }

                    //MessageBox.Show("Record inserted successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    toolStripStatusLabel1.Text = "Record inserted successfully";
                    string errorMessage = $"[SUCCESS] Record inserted successfully at {DateTime.Now}";
                    Updatelog(errorMessage);
                    Application.DoEvents();
                    Application.Exit();
                    //sendEmail();

                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Updatelog(errorMessage);
                Application.Exit();
            }
        }

        private void sendEmail()
        {
            try
            {
                MailMessage mail = new MailMessage();
                string smtp = LoadDirFromIni("SmtpClient");
                SmtpClient smtpClient = new SmtpClient(smtp);
                Console.WriteLine(smtp);

                string emailFrom = LoadDirFromIni("EmailFrom");
                mail.From = new MailAddress(emailFrom);
                Console.WriteLine(emailFrom);

                string emailTo = LoadDirFromIni("EmailTo");
                mail.To.Add(emailTo);
                Console.WriteLine(emailTo);

                mail.Subject = "CSV import to SQL";
                mail.Body = "Attached is the log file.";

                string logFile = Path.Combine(Environment.CurrentDirectory, "log.txt");
                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment(logFile);
                mail.Attachments.Add(attachment);
                Console.WriteLine(attachment);

                string port = LoadDirFromIni("SmtpPort");
                smtpClient.Port = (int)Convert.ToInt64(port);
                Console.WriteLine(port);

                string username = LoadDirFromIni("EmailFromUsername");
                string password = LoadDirFromIni("EmailFromPassword");
                smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                Console.WriteLine(username);
                Console.WriteLine(password);

                smtpClient.EnableSsl = true;
                smtpClient.Send(mail);

                string errorMessage = $"[SUCCESS] Email sent successfully at {DateTime.Now}";
                Updatelog(errorMessage);
                Application.Exit();
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Updatelog(errorMessage);
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void browseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
            openFileDialog.Title = "Select a CSV file";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                BindData(filePath);
                Application.DoEvents();
            }
        }

        private void BindData(string filePath)
        {
            try
            {
                toolStripStatusLabel1.Text = "Loading...";
                statusStrip1.Update();
                DataTable dataTable = new DataTable();
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string[] headers = sr.ReadLine().Split(",");

                    foreach (string header in headers)
                    {
                        dataTable.Columns.Add(header.Trim());
                    }

                    while (!sr.EndOfStream)
                    {
                        string[] fields = sr.ReadLine().Split(",");
                        DataRow row = dataTable.NewRow();

                        if (fields.Length == dataTable.Columns.Count)
                        {
                            row.ItemArray = fields;
                            dataTable.Rows.Add(row);
                        }
                    }
                }

                //dataTable.PrimaryKey = null;
                dataGridView1.DataSource = dataTable;
                toolStripStatusLabel1.Text = filePath + " is loaded...";
                statusStrip1.Update();
                toolStripProgressBar1.Value = 100;

                CopyCSVFileToBackupFolder(filePath);
                toolStripProgressBar1.Value = 0;
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Updatelog(errorMessage);
            }
        }

        private void CopyCSVFileToBackupFolder(string sourceFilePath)
        {
            try
            {
                //string backupFolderPath = @"C:\Users\kairi\Desktop\csv\BACKUP";
                string backupFolderPath = LoadDirFromIni("BackupFolder");
                if (backupFolderPath == null)
                {
                    return;
                }

                string fileName = Path.GetFileName(sourceFilePath);
                string destinationFilePath = Path.Combine(backupFolderPath, fileName);

                //Create backup
                File.Copy(sourceFilePath, destinationFilePath, true);
                toolStripProgressBar1.Value = 100;
                toolStripStatusLabel1.Text = "Backup copy created successfully";
                string errorMessage = $"[SUCCESS] Backup copy created successfully at {DateTime.Now}";
                Updatelog(errorMessage);

                //Delete original CSV file
                toolStripProgressBar1.Value = 0;
                File.Delete(sourceFilePath);
                toolStripProgressBar1.Value = 100;
                toolStripStatusLabel1.Text = "Original CSV file deleted successfully";
                string errorMessage2 = $"[SUCCESS] Original CSV file deleted successfully at {DateTime.Now}";
                Updatelog(errorMessage2);
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERR0R] {ex.Message} at {DateTime.Now}";
                Updatelog(errorMessage);
            }
        }

        private void esitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            sendEmail();
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UploadToSQlDatabase();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "csv.ini");
            System.Diagnostics.Process.Start("notepad.exe", filename);
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("About CSV reader by ICT, JPB\r\n20 Febuary 2024\r\n© JPB. All rights reserved.\r\nAuthor: Khairi Juri", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void logsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "log.txt");
            System.Diagnostics.Process.Start("notepad.exe", filename);
        }

        private void batchFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] filenames = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "RAMCO"), "ramco_csv.bat");
            if (filenames.Length > 0)
            {
                string filename = filenames[0];
                System.Diagnostics.Process.Start("notepad.exe", filename);
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form2 frm = new Form2();
            frm.Show();
            this.Hide();
        }
    }
}

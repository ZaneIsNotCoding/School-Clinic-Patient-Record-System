using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using MySql.Data.MySqlClient;
using School_Clinic_Patient_Record_System.Classes;

namespace School_Clinic_Patient_Record_System.Forms
{
    internal static class Ui
    {
        public static readonly Color Blue = Color.FromArgb(63, 126, 145), Teal = Color.FromArgb(75, 145, 132), Ink = Color.FromArgb(38, 58, 67), Pale = Color.FromArgb(244, 247, 247), Muted = Color.FromArgb(111, 130, 136), Line = Color.FromArgb(225, 232, 233), Sidebar = Color.FromArgb(36, 57, 66);
        public static Label Label(string text) { return new Label { Text = text, AutoSize = true, ForeColor = Ink, Font = new Font("Segoe UI", 9F) }; }
        public static TextBox TextBox(bool multiline = false) { return new TextBox { Font = new Font("Segoe UI", 9F), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.FromArgb(252, 253, 253), ForeColor = Ink, Multiline = multiline, Height = multiline ? 62 : 30, ScrollBars = multiline ? ScrollBars.Vertical : ScrollBars.None, Margin = new Padding(0, 4, 0, 7) }; }
        public static Button Button(string text, Color color) { return new Button { Text = text, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0, MouseOverBackColor = Color.FromArgb(Math.Min(color.R + 12,255), Math.Min(color.G + 12,255), Math.Min(color.B + 12,255)) }, Font = new Font("Segoe UI Semibold", 9F), Height = 36, Cursor = Cursors.Hand, UseVisualStyleBackColor = false }; }
        public static Panel Card() { return new SoftCard { BackColor = Color.White, Padding = new Padding(0) }; }
        public static void FitToScreen(Form form, Size preferred, Size minimum)
        {
            Rectangle area=Screen.PrimaryScreen.WorkingArea;
            int width=Math.Min(preferred.Width,Math.Max(minimum.Width,area.Width-32));
            int height=Math.Min(preferred.Height,Math.Max(minimum.Height,area.Height-48));
            form.Size=new Size(width,height);
            form.MinimumSize=new Size(Math.Min(minimum.Width,width),Math.Min(minimum.Height,height));
            form.StartPosition=FormStartPosition.CenterParent;
        }
        private sealed class SoftCard : Panel
        {
            public SoftCard() { DoubleBuffered = true; }
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e); e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                var bounds = new Rectangle(0, 0, Width - 1, Height - 1); int radius = 12;
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                using (var pen = new Pen(Line))
                using (var brush = new SolidBrush(BackColor))
                {
                    path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90); path.AddArc(bounds.Right-radius, bounds.Y, radius, radius, 270, 90);
                    path.AddArc(bounds.Right-radius, bounds.Bottom-radius, radius, radius, 0, 90); path.AddArc(bounds.X, bounds.Bottom-radius, radius, radius, 90, 90); path.CloseFigure();
                    e.Graphics.FillPath(brush, path); e.Graphics.DrawPath(pen, path);
                }
            }
        }
        public static void Audit(string action, string entity, object id)
        {
            try { DatabaseConnection.Execute("INSERT INTO audit_log(user_id,action,entity,record_id) VALUES(@u,@a,@e,@r)", DatabaseConnection.P("@u", SessionManager.UserId == 0 ? (object)null : SessionManager.UserId), DatabaseConnection.P("@a", action), DatabaseConnection.P("@e", entity), DatabaseConnection.P("@r", Convert.ToString(id))); }
            catch { }
        }
    }

    public class LoginForm : Form
    {
        private TextBox user, pass;
        private CheckBox show;
        public LoginForm()
        {
            Text = "School Clinic | Sign in"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(880, 540); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; BackColor = Ui.Pale; Font = new Font("Segoe UI", 9F);
            var layout=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=1,Margin=Padding.Empty,Padding=Padding.Empty,BackColor=Ui.Pale};layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,320));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));Controls.Add(layout);
            var brand=new Panel{Dock=DockStyle.Fill,BackColor=Ui.Sidebar,Margin=Padding.Empty};layout.Controls.Add(brand,0,0);
            var mark=new Label{Text="✚",Font=new Font("Segoe UI",25,FontStyle.Bold),ForeColor=Ui.Teal,BackColor=Color.FromArgb(235,244,241),TextAlign=ContentAlignment.MiddleCenter,Size=new Size(66,66),Location=new Point(34,58)};brand.Controls.Add(mark);
            brand.Controls.Add(new Label{Text="School Clinic",Font=new Font("Segoe UI",22,FontStyle.Bold),ForeColor=Color.White,AutoSize=true,Location=new Point(34,156)});
            brand.Controls.Add(new Label{Text="Student and teacher\nhealth records",Font=new Font("Segoe UI",12),ForeColor=Color.FromArgb(203,216,218),AutoSize=true,Location=new Point(36,205)});
            brand.Controls.Add(new Panel{BackColor=Color.FromArgb(79,105,111),Location=new Point(36,286),Size=new Size(190,1)});
            brand.Controls.Add(new Label{Text="ILAGAN CAMPUS",Font=new Font("Segoe UI Semibold",8),ForeColor=Color.FromArgb(168,191,193),AutoSize=true,Location=new Point(36,310)});
            var area=new Panel{Dock=DockStyle.Fill,BackColor=Ui.Pale,Margin=Padding.Empty};layout.Controls.Add(area,1,0);
            var card=Ui.Card();card.Location=new Point(38,28);card.Size=new Size(462,476);card.Padding=new Padding(30);area.Controls.Add(card);
            card.Controls.Add(new Label{Text="Welcome back",Font=new Font("Segoe UI",23,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(35,36)});
            card.Controls.Add(new Label{Text="Sign in to continue to the clinic system.",Font=new Font("Segoe UI",9.5F),ForeColor=Ui.Muted,AutoSize=true,Location=new Point(38,78)});
            var usernameLabel=Ui.Label("Username");usernameLabel.Location=new Point(38,103);card.Controls.Add(usernameLabel);user=Ui.TextBox();user.Location=new Point(38,126);user.Width=380;card.Controls.Add(user);
            card.Controls.Add(Ui.Label("Password"));var passwordLabel=card.Controls[card.Controls.Count-1];passwordLabel.Location=new Point(38,188);pass=Ui.TextBox();pass.Location=new Point(38,210);pass.Width=380;pass.UseSystemPasswordChar=true;card.Controls.Add(pass);
            show=new CheckBox{Text="Show password",ForeColor=Ui.Muted,Location=new Point(38,248),AutoSize=true};show.CheckedChanged+=(s,e)=>pass.UseSystemPasswordChar=!show.Checked;card.Controls.Add(show);
            var login=Ui.Button("Sign in",Ui.Blue);login.Location=new Point(38,286);login.Width=380;login.Height=40;login.Click+=SignIn;card.Controls.Add(login);
            var setup=new LinkLabel{Text="Set up first administrator",LinkColor=Ui.Blue,ActiveLinkColor=Ui.Teal,AutoSize=true,Location=new Point(146,354)};setup.LinkClicked+=(s,e)=>SetupFirstAdmin();card.Controls.Add(setup);
            var exit=new LinkLabel{Text="Exit",LinkColor=Ui.Muted,AutoSize=true,Location=new Point(218,393)};exit.LinkClicked+=(s,e)=>Close();card.Controls.Add(exit);
            AcceptButton = login;
        }
        private void SignIn(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(user.Text) || String.IsNullOrEmpty(pass.Text)) { MessageBox.Show("Enter your username and password.", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            try
            {
                var table = DatabaseConnection.Query("SELECT user_id,full_name,username,password_hash,role FROM users WHERE username=@name AND is_active=1 LIMIT 1", DatabaseConnection.P("@name", user.Text.Trim()));
                if (table.Rows.Count == 0 || !PasswordHelper.Verify(pass.Text, Convert.ToString(table.Rows[0]["password_hash"]))) { MessageBox.Show("The username or password is incorrect.", "Sign in", MessageBoxButtons.OK, MessageBoxIcon.Warning); Ui.Audit("LOGIN_FAILED", "users", user.Text.Trim()); return; }
                var row = table.Rows[0]; SessionManager.Start(Convert.ToInt32(row["user_id"]), Convert.ToString(row["full_name"]), Convert.ToString(row["role"])); Ui.Audit("LOGIN", "users", SessionManager.UserId);
                Hide(); using (var dashboard = new DashboardForm()) dashboard.ShowDialog(); SessionManager.End(); Show(); pass.Clear();
            }
            catch (Exception ex) { MessageBox.Show("Could not sign in. Check the MySQL Server connection and database setup.\n\n" + ex.Message, "Database connection", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void SetupFirstAdmin()
        {
            string message; if (!DatabaseConnection.CanConnect(out message)) { MessageBox.Show("Connect to MySQL and import Database/school_clinic_db.sql first.\n\n" + message, "Database setup", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            try { if (Convert.ToInt32(DatabaseConnection.Scalar("SELECT COUNT(*) FROM users")) > 0) { MessageBox.Show("An account already exists. Ask an administrator to add your account."); return; } }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Database setup"); return; }
            using (var form = new FirstAdminForm()) if (form.ShowDialog(this) == DialogResult.OK) MessageBox.Show("Administrator created. Sign in with the new account.", "Setup complete");
        }
    }

    internal class FirstAdminForm : Form
    {
        public FirstAdminForm()
        {
            Text = "Create first administrator"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; ClientSize = new Size(390, 330); BackColor = Ui.Pale;
            var fields = new Dictionary<string,TextBox>(); string[] names = { "Full name", "Username", "Password", "Confirm password" };
            for (int i=0;i<names.Length;i++) { Controls.Add(Ui.Label(names[i])); Controls[Controls.Count-1].Location=new Point(32,24+i*63); var t=Ui.TextBox(); t.Location=new Point(32,45+i*63); t.Width=320; if(i>=2)t.UseSystemPasswordChar=true; Controls.Add(t); fields[names[i]]=t; }
            var b=Ui.Button("Create administrator",Ui.Teal); b.Location=new Point(32,280); b.Width=320; b.Click+=(s,e)=> { if(fields.Values.Any(x=>String.IsNullOrWhiteSpace(x.Text)) || fields["Username"].Text.Trim().Length<4 || fields["Password"].Text.Length<10 || fields["Password"].Text!=fields["Confirm password"].Text) { MessageBox.Show("Enter all fields, use a username of at least 4 characters, and a matching password of at least 10 characters."); return; } try { DatabaseConnection.Execute("INSERT INTO users(full_name,username,password_hash,role) VALUES(@n,@u,@p,'Admin')",DatabaseConnection.P("@n",fields["Full name"].Text.Trim()),DatabaseConnection.P("@u",fields["Username"].Text.Trim()),DatabaseConnection.P("@p",PasswordHelper.Hash(fields["Password"].Text))); DialogResult=DialogResult.OK; Close(); } catch(Exception ex){MessageBox.Show("Could not create administrator. Username may already be in use.\n"+ex.Message);} }; Controls.Add(b);
        }
    }

    public class DashboardForm : Form
    {
        private Panel content; private Label status; private Timer clock;
        public DashboardForm()
        {
            Text="School Clinic Patient Record System"; WindowState=FormWindowState.Maximized; MinimumSize=new Size(1050,680); BackColor=Ui.Pale; Font=new Font("Segoe UI",9F);
            var layout=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=2,RowCount=2,Margin=Padding.Empty,Padding=Padding.Empty,BackColor=Ui.Pale};
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,238));layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent,100));layout.RowStyles.Add(new RowStyle(SizeType.Absolute,32));
            Controls.Add(layout);
            var side=new Panel{Dock=DockStyle.Fill,BackColor=Ui.Sidebar,Margin=Padding.Empty};
            layout.Controls.Add(side,0,0);layout.SetRowSpan(side,2);
            side.Controls.Add(new Label{Text="✚",Font=new Font("Segoe UI",19,FontStyle.Bold),ForeColor=Color.FromArgb(139,197,185),AutoSize=true,Location=new Point(20,20)});
            side.Controls.Add(new Label{Text="School Clinic",Font=new Font("Segoe UI",14,FontStyle.Bold),ForeColor=Color.White,AutoSize=true,Location=new Point(58,22)});
            side.Controls.Add(new Label{Text="PATIENT RECORDS",Font=new Font("Segoe UI",8,FontStyle.Bold),ForeColor=Color.FromArgb(160,179,184),AutoSize=true,Location=new Point(22,61)});
            string[] menu={"Dashboard","Patient Records","Medical Records","Medical Checkup","Medical History","Medicine Inventory","Reports","User Management","Logout"};
            for(int i=0;i<menu.Length;i++){string item=menu[i];int y=88+i*49;var b=new Button{Text="   "+item,TextAlign=ContentAlignment.MiddleLeft,FlatStyle=FlatStyle.Flat,FlatAppearance={BorderSize=0,MouseOverBackColor=Color.FromArgb(49,76,84)},ForeColor=Color.FromArgb(235,242,243),BackColor=Ui.Sidebar,Font=new Font("Segoe UI",9.5F),Location=new Point(10,y),Size=new Size(218,44),Cursor=Cursors.Hand,UseVisualStyleBackColor=false};if(i==0){b.BackColor=Color.FromArgb(56,83,91);b.ForeColor=Color.White;}b.Click+=(s,e)=>Navigate(item);side.Controls.Add(b);}
            content=new Panel{Dock=DockStyle.Fill,Padding=new Padding(28),Margin=Padding.Empty,BackColor=Ui.Pale};layout.Controls.Add(content,1,0);
            status=new Label{Dock=DockStyle.Fill,ForeColor=Ui.Muted,TextAlign=ContentAlignment.MiddleRight,Padding=new Padding(0,0,18,0),Margin=Padding.Empty,BackColor=Ui.Pale};layout.Controls.Add(status,1,1);
            clock=new Timer{Interval=1000};clock.Tick+=(s,e)=>status.Text=DateTime.Now.ToString("dddd, dd MMMM yyyy  |  h:mm:ss tt")+"     Signed in as "+SessionManager.FullName+" ("+SessionManager.Role+")";clock.Start(); ShowHome();
        }
        private void Navigate(string name)
        {
            if(name=="Logout"){if(MessageBox.Show("Sign out of the clinic system?","Sign out",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)Close();return;}
            if(name=="Dashboard"){ShowHome();return;}
            if(name=="User Management"&&!SessionManager.IsAdmin){MessageBox.Show("Only an administrator can manage accounts.");return;}
            Form form=null;
            switch(name){case "Patient Records":form=RecordForm.Patients();break;case "Medical Records":form=new MedicalRecordsForm();break;case "Medical Checkup":form=RecordForm.Checkups();break;case "Medical History":form=RecordForm.History();break;case "Medicine Inventory":form=RecordForm.Medicines();break;case "Reports":form=new ReportsForm();break;case "User Management":form=new UsersForm();break;}
            if(form!=null)using(form)form.ShowDialog(this); ShowHome();
        }
        private void ShowHome()
        {
            content.Controls.Clear(); var heading=new Label{Text="Ilagan clinic overview",Font=new Font("Segoe UI",22,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(28,20)};content.Controls.Add(heading);
            var welcome=new Label{Text="Student and teacher health statistics · Live clinic records",ForeColor=Ui.Muted,AutoSize=true,Location=new Point(30,60)};content.Controls.Add(welcome);
            string[][] cards=new string[][] {new[]{"Male students","SELECT COUNT(*) FROM patients WHERE person_type='Student' AND gender='Male'"},new[]{"Female students","SELECT COUNT(*) FROM patients WHERE person_type='Student' AND gender='Female'"},new[]{"Male teachers","SELECT COUNT(*) FROM patients WHERE person_type='Teacher' AND gender='Male'"},new[]{"Female teachers","SELECT COUNT(*) FROM patients WHERE person_type='Teacher' AND gender='Female'"}};
            int x=28;for(int i=0;i<cards.Length;i++){var p=Ui.Card();p.Location=new Point(x,100);p.Size=new Size(178,88);content.Controls.Add(p);p.Controls.Add(new Panel{BackColor=i<2?Ui.Blue:Ui.Teal,Location=new Point(13,15),Size=new Size(4,58)});p.Controls.Add(new Label{Text=cards[i][0],ForeColor=Ui.Muted,AutoSize=true,Location=new Point(26,16)});string value="—";try{value=Convert.ToString(DatabaseConnection.Scalar(cards[i][1]));}catch{}p.Controls.Add(new Label{Text=value,Font=new Font("Segoe UI",22,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(26,39)});x+=190;}
            var totals=new Label{Text="Total patients: "+ReadScalar("SELECT COUNT(*) FROM patients")+"     Today's checkups: "+ReadScalar("SELECT COUNT(*) FROM medical_checkups WHERE visit_date=CURDATE()")+"     Medicine stock: "+ReadScalar("SELECT COALESCE(SUM(quantity),0) FROM medicines"),ForeColor=Ui.Muted,AutoSize=true,Location=new Point(30,202)};content.Controls.Add(totals);
            var coursesTitle=new Label{Text="Students by course",Font=new Font("Segoe UI Semibold",11),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(30,235)};content.Controls.Add(coursesTitle);
            var yearsTitle=new Label{Text="Students by year level",Font=new Font("Segoe UI Semibold",11),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(425,235)};content.Controls.Add(yearsTitle);
            var courses=Grid();courses.Location=new Point(28,262);courses.Size=new Size(375,145);content.Controls.Add(courses);try{courses.DataSource=DatabaseConnection.Query("SELECT COALESCE(NULLIF(section_course,''),'Not set') AS Course,COUNT(*) AS Students FROM patients WHERE person_type='Student' GROUP BY section_course ORDER BY Course");}catch{}
            var years=Grid();years.Location=new Point(420,262);years.Size=new Size(375,145);content.Controls.Add(years);try{years.DataSource=DatabaseConnection.Query("SELECT COALESCE(NULLIF(grade_level,''),'Not set') AS `Year level`,COUNT(*) AS Students FROM patients WHERE person_type='Student' GROUP BY grade_level ORDER BY grade_level");}catch{}
            var title=new Label{Text="Recent patient visits",Font=new Font("Segoe UI",14,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(28,425)};content.Controls.Add(title);
            var grid=Grid();grid.Location=new Point(28,456);grid.Size=new Size(Math.Max(600,content.ClientSize.Width-80),Math.Max(130,content.ClientSize.Height-470));grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;content.Controls.Add(grid);try{grid.DataSource=DatabaseConnection.Query("SELECT c.checkup_id AS 'Checkup #', p.student_id AS 'ID Number', CONCAT_WS(' ',p.first_name,NULLIF(p.middle_name,''),p.last_name) AS 'Patient', p.person_type AS 'Type', c.visit_date AS 'Visit date', c.chief_complaint AS 'Complaint' FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id ORDER BY c.visit_date DESC,c.checkup_id DESC LIMIT 10");}catch{grid.DataSource=null;}
        }
        private static string ReadScalar(string sql){try{return Convert.ToString(DatabaseConnection.Scalar(sql));}catch{return "—";}}
        internal static DataGridView Grid(){return new DataGridView{BackgroundColor=Color.White,BorderStyle=BorderStyle.None,GridColor=Ui.Line,CellBorderStyle=DataGridViewCellBorderStyle.SingleHorizontal,AllowUserToAddRows=false,ReadOnly=true,MultiSelect=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,RowHeadersVisible=false,EnableHeadersVisualStyles=false,ColumnHeadersHeight=38,ColumnHeadersBorderStyle=DataGridViewHeaderBorderStyle.None,ColumnHeadersDefaultCellStyle={BackColor=Color.FromArgb(237,243,243),ForeColor=Ui.Ink,Font=new Font("Segoe UI Semibold",9F),Padding=new Padding(7)},DefaultCellStyle={BackColor=Color.White,ForeColor=Ui.Ink,SelectionBackColor=Color.FromArgb(226,239,238),SelectionForeColor=Ui.Ink,Padding=new Padding(7)},AlternatingRowsDefaultCellStyle={BackColor=Color.FromArgb(250,252,252)},RowTemplate={Height=36}};}
    }

    internal sealed class FieldDef { public string Key, Label, Kind; public FieldDef(string k,string l,string t="text"){Key=k;Label=l;Kind=t;} }

    public class RecordForm : Form
    {
        private readonly string table, key, title; private readonly FieldDef[] fields; private readonly string[] searchFields; private readonly DataGridView grid; private readonly Dictionary<string,Control> inputs=new Dictionary<string,Control>(); private readonly TextBox search; private int selectedId;
        private RecordForm(string title,string table,string key,FieldDef[] fields,string[] searchFields)
        {
            this.title=title;this.table=table;this.key=key;this.fields=fields;this.searchFields=searchFields;Text=title;BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);MaximizeBox=true;
            Ui.FitToScreen(this,new Size(1180,760),new Size(920,560));
            var head=new Label{Text=title,Font=new Font("Segoe UI",20,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(22,18),AutoSize=true};Controls.Add(head);
            var panel=Ui.Card();panel.Location=new Point(22,64);panel.Size=new Size(390,ClientSize.Height-86);panel.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;panel.Padding=new Padding(12);Controls.Add(panel);
            var editorLayout=new TableLayoutPanel{Dock=DockStyle.Fill,ColumnCount=1,RowCount=2,Margin=Padding.Empty,Padding=Padding.Empty,BackColor=Color.White};editorLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));editorLayout.RowStyles.Add(new RowStyle(SizeType.Percent,100));editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute,52));panel.Controls.Add(editorLayout);
            var fieldsPanel=new Panel{Dock=DockStyle.Fill,AutoScroll=true,BackColor=Color.White,Margin=Padding.Empty};editorLayout.Controls.Add(fieldsPanel,0,0);
            int top=8;foreach(var f in fields){fieldsPanel.Controls.Add(new Label{Text=f.Label,AutoSize=true,ForeColor=Ui.Ink,Location=new Point(8,top)});top+=19;Control ctl;
                if(f.Kind=="select"||f.Kind=="combo"){var c=new ComboBox{DropDownStyle=f.Kind=="combo"?ComboBoxStyle.DropDown:ComboBoxStyle.DropDownList,Location=new Point(8,top),Width=340,Height=26,Font=new Font("Segoe UI",9F)};string[] options=f.Key=="gender"?new[]{"Male","Female","Other"}:f.Key=="person_type"?new[]{"Student","Teacher"}:f.Key=="grade_level"?new[]{"1st Year","2nd Year","3rd Year","4th Year","5th Year","N/A"}:f.Key=="section_course"?new[]{"BSIT","BSA","BSN","BSCE","BSEE","BSINDT","BSED","BTVTED","BTLED","BSM"}:new[]{"Admin","Nurse"};foreach(string v in options)c.Items.Add(v);if(c.Items.Count>0&&f.Kind=="select"&&f.Key!="section_course")c.SelectedIndex=0;ctl=c;}
                else if(f.Kind=="date"){ctl=new DateTimePicker{Format=DateTimePickerFormat.Custom,CustomFormat="yyyy-MM-dd",ShowCheckBox=true,Checked=false,Location=new Point(8,top),Width=340,Font=new Font("Segoe UI",9F)};}
                else if(f.Kind=="patient"){var c=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Location=new Point(8,top),Width=340,Height=26,Font=new Font("Segoe UI",9F)};try{var people=DatabaseConnection.Query("SELECT patient_id,CONCAT(CONCAT_WS(' ',NULLIF(first_name,''),NULLIF(middle_name,''),NULLIF(last_name,'')),' (',student_id,')') AS patient_display FROM patients ORDER BY first_name,last_name");c.DisplayMember="patient_display";c.ValueMember="patient_id";c.DataSource=people;}catch{}ctl=c;}
                else if(f.Kind=="time"){ctl=new DateTimePicker{Format=DateTimePickerFormat.Custom,CustomFormat="hh:mm tt",ShowUpDown=true,Value=DateTime.Now,Location=new Point(8,top),Width=340,Font=new Font("Segoe UI",9F)};}
                else if(f.Kind=="long"){ctl=Ui.TextBox(true);ctl.Height=54;ctl.Location=new Point(8,top);ctl.Width=340;}
                else{ctl=Ui.TextBox();ctl.Location=new Point(8,top);ctl.Width=340;if(f.Kind=="calculated"){((TextBox)ctl).ReadOnly=true;ctl.BackColor=Color.FromArgb(237,243,243);}}
                fieldsPanel.Controls.Add(ctl);inputs[f.Key]=ctl;top+=ctl.Height+11;
            }
            if(table=="patients") { var birth=(DateTimePicker)inputs["date_of_birth"];birth.ValueChanged+=(s,e)=>UpdateAgeFromBirthdate();birth.MouseUp+=(s,e)=>UpdateAgeFromBirthdate();birth.KeyUp+=(s,e)=>UpdateAgeFromBirthdate();UpdateAgeFromBirthdate(); }
            if(table=="patients")
            {
                var type=(ComboBox)inputs["person_type"];var year=(ComboBox)inputs["grade_level"];
                var course=(ComboBox)inputs["section_course"];course.SelectedIndex=-1;
                type.SelectedIndexChanged+=(s,e)=>{if(type.Text=="Teacher"){year.SelectedItem="N/A";if(!course.Items.Contains("N/A"))course.Items.Add("N/A");course.SelectedItem="N/A";}else{if(course.Items.Contains("N/A"))course.Items.Remove("N/A");course.SelectedIndex=-1;if(year.Text=="N/A")year.SelectedItem="1st Year";}};
            }
            var footer=new Panel{Dock=DockStyle.Fill,BackColor=Color.White,Margin=Padding.Empty,Padding=new Padding(4,6,0,0)};editorLayout.Controls.Add(footer,0,1);
            var actions=new FlowLayoutPanel{Dock=DockStyle.Fill,WrapContents=false,AutoScroll=true,Margin=Padding.Empty,Padding=Padding.Empty};footer.Controls.Add(actions);
            AddAction(actions,"Add",Ui.Teal,(s,e)=>AddRecord());AddAction(actions,"Update",Ui.Blue,(s,e)=>UpdateRecord());AddAction(actions,"Delete",Color.FromArgb(189,74,74),(s,e)=>DeleteRecord());AddAction(actions,"Clear",Color.Gray,(s,e)=>Clear());
            search=Ui.TextBox();search.Location=new Point(430,73);search.Width=300;search.AccessibleName="Search records";search.AutoCompleteMode=AutoCompleteMode.SuggestAppend;search.AutoCompleteSource=AutoCompleteSource.CustomSource;Controls.Add(search);var sb=Ui.Button("Search",Ui.Blue);sb.Location=new Point(740,69);sb.Width=90;sb.Click+=(s,e)=>LoadRows(search.Text);Controls.Add(sb);var refresh=Ui.Button("Refresh",Ui.Teal);refresh.Location=new Point(838,69);refresh.Width=90;refresh.Click+=(s,e)=>{search.Clear();LoadRows("");LoadSuggestions();};Controls.Add(refresh);
            grid=DashboardForm.Grid();grid.Location=new Point(430,116);grid.Size=new Size(ClientSize.Width-454,ClientSize.Height-143);grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;grid.CellClick+=GridClick;Controls.Add(grid);LoadRows("");LoadSuggestions();
        }
        private void AddAction(FlowLayoutPanel p,string text,Color color,EventHandler handler){var b=Ui.Button(text,color);b.Width=74;b.Height=34;b.Margin=new Padding(2,2,3,2);b.Click+=handler;p.Controls.Add(b);}
        private string Projection(FieldDef f){if(f.Key=="full_name")return "CONCAT_WS(' ',NULLIF(first_name,''),NULLIF(middle_name,''),NULLIF(last_name,'')) AS `"+f.Label+"`";if(table=="patients"&&f.Key=="age")return "CASE WHEN date_of_birth IS NULL THEN NULL ELSE TIMESTAMPDIFF(YEAR,date_of_birth,CURDATE()) END AS `"+f.Label+"`";return f.Key+" AS `"+f.Label.Replace("`","")+"`";}
        private IEnumerable<string> Columns(FieldDef f){return f.Key=="full_name"?new[]{"first_name","middle_name","last_name"}:new[]{f.Key};}
        private void LoadSuggestions(){try{string sql=table=="patients"?"SELECT student_id AS suggestion FROM patients UNION SELECT CAST(patient_id AS CHAR) FROM patients UNION SELECT CONCAT_WS(' ',NULLIF(first_name,''),NULLIF(middle_name,''),NULLIF(last_name,'')) FROM patients":table=="medical_checkups"?"SELECT CAST(patient_id AS CHAR) AS suggestion FROM patients UNION SELECT chief_complaint FROM medical_checkups WHERE chief_complaint IS NOT NULL UNION SELECT diagnosis FROM medical_checkups WHERE diagnosis IS NOT NULL":table=="medical_history"?"SELECT CAST(patient_id AS CHAR) AS suggestion FROM patients UNION SELECT allergies FROM medical_history WHERE allergies IS NOT NULL":table=="medicines"?"SELECT medicine_name AS suggestion FROM medicines UNION SELECT description FROM medicines WHERE description IS NOT NULL":"";if(String.IsNullOrEmpty(sql))return;var rows=DatabaseConnection.Query(sql);var values=new AutoCompleteStringCollection();foreach(DataRow row in rows.Rows){string value=Convert.ToString(row[0]);if(!String.IsNullOrWhiteSpace(value)&&!values.Contains(value))values.Add(value);}search.AutoCompleteCustomSource=values;}catch{}}
        private void LoadRows(string term){try{string where=String.Join(" OR ",searchFields.Select(f=>"CAST("+f+" AS CHAR) LIKE @q"));grid.DataSource=DatabaseConnection.Query("SELECT "+key+" AS 'Record #',"+String.Join(",",fields.Select(Projection))+" FROM "+table+(String.IsNullOrEmpty(where)?"":" WHERE "+where)+" ORDER BY "+key+" DESC LIMIT 500",DatabaseConnection.P("@q","%"+(term??"").Trim()+"%"));selectedId=0;}catch(Exception ex){MessageBox.Show("Could not load "+title+".\n"+ex.Message,"Database",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
        private void GridClick(object s,DataGridViewCellEventArgs e){if(e.RowIndex<0)return;var row=grid.Rows[e.RowIndex];selectedId=Convert.ToInt32(row.Cells[0].Value);foreach(var f in fields){object value=row.Cells[f.Label].Value;Control c=inputs[f.Key];if(c is ComboBox){var combo=(ComboBox)c;if(f.Kind=="patient"){combo.SelectedValue=value==DBNull.Value?null:value;}else{string text=value==DBNull.Value?"":Convert.ToString(value);if(combo.DropDownStyle==ComboBoxStyle.DropDownList){combo.SelectedItem=text;if(combo.SelectedIndex<0)combo.SelectedIndex=-1;}else combo.Text=text;}}else if(c is DateTimePicker){var picker=(DateTimePicker)c;if(f.Kind=="time"){TimeSpan parsed;if(value!=DBNull.Value&&TimeSpan.TryParse(Convert.ToString(value),out parsed))picker.Value=DateTime.Today.Add(parsed);}else{DateTime dt=DateTime.Today;picker.Checked=value!=DBNull.Value&&DateTime.TryParse(Convert.ToString(value),out dt);if(picker.Checked)picker.Value=dt;}}else c.Text=value==DBNull.Value?"":Convert.ToString(value);}if(table=="patients")UpdateAgeFromBirthdate();}
        private void UpdateAgeFromBirthdate(){if(table!="patients"||!inputs.ContainsKey("date_of_birth"))return;var dob=(DateTimePicker)inputs["date_of_birth"];var age=(TextBox)inputs["age"];if(!dob.Checked||dob.Value.Date>DateTime.Today){age.Clear();return;}int years=DateTime.Today.Year-dob.Value.Year;if(dob.Value.Date>DateTime.Today.AddYears(-years))years--;age.Text=years.ToString();}
        private object Value(FieldDef f){Control c=inputs[f.Key];if(table=="patients"&&((ComboBox)inputs["person_type"]).Text=="Teacher"){if(f.Key=="section_course"||f.Key=="grade_level")return "N/A";}if(f.Kind=="patient"){var combo=(ComboBox)c;return combo.SelectedValue==null?(object)DBNull.Value:Convert.ToInt32(combo.SelectedValue);}if(c is ComboBox){string selected=((ComboBox)c).Text.Trim();return selected==""?(object)DBNull.Value:selected;}if(c is DateTimePicker){var d=(DateTimePicker)c;if(f.Kind=="time")return d.Value.ToString("HH:mm:ss");return d.Checked?(object)d.Value.Date:DBNull.Value;}string value=c.Text.Trim();if(value=="")return DBNull.Value;if(f.Key=="age"||f.Key=="quantity") {int n;if(!Int32.TryParse(value,out n)||n<0)throw new InvalidOperationException(f.Label+" must be a non-negative whole number.");return n;}if(f.Key=="temperature"||f.Key=="weight_kg"||f.Key=="height_cm"){decimal n;if(!Decimal.TryParse(value,out n)||n<0)throw new InvalidOperationException(f.Label+" must be a valid non-negative number.");return n;}return value;}
        private MySqlParameter[] Parameters(){var all=new List<MySqlParameter>();foreach(var f in fields){if(f.Key=="full_name"){string[] parts=Convert.ToString(Value(f)).Split(new[]{' '},StringSplitOptions.RemoveEmptyEntries);string first=parts.Length>0?parts[0]:"";string last=parts.Length>1?parts[parts.Length-1]:"";string middle=parts.Length>2?String.Join(" ",parts.Skip(1).Take(parts.Length-2)):null;all.Add(DatabaseConnection.P("@first_name",first));all.Add(DatabaseConnection.P("@middle_name",middle));all.Add(DatabaseConnection.P("@last_name",last));}else all.Add(DatabaseConnection.P("@"+f.Key,Value(f)));}return all.ToArray();}
        private void ValidateRecord(){if(table=="patients"){if(String.IsNullOrWhiteSpace(inputs["student_id"].Text)||String.IsNullOrWhiteSpace(inputs["full_name"].Text))throw new InvalidOperationException("ID Number and Full name are required.");if(String.IsNullOrWhiteSpace(((ComboBox)inputs["person_type"]).Text)||String.IsNullOrWhiteSpace(((ComboBox)inputs["gender"]).Text))throw new InvalidOperationException("Patient type and gender are required.");if(((DateTimePicker)inputs["date_of_birth"]).Checked&&((DateTimePicker)inputs["date_of_birth"]).Value.Date>DateTime.Today)throw new InvalidOperationException("Birthdate cannot be in the future.");if(((ComboBox)inputs["person_type"]).Text=="Student"&&String.IsNullOrWhiteSpace(((ComboBox)inputs["section_course"]).Text))throw new InvalidOperationException("Select the student's course from the list.");if(((ComboBox)inputs["person_type"]).Text=="Student"&&((ComboBox)inputs["grade_level"]).Text=="N/A")throw new InvalidOperationException("Select a year level from 1st Year to 5th Year for students.");}if((table=="medical_checkups"||table=="medical_history")&&((ComboBox)inputs["patient_id"]).SelectedValue==null)throw new InvalidOperationException("Select a patient from the list.");}
        private void AddRecord(){try{ValidateRecord();var p=Parameters();var cols=fields.SelectMany(Columns).ToArray();string sql="INSERT INTO "+table+"("+String.Join(",",cols)+ ") VALUES("+String.Join(",",cols.Select(c=>"@"+c))+")";DatabaseConnection.Execute(sql,p);Ui.Audit("CREATE",table,"new");LoadRows(search.Text);LoadSuggestions();Clear();}catch(Exception ex){MessageBox.Show("Please check the entered values.\n"+ex.Message,"Save record",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void UpdateRecord(){if(selectedId==0){MessageBox.Show("Select a row first.");return;}try{ValidateRecord();var p=Parameters().ToList();p.Add(DatabaseConnection.P("@id",selectedId));var cols=fields.SelectMany(Columns);DatabaseConnection.Execute("UPDATE "+table+" SET "+String.Join(",",cols.Select(c=>c+"=@"+c))+" WHERE "+key+"=@id",p.ToArray());Ui.Audit("UPDATE",table,selectedId);LoadRows(search.Text);LoadSuggestions();}catch(Exception ex){MessageBox.Show("Could not update record.\n"+ex.Message,"Save record",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void DeleteRecord(){if(selectedId==0){MessageBox.Show("Select a row first.");return;}if(MessageBox.Show("Delete this record?","Confirm delete",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return;try{DatabaseConnection.Execute("DELETE FROM "+table+" WHERE "+key+"=@id",DatabaseConnection.P("@id",selectedId));Ui.Audit("DELETE",table,selectedId);Clear();LoadRows(search.Text);}catch(Exception ex){MessageBox.Show("Could not delete this record. Related patient records may depend on it.\n"+ex.Message,"Delete record",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void Clear(){selectedId=0;foreach(var f in fields){var c=inputs[f.Key];if(c is ComboBox){if(f.Kind=="patient")((ComboBox)c).SelectedIndex=-1;else if(f.Key=="section_course")((ComboBox)c).SelectedIndex=-1;else if(f.Kind=="select")((ComboBox)c).SelectedIndex=0;else ((ComboBox)c).Text="";}else if(c is DateTimePicker){if(f.Kind=="time")((DateTimePicker)c).Value=DateTime.Now;else ((DateTimePicker)c).Checked=false;}else c.Text="";}}
        public static RecordForm Patients(){return new RecordForm("Patient records","patients","patient_id",new[]{new FieldDef("student_id","ID Number"),new FieldDef("person_type","Patient type (student / teacher)","select"),new FieldDef("full_name","Full name"),new FieldDef("age","Age","calculated"),new FieldDef("gender","Gender","select"),new FieldDef("date_of_birth","Birthdate","date"),new FieldDef("grade_level","Year level","select"),new FieldDef("section_course","Course","select"),new FieldDef("address","Address","long"),new FieldDef("contact_number","Contact number"),new FieldDef("guardian_name","Emergency contact person"),new FieldDef("guardian_contact","Emergency contact number")},new[]{"student_id","first_name","middle_name","last_name"});}
        public static RecordForm Checkups(){return new RecordForm("Medical checkups","medical_checkups","checkup_id",new[]{new FieldDef("patient_id","Select patient","patient"),new FieldDef("visit_date","Date of visit","date"),new FieldDef("visit_time","Time of visit","time"),new FieldDef("chief_complaint","Chief complaint","long"),new FieldDef("symptoms","Symptoms","long"),new FieldDef("temperature","Temperature °C"),new FieldDef("blood_pressure","Blood pressure"),new FieldDef("weight_kg","Weight kg"),new FieldDef("height_cm","Height cm"),new FieldDef("diagnosis","Diagnosis","long"),new FieldDef("treatment_given","Treatment given","long"),new FieldDef("attending_staff","Attending staff"),new FieldDef("remarks","Remarks","long")},new[]{"patient_id","chief_complaint","diagnosis"});}
        public static RecordForm History(){return new RecordForm("Medical history","medical_history","history_id",new[]{new FieldDef("patient_id","Select patient","patient"),new FieldDef("allergies","Allergies","long"),new FieldDef("existing_conditions","Existing conditions","long"),new FieldDef("previous_illnesses","Previous illnesses","long"),new FieldDef("current_medications","Current medications","long"),new FieldDef("immunization_history","Immunization history","long"),new FieldDef("emergency_contact","Emergency contact"),new FieldDef("medical_notes","Medical notes","long")},new[]{"patient_id","allergies","existing_conditions"});}
        public static RecordForm Medicines(){return new RecordForm("Medicine inventory","medicines","medicine_id",new[]{new FieldDef("medicine_name","Medicine name"),new FieldDef("description","Description"),new FieldDef("quantity","Quantity"),new FieldDef("unit","Unit"),new FieldDef("expiration_date","Expiration date","date"),new FieldDef("date_added","Date added","date"),new FieldDef("remarks","Remarks","long")},new[]{"medicine_name","description"});}
    }

    public class MedicalRecordsForm : Form
    {
        private readonly TextBox search;
        private readonly Label patientName, patientDetails;
        private readonly Button issueButton;
        private readonly DataGridView checkups, history, issues;
        private readonly Timer searchTimer;
        private int selectedPatientId;

        public MedicalRecordsForm()
        {
            Text="Medical records lookup";BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);MaximizeBox=true;Ui.FitToScreen(this,new Size(1120,760),new Size(900,560));
            Controls.Add(new Label{Text="Medical records",Font=new Font("Segoe UI",20,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(22,18),AutoSize=true});
            Controls.Add(new Label{Text="Search using the system Patient ID or the student / teacher ID number.",ForeColor=Ui.Muted,Location=new Point(24,55),AutoSize=true});
            search=Ui.TextBox();search.Location=new Point(24,83);search.Width=300;search.AccessibleName="Patient ID or ID Number";search.AutoCompleteMode=AutoCompleteMode.SuggestAppend;search.AutoCompleteSource=AutoCompleteSource.CustomSource;Controls.Add(search);
            var find=Ui.Button("Find patient",Ui.Blue);find.Location=new Point(338,79);find.Width=112;find.Click+=(s,e)=>LoadPatient();Controls.Add(find);
            issueButton=Ui.Button("Issue medicine",Ui.Teal);issueButton.Location=new Point(464,79);issueButton.Width=135;issueButton.Enabled=false;issueButton.Click+=IssueMedicine;Controls.Add(issueButton);
            var person=Ui.Card();person.Location=new Point(22,130);person.Size=new Size(ClientSize.Width-44,128);person.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;Controls.Add(person);
            patientName=new Label{Text="No patient selected",Font=new Font("Segoe UI",13,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(18,14),AutoSize=true};person.Controls.Add(patientName);
            patientDetails=new Label{Text="Enter a Patient ID or ID Number to view the patient's records.",ForeColor=Ui.Muted,Location=new Point(19,47),Size=new Size(person.ClientSize.Width-38,72)};patientDetails.Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right;person.Controls.Add(patientDetails);
            var tabs=new TabControl{Location=new Point(22,270),Size=new Size(ClientSize.Width-44,Math.Max(220,ClientSize.Height-292)),Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right,Font=new Font("Segoe UI",9F)};Controls.Add(tabs);
            checkups=AddTab(tabs,"Checkups");history=AddTab(tabs,"Medical history");issues=AddTab(tabs,"Medicines issued");
            search.KeyDown+=(s,e)=>{if(e.KeyCode==Keys.Enter){e.SuppressKeyPress=true;LoadPatient();}};
            searchTimer=new Timer{Interval=500};searchTimer.Tick+=(s,e)=>{searchTimer.Stop();if(!String.IsNullOrWhiteSpace(search.Text))LoadPatient();else ClearPatient();};search.TextChanged+=(s,e)=>{searchTimer.Stop();searchTimer.Start();};LoadPatientSuggestions();
        }
        private DataGridView AddTab(TabControl tabs,string name){var page=new TabPage(name){BackColor=Color.White};var g=DashboardForm.Grid();g.Dock=DockStyle.Fill;page.Controls.Add(g);tabs.TabPages.Add(page);return g;}
        private void LoadPatient()
        {
            string query=search.Text.Trim();if(query.Length==0){ClearPatient();return;}
            try
            {
                DataTable rows=DatabaseConnection.Query("SELECT patient_id,student_id,person_type,CONCAT_WS(' ',first_name,NULLIF(middle_name,''),last_name) AS full_name,CASE WHEN date_of_birth IS NULL THEN NULL ELSE TIMESTAMPDIFF(YEAR,date_of_birth,CURDATE()) END AS age,gender,date_of_birth,address,contact_number,guardian_name,guardian_contact,grade_level,section_course FROM patients WHERE CAST(patient_id AS CHAR)=@q OR student_id=@q OR CONCAT_WS(' ',first_name,NULLIF(middle_name,''),last_name) LIKE @name LIMIT 1",DatabaseConnection.P("@q",query),DatabaseConnection.P("@name","%"+query+"%"));
                if(rows.Rows.Count==0){ClearPatient();patientName.Text="Patient not found";patientDetails.Text="Check the Patient ID or ID Number and try again.";return;}
                DataRow p=rows.Rows[0];selectedPatientId=Convert.ToInt32(p["patient_id"]);issueButton.Enabled=true;
                patientName.Text=Convert.ToString(p["full_name"])+"  ·  "+Convert.ToString(p["person_type"]);
                patientDetails.Text="Patient ID: "+p["patient_id"]+"   |   ID Number: "+p["student_id"]+"   |   "+p["gender"]+"   |   Age: "+(p["age"]==DBNull.Value?"—":Convert.ToString(p["age"]))+"   |   Birthdate: "+(p["date_of_birth"]==DBNull.Value?"—":Convert.ToDateTime(p["date_of_birth"]).ToString("yyyy-MM-dd"))+"   |   Year: "+(p["grade_level"]==DBNull.Value?"—":Convert.ToString(p["grade_level"]))+"   |   Course: "+(p["section_course"]==DBNull.Value?"—":Convert.ToString(p["section_course"]))+"\r\nAddress: "+(p["address"]==DBNull.Value?"—":Convert.ToString(p["address"]))+"   |   Contact: "+(p["contact_number"]==DBNull.Value?"—":Convert.ToString(p["contact_number"]))+"\r\nEmergency contact: "+(p["guardian_name"]==DBNull.Value?"—":Convert.ToString(p["guardian_name"]))+"   |   "+(p["guardian_contact"]==DBNull.Value?"—":Convert.ToString(p["guardian_contact"]));
                checkups.DataSource=DatabaseConnection.Query("SELECT checkup_id AS 'Visit #',visit_date AS 'Date',visit_time AS 'Time',chief_complaint AS 'Complaint',diagnosis AS 'Diagnosis',treatment_given AS 'Treatment',attending_staff AS 'Staff' FROM medical_checkups WHERE patient_id=@id ORDER BY visit_date DESC,checkup_id DESC",DatabaseConnection.P("@id",selectedPatientId));
                history.DataSource=DatabaseConnection.Query("SELECT allergies AS 'Allergies',existing_conditions AS 'Existing conditions',previous_illnesses AS 'Previous illnesses',current_medications AS 'Current medications',immunization_history AS 'Immunizations',emergency_contact AS 'Emergency contact',medical_notes AS 'Notes',updated_at AS 'Updated' FROM medical_history WHERE patient_id=@id",DatabaseConnection.P("@id",selectedPatientId));
                issues.DataSource=DatabaseConnection.Query("SELECT i.issue_id AS 'Issue #',m.medicine_name AS 'Medicine',i.quantity AS 'Quantity',m.unit AS 'Unit',i.issued_at AS 'Issued at',u.full_name AS 'Issued by' FROM medicine_issues i JOIN medicines m ON m.medicine_id=i.medicine_id LEFT JOIN users u ON u.user_id=i.issued_by WHERE i.patient_id=@id ORDER BY i.issued_at DESC",DatabaseConnection.P("@id",selectedPatientId));
            }
            catch(Exception ex){MessageBox.Show("Could not retrieve this patient's records. If this database was created earlier, run the client update script first.\n\n"+ex.Message,"Medical records",MessageBoxButtons.OK,MessageBoxIcon.Error);}
        }
        private void LoadPatientSuggestions(){try{var rows=DatabaseConnection.Query("SELECT student_id AS suggestion FROM patients UNION SELECT CAST(patient_id AS CHAR) FROM patients UNION SELECT CONCAT_WS(' ',NULLIF(first_name,''),NULLIF(middle_name,''),NULLIF(last_name,'')) FROM patients");var source=new AutoCompleteStringCollection();foreach(DataRow row in rows.Rows){string value=Convert.ToString(row[0]);if(!String.IsNullOrWhiteSpace(value)&&!source.Contains(value))source.Add(value);}search.AutoCompleteCustomSource=source;}catch{}}
        private void ClearPatient(){selectedPatientId=0;issueButton.Enabled=false;patientName.Text="No patient selected";patientDetails.Text="Enter a Patient ID or ID Number to view the patient's records.";checkups.DataSource=null;history.DataSource=null;issues.DataSource=null;}
        private void IssueMedicine(object sender,EventArgs e){if(selectedPatientId==0)return;using(var form=new MedicineIssueForm(selectedPatientId,patientName.Text))if(form.ShowDialog(this)==DialogResult.OK)LoadPatient();}
    }

    internal class MedicineIssueForm : Form
    {
        private readonly int patientId;
        private readonly ComboBox medicine;
        private readonly NumericUpDown quantity;
        private readonly Label stockLabel;
        private DataTable stockRows;
        public MedicineIssueForm(int patientId,string patientName)
        {
            this.patientId=patientId;Text="Issue medicine";StartPosition=FormStartPosition.CenterParent;FormBorderStyle=FormBorderStyle.FixedDialog;MaximizeBox=false;MinimizeBox=false;ClientSize=new Size(510,360);BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);
            var card=Ui.Card();card.Location=new Point(18,18);card.Size=new Size(474,322);Controls.Add(card);
            card.Controls.Add(new Label{Text="Issue medicine to",Font=new Font("Segoe UI",9,FontStyle.Bold),ForeColor=Ui.Muted,Location=new Point(22,18),AutoSize=true});
            card.Controls.Add(new Label{Text=patientName,Font=new Font("Segoe UI",12,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(22,40),Size=new Size(420,28)});
            card.Controls.Add(Ui.Label("Medicine"));card.Controls[card.Controls.Count-1].Location=new Point(22,86);
            medicine=new ComboBox{Location=new Point(22,108),Width=425,DropDownStyle=ComboBoxStyle.DropDownList,Font=new Font("Segoe UI",9F)};medicine.SelectedIndexChanged+=(s,e)=>UpdateStock();card.Controls.Add(medicine);
            stockLabel=new Label{Text="Available stock: —",ForeColor=Ui.Muted,AutoSize=true,Location=new Point(22,145)};card.Controls.Add(stockLabel);
            card.Controls.Add(Ui.Label("Quantity to issue"));card.Controls[card.Controls.Count-1].Location=new Point(22,182);
            quantity=new NumericUpDown{Location=new Point(22,204),Width=130,Minimum=1,Maximum=1,Value=1,Font=new Font("Segoe UI",10F)};card.Controls.Add(quantity);
            var note=new Label{Text="Stock is deducted only when the issue is saved.",ForeColor=Ui.Muted,AutoSize=true,Location=new Point(22,242)};card.Controls.Add(note);
            var save=Ui.Button("Record issue",Ui.Teal);save.Location=new Point(22,274);save.Width=140;save.Click+=SaveIssue;card.Controls.Add(save);var cancel=Ui.Button("Cancel",Color.FromArgb(130,145,148));cancel.Location=new Point(172,274);cancel.Width=100;cancel.Click+=(s,e)=>Close();card.Controls.Add(cancel);
            LoadStock();
        }
        private void LoadStock(){try{stockRows=DatabaseConnection.Query("SELECT medicine_id,medicine_name,quantity,unit FROM medicines WHERE quantity>0 AND (expiration_date IS NULL OR expiration_date>=CURDATE()) ORDER BY medicine_name");medicine.DisplayMember="medicine_name";medicine.ValueMember="medicine_id";medicine.DataSource=stockRows;if(stockRows.Rows.Count==0){stockLabel.Text="No unexpired stock is currently available.";medicine.Enabled=false;}}catch(Exception ex){MessageBox.Show("Could not load inventory.\n"+ex.Message);}}
        private void UpdateStock(){if(medicine.SelectedItem==null||stockRows==null)return;var row=((DataRowView)medicine.SelectedItem).Row;int available=Convert.ToInt32(row["quantity"]);stockLabel.Text="Available stock: "+available+" "+row["unit"];quantity.Maximum=Math.Max(1,available);if(quantity.Value>quantity.Maximum)quantity.Value=quantity.Maximum;}
        private void SaveIssue(object sender,EventArgs e)
        {
            if(medicine.SelectedValue==null||stockRows==null||stockRows.Rows.Count==0){MessageBox.Show("No available medicine can be issued.");return;}
            int medicineId=Convert.ToInt32(medicine.SelectedValue),amount=Convert.ToInt32(quantity.Value);
            try
            {
                using(var connection=DatabaseConnection.Open())using(var transaction=connection.BeginTransaction())
                {
                    using(var update=new MySqlCommand("UPDATE medicines SET quantity=quantity-@qty WHERE medicine_id=@medicine AND quantity>=@qty AND (expiration_date IS NULL OR expiration_date>=CURDATE())",connection,transaction))
                    {update.Parameters.AddWithValue("@qty",amount);update.Parameters.AddWithValue("@medicine",medicineId);if(update.ExecuteNonQuery()!=1)throw new InvalidOperationException("There is not enough unexpired stock for this issue. Refresh the inventory and try again.");}
                    using(var insert=new MySqlCommand("INSERT INTO medicine_issues(patient_id,medicine_id,quantity,issued_by) VALUES(@patient,@medicine,@qty,@user)",connection,transaction))
                    {insert.Parameters.AddWithValue("@patient",patientId);insert.Parameters.AddWithValue("@medicine",medicineId);insert.Parameters.AddWithValue("@qty",amount);insert.Parameters.AddWithValue("@user",SessionManager.UserId==0?(object)DBNull.Value:SessionManager.UserId);insert.ExecuteNonQuery();}
                    transaction.Commit();
                }
                Ui.Audit("ISSUE","medicine",medicineId);DialogResult=DialogResult.OK;Close();
            }
            catch(Exception ex){MessageBox.Show("Medicine was not issued. No stock was deducted.\n\n"+ex.Message,"Issue medicine",MessageBoxButtons.OK,MessageBoxIcon.Warning);}
        }
    }

    public class UsersForm : Form
    {
        private readonly DataGridView grid; private readonly TextBox name,user,password; private readonly ComboBox role; private int id;
        public UsersForm(){if(!SessionManager.IsAdmin){Load+=(s,e)=>Close();return;}Text="User management";StartPosition=FormStartPosition.CenterParent;Size=new Size(900,600);BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);Controls.Add(new Label{Text="User management",Font=new Font("Segoe UI",20,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(20,16),AutoSize=true});var p=Ui.Card();p.Location=new Point(20,62);p.Size=new Size(840,156);Controls.Add(p);
            string[] labels={"Full name","Username","Temporary password","Role"};int[] xs={15,220,425,630};for(int i=0;i<4;i++){p.Controls.Add(Ui.Label(labels[i]));p.Controls[p.Controls.Count-1].Location=new Point(xs[i],16);}
            name=Ui.TextBox();user=Ui.TextBox();password=Ui.TextBox();role=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Font=new Font("Segoe UI",9F)};role.Items.AddRange(new object[]{"Nurse","Admin"});role.SelectedIndex=0;Control[] c={name,user,password,role};for(int i=0;i<4;i++){c[i].Location=new Point(xs[i],38);c[i].Width=185;p.Controls.Add(c[i]);}
            var add=Ui.Button("Add user",Ui.Teal);add.Location=new Point(15,89);add.Width=100;add.Click+=(s,e)=>Save(false);p.Controls.Add(add);var edit=Ui.Button("Update selected",Ui.Blue);edit.Location=new Point(125,89);edit.Width=130;edit.Click+=(s,e)=>Save(true);p.Controls.Add(edit);var del=Ui.Button("Delete selected",Color.FromArgb(189,74,74));del.Location=new Point(265,89);del.Width=130;del.Click+=(s,e)=>Delete();p.Controls.Add(del);
            grid=DashboardForm.Grid();grid.Location=new Point(20,235);grid.Size=new Size(840,290);grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;grid.CellClick+=(s,e)=>{if(e.RowIndex>=0){var r=grid.Rows[e.RowIndex];id=Convert.ToInt32(r.Cells[0].Value);name.Text=Convert.ToString(r.Cells[1].Value);user.Text=Convert.ToString(r.Cells[2].Value);role.SelectedItem=Convert.ToString(r.Cells[3].Value);password.Clear();}};Controls.Add(grid);LoadRows();}
        private void LoadRows(){try{grid.DataSource=DatabaseConnection.Query("SELECT user_id AS 'ID',full_name AS 'Full name',username AS 'Username',role AS 'Role',is_active AS 'Active' FROM users ORDER BY user_id");}catch(Exception ex){MessageBox.Show(ex.Message);}}
        private void Save(bool update){if(String.IsNullOrWhiteSpace(name.Text)||String.IsNullOrWhiteSpace(user.Text)||(!update&&password.Text.Length<10)){MessageBox.Show("Enter a name and username; new passwords must be at least 10 characters.");return;}try{if(update){if(id==0){MessageBox.Show("Select a user first.");return;}if(String.IsNullOrEmpty(password.Text))DatabaseConnection.Execute("UPDATE users SET full_name=@n,username=@u,role=@r WHERE user_id=@id",DatabaseConnection.P("@n",name.Text.Trim()),DatabaseConnection.P("@u",user.Text.Trim()),DatabaseConnection.P("@r",role.Text),DatabaseConnection.P("@id",id));else if(password.Text.Length<10)throw new InvalidOperationException("Password must be at least 10 characters.");else DatabaseConnection.Execute("UPDATE users SET full_name=@n,username=@u,role=@r,password_hash=@p WHERE user_id=@id",DatabaseConnection.P("@n",name.Text.Trim()),DatabaseConnection.P("@u",user.Text.Trim()),DatabaseConnection.P("@r",role.Text),DatabaseConnection.P("@p",PasswordHelper.Hash(password.Text)),DatabaseConnection.P("@id",id));Ui.Audit("UPDATE","users",id);}else{DatabaseConnection.Execute("INSERT INTO users(full_name,username,password_hash,role) VALUES(@n,@u,@p,@r)",DatabaseConnection.P("@n",name.Text.Trim()),DatabaseConnection.P("@u",user.Text.Trim()),DatabaseConnection.P("@p",PasswordHelper.Hash(password.Text)),DatabaseConnection.P("@r",role.Text));Ui.Audit("CREATE","users","new");}LoadRows();id=0;name.Clear();user.Clear();password.Clear();}catch(Exception ex){MessageBox.Show("Could not save account. Username may be in use.\n"+ex.Message);}}
        private void Delete(){if(id==0||id==SessionManager.UserId){MessageBox.Show("Select another account to delete; your current account is protected.");return;}if(MessageBox.Show("Delete this account?","Confirm delete",MessageBoxButtons.YesNo)!=DialogResult.Yes)return;try{DatabaseConnection.Execute("DELETE FROM users WHERE user_id=@id",DatabaseConnection.P("@id",id));Ui.Audit("DELETE","users",id);LoadRows();id=0;}catch(Exception ex){MessageBox.Show(ex.Message);}}
    }

    public class ReportsForm : Form
    {
        private readonly DataGridView grid; private readonly ComboBox type; private readonly DateTimePicker fromDate,toDate; private readonly PrintDocument printDocument; private int printRow;
        public ReportsForm(){Text="Reports";BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);Ui.FitToScreen(this,new Size(1100,700),new Size(900,560));Controls.Add(new Label{Text="Clinic reports",Font=new Font("Segoe UI",20,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(22,16),AutoSize=true});var toolbar=new FlowLayoutPanel{Location=new Point(20,60),Size=new Size(ClientSize.Width-40,82),Anchor=AnchorStyles.Top|AnchorStyles.Left|AnchorStyles.Right,WrapContents=true,AutoScroll=true,Padding=new Padding(2),Margin=Padding.Empty};Controls.Add(toolbar);type=new ComboBox{Width=200,Height=30,DropDownStyle=ComboBoxStyle.DropDownList,Font=new Font("Segoe UI",9F),Margin=new Padding(2,8,8,2)};type.Items.AddRange(new object[]{"Patient visits (date range)","Daily patient visits","Weekly patient visits","Monthly patient visits","Medical checkups","Patient list","Student/teacher summary","Students by course","Students by year level","Medicine inventory"});type.SelectedIndex=0;toolbar.Controls.Add(type);fromDate=new DateTimePicker{Format=DateTimePickerFormat.Custom,CustomFormat="yyyy-MM-dd",Width=115,Value=DateTime.Today.AddDays(-29),Margin=new Padding(2,8,4,2)};toolbar.Controls.Add(new Label{Text="From",AutoSize=true,ForeColor=Ui.Muted,Margin=new Padding(4,15,0,0)});toolbar.Controls.Add(fromDate);toDate=new DateTimePicker{Format=DateTimePickerFormat.Custom,CustomFormat="yyyy-MM-dd",Width=115,Margin=new Padding(2,8,8,2)};toolbar.Controls.Add(new Label{Text="To",AutoSize=true,ForeColor=Ui.Muted,Margin=new Padding(2,15,0,0)});toolbar.Controls.Add(toDate);var load=Ui.Button("Generate",Ui.Blue);load.Width=86;load.Margin=new Padding(2,6,4,2);load.Click+=(s,e)=>LoadReport();toolbar.Controls.Add(load);var print=Ui.Button("Print",Ui.Teal);print.Width=76;print.Margin=new Padding(2,6,4,2);print.Click+=(s,e)=>PrintReport();toolbar.Controls.Add(print);var pdf=Ui.Button("Export PDF",Ui.Teal);pdf.Width=95;pdf.Margin=new Padding(2,6,4,2);pdf.Click+=(s,e)=>ExportPdf();toolbar.Controls.Add(pdf);var csv=Ui.Button("Export CSV",Color.FromArgb(91,125,132));csv.Width=95;csv.Margin=new Padding(2,6,4,2);csv.Click+=(s,e)=>ExportCsv();toolbar.Controls.Add(csv);var tableCard=Ui.Card();tableCard.Location=new Point(20,150);tableCard.Size=new Size(ClientSize.Width-40,ClientSize.Height-170);tableCard.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;Controls.Add(tableCard);grid=DashboardForm.Grid();grid.Location=new Point(10,10);grid.Size=new Size(tableCard.ClientSize.Width-20,tableCard.ClientSize.Height-20);grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;tableCard.Controls.Add(grid);printDocument=new PrintDocument();printDocument.DefaultPageSettings.Landscape=true;printDocument.PrintPage+=PrintPage;LoadReport();}
        private void LoadReport(){if(fromDate.Value.Date>toDate.Value.Date){MessageBox.Show("The From date must be on or before the To date.","Report dates",MessageBoxButtons.OK,MessageBoxIcon.Information);return;}string sql;MySqlParameter[] parameters=null;switch(type.Text){case "Patient list":sql="SELECT patient_id,student_id AS id_number,person_type,CONCAT_WS(' ',NULLIF(first_name,''),NULLIF(middle_name,''),NULLIF(last_name,'')) AS full_name,CASE WHEN date_of_birth IS NULL THEN NULL ELSE TIMESTAMPDIFF(YEAR,date_of_birth,CURDATE()) END AS age,gender,grade_level AS year_level,section_course AS course FROM patients ORDER BY last_name,first_name";break;case "Medical checkups":sql="SELECT c.checkup_id,c.visit_date,c.visit_time,p.student_id AS id_number,p.person_type,CONCAT_WS(' ',NULLIF(p.first_name,''),NULLIF(p.middle_name,''),NULLIF(p.last_name,'')) AS patient,c.chief_complaint,c.diagnosis FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id WHERE c.visit_date BETWEEN @from AND @to ORDER BY c.visit_date DESC,c.checkup_id DESC";parameters=new MySqlParameter[]{DatabaseConnection.P("@from",fromDate.Value.Date),DatabaseConnection.P("@to",toDate.Value.Date)};break;case "Medicine inventory":sql="SELECT medicine_id,medicine_name,quantity,unit,expiration_date,remarks FROM medicines ORDER BY medicine_name";break;case "Student/teacher summary":sql="SELECT person_type,gender,COUNT(*) AS total FROM patients GROUP BY person_type,gender ORDER BY person_type,gender";break;case "Students by course":sql="SELECT COALESCE(NULLIF(section_course,''),'Not set') AS course,COUNT(*) AS students FROM patients WHERE person_type='Student' GROUP BY section_course ORDER BY course";break;case "Students by year level":sql="SELECT COALESCE(NULLIF(grade_level,''),'Not set') AS year_level,COUNT(*) AS students FROM patients WHERE person_type='Student' GROUP BY grade_level ORDER BY grade_level";break;default:if(type.Text=="Daily patient visits") {sql="SELECT c.checkup_id,c.visit_date,c.visit_time,p.student_id AS id_number,p.person_type,CONCAT_WS(' ',NULLIF(p.first_name,''),NULLIF(p.middle_name,''),NULLIF(p.last_name,'')) AS patient,c.chief_complaint,c.diagnosis FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id WHERE c.visit_date=@from ORDER BY c.visit_date DESC,c.checkup_id DESC";parameters=new MySqlParameter[]{DatabaseConnection.P("@from",fromDate.Value.Date)};}else{DateTime start=fromDate.Value.Date;if(type.Text=="Weekly patient visits")start=toDate.Value.Date.AddDays(-6);if(type.Text=="Monthly patient visits")start=toDate.Value.Date.AddMonths(-1).AddDays(1);sql="SELECT c.checkup_id,c.visit_date,c.visit_time,p.student_id AS id_number,p.person_type,CONCAT_WS(' ',NULLIF(p.first_name,''),NULLIF(p.middle_name,''),NULLIF(p.last_name,'')) AS patient,c.chief_complaint,c.diagnosis FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id WHERE c.visit_date BETWEEN @from AND @to ORDER BY c.visit_date DESC,c.checkup_id DESC";parameters=new MySqlParameter[]{DatabaseConnection.P("@from",start),DatabaseConnection.P("@to",toDate.Value.Date)};}break;}try{grid.DataSource=parameters==null?DatabaseConnection.Query(sql):DatabaseConnection.Query(sql,parameters);}catch(Exception ex){MessageBox.Show("Could not generate report.\n"+ex.Message);}}
        private bool HasRows(){if(grid.DataSource==null||grid.Rows.Count==0){MessageBox.Show("Generate a report with rows first.","Clinic reports",MessageBoxButtons.OK,MessageBoxIcon.Information);return false;}return true;}
        private void ExportCsv(){if(!HasRows())return;using(var d=new SaveFileDialog{Filter="CSV files (*.csv)|*.csv",FileName="ClinicReport_"+DateTime.Now.ToString("yyyyMMdd")+".csv"})if(d.ShowDialog()==DialogResult.OK){var sb=new StringBuilder();sb.AppendLine(String.Join(",",grid.Columns.Cast<DataGridViewColumn>().Select(c=>Quote(c.HeaderText))));foreach(DataGridViewRow row in grid.Rows)if(!row.IsNewRow)sb.AppendLine(String.Join(",",row.Cells.Cast<DataGridViewCell>().Select(c=>Quote(Convert.ToString(c.Value)))));System.IO.File.WriteAllText(d.FileName,sb.ToString(),Encoding.UTF8);MessageBox.Show("Report exported.");}}
        private void PrintReport(){if(!HasRows())return;using(var dialog=new PrintDialog{Document=printDocument,UseEXDialog=true})if(dialog.ShowDialog(this)==DialogResult.OK){printRow=0;try{printDocument.PrinterSettings=dialog.PrinterSettings;printDocument.Print();}catch(Exception ex){MessageBox.Show("Could not print the report.\n"+ex.Message,"Print report",MessageBoxButtons.OK,MessageBoxIcon.Error);}}}
        private void ExportPdf(){if(!HasRows())return;string printer=PrinterSettings.InstalledPrinters.Cast<string>().FirstOrDefault(p=>p.IndexOf("Microsoft Print to PDF",StringComparison.OrdinalIgnoreCase)>=0);if(printer==null){MessageBox.Show("Microsoft Print to PDF is not installed. Use Print and select an available PDF printer, or enable Microsoft Print to PDF in Windows.","PDF export",MessageBoxButtons.OK,MessageBoxIcon.Information);return;}using(var d=new SaveFileDialog{Filter="PDF files (*.pdf)|*.pdf",FileName="ClinicReport_"+DateTime.Now.ToString("yyyyMMdd")+".pdf",AddExtension=true,DefaultExt="pdf"})if(d.ShowDialog()==DialogResult.OK){try{var settings=new PrinterSettings{PrinterName=printer,PrintToFile=true,PrintFileName=d.FileName};printDocument.PrinterSettings=settings;printRow=0;printDocument.Print();MessageBox.Show("PDF report exported.");}catch(Exception ex){MessageBox.Show("Could not export the PDF.\n"+ex.Message,"PDF export",MessageBoxButtons.OK,MessageBoxIcon.Error);}}}
        private void PrintPage(object sender,PrintPageEventArgs e){Graphics g=e.Graphics;float y=e.MarginBounds.Top;using(var titleFont=new Font("Segoe UI",14,FontStyle.Bold))g.DrawString("School Clinic — "+type.Text,titleFont,Brushes.Black,e.MarginBounds.Left,y);y+=32;using(var small=new Font("Segoe UI",9))g.DrawString("Date range: "+fromDate.Value.ToString("yyyy-MM-dd")+" to "+toDate.Value.ToString("yyyy-MM-dd")+"     Generated: "+DateTime.Now.ToString("yyyy-MM-dd h:mm tt"),small,Brushes.DimGray,e.MarginBounds.Left,y);y+=26;var columns=grid.Columns.Cast<DataGridViewColumn>().Where(c=>c.Visible).ToList();if(columns.Count==0){e.HasMorePages=false;return;}float colWidth=(float)e.MarginBounds.Width/columns.Count;float rowHeight=25;using(var font=new Font("Segoe UI",8))using(var headerBrush=new SolidBrush(Color.FromArgb(225,235,235)))using(var border=new Pen(Color.LightGray)){for(int i=0;i<columns.Count;i++){var rect=new RectangleF(e.MarginBounds.Left+i*colWidth,y,colWidth,rowHeight);g.FillRectangle(headerBrush,rect);g.DrawRectangle(border,rect.X,rect.Y,rect.Width,rect.Height);g.DrawString(columns[i].HeaderText,font,Brushes.Black,rect);}y+=rowHeight;while(printRow<grid.Rows.Count){var row=grid.Rows[printRow];if(row.IsNewRow){printRow++;continue;}if(y+rowHeight>e.MarginBounds.Bottom){e.HasMorePages=true;return;}for(int i=0;i<columns.Count;i++){var rect=new RectangleF(e.MarginBounds.Left+i*colWidth,y,colWidth,rowHeight);g.DrawRectangle(border,rect.X,rect.Y,rect.Width,rect.Height);string value=Convert.ToString(row.Cells[columns[i].Index].Value)??"";g.DrawString(value,font,Brushes.Black,new RectangleF(rect.X+3,rect.Y+3,rect.Width-6,rect.Height-6));}y+=rowHeight;printRow++;}}e.HasMorePages=false;printRow=0;}
        private static string Quote(string s){return "\""+(s??"").Replace("\"","\"\"")+"\"";}
    }
}

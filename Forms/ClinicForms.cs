using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
            Text = "School Clinic | Sign in"; StartPosition = FormStartPosition.CenterScreen; ClientSize = new Size(460, 520); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; BackColor = Ui.Pale; Font = new Font("Segoe UI", 9F);
            var card = Ui.Card(); card.Location = new Point(40, 24); card.Size = new Size(380, 468); card.Padding = new Padding(30); Controls.Add(card);
            var icon = new Label { Text = "✚", Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = Ui.Teal, BackColor = Color.FromArgb(232, 243, 240), TextAlign = ContentAlignment.MiddleCenter, Size = new Size(58,58), Location = new Point(161, 22) }; card.Controls.Add(icon);
            card.Controls.Add(new Label { Text = "School Clinic", Font = new Font("Segoe UI", 20, FontStyle.Bold), ForeColor = Ui.Ink, AutoSize = true, Location = new Point(90, 91) });
            card.Controls.Add(new Label { Text = "Patient Record System", ForeColor = Ui.Muted, AutoSize = true, Location = new Point(111, 125) });
            card.Controls.Add(Ui.Label("Username")); user = Ui.TextBox(); user.Location = new Point(32, 174); user.Width = 316; card.Controls.Add(user);
            card.Controls.Add(Ui.Label("Password")); var l = card.Controls[card.Controls.Count-1]; l.Location = new Point(32, 225); pass = Ui.TextBox(); pass.Location = new Point(32, 247); pass.Width = 316; pass.UseSystemPasswordChar = true; card.Controls.Add(pass);
            show = new CheckBox { Text = "Show password", ForeColor = Ui.Muted, Location = new Point(32, 281), AutoSize = true }; show.CheckedChanged += (s,e) => pass.UseSystemPasswordChar = !show.Checked; card.Controls.Add(show);
            var login = Ui.Button("Sign in", Ui.Blue); login.Location = new Point(32, 317); login.Width = 316; login.Click += SignIn; card.Controls.Add(login);
            var setup = new LinkLabel { Text = "Set up first administrator", LinkColor = Ui.Blue, ActiveLinkColor = Ui.Teal, AutoSize = true, Location = new Point(105, 366) }; setup.LinkClicked += (s,e) => SetupFirstAdmin(); card.Controls.Add(setup);
            var exit = new LinkLabel { Text = "Exit", LinkColor = Ui.Muted, AutoSize = true, Location = new Point(180, 395) }; exit.LinkClicked += (s,e) => Close(); card.Controls.Add(exit);
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
            var side=new Panel{Dock=DockStyle.Left,Width=238,BackColor=Ui.Sidebar}; Controls.Add(side);
            side.Controls.Add(new Label{Text="✚",Font=new Font("Segoe UI",19,FontStyle.Bold),ForeColor=Color.FromArgb(139,197,185),AutoSize=true,Location=new Point(20,20)});
            side.Controls.Add(new Label{Text="School Clinic",Font=new Font("Segoe UI",14,FontStyle.Bold),ForeColor=Color.White,AutoSize=true,Location=new Point(58,22)});
            side.Controls.Add(new Label{Text="PATIENT RECORDS",Font=new Font("Segoe UI",8,FontStyle.Bold),ForeColor=Color.FromArgb(160,179,184),AutoSize=true,Location=new Point(22,61)});
            string[] menu={"Dashboard","Patient Records","Medical Checkup","Medical History","Medicine Inventory","Reports","User Management","Logout"};
            for(int i=0;i<menu.Length;i++){string item=menu[i];int y=88+i*49;var b=new Button{Text="   "+item,TextAlign=ContentAlignment.MiddleLeft,FlatStyle=FlatStyle.Flat,FlatAppearance={BorderSize=0,MouseOverBackColor=Color.FromArgb(49,76,84)},ForeColor=Color.FromArgb(235,242,243),BackColor=Ui.Sidebar,Font=new Font("Segoe UI",9.5F),Location=new Point(10,y),Size=new Size(218,44),Cursor=Cursors.Hand,UseVisualStyleBackColor=false};if(i==0){b.BackColor=Color.FromArgb(56,83,91);b.ForeColor=Color.White;}b.Click+=(s,e)=>Navigate(item);side.Controls.Add(b);}
            content=new Panel{Dock=DockStyle.Fill,Padding=new Padding(28)};Controls.Add(content); status=new Label{Dock=DockStyle.Bottom,Height=28,ForeColor=Color.Gray,TextAlign=ContentAlignment.MiddleRight};Controls.Add(status);
            clock=new Timer{Interval=1000};clock.Tick+=(s,e)=>status.Text=DateTime.Now.ToString("dddd, dd MMMM yyyy  |  h:mm:ss tt")+"     Signed in as "+SessionManager.FullName+" ("+SessionManager.Role+")";clock.Start(); ShowHome();
        }
        private void Navigate(string name)
        {
            if(name=="Logout"){if(MessageBox.Show("Sign out of the clinic system?","Sign out",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)Close();return;}
            if(name=="Dashboard"){ShowHome();return;}
            if(name=="User Management"&&!SessionManager.IsAdmin){MessageBox.Show("Only an administrator can manage accounts.");return;}
            Form form=null;
            switch(name){case "Patient Records":form=RecordForm.Patients();break;case "Medical Checkup":form=RecordForm.Checkups();break;case "Medical History":form=RecordForm.History();break;case "Medicine Inventory":form=RecordForm.Medicines();break;case "Reports":form=new ReportsForm();break;case "User Management":form=new UsersForm();break;}
            if(form!=null)using(form)form.ShowDialog(this); ShowHome();
        }
        private void ShowHome()
        {
            content.Controls.Clear(); var heading=new Label{Text="Clinic overview",Font=new Font("Segoe UI",22,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(28,24)};content.Controls.Add(heading);
            var welcome=new Label{Text="Welcome, "+SessionManager.FullName+". Here’s today at a glance.",ForeColor=Ui.Muted,AutoSize=true,Location=new Point(30,66)};content.Controls.Add(welcome);
            string[][] cards=new string[][] {new[]{"Total Patients","SELECT COUNT(*) FROM patients"},new[]{"Today's Checkups","SELECT COUNT(*) FROM medical_checkups WHERE visit_date=CURDATE()"},new[]{"Male Patients","SELECT COUNT(*) FROM patients WHERE gender='Male'"},new[]{"Female Patients","SELECT COUNT(*) FROM patients WHERE gender='Female'"},new[]{"Available Medicines","SELECT COALESCE(SUM(quantity),0) FROM medicines"}};
            int x=28,y=112;for(int i=0;i<cards.Length;i++){var p=Ui.Card();p.Location=new Point(x,y);p.Size=new Size(205,100);content.Controls.Add(p);p.Controls.Add(new Panel{BackColor=i%2==0?Ui.Blue:Ui.Teal,Location=new Point(14,17),Size=new Size(4,65)});p.Controls.Add(new Label{Text=cards[i][0],ForeColor=Ui.Muted,AutoSize=true,Location=new Point(29,18)});string value="—";try{value=Convert.ToString(DatabaseConnection.Scalar(cards[i][1]));}catch{}p.Controls.Add(new Label{Text=value,Font=new Font("Segoe UI",24,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(28,42)});x+=220;if((i+1)%3==0){x=28;y+=115;}}
            var title=new Label{Text="Recent patient visits",Font=new Font("Segoe UI",14,FontStyle.Bold),ForeColor=Ui.Ink,AutoSize=true,Location=new Point(28,350)};content.Controls.Add(title);
            var grid=Grid();grid.Location=new Point(28,382);grid.Size=new Size(Math.Max(600,content.ClientSize.Width-80),250);content.Controls.Add(grid);try{grid.DataSource=DatabaseConnection.Query("SELECT c.checkup_id AS 'Checkup #', p.student_id AS 'Student ID', CONCAT(p.first_name,' ',p.last_name) AS 'Patient', c.visit_date AS 'Visit date', c.chief_complaint AS 'Complaint' FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id ORDER BY c.visit_date DESC,c.checkup_id DESC LIMIT 10");}catch{grid.DataSource=null;}
        }
        internal static DataGridView Grid(){return new DataGridView{BackgroundColor=Color.White,BorderStyle=BorderStyle.None,GridColor=Ui.Line,CellBorderStyle=DataGridViewCellBorderStyle.SingleHorizontal,AllowUserToAddRows=false,ReadOnly=true,MultiSelect=false,SelectionMode=DataGridViewSelectionMode.FullRowSelect,AutoSizeColumnsMode=DataGridViewAutoSizeColumnsMode.Fill,RowHeadersVisible=false,EnableHeadersVisualStyles=false,ColumnHeadersHeight=38,ColumnHeadersBorderStyle=DataGridViewHeaderBorderStyle.None,ColumnHeadersDefaultCellStyle={BackColor=Color.FromArgb(237,243,243),ForeColor=Ui.Ink,Font=new Font("Segoe UI Semibold",9F),Padding=new Padding(7)},DefaultCellStyle={BackColor=Color.White,ForeColor=Ui.Ink,SelectionBackColor=Color.FromArgb(226,239,238),SelectionForeColor=Ui.Ink,Padding=new Padding(7)},AlternatingRowsDefaultCellStyle={BackColor=Color.FromArgb(250,252,252)},RowTemplate={Height=36}};}
    }

    internal sealed class FieldDef { public string Key, Label, Kind; public FieldDef(string k,string l,string t="text"){Key=k;Label=l;Kind=t;} }

    public class RecordForm : Form
    {
        private readonly string table, key, title; private readonly FieldDef[] fields; private readonly string[] searchFields; private readonly DataGridView grid; private readonly Dictionary<string,Control> inputs=new Dictionary<string,Control>(); private readonly TextBox search; private int selectedId;
        private RecordForm(string title,string table,string key,FieldDef[] fields,string[] searchFields)
        {
            this.title=title;this.table=table;this.key=key;this.fields=fields;this.searchFields=searchFields;Text=title;StartPosition=FormStartPosition.CenterParent;Size=new Size(1180,760);MinimumSize=new Size(950,640);BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);
            var head=new Label{Text=title,Font=new Font("Segoe UI",20,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(22,18),AutoSize=true};Controls.Add(head);
            var panel=Ui.Card();panel.Location=new Point(22,64);panel.Size=new Size(390,ClientSize.Height-86);panel.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left;panel.Padding=new Padding(16);panel.AutoScroll=true;Controls.Add(panel);
            int top=12;foreach(var f in fields){panel.Controls.Add(new Label{Text=f.Label,AutoSize=true,ForeColor=Ui.Ink,Location=new Point(15,top)});top+=19;Control ctl;
                if(f.Kind=="select"){var c=new ComboBox{DropDownStyle=ComboBoxStyle.DropDownList,Location=new Point(15,top),Width=340,Height=26,Font=new Font("Segoe UI",9F)};foreach(string v in f.Key=="gender"?new[]{"Male","Female","Other"}:new[]{"Admin","Nurse"})c.Items.Add(v);if(c.Items.Count>0)c.SelectedIndex=0;ctl=c;}
                else if(f.Kind=="date"){ctl=new DateTimePicker{Format=DateTimePickerFormat.Custom,CustomFormat="yyyy-MM-dd",ShowCheckBox=true,Checked=false,Location=new Point(15,top),Width=340,Font=new Font("Segoe UI",9F)};}
                else if(f.Kind=="long"){ctl=Ui.TextBox(true);ctl.Height=54;ctl.Location=new Point(15,top);ctl.Width=340;}
                else{ctl=Ui.TextBox();ctl.Location=new Point(15,top);ctl.Width=340;}
                panel.Controls.Add(ctl);inputs[f.Key]=ctl;top+=ctl.Height+11;
            }
            var actions=new FlowLayoutPanel{Location=new Point(15,Math.Min(top+4,panel.Height-49)),Size=new Size(350,40),Anchor=AnchorStyles.Bottom|AnchorStyles.Left,WrapContents=false};panel.Controls.Add(actions);
            AddAction(actions,"Add",Ui.Teal,(s,e)=>AddRecord());AddAction(actions,"Update",Ui.Blue,(s,e)=>UpdateRecord());AddAction(actions,"Delete",Color.FromArgb(189,74,74),(s,e)=>DeleteRecord());AddAction(actions,"Clear",Color.Gray,(s,e)=>Clear());
            search=Ui.TextBox();search.Location=new Point(430,73);search.Width=300;search.AccessibleName="Search records";Controls.Add(search);var sb=Ui.Button("Search",Ui.Blue);sb.Location=new Point(740,69);sb.Width=90;sb.Click+=(s,e)=>LoadRows(search.Text);Controls.Add(sb);var refresh=Ui.Button("Refresh",Ui.Teal);refresh.Location=new Point(838,69);refresh.Width=90;refresh.Click+=(s,e)=>{search.Clear();LoadRows("");};Controls.Add(refresh);
            grid=DashboardForm.Grid();grid.Location=new Point(430,116);grid.Size=new Size(ClientSize.Width-454,ClientSize.Height-143);grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;grid.CellClick+=GridClick;Controls.Add(grid);LoadRows("");
        }
        private void AddAction(FlowLayoutPanel p,string text,Color color,EventHandler handler){var b=Ui.Button(text,color);b.Width=78;b.Click+=handler;p.Controls.Add(b);}
        private void LoadRows(string term){try{string where=String.Join(" OR ",searchFields.Select(f=>"CAST("+f+" AS CHAR) LIKE @q"));grid.DataSource=DatabaseConnection.Query("SELECT "+key+" AS 'Record #',"+String.Join(",",fields.Select(f=>f.Key+" AS `"+f.Label.Replace("`","")+"`"))+" FROM "+table+(String.IsNullOrEmpty(where)?"":" WHERE "+where)+" ORDER BY "+key+" DESC LIMIT 500",DatabaseConnection.P("@q","%"+(term??"").Trim()+"%"));selectedId=0;}catch(Exception ex){MessageBox.Show("Could not load "+title+".\n"+ex.Message,"Database",MessageBoxButtons.OK,MessageBoxIcon.Error);}}
        private void GridClick(object s,DataGridViewCellEventArgs e){if(e.RowIndex<0)return;var row=grid.Rows[e.RowIndex];selectedId=Convert.ToInt32(row.Cells[0].Value);foreach(var f in fields){object value=row.Cells[f.Label].Value;Control c=inputs[f.Key];if(c is ComboBox)((ComboBox)c).SelectedItem=Convert.ToString(value);else if(c is DateTimePicker){DateTime dt=DateTime.Today;var picker=(DateTimePicker)c;picker.Checked=value!=DBNull.Value&&DateTime.TryParse(Convert.ToString(value),out dt);if(picker.Checked)picker.Value=dt;}else c.Text=value==DBNull.Value?"":Convert.ToString(value);}}
        private object Value(FieldDef f){Control c=inputs[f.Key];if(c is ComboBox)return ((ComboBox)c).SelectedItem;if(c is DateTimePicker){var d=(DateTimePicker)c;return d.Checked?(object)d.Value.Date:DBNull.Value;}string value=c.Text.Trim();if(value=="")return DBNull.Value;if(f.Key=="age"||f.Key=="quantity") {int n;if(!Int32.TryParse(value,out n)||n<0)throw new InvalidOperationException(f.Label+" must be a non-negative whole number.");return n;}if(f.Key=="temperature"||f.Key=="weight_kg"||f.Key=="height_cm"){decimal n;if(!Decimal.TryParse(value,out n)||n<0)throw new InvalidOperationException(f.Label+" must be a valid non-negative number.");return n;}return value;}
        private MySqlParameter[] Parameters(){return fields.Select(f=>DatabaseConnection.P("@"+f.Key,Value(f))).ToArray();}
        private void AddRecord(){try{var p=Parameters();if(fields.Any(f=>f.Key=="created_by")){}string sql="INSERT INTO "+table+"("+String.Join(",",fields.Select(f=>f.Key))+ ") VALUES("+String.Join(",",fields.Select(f=>"@"+f.Key))+")";DatabaseConnection.Execute(sql,p);Ui.Audit("CREATE",table,"new");LoadRows(search.Text);Clear();}catch(Exception ex){MessageBox.Show("Please check the entered values.\n"+ex.Message,"Save record",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void UpdateRecord(){if(selectedId==0){MessageBox.Show("Select a row first.");return;}try{var p=Parameters().ToList();p.Add(DatabaseConnection.P("@id",selectedId));DatabaseConnection.Execute("UPDATE "+table+" SET "+String.Join(",",fields.Select(f=>f.Key+"=@"+f.Key))+" WHERE "+key+"=@id",p.ToArray());Ui.Audit("UPDATE",table,selectedId);LoadRows(search.Text);}catch(Exception ex){MessageBox.Show("Could not update record.\n"+ex.Message,"Save record",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void DeleteRecord(){if(selectedId==0){MessageBox.Show("Select a row first.");return;}if(MessageBox.Show("Delete this record?","Confirm delete",MessageBoxButtons.YesNo,MessageBoxIcon.Warning)!=DialogResult.Yes)return;try{DatabaseConnection.Execute("DELETE FROM "+table+" WHERE "+key+"=@id",DatabaseConnection.P("@id",selectedId));Ui.Audit("DELETE",table,selectedId);Clear();LoadRows(search.Text);}catch(Exception ex){MessageBox.Show("Could not delete this record. Related patient records may depend on it.\n"+ex.Message,"Delete record",MessageBoxButtons.OK,MessageBoxIcon.Warning);}}
        private void Clear(){selectedId=0;foreach(var f in fields){var c=inputs[f.Key];if(c is ComboBox)((ComboBox)c).SelectedIndex=0;else if(c is DateTimePicker)((DateTimePicker)c).Checked=false;else c.Text="";}}
        public static RecordForm Patients(){return new RecordForm("Patient records","patients","patient_id",new[]{new FieldDef("student_id","Student ID"),new FieldDef("first_name","First name"),new FieldDef("middle_name","Middle name"),new FieldDef("last_name","Last name"),new FieldDef("age","Age"),new FieldDef("gender","Gender","select"),new FieldDef("date_of_birth","Date of birth","date"),new FieldDef("grade_level","Grade / Year"),new FieldDef("section_course","Section / Course"),new FieldDef("address","Address","long"),new FieldDef("contact_number","Contact number"),new FieldDef("guardian_name","Parent / guardian"),new FieldDef("guardian_contact","Guardian contact")},new[]{"student_id","first_name","last_name"});}
        public static RecordForm Checkups(){return new RecordForm("Medical checkups","medical_checkups","checkup_id",new[]{new FieldDef("patient_id","Patient ID"),new FieldDef("visit_date","Date of visit","date"),new FieldDef("visit_time","Time of visit"),new FieldDef("chief_complaint","Chief complaint","long"),new FieldDef("symptoms","Symptoms","long"),new FieldDef("temperature","Temperature °C"),new FieldDef("blood_pressure","Blood pressure"),new FieldDef("weight_kg","Weight kg"),new FieldDef("height_cm","Height cm"),new FieldDef("diagnosis","Diagnosis","long"),new FieldDef("treatment_given","Treatment given","long"),new FieldDef("medicine_prescribed","Medicine prescribed"),new FieldDef("attending_staff","Attending staff"),new FieldDef("remarks","Remarks","long")},new[]{"patient_id","chief_complaint","diagnosis"});}
        public static RecordForm History(){return new RecordForm("Medical history","medical_history","history_id",new[]{new FieldDef("patient_id","Patient ID"),new FieldDef("allergies","Allergies","long"),new FieldDef("existing_conditions","Existing conditions","long"),new FieldDef("previous_illnesses","Previous illnesses","long"),new FieldDef("current_medications","Current medications","long"),new FieldDef("immunization_history","Immunization history","long"),new FieldDef("emergency_contact","Emergency contact"),new FieldDef("medical_notes","Medical notes","long")},new[]{"patient_id","allergies","existing_conditions"});}
        public static RecordForm Medicines(){return new RecordForm("Medicine inventory","medicines","medicine_id",new[]{new FieldDef("medicine_name","Medicine name"),new FieldDef("description","Description"),new FieldDef("quantity","Quantity"),new FieldDef("unit","Unit"),new FieldDef("expiration_date","Expiration date","date"),new FieldDef("date_added","Date added","date"),new FieldDef("remarks","Remarks","long")},new[]{"medicine_name","description"});}
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
        private DataGridView grid; private ComboBox type;
        public ReportsForm(){Text="Reports";StartPosition=FormStartPosition.CenterParent;Size=new Size(1100,700);BackColor=Ui.Pale;Font=new Font("Segoe UI",9F);Controls.Add(new Label{Text="Clinic reports",Font=new Font("Segoe UI",20,FontStyle.Bold),ForeColor=Ui.Ink,Location=new Point(22,20),AutoSize=true});type=new ComboBox{Location=new Point(24,73),Width=280,DropDownStyle=ComboBoxStyle.DropDownList,Font=new Font("Segoe UI",9F)};type.Items.AddRange(new object[]{"Daily patient visits","Weekly patient visits","Monthly patient visits","Patient list","Medical checkups","Medicine inventory"});type.SelectedIndex=0;Controls.Add(type);var load=Ui.Button("Generate",Ui.Blue);load.Location=new Point(320,69);load.Width=100;load.Click+=(s,e)=>LoadReport();Controls.Add(load);var export=Ui.Button("Export CSV",Ui.Teal);export.Location=new Point(430,69);export.Width=110;export.Click+=(s,e)=>Export();Controls.Add(export);var tableCard=Ui.Card();tableCard.Location=new Point(20,115);tableCard.Size=new Size(1040,525);tableCard.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;Controls.Add(tableCard);grid=DashboardForm.Grid();grid.Location=new Point(10,10);grid.Size=new Size(1020,505);grid.Anchor=AnchorStyles.Top|AnchorStyles.Bottom|AnchorStyles.Left|AnchorStyles.Right;tableCard.Controls.Add(grid);LoadReport();}
        private void LoadReport(){string sql;switch(type.Text){case "Patient list":sql="SELECT patient_id,student_id,first_name,middle_name,last_name,gender,grade_level,section_course FROM patients ORDER BY last_name";break;case "Medical checkups":sql="SELECT c.checkup_id,c.visit_date,c.visit_time,p.student_id,CONCAT(p.first_name,' ',p.last_name) patient,c.chief_complaint,c.diagnosis FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id ORDER BY c.visit_date DESC";break;case "Medicine inventory":sql="SELECT medicine_id,medicine_name,quantity,unit,expiration_date,remarks FROM medicines ORDER BY medicine_name";break;default:string condition=type.Text=="Daily patient visits"?"=CURDATE()":type.Text=="Weekly patient visits"?">=DATE_SUB(CURDATE(),INTERVAL 7 DAY)":">=DATE_SUB(CURDATE(),INTERVAL 1 MONTH)";sql="SELECT c.checkup_id,c.visit_date,c.visit_time,p.student_id,CONCAT(p.first_name,' ',p.last_name) patient,c.chief_complaint,c.diagnosis FROM medical_checkups c JOIN patients p ON p.patient_id=c.patient_id WHERE c.visit_date "+condition+" ORDER BY c.visit_date DESC";break;}try{grid.DataSource=DatabaseConnection.Query(sql);}catch(Exception ex){MessageBox.Show("Could not generate report.\n"+ex.Message);}}
        private void Export(){if(grid.DataSource==null||grid.Rows.Count==0){MessageBox.Show("Generate a report first.");return;}using(var d=new SaveFileDialog{Filter="CSV files (*.csv)|*.csv",FileName="ClinicReport_"+DateTime.Now.ToString("yyyyMMdd")+".csv"})if(d.ShowDialog()==DialogResult.OK){var sb=new StringBuilder();sb.AppendLine(String.Join(",",grid.Columns.Cast<DataGridViewColumn>().Select(c=>Quote(c.HeaderText))));foreach(DataGridViewRow row in grid.Rows)if(!row.IsNewRow)sb.AppendLine(String.Join(",",row.Cells.Cast<DataGridViewCell>().Select(c=>Quote(Convert.ToString(c.Value)))));System.IO.File.WriteAllText(d.FileName,sb.ToString(),Encoding.UTF8);MessageBox.Show("Report exported.");}}
        private static string Quote(string s){return "\""+(s??"").Replace("\"","\"\"")+"\"";}
    }
}

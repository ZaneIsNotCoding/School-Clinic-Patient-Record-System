# School Clinic Patient Record System

A Windows desktop application for managing school clinic patient records, visits, medical history, and medicine stock. It is built with C# Windows Forms on .NET Framework 4.8 and stores data in MySQL.

MySQL Workbench is used to connect to and manage the database. **MySQL Server is a separate program and must be installed and running** for the clinic app to access data.

## Features

- Staff sign-in, first-admin setup, administrator and nurse roles
- Live dashboard showing patient, visit, and medicine totals
- Separate male/female student and teacher counts for the Ilagan clinic, with student totals by course and year level
- Student and teacher patient records with ID number, full name, year level, course, contact, and emergency-contact information
- Patient age is calculated from birthdate and refreshed from the birthdate when records are displayed
- Course dropdown limited to BSIT, BSA, BSN, BSCE, BSEE, BSINDT, BSED, BTVTED, BTLED, and BSM
- Medical record lookup by system Patient ID or student/teacher ID Number, including patient details, checkups, history, and medicines issued
- Checkups and medical history select a patient from the saved patient list instead of requiring a manually typed database ID
- Medicine stock management with patient-linked issues; issuing medicine reduces available inventory in the same database transaction
- Date-filtered daily, weekly, monthly, and checkup reports, plus patient and inventory reports
- Student/teacher and gender totals, student summaries by course and year level
- CSV export, print, and PDF export using the Windows **Microsoft Print to PDF** printer
- Administrator-only user management
- Parameterized database commands, salted PBKDF2 password hashes, and an audit log

## Requirements

- Windows 10 or 11
- Visual Studio 2022 with the **.NET desktop development** workload
- .NET Framework 4.8 Developer Pack
- MySQL Server 8.0 or a compatible version
- MySQL Workbench
- Internet access the first time the project restores its NuGet packages

Install MySQL Server and Workbench using the official [MySQL Installer for Windows](https://dev.mysql.com/downloads/installer/) or their standalone installers. Workbench alone does not include the database server.

## Set up the database

1. Start the MySQL Server service in Windows. A standard installation often names it `MySQL80`.
2. Open MySQL Workbench from the Windows Start menu.
3. Open your local connection. For a typical local setup, use hostname `localhost`, port `3306`, and the MySQL account you created during server installation.
4. For a new database, open `Database/school_clinic_db.sql` in Workbench. If you already created the earlier version of this project's database, run `Database/upgrade_student_teacher_and_medicine_issues.sql` instead to add the new patient-type and medicine-issue structures.
5. Run the script with the lightning-bolt **Execute** button. Refresh the **Schemas** panel and confirm `school_clinic_db` appears.

The setup script creates the database tables, indexes, foreign keys, and audit log. It does not add a default user or sample patient records. The upgrade script preserves existing patient records and marks them as students by default; edit a person's type in Patient Records if needed.

## Configure the app

Copy `App.config.example` to `App.config`, then set the `ClinicDb` connection string to match your MySQL Server address, port, username, and password. `App.config` is ignored by Git so local credentials are not uploaded. For a local installation, the connection begins like this:

```xml
<add name="ClinicDb"
     providerName="MySql.Data.MySqlClient"
     connectionString="Server=localhost;Port=3306;Database=school_clinic_db;Uid=YOUR_MYSQL_USER;Pwd=YOUR_MYSQL_PASSWORD;SslMode=None;AllowPublicKeyRetrieval=True;CharSet=utf8mb4;" />
```

`AllowPublicKeyRetrieval=True` can resolve the RSA public-key error for a local development server when TLS is disabled. Use it only when connecting to a server you trust, preferably through `localhost`. For a networked or production database, configure TLS and use a restricted database account instead of disabling SSL or using the MySQL `root` account.

> **Before uploading this project:** do not publish database credentials, patient information, exported reports, or database backups. Upload `App.config.example`; keep your real `App.config` on your computer.

## Open and run in Visual Studio

1. Open `School Clinic Patient Record System.slnx` in Visual Studio 2022. You can also open `School Clinic Patient Record System.csproj` directly.
2. Allow Visual Studio to restore the NuGet packages.
3. Confirm MySQL Server is running and the database connection string is configured.
4. Select **Start** (or press **F5**).
5. On the sign-in screen, select **Set up first administrator** and create the first account. Usernames must be at least 4 characters, and the initial password must be at least 10 characters.
6. Sign in with the account you created. Add nurse accounts through **User Management**.

## Project layout

```text
School Clinic Patient Record System/
├── Classes/
│   ├── DatabaseConnection.cs
│   ├── PasswordHelper.cs
│   └── SessionManager.cs
├── Database/
│   ├── school_clinic_db.sql
│   └── upgrade_student_teacher_and_medicine_issues.sql
├── Forms/
│   └── ClinicForms.cs
├── App.config
├── Program.cs
├── School Clinic Patient Record System.csproj
└── School Clinic Patient Record System.slnx
```

## Troubleshooting

### Workbench cannot connect

Check that the **MySQL Server** service is running, and confirm the connection hostname, port, username, and password. Closing Workbench does not stop the server service.

### The app reports “RSA public key is not enabled”

For a trusted local development connection, check that the connection string includes `AllowPublicKeyRetrieval=True;`. Keep the server set to `localhost` while using this option without TLS.

### The app cannot sign in or load records

Confirm that you ran `Database/school_clinic_db.sql`, the `ClinicDb` settings in `App.config` match the MySQL Server account, and the server is running. The app login is separate from the MySQL login: create the clinic administrator with **Set up first administrator** after the database is set up.

### Medical records says a database table or column is missing

If this database was created with an earlier version, run `Database/upgrade_student_teacher_and_medicine_issues.sql` in Workbench, then reopen the app.

### A medicine does not appear in the issue list

Only medicines with stock above zero and no expired expiration date can be issued. Add stock or update the expiration date in Medicine Inventory, then reopen **Medical Records** and try again.

## Data and security

This project is a starter system. Medical information is sensitive: use fictional data for demonstrations, limit database access, protect backups, and follow your school's privacy and security policies before entering real student records. The application does not replace a formal security review or a managed backup and recovery plan.

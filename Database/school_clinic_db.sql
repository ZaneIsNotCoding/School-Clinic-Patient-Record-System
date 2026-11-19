CREATE DATABASE IF NOT EXISTS school_clinic_db CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE school_clinic_db;

CREATE TABLE IF NOT EXISTS users (
  user_id INT AUTO_INCREMENT PRIMARY KEY,
  full_name VARCHAR(120) NOT NULL,
  username VARCHAR(60) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  role ENUM('Admin','Nurse') NOT NULL DEFAULT 'Nurse',
  is_active BOOLEAN NOT NULL DEFAULT TRUE,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS patients (
  patient_id INT AUTO_INCREMENT PRIMARY KEY,
  student_id VARCHAR(40) NOT NULL UNIQUE,
  person_type ENUM('Student','Teacher') NOT NULL DEFAULT 'Student',
  first_name VARCHAR(80) NOT NULL,
  middle_name VARCHAR(80) NULL,
  last_name VARCHAR(80) NOT NULL,
  age TINYINT UNSIGNED NULL,
  gender ENUM('Male','Female','Other') NOT NULL,
  date_of_birth DATE NULL,
  grade_level VARCHAR(40) NULL,
  section_course VARCHAR(80) NULL,
  address VARCHAR(255) NULL,
  contact_number VARCHAR(40) NULL,
  guardian_name VARCHAR(120) NULL,
  guardian_contact VARCHAR(40) NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  INDEX idx_patient_name(last_name, first_name)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS medical_checkups (
  checkup_id INT AUTO_INCREMENT PRIMARY KEY,
  patient_id INT NOT NULL,
  visit_date DATE NOT NULL,
  visit_time TIME NOT NULL,
  chief_complaint TEXT NULL,
  symptoms TEXT NULL,
  temperature DECIMAL(4,1) NULL,
  blood_pressure VARCHAR(20) NULL,
  weight_kg DECIMAL(6,2) NULL,
  height_cm DECIMAL(6,2) NULL,
  diagnosis TEXT NULL,
  treatment_given TEXT NULL,
  medicine_prescribed VARCHAR(255) NULL,
  attending_staff VARCHAR(120) NULL,
  remarks TEXT NULL,
  created_by INT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_checkup_patient FOREIGN KEY(patient_id) REFERENCES patients(patient_id) ON DELETE RESTRICT,
  CONSTRAINT fk_checkup_user FOREIGN KEY(created_by) REFERENCES users(user_id) ON DELETE SET NULL,
  INDEX idx_checkup_date(visit_date)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS medical_history (
  history_id INT AUTO_INCREMENT PRIMARY KEY,
  patient_id INT NOT NULL UNIQUE,
  allergies TEXT NULL,
  existing_conditions TEXT NULL,
  previous_illnesses TEXT NULL,
  current_medications TEXT NULL,
  immunization_history TEXT NULL,
  emergency_contact VARCHAR(120) NULL,
  medical_notes TEXT NULL,
  updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_history_patient FOREIGN KEY(patient_id) REFERENCES patients(patient_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS medicines (
  medicine_id INT AUTO_INCREMENT PRIMARY KEY,
  medicine_name VARCHAR(120) NOT NULL,
  description VARCHAR(255) NULL,
  quantity INT NOT NULL DEFAULT 0,
  unit VARCHAR(40) NOT NULL,
  expiration_date DATE NULL,
  date_added DATE NOT NULL,
  remarks TEXT NULL,
  INDEX idx_medicine_expiration(expiration_date)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS medicine_issues (
  issue_id BIGINT AUTO_INCREMENT PRIMARY KEY,
  patient_id INT NOT NULL,
  checkup_id INT NULL,
  medicine_id INT NOT NULL,
  quantity INT NOT NULL,
  issued_by INT NULL,
  issued_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_issue_patient FOREIGN KEY(patient_id) REFERENCES patients(patient_id) ON DELETE RESTRICT,
  CONSTRAINT fk_issue_checkup FOREIGN KEY(checkup_id) REFERENCES medical_checkups(checkup_id) ON DELETE SET NULL,
  CONSTRAINT fk_issue_medicine FOREIGN KEY(medicine_id) REFERENCES medicines(medicine_id) ON DELETE RESTRICT,
  CONSTRAINT fk_issue_user FOREIGN KEY(issued_by) REFERENCES users(user_id) ON DELETE SET NULL,
  CONSTRAINT chk_issue_quantity CHECK(quantity > 0),
  INDEX idx_issue_patient(patient_id),
  INDEX idx_issue_date(issued_at)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS audit_log (
  audit_id BIGINT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NULL,
  action VARCHAR(80) NOT NULL,
  entity VARCHAR(80) NOT NULL,
  record_id VARCHAR(40) NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_audit_user FOREIGN KEY(user_id) REFERENCES users(user_id) ON DELETE SET NULL
) ENGINE=InnoDB;

-- No default account is inserted. On first launch choose “Set up first administrator” and
-- create a unique username and a strong password; the application stores a PBKDF2 hash.
-- Optional inventory sample row (uncomment to add):
-- INSERT INTO medicines(medicine_name,description,quantity,unit,expiration_date,date_added,remarks)
-- VALUES ('Sample bandage','First aid supplies',25,'pieces','2028-12-31',CURRENT_DATE,'Replace with clinic stock');

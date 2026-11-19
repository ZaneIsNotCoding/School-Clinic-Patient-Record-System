-- Run once in MySQL Workbench against an existing school_clinic_db database.
-- New databases should instead be created with school_clinic_db.sql.
USE school_clinic_db;

SET @add_person_type = (
  SELECT IF(COUNT(*) = 0,
    'ALTER TABLE patients ADD COLUMN person_type ENUM(''Student'',''Teacher'') NOT NULL DEFAULT ''Student'' AFTER student_id',
    'SELECT ''patients.person_type already exists'' AS status')
  FROM information_schema.columns
  WHERE table_schema = DATABASE() AND table_name = 'patients' AND column_name = 'person_type'
);
PREPARE person_type_stmt FROM @add_person_type;
EXECUTE person_type_stmt;
DEALLOCATE PREPARE person_type_stmt;

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

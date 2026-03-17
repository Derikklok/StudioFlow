-- ===========================
-- StudioFlow Database Schema
-- Initialization Script for Docker
-- ===========================

USE studio_db;

-- ===========================
-- 1. Departments Table
-- ===========================
CREATE TABLE IF NOT EXISTS Departments (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Description VARCHAR(500) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===========================
-- 2. Users Table
-- ===========================
CREATE TABLE IF NOT EXISTS Users (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Name VARCHAR(100) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    Password LONGTEXT NOT NULL,
    Role INT NOT NULL,
    IsActive TINYINT(1) NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    INDEX IX_Users_Email (Email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===========================
-- 3. Projects Table
-- ===========================
CREATE TABLE IF NOT EXISTS Projects (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Title LONGTEXT NOT NULL,
    ArtistName LONGTEXT NOT NULL,
    Description LONGTEXT NULL,
    Deadline DATETIME(6) NULL,
    TargetReleaseDate DATETIME(6) NULL,
    Status INT NOT NULL,
    CreatedBy INT NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===========================
-- 4. Samples Table
-- ===========================
CREATE TABLE IF NOT EXISTS Samples (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    ProjectId INT NOT NULL,
    Title LONGTEXT NOT NULL,
    SourceArtist LONGTEXT NULL,
    SourceTrack LONGTEXT NULL,
    RightsHolder LONGTEXT NULL,
    Status INT NOT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    UpdatedAt DATETIME(6) NULL,
    FOREIGN KEY (ProjectId) REFERENCES Projects(Id) ON DELETE CASCADE,
    INDEX IX_Samples_ProjectId (ProjectId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===========================
-- 5. Clearances Table
-- ===========================
CREATE TABLE IF NOT EXISTS Clearances (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    SampleId INT NOT NULL UNIQUE,
    RightsOwner LONGTEXT NOT NULL,
    LicenseType LONGTEXT NULL,
    IsApproved TINYINT(1) NOT NULL,
    ApprovedAt DATETIME(6) NULL,
    Notes LONGTEXT NULL,
    CreatedAt DATETIME(6) NOT NULL,
    FOREIGN KEY (SampleId) REFERENCES Samples(Id) ON DELETE CASCADE,
    INDEX IX_Clearances_SampleId (SampleId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===========================
-- 6. Entity Framework Migrations History Table
-- ===========================
CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
    MigrationId NVARCHAR(150) NOT NULL PRIMARY KEY,
    ProductVersion NVARCHAR(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ===========================
-- 7. Record Migration History
-- ===========================
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES 
('20260314150057_InitialCreate', '8.0.0'),
('20260315083052_CreateUsersTable', '8.0.0'),
('20260315084915_AddUniqueEmailConstraint', '8.0.0'),
('20260316021104_AddProjects', '8.0.0'),
('20260316041417_AddUpdateAtToProjects', '8.0.0'),
('20260316134817_AddSamples', '8.0.0'),
('20260316184756_AddClearanceModule', '8.0.0')
ON DUPLICATE KEY UPDATE ProductVersion=VALUES(ProductVersion);

-- ===========================
-- Success Message
-- ===========================
SELECT 'Database initialization completed successfully' AS status;



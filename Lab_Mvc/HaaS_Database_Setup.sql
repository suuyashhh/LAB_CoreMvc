-- =============================================================================
-- HaaS – Heat as a Service – Database Setup Script
-- Run once on your existing SQL Server database
-- =============================================================================

-- 1. Data Center Metrics
-- Stores every snapshot of power consumption, waste heat and temperature
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HAAS_DATACENTER_METRICS')
BEGIN
    CREATE TABLE [dbo].[HAAS_DATACENTER_METRICS] (
        [METRICS_ID]               BIGINT IDENTITY(1,1) PRIMARY KEY,
        [POWER_CONSUMPTION_KW]     FLOAT NOT NULL,
        [WASTE_HEAT_GENERATED_KW]  FLOAT NOT NULL,
        [TEMPERATURE_CELSIUS]      FLOAT NOT NULL,
        [PUE]                      FLOAT NOT NULL DEFAULT 1.3,
        [TIMESTAMP]                DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Created table: HAAS_DATACENTER_METRICS';
END
ELSE
    PRINT 'Table already exists: HAAS_DATACENTER_METRICS';

-- =============================================================================
-- 2. Ecosystem Demand
-- Stores demand values per ecosystem per season (16 rows total after seed)
-- ECOSYSTEM_TYPE: 1=DistrictHeating, 2=Agriculture, 3=ThermalStorage, 4=Industry
-- SEASON_TYPE:    1=Winter, 2=Spring, 3=Summer, 4=Autumn
-- PRIORITY:       1=Highest, 4=Lowest
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HAAS_ECOSYSTEM_DEMAND')
BEGIN
    CREATE TABLE [dbo].[HAAS_ECOSYSTEM_DEMAND] (
        [DEMAND_ID]          BIGINT IDENTITY(1,1) PRIMARY KEY,
        [ECOSYSTEM_NAME]     NVARCHAR(50)  NOT NULL,
        [ECOSYSTEM_TYPE]     INT           NOT NULL,
        [DEMAND_VALUE_KW]    FLOAT         NOT NULL,
        [MIN_TEMP_REQUIRED]  FLOAT         NOT NULL DEFAULT 20,
        [MAX_TEMP_ACCEPTED]  FLOAT         NOT NULL DEFAULT 95,
        [SEASON_TYPE]        INT           NOT NULL,
        [SEASON_NAME]        NVARCHAR(20)  NOT NULL,
        [PRIORITY]           INT           NOT NULL DEFAULT 4,
        [TIMESTAMP]          DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT UQ_HAAS_ECOSYSTEM_SEASON UNIQUE (ECOSYSTEM_TYPE, SEASON_TYPE)
    );
    PRINT 'Created table: HAAS_ECOSYSTEM_DEMAND';
END
ELSE
    PRINT 'Table already exists: HAAS_ECOSYSTEM_DEMAND';

-- =============================================================================
-- 3. Thermal Storage Status Log
-- Each row = one point-in-time snapshot of the storage tank
-- STATE: 0=Idle, 1=Charging, 2=Discharging
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HAAS_THERMAL_STORAGE')
BEGIN
    CREATE TABLE [dbo].[HAAS_THERMAL_STORAGE] (
        [STORAGE_ID]              BIGINT IDENTITY(1,1) PRIMARY KEY,
        [STORAGE_CAPACITY_KWH]    FLOAT        NOT NULL DEFAULT 2000,
        [CURRENT_STORED_HEAT_KWH] FLOAT        NOT NULL DEFAULT 0,
        [CHARGE_PERCENTAGE]       FLOAT        NOT NULL DEFAULT 0,
        [STATE]                   INT          NOT NULL DEFAULT 0,
        [STATE_NAME]              NVARCHAR(20) NOT NULL DEFAULT 'Idle',
        [CHARGING_RATE_KW]        FLOAT        NOT NULL DEFAULT 0,
        [DISCHARGING_RATE_KW]     FLOAT        NOT NULL DEFAULT 0,
        [TIMESTAMP]               DATETIME2    NOT NULL DEFAULT GETUTCDATE()
    );
    PRINT 'Created table: HAAS_THERMAL_STORAGE';
END
ELSE
    PRINT 'Table already exists: HAAS_THERMAL_STORAGE';

-- =============================================================================
-- 4. Optimization Decision Log
-- Every run of the optimization engine produces rows here
-- STATUS: 'SUCCESS' | 'PARTIAL' | 'DEFICIT'
-- =============================================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HAAS_OPTIMIZATION_LOG')
BEGIN
    CREATE TABLE [dbo].[HAAS_OPTIMIZATION_LOG] (
        [LOG_ID]              BIGINT IDENTITY(1,1) PRIMARY KEY,
        [TIMESTAMP]           DATETIME2     NOT NULL DEFAULT GETUTCDATE(),
        [ACTION]              NVARCHAR(100) NOT NULL,
        [DETAILS]             NVARCHAR(MAX),
        [HEAT_REDIRECTED_KW]  FLOAT         NOT NULL DEFAULT 0,
        [TARGET_ECOSYSTEM]    NVARCHAR(50),
        [SUPPLY_KW]           FLOAT         NOT NULL DEFAULT 0,
        [TOTAL_DEMAND_KW]     FLOAT         NOT NULL DEFAULT 0,
        [STATUS]              NVARCHAR(20)  NOT NULL DEFAULT 'SUCCESS'
    );
    PRINT 'Created table: HAAS_OPTIMIZATION_LOG';
END
ELSE
    PRINT 'Table already exists: HAAS_OPTIMIZATION_LOG';

-- =============================================================================
-- 5. Seed Default Ecosystem Demand Data (16 rows: 4 ecosystems × 4 seasons)
-- Safe to run multiple times (uses MERGE / INSERT IF NOT EXISTS pattern)
-- =============================================================================
PRINT 'Seeding default ecosystem demand data...';

MERGE [dbo].[HAAS_ECOSYSTEM_DEMAND] AS target
USING (VALUES
    -- District Heating (Priority 2): High in winter, negligible in summer
    (1, 'DistrictHeating', 450, 30, 90,  1, 'Winter',  2),
    (1, 'DistrictHeating', 200, 30, 90,  2, 'Spring',  2),
    (1, 'DistrictHeating',  50, 30, 90,  3, 'Summer',  2),
    (1, 'DistrictHeating', 300, 30, 90,  4, 'Autumn',  2),

    -- Agriculture / Greenhouses (Priority 3): Moderate, peaks in cold months
    (2, 'Agriculture',  180, 20, 45,  1, 'Winter',  3),
    (2, 'Agriculture',  120, 20, 45,  2, 'Spring',  3),
    (2, 'Agriculture',   80, 20, 45,  3, 'Summer',  3),
    (2, 'Agriculture',  150, 20, 45,  4, 'Autumn',  3),

    -- Thermal Storage (Priority 4): Acts as buffer; absorbs excess heat
    (3, 'ThermalStorage', 100, 25, 95,  1, 'Winter',  4),
    (3, 'ThermalStorage', 200, 25, 95,  2, 'Spring',  4),
    (3, 'ThermalStorage', 300, 25, 95,  3, 'Summer',  4),
    (3, 'ThermalStorage', 150, 25, 95,  4, 'Autumn',  4),

    -- Industry / Foundries (Priority 1): Constant high-temp demand year-round
    (4, 'Industry',  250, 35, 120,  1, 'Winter',  1),
    (4, 'Industry',  250, 35, 120,  2, 'Spring',  1),
    (4, 'Industry',  250, 35, 120,  3, 'Summer',  1),
    (4, 'Industry',  250, 35, 120,  4, 'Autumn',  1)
) AS source (ECOSYSTEM_TYPE, ECOSYSTEM_NAME, DEMAND_VALUE_KW, MIN_TEMP_REQUIRED, MAX_TEMP_ACCEPTED, SEASON_TYPE, SEASON_NAME, PRIORITY)
ON target.ECOSYSTEM_TYPE = source.ECOSYSTEM_TYPE AND target.SEASON_TYPE = source.SEASON_TYPE
WHEN MATCHED THEN
    UPDATE SET
        DEMAND_VALUE_KW   = source.DEMAND_VALUE_KW,
        MIN_TEMP_REQUIRED = source.MIN_TEMP_REQUIRED,
        MAX_TEMP_ACCEPTED = source.MAX_TEMP_ACCEPTED,
        PRIORITY          = source.PRIORITY,
        TIMESTAMP         = GETUTCDATE()
WHEN NOT MATCHED THEN
    INSERT (ECOSYSTEM_TYPE, ECOSYSTEM_NAME, DEMAND_VALUE_KW, MIN_TEMP_REQUIRED,
            MAX_TEMP_ACCEPTED, SEASON_TYPE, SEASON_NAME, PRIORITY, TIMESTAMP)
    VALUES (source.ECOSYSTEM_TYPE, source.ECOSYSTEM_NAME, source.DEMAND_VALUE_KW,
            source.MIN_TEMP_REQUIRED, source.MAX_TEMP_ACCEPTED, source.SEASON_TYPE,
            source.SEASON_NAME, source.PRIORITY, GETUTCDATE());

-- =============================================================================
-- 6. Seed initial thermal storage status (empty tank, 2000 kWh capacity)
-- =============================================================================
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[HAAS_THERMAL_STORAGE])
BEGIN
    INSERT INTO [dbo].[HAAS_THERMAL_STORAGE]
        (STORAGE_CAPACITY_KWH, CURRENT_STORED_HEAT_KWH, CHARGE_PERCENTAGE, STATE, STATE_NAME, TIMESTAMP)
    VALUES
        (2000, 400, 20.0, 0, 'Idle', GETUTCDATE());
    PRINT 'Seeded initial thermal storage record (20% pre-charged).';
END

PRINT '=== HaaS database setup complete! ===';
SELECT 'HAAS_DATACENTER_METRICS'  AS [Table], COUNT(*) AS [Rows] FROM [dbo].[HAAS_DATACENTER_METRICS]  UNION ALL
SELECT 'HAAS_ECOSYSTEM_DEMAND',               COUNT(*)            FROM [dbo].[HAAS_ECOSYSTEM_DEMAND]    UNION ALL
SELECT 'HAAS_THERMAL_STORAGE',                COUNT(*)            FROM [dbo].[HAAS_THERMAL_STORAGE]     UNION ALL
SELECT 'HAAS_OPTIMIZATION_LOG',               COUNT(*)            FROM [dbo].[HAAS_OPTIMIZATION_LOG];

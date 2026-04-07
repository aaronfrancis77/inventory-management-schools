CREATE DATABASE IF NOT EXISTS DayMap_Inventory;
USE DayMap_Inventory;

CREATE TABLE Iteminstances (
    InstanceID INT PRIMARY KEY AUTO_INCREMENT COMMENT 'Unique ID for specific physical unit',
    ItemID     INT NOT NULL COMMENT 'Links to the generic item (FK)',
    SerialNumber VARCHAR(100) UNIQUE COMMENT 'Unique per unit',
    ExpiryDate DATETIME COMMENT 'For medical/perishable items',
    Status     VARCHAR(50) COMMENT 'Available, Loaned, Unavailable, Faulty',
    Stockcount INT COMMENT 'Aggregated total (cached for performance)'
);
// Define the table name
var tableName = "Datesss";

// Define the start and end dates using variables
var startDate = new DateTime(2020, 1, 1);
var endDate = new DateTime(2030, 12, 31);

// Generate a DAX expression for the date table
var dateTableDax = string.Format(@"
ADDCOLUMNS (
    CALENDAR ( DATE({0}, {1}, {2}), DATE({3}, {4}, {5}) ),
    
    ""Year"", YEAR([Date]),
    ""Month Number"", MONTH([Date]),
    ""Month Name"", FORMAT([Date], ""MMMM""),
    ""Month Short Name"", FORMAT([Date], ""MMM""),
    ""Quarter"", QUARTER([Date]),
    ""Quarter Name"", ""Q"" & QUARTER([Date]),
    ""Year-Month"", FORMAT([Date], ""YYYY-MM""),
    ""Year-Quarter"", FORMAT([Date], ""YYYY"") & ""-Q"" & QUARTER([Date]),
    
    ""Day of Week"", WEEKDAY([Date], 2),
    ""Day Name"", FORMAT([Date], ""dddd""),
    ""Day Short Name"", FORMAT([Date], ""ddd""),
    ""Is Weekend"", IF(WEEKDAY([Date], 2) > 5, TRUE, FALSE),
    
    ""Day of Month"", DAY([Date]),
    ""Day of Year"", DATEDIFF(DATE(YEAR([Date]),1,1), [Date], DAY) + 1,
    
    ""Week Number"", WEEKNUM([Date], 2),
    ""ISO Week Number"", WEEKNUM([Date], 21),
    ""Week Start Date"", [Date] - WEEKDAY([Date], 2) + 1,
    ""Week End Date"", [Date] + (7 - WEEKDAY([Date], 2)),
    
    ""Is Today"", IF([Date] = TODAY(), TRUE, FALSE),
    ""Is Future"", IF([Date] > TODAY(), TRUE, FALSE),
    ""Is Past"", IF([Date] < TODAY(), TRUE, FALSE)
)",
    startDate.Year, startDate.Month, startDate.Day,
    endDate.Year, endDate.Month, endDate.Day
);

// Create or replace the table
var dateTable = Model.Tables.Contains(tableName) 
    ? Model.Tables[tableName]
    : Model.AddCalculatedTable(tableName, dateTableDax);

// Set metadata
dateTable.Description = "Extended date dimension table with rich calendar attributes.";
dateTable.IsHidden = false;

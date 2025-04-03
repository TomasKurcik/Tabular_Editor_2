// Define the table name
var tableName = "Dates";  // to change

// Define the fact table and date column to analyze
var factTable = "Orders";     // to change
var dateColumn = "OrderDate"; // to change

// Generate the DAX expression using MIN and MAX from the fact table
var dateTableDax = string.Format(@"
ADDCOLUMNS (
    CALENDAR ( 
        CALCULATE(MIN({0}[{1}])), 
        CALCULATE(MAX({0}[{1}])) 
    ),
    
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
    factTable, dateColumn
);

// Create or replace the table
var dateTable = Model.Tables.Contains(tableName) 
    ? Model.Tables[tableName]
    : Model.AddCalculatedTable(tableName, dateTableDax);

// Set metadata
dateTable.Description = "Dynamic date table based on FactSales[OrderDate] range.";
dateTable.IsHidden = false;

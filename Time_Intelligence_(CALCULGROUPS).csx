// === CONFIGURATION ===
var calcGroupName = "Trrrrrrrrrrrrr";
var dateColumn = "'Dates'[Date]";  // Full column reference
// =======================

// === Create Calculation Group ===
var calcGroup = Model.AddCalculationGroup(calcGroupName);

// === Define Calculation Items ===
var calcItems = new[] {
    new { Name = "Base", Expression = "SELECTEDMEASURE()" },

    // === Yearly & Previous Periods ===
    new { Name = "Previous Year", Expression = "CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + "))" },
    new { Name = "Previous Quarter", Expression = "CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER))" },
    new { Name = "Previous Month", Expression = "CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH))" },

    new { Name = "YoY", Expression = "SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + "))" },
    new { Name = "YoY%", Expression = "DIVIDE(SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + ")), CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + ")))" },

    // === Standard Periods ===
    new { Name = "MTD", Expression = "CALCULATE(SELECTEDMEASURE(), DATESMTD(" + dateColumn + "))" },
    new { Name = "QTD", Expression = "CALCULATE(SELECTEDMEASURE(), DATESQTD(" + dateColumn + "))" },
    new { Name = "YTD", Expression = "CALCULATE(SELECTEDMEASURE(), DATESYTD(" + dateColumn + "))" },

    // === Month-over-Month ===
    new { Name = "MoM", Expression = "SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH))" },
    new { Name = "MoM%", Expression = "DIVIDE(SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH)), CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH)))" },

    // === Quarter-over-Quarter ===
    new { Name = "QoQ", Expression = "SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER))" },
    new { Name = "QoQ%", Expression = "DIVIDE(SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER)), CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER)))" },

    // === Rolling Periods ===
    new { Name = "Rolling 3M", Expression = "CALCULATE(SELECTEDMEASURE(), DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -3, MONTH))" },
    new { Name = "Rolling 6M", Expression = "CALCULATE(SELECTEDMEASURE(), DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -6, MONTH))" },
    new { Name = "Rolling 12M", Expression = "CALCULATE(SELECTEDMEASURE(), DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -12, MONTH))" }
};

// === Add Calculation Items ===
foreach (var item in calcItems)
{
    var calcItem = calcGroup.AddCalculationItem(item.Name);
    calcItem.Expression = item.Expression;
}

// === Create Physical Measures for Each Selected Measure ===
foreach (var m in Selected.Measures)
{
    var name = m.Name;
    var dax = m.DaxObjectName;


    // === Standard Periods ===
    m.Table.AddMeasure(name + " YTD", "TOTALYTD(" + dax + ", " + dateColumn + ")", m.DisplayFolder);
    m.Table.AddMeasure(name + " QTD", "TOTALQTD(" + dax + ", " + dateColumn + ")", m.DisplayFolder);
    m.Table.AddMeasure(name + " MTD", "TOTALMTD(" + dax + ", " + dateColumn + ")", m.DisplayFolder);

    // === Previous Periods ===
    m.Table.AddMeasure(name + " PY", "CALCULATE(" + dax + ", SAMEPERIODLASTYEAR(" + dateColumn + "))", m.DisplayFolder);
    m.Table.AddMeasure(name + " PQ", "CALCULATE(" + dax + ", PARALLELPERIOD(" + dateColumn + ", -1, QUARTER))", m.DisplayFolder);
    m.Table.AddMeasure(name + " PM", "CALCULATE(" + dax + ", PARALLELPERIOD(" + dateColumn + ", -1, MONTH))", m.DisplayFolder);

    // === YoY / YoY%
    m.Table.AddMeasure(name + " YoY", dax + " - [" + name + " PY]", m.DisplayFolder);
    m.Table.AddMeasure(name + " YoY%", "DIVIDE([" + name + " YoY], [" + name + " PY])", m.DisplayFolder).FormatString = "0.0 %";

    // === MoM / MoM%
    m.Table.AddMeasure(name + " MoM", dax + " - [" + name + " PM]", m.DisplayFolder);
    m.Table.AddMeasure(name + " MoM%", "DIVIDE([" + name + " MoM], [" + name + " PM])", m.DisplayFolder).FormatString = "0.0 %";

    // === QoQ / QoQ%
    m.Table.AddMeasure(name + " QoQ", dax + " - [" + name + " PQ]", m.DisplayFolder);
    m.Table.AddMeasure(name + " QoQ%", "DIVIDE([" + name + " QoQ], [" + name + " PQ])", m.DisplayFolder).FormatString = "0.0 %";

    // === Rolling Windows ===
    m.Table.AddMeasure(name + " Rolling 3M", "CALCULATE(" + dax + ", DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -3, MONTH))", m.DisplayFolder);
    m.Table.AddMeasure(name + " Rolling 6M", "CALCULATE(" + dax + ", DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -6, MONTH))", m.DisplayFolder);
    m.Table.AddMeasure(name + " Rolling 12M", "CALCULATE(" + dax + ", DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -12, MONTH))", m.DisplayFolder);
}

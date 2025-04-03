// === CONFIGURATION ===
var calcGroupName = "oooo";
var dateColumn = "'Dates'[Date]";  // Full column reference
// =======================

// === Create Calculation Group ===
var calcGroup = Model.AddCalculationGroup(calcGroupName);

// === Define Calculation Items ===
var calcItems = new[] {
    new { Name = "Base", Expression = "SELECTEDMEASURE()" },

    new { Name = "Previous Year", Expression = "CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + "))" },
    new { Name = "Previous Quarter", Expression = "CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER))" },
    new { Name = "Previous Month", Expression = "CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH))" },

    new { Name = "YoY", Expression = "SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + "))" },
    new { Name = "YoY%", Expression = "DIVIDE(SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + ")), CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR(" + dateColumn + ")))" },

    new { Name = "MTD", Expression = "CALCULATE(SELECTEDMEASURE(), DATESMTD(" + dateColumn + "))" },
    new { Name = "QTD", Expression = "CALCULATE(SELECTEDMEASURE(), DATESQTD(" + dateColumn + "))" },
    new { Name = "YTD", Expression = "CALCULATE(SELECTEDMEASURE(), DATESYTD(" + dateColumn + "))" },

    new { Name = "MoM", Expression = "SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH))" },
    new { Name = "MoM%", Expression = "DIVIDE(SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH)), CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, MONTH)))" },

    new { Name = "QoQ", Expression = "SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER))" },
    new { Name = "QoQ%", Expression = "DIVIDE(SELECTEDMEASURE() - CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER)), CALCULATE(SELECTEDMEASURE(), PARALLELPERIOD(" + dateColumn + ", -1, QUARTER)))" },

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
if (!Selected.Measures.Any())
{
    Error("Please select one or more measures before running this script.");
}
else
{
    foreach (var m in Selected.Measures)
    {
        var name = m.Name;
        var dax = m.DaxObjectName;
        var table = m.Table;
        var folder = m.DisplayFolder;

        table.AddMeasure(name + " YTD", "TOTALYTD(" + dax + ", " + dateColumn + ")", folder);
        table.AddMeasure(name + " QTD", "TOTALQTD(" + dax + ", " + dateColumn + ")", folder);
        table.AddMeasure(name + " MTD", "TOTALMTD(" + dax + ", " + dateColumn + ")", folder);

        table.AddMeasure(name + " PY", "CALCULATE(" + dax + ", SAMEPERIODLASTYEAR(" + dateColumn + "))", folder);
        table.AddMeasure(name + " PQ", "CALCULATE(" + dax + ", PARALLELPERIOD(" + dateColumn + ", -1, QUARTER))", folder);
        table.AddMeasure(name + " PM", "CALCULATE(" + dax + ", PARALLELPERIOD(" + dateColumn + ", -1, MONTH))", folder);

        table.AddMeasure(name + " YoY", dax + " - [" + name + " PY]", folder);
        var yoyPct = table.AddMeasure(name + " YoY%", "DIVIDE([" + name + " YoY], [" + name + " PY])", folder);
        yoyPct.FormatString = "0.0 %";

        table.AddMeasure(name + " MoM", dax + " - [" + name + " PM]", folder);
        var momPct = table.AddMeasure(name + " MoM%", "DIVIDE([" + name + " MoM], [" + name + " PM])", folder);
        momPct.FormatString = "0.0 %";

        table.AddMeasure(name + " QoQ", dax + " - [" + name + " PQ]", folder);
        var qoqPct = table.AddMeasure(name + " QoQ%", "DIVIDE([" + name + " QoQ], [" + name + " PQ])", folder);
        qoqPct.FormatString = "0.0 %";

        table.AddMeasure(name + " Rolling 3M", "CALCULATE(" + dax + ", DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -3, MONTH))", folder);
        table.AddMeasure(name + " Rolling 6M", "CALCULATE(" + dax + ", DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -6, MONTH))", folder);
        table.AddMeasure(name + " Rolling 12M", "CALCULATE(" + dax + ", DATESINPERIOD(" + dateColumn + ", MAX(" + dateColumn + "), -12, MONTH))", folder);
    }
}

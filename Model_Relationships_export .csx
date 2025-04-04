string exportPath = @"-Path-";

var sb = new System.Text.StringBuilder();
sb.AppendLine("FromTable\tFromColumn\t→\tToTable\tToColumn\tCardinality\tIsActive\tCrossFilteringBehavior");

foreach (var rel in Model.Relationships)
{
    string arrow = "→";
    string cardinality = rel.FromCardinality.ToString() + " to " + rel.ToCardinality.ToString();

    sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}",
        rel.FromTable.Name,
        rel.FromColumn.Name,
        arrow,
        rel.ToTable.Name,
        rel.ToColumn.Name,
        cardinality,
        rel.IsActive,
        rel.CrossFilteringBehavior));
}

System.IO.File.WriteAllText(exportPath, sb.ToString());

Output("✅ Relationships exported with cardinality and arrows to:\n" + exportPath);

using System.Globalization;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace RushTodo.IntegrationTests;

public static class CsvAssert
{
    public static void Contains<T>(string expectedCsv, IEnumerable<T> actual)
    {
        var expected = CsvTable.Parse(expectedCsv);
        var actualTable = CsvTable.From(expected.Headers, actual);
        var unmatchedActualRows = actualTable.Rows.Select(RowKey).ToList();
        var missingRows = new List<string[]>();

        foreach (var expectedRow in expected.Rows)
        {
            var index = unmatchedActualRows.IndexOf(RowKey(expectedRow));
            if (index < 0)
                missingRows.Add(expectedRow);
            else
                unmatchedActualRows.RemoveAt(index);
        }

        if (missingRows.Count == 0) return;

        Assert.Fail($"""
            Expected CSV rows were not found:
            {new CsvTable(expected.Headers, missingRows).Write()}
            Actual CSV:
            {actualTable.Write()}
            .
            """);
    }

    private static string RowKey(IEnumerable<string> values) => string.Join('\u001f', values);


    private record CsvTable(string[] Headers, IReadOnlyList<string[]> Rows)
    {
        public static CsvTable From<T>(string[] headers, IEnumerable<T> items)
        {
            var properties = headers.Select(header => GetProperty<T>(header)).ToArray();
            var rows = items
                .Select(item => properties.Select(property => Format(property.GetValue(item))).ToArray())
                .ToArray();

            return new(headers, rows);
        }

        public static CsvTable Parse(string csv)
        {
            var rows = ParseRows(csv);
            if (rows.Count == 0) throw new ArgumentException("Expected CSV must include a header row.", nameof(csv));

            var headers = rows[0];
            var values = rows.Skip(1).ToArray();
            var invalidRow = values.FirstOrDefault(row => row.Length != headers.Length);
            if (invalidRow is not null)
                throw new ArgumentException($"Expected {headers.Length} CSV values per row but found {invalidRow.Length}.", nameof(csv));

            return new(headers, values);
        }

        public string Write()
        {
            var builder = new StringBuilder();
            WriteRow(builder, Headers);
            foreach (var row in Rows) WriteRow(builder, row);
            return builder.ToString().TrimEnd();
        }

        private static PropertyInfo GetProperty<T>(string name)
        {
            return typeof(T).GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)
                   ?? throw new ArgumentException($"{typeof(T).Name} has no public property named '{name}'.");
        }

        private static string Format(object? value)
        {
            return value switch
            {
                null => "",
                bool boolean => boolean ? "TRUE" : "FALSE",
                DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString(),
            } ?? "";
        }

        private static List<string[]> ParseRows(string csv)
        {
            var rows = new List<string[]>();
            var row = new List<string>();
            var field = new StringBuilder();
            var inQuotes = false;

            for (var index = 0; index < csv.Length; index++)
            {
                var character = csv[index];
                if (character == '"')
                {
                    if (inQuotes && index + 1 < csv.Length && csv[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (character == ',' && !inQuotes)
                {
                    row.Add(field.ToString());
                    field.Clear();
                }
                else if (character is '\r' or '\n' && !inQuotes)
                {
                    if (character == '\r' && index + 1 < csv.Length && csv[index + 1] == '\n') index++;
                    row.Add(field.ToString());
                    field.Clear();
                    if (row.Any(value => value.Length > 0)) rows.Add(row.ToArray());
                    row.Clear();
                }
                else
                {
                    field.Append(character);
                }
            }

            if (inQuotes) throw new ArgumentException("Expected CSV contains an unterminated quoted value.", nameof(csv));
            row.Add(field.ToString());
            if (row.Any(value => value.Length > 0)) rows.Add(row.ToArray());

            return rows;
        }

        private static void WriteRow(StringBuilder builder, IEnumerable<string> values)
        {
            builder.AppendJoin(',', values.Select(Escape));
            builder.AppendLine();
        }

        private static string Escape(string value)
        {
            return value.IndexOfAny([',', '"', '\r', '\n']) >= 0
                ? $"\"{value.Replace("\"", "\"\"")}\""
                : value;
        }
    }
}

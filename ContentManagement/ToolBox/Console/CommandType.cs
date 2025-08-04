namespace ToolBox.Console;

public class CommandType
{
    public string Name { get; }
    public Action<IEnumerable<string>>? Execute { get; set; }
    public string? Description { get; set; }

    public byte MinParms { get; set; }
    public byte MaxParms { get; set; }

    public CommandType(string name, Action<IEnumerable<string>>? method = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name[0] == '-' ? name : '-' + name;
        Execute ??= method;

        MaxParms = Math.Max(MaxParms, MinParms);
    }

    public CommandType(CommandType other, string? newName = null)
    {
        Name = newName ?? other.Name;
        Execute ??= other.Execute;
        Description ??= other.Description;
        MinParms = other.MinParms;
        MaxParms = other.MaxParms;

        MaxParms = Math.Max(MaxParms, MinParms);
    }
}

public class CommandAction(CommandType command) : CommandType(command)
{
    public List<string> Parameters { get; init; } = [];

    public bool IsValidParmCount => Parameters.Count >= MinParms && Parameters.Count <= MaxParms;
    public bool CanAddParm => Parameters.Count < MaxParms;

    public bool AddParameter(string value)
    {
        if (CanAddParm)
        {
            Parameters.Add(value);
            return true;
        }
        return false;
    }


}

public class Quest
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public bool IsCompleted { get; private set; }

    public Quest(string title, string description)
    {
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    public void Complete()
    {
        IsCompleted = true;
    }
}

using System.ComponentModel.DataAnnotations;

public class Note
{
    public int id { get; set; }
    public string title { get; set; } = string.Empty;
    public string content { get; set; } = string.Empty;
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}

public class NoteRequest
{
    [Required, StringLength(100)]
    public string title { get; set; } = string.Empty;

    public string content { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int userId { get; set; }
}

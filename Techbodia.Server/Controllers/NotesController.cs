using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
    private readonly IConfiguration configuration;

    public NotesController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private SqlConnection GetConnection() =>
        new(configuration.GetConnectionString("maindb"));

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetNotes(int userId)
    {
        using var connection = GetConnection();
        const string sql = """
            SELECT note_id AS id, title, content,
                   created_dt AS createdAt, updated_dt AS updatedAt
            FROM tbl_notes
            WHERE user_id = @userId
            ORDER BY updated_dt DESC
            """;

        return Ok(await connection.QueryAsync<Note>(sql, new { userId }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateNote(NoteRequest request)
    {
        using var connection = GetConnection();
        const string sql = """
            INSERT INTO tbl_notes(title, content, user_id)
            OUTPUT INSERTED.note_id AS id, INSERTED.title, INSERTED.content,
                   INSERTED.created_dt AS createdAt, INSERTED.updated_dt AS updatedAt
            VALUES (@title, @content, @userId)
            """;

        var note = await connection.QuerySingleAsync<Note>(sql, request);
        return Created($"/api/Notes/{note.id}", note);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateNote(int id, NoteRequest request)
    {
        using var connection = GetConnection();
        const string sql = """
            UPDATE tbl_notes
            SET title = @title, content = @content, updated_dt = GETDATE()
            OUTPUT INSERTED.note_id AS id, INSERTED.title, INSERTED.content,
                   INSERTED.created_dt AS createdAt, INSERTED.updated_dt AS updatedAt
            WHERE note_id = @id AND user_id = @userId
            """;

        var note = await connection.QuerySingleOrDefaultAsync<Note>(sql, new
        {
            id,
            request.title,
            request.content,
            request.userId,
        });

        return note is null ? NotFound() : Ok(note);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteNote(int id, [FromQuery] int userId)
    {
        using var connection = GetConnection();
        const string sql = "DELETE FROM tbl_notes WHERE note_id = @id AND user_id = @userId";

        var deleted = await connection.ExecuteAsync(sql, new { id, userId });
        return deleted == 0 ? NotFound() : NoContent();
    }
}
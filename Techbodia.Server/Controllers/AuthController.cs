using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using BCrypt.Net;


[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    private SqlConnection GetConnection()
    {
        return new SqlConnection(_configuration.GetConnectionString("maindb"));
    }
    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser([FromBody] dto_registerUser dto)
    {
        using var connection = GetConnection();

        string registersql = @"SELECT TOP(1) * from tbl_auth
                       WHERE userName = @userName 
                       OR email = @email";

        var exisitingUser = await connection.QueryFirstOrDefaultAsync<tbl_auth>(registersql, new
        {
            userName = dto.userName,
            email = dto.email
        });

        if(exisitingUser != null)
        {
            if (exisitingUser.userName.Equals(dto.userName, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "the user name is already taken. Please try again." });
            }
            if (exisitingUser.email.Equals(dto.email, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { message = "the email is already register. Please try again." });
            }
        }
        string pwdHash = BCrypt.Net.BCrypt.HashPassword(dto.userPassword);

        string insertsql = @"INSERT INTO tbl_auth(userName, email, userPassword, is_active, created_dt, updated_dt)
                            VALUES (@userName, @email, @userPassword, @is_active, GETDATE(), GETDATE())
                            SELECT CAST(SCOPE_IDENTITY() as INT);";

        int newUserID = await connection.ExecuteScalarAsync<int>(insertsql, new
        {
            userName = dto.userName,
            email = dto.email,
            userPassword = pwdHash,
            is_active = true
        });

        return Ok(new { message = "Registration is completed", userID = newUserID });
    }
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] dto_Login dto)
    {
        using var connection = GetConnection();

        string loginsql = @"select user_id, userName, email, userPassword, is_active
                           from tbl_auth
                           WHERE userName = @userName";

        var user = await connection.QueryFirstOrDefaultAsync<tbl_auth>(loginsql, new
        {
            dto.userName
        });


        if (user == null || !user.is_active)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        bool isValidPwd = BCrypt.Net.BCrypt.Verify(dto.userPassword, user.userPassword);
        if (!isValidPwd)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        // 3. Success Response
        return Ok(new
        {
            message = "Login successful!",
            userId = user.user_id,
            userName = user.userName,
            email = user.email
        });
    }
}
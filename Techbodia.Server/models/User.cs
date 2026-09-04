
using System.ComponentModel.DataAnnotations;

public class tbl_auth
{
    public int user_id {get;set;}
    public string userName {get;set;} = string.Empty;
    public string email {get;set;} = string.Empty;
    public string userPassword {get;set;} = string.Empty;
    public bool is_active {get;set;} = true;
    public DateTime created_dt {get;set;}
    public DateTime updated_dt {get;set;}
}

public class dto_registerUser
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string userName {get;set;} = string.Empty;
    [Required, EmailAddress, StringLength(255)]
    public string email {get;set;} = string.Empty;
    [Required, StringLength(100, MinimumLength = 6)]
    public string userPassword{get;set;} = string.Empty;

}
public class dto_Login
{
    [Required, StringLength(50, MinimumLength = 3)]
    public string userName {get;set;} = string.Empty;
    [Required, StringLength(100, MinimumLength = 6)]
    public string userPassword {get;set;} = string.Empty;
}


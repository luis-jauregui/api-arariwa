namespace ApiArariwa.Auditoria;

public class UserLoginLogs
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime AttemptDate { get; set; }
    public string IpAddress { get; set; }
    public string Details { get; set; }
}
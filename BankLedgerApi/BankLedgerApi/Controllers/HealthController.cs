using Microsoft.AspNetCore.Mvc;

namespace BankLedgerApi.Controllers;

[ApiController]
[Route("health")]
public class HealthController:ControllerBase
{
    [HttpGet]
    public ActionResult<bool> GetApiHealth()
    {
        return Ok(true);
    }
}
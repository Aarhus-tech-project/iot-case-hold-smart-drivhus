using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiSDH.Controllers;

public record TestBody(string name);

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostTest([FromBody]TestBody body)
    {
        return Ok("Test");
    }
}
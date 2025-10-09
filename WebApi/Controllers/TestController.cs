using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("[controller]")]
public class TestController : ControllerBase
{


    [HttpGet("/api/get")]
    public string Get()
    {
        return "demo";
    }
    
    
    [HttpGet("/api/getbyid")]
    public string GetById(int id)
    {
        return "demo";
    }
    
    [HttpPost]
    public string Add(string demo)
    {
        return "demo";
    }
    
    
    [HttpPut]
    public string Update(string demo)
    {
        return "demo";
    }
    
    [HttpDelete]
    public string Delete(int id)
    {
        return "demo";
    }
}
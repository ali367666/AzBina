using Application.Abstracts.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CityController : ControllerBase
{
    private readonly IRepository<City, int> _repo;

    public CityController(IRepository<City, int> repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var city = _repo.GetAll();
        return Ok(city);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var citi = _repo.GetById(id);
        if(citi == null)
        {
            return NotFound();
        }
        _repo.Delete(id);
        _repo.SaveChanges();
        return Ok(citi);
    }

    [HttpPost]

    public IActionResult Add(City city)
    {
        _repo.Add(city);
        _repo.SaveChanges() ;
        return Ok(city);
    }

    [HttpPut]

    public IActionResult Update(int id, [FromBody] City updatecity)
    {
        var existing = _repo.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }
        existing.Name= updatecity.Name;
        existing.UpdatedAt= updatecity.UpdatedAt;
        existing.CreatedAt= updatecity.CreatedAt;
        _repo.SaveChanges();
        return Ok(existing);
    }

}

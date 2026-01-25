using Application.Abstracts.Repositories;
using Application.DTOs.DistrictDTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DistrictController : ControllerBase
{
    private readonly IRepository<District, int> _repo;
    public DistrictController(IRepository<District,int> repo)
    {
        _repo = repo;
        
    }
    [HttpGet]

    public IActionResult GetAll()
    {
        var district=_repo.GetAll();
        return Ok(district);
    }

    [HttpDelete]

    public IActionResult Delete(int id)
    {
        var district = _repo.GetById(id);
        if (district == null)
        {
            return NotFound();
        }
        _repo.Delete(id);
        _repo.SaveChanges();
        return Ok(district);
    }
    [HttpPost]

    public IActionResult Add(District district)
    {
        _repo.Add(district);
        _repo.SaveChanges();

        return Ok(district);
    }

    [HttpPut]

    public IActionResult Update(int id, [FromBody] District updatedistrict)
    {
        var existing = _repo.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }
        existing.Name = updatedistrict.Name;
        existing.CreatedAt = updatedistrict.CreatedAt;
        existing.UpdatedAt = updatedistrict.UpdatedAt;
        _repo.Update(existing);
        _repo.SaveChanges();
        return Ok(existing);
    }
}

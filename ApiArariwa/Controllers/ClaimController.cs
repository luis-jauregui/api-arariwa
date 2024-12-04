﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiArariwa.Dapper;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace ApiArariwa.Controllers;

public class ClaimRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ClaimController : ControllerBase
{
    private readonly GenericRepository _repository;

    public ClaimController(GenericRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("add-claim")]
    public async Task<IActionResult> AddClaim([FromBody] ClaimRequest request)
    {
        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", request.Name);
        parameters.Add("@Description", request.Description);
        try
        {
            await _repository.Insert("sp_ins_Claims", parameters);
            return Ok("Claim agregado al sistema.");
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    [HttpGet("list-claim")]
    public async Task<IActionResult> GetClaims()
    {
        try
        {
            List<Claims> claims = (await _repository.List<Claims>("sp_get_Claims")).ToList();
            return Ok(claims);
        }
        catch (SqlException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }

}

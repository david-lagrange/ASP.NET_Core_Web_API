using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NET_Core_Web_API.ModelBinders;
using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_Core_Web_API.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }
        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(companyDto);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyForCreationDto company)
        {
            if(company == null)
            {
                _logger.LogInfo("CompanyForCreationDto object sent from client is null.");
                return BadRequest("ComanyForCreationDto object is null.");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the CompanyCreationDto object.");
                return UnprocessableEntity(ModelState);
            }
            var companyEntity = _mapper.Map<Company>(company);
            _repository.Company.CreateCompany(companyEntity);
            await _repository.SaveAsync();
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
        }
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if(ids == null)
            {
                _logger.LogError("Parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges: false);
            if(ids.Count() != companyEntities.Count())
            {
                _logger.LogError("Some ids are not valid in a collection");
                return NotFound();
            }
            var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            return Ok(companiesToReturn);
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if(companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach(Company company in companyEntities)
            {
                _repository.Company.CreateCompany(company);
            }
            await _repository.SaveAsync();
            var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
            if(company == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            return NoContent();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(Guid id, CompanyForUpdateDto company)
        {
            if(company == null)
            {
                _logger.LogInfo("CompanyForUpdateDto object sent from client is null.");
                return BadRequest("CompanyForUpdateDto object is null.");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for CompanyForUpdateDto object.");
                return UnprocessableEntity(ModelState);
            }
            var companyFromDb = await _repository.Company.GetCompanyAsync(id, true);
            if(companyFromDb == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(company, companyFromDb);
            await _repository.SaveAsync();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                _logger.LogInfo("patchDoc object sent from client is null.");
                return BadRequest("patchDoc object is null.");
            }
            var companyEntity = await _repository.Company.GetCompanyAsync(id, true);
            if(companyEntity == null)
            {
                _logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            var companyToPatch = _mapper.Map<CompanyForUpdateDto>(companyEntity);
            patchDoc.ApplyTo(companyToPatch, ModelState);
            TryValidateModel(companyToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the patch document.");
                return UnprocessableEntity(ModelState);
            }
            patchDoc.ApplyTo(companyToPatch);
            _mapper.Map(companyToPatch, companyEntity);
            await _repository.SaveAsync();
            return NoContent();
        }
    }
}
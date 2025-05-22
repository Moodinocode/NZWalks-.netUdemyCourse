using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionsFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;



namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper, ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("Get all action was invoked");
            var regionsDomain = await regionRepository.GetAllAsync();
           
            //if (regionsDomain == null) return NotFound();
            
            
            //var regionsDto = new List<RegionDto>();
            //foreach (var region in regionsDomain) {
            //    regionsDto.Add(new RegionDto()
            //    {
            //        ID = region.ID,
            //        Code = region.Code,
            //        Name = region.Name,
            //        RegionImageURL = region.RegionImageURL,
            //    });
            //}

            var regionsDto = mapper.Map<List<Region>>(regionsDomain);

            return Ok(regionsDto);
        }



        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            //var region = dbContext.Regions.Find(id);
            var regionDomain = await regionRepository.GetByIdAsync(id);
         
            if (regionDomain == null)
            {
                return NotFound();
            }

            var regionsDto = mapper.Map<Region>(regionDomain);

            return Ok(regionsDto);
        }


        [HttpPost]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDtocs addRegionRequestDto)
        {
            //if(!ModelState.IsValid) return BadRequest(ModelState);
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            regionDomainModel = await regionRepository.CreateAsync(regionDomainModel);

            var regionsDto = mapper.Map<Region>(regionDomainModel);

            return CreatedAtAction(nameof(GetById),new { id = regionsDto.ID }, regionsDto);
        }



        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            //if (!ModelState.IsValid) return BadRequest(ModelState);
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            var regionsDto = mapper.Map<Region>(regionDomainModel);
            return Ok(regionsDto);
        }




        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Writer")]
        public async Task<IActionResult> Delete([FromRoute] Guid id) 
        {
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            var regionsDto = mapper.Map<Region>(regionDomainModel);

            return Ok(regionsDto);
        }
    }
}

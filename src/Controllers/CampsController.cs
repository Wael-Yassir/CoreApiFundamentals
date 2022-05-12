using System;
using AutoMapper;
using System.Linq;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]

    // Will help do body binding for Post, and Put as it knows that the controller is an api
    // it also help in doing the validation check instead of using Model.IsValid
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public CampsController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._campRepository = campRepository;
            this._mapper = mapper;
            this._linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetCamps(bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsAsync(includeTalks);
                return _mapper.Map<CampModel[]>(results);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failer!");
            }
        }

        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker);

                if (result == null) return NotFound("The requested camp does not exist");

                return _mapper.Map<CampModel>(result);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> Get(DateTime date, bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsByEventDate(date, includeTalks);

                if (!results.Any()) return NotFound("The requested camp does not exist");

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                // check if there is a camp with the same moniker in the Db
                var existingCamp = _campRepository.GetCampAsync(model.Moniker);
                if (existingCamp != null)
                {
                    return BadRequest("Moniker in use!");
                }

                string createdCampLink = _linkGenerator.GetPathByAction("Get", "camps",
                    new { moniker = model.Moniker });

                if (string.IsNullOrWhiteSpace(createdCampLink))
                    return BadRequest("Could not use the current moniker!");

                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    // return Created($"/api/camps/{camp.Moniker}", _mapper.Map<CampModel>(camp));
                    return Created($"/api/camps/{camp.Moniker}", _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                // check if there is a camp with send moniker in the db
                var oldCamp = await _campRepository.GetCampAsync(moniker);
                if (oldCamp == null)
                {
                    return NotFound($"Could not find camp with moniker of {moniker}!");
                }

                // map the value from the model to the new camp
                _mapper.Map(model, oldCamp);
                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(oldCamp);
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                // get the camp from the db
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null)
                {
                    return NotFound($"Could not find camp with moniker of {moniker}!");
                }

                _campRepository.Delete(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return StatusCode(StatusCodes.Status200OK, "The camp had been deleted successfully!");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database failure");
            }

            return BadRequest("Failed to delete the camp");
        }
    }
}

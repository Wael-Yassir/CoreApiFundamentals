using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("api/camps/{moniker}/talks")]
    public class TalksContorller : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksContorller(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            this._campRepository = campRepository;
            this._mapper = mapper;
            this._linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> Get(string moniker)
        {
            try
            {
                var talks = await _campRepository.GetTalksByMonikerAsync(moniker, true);
                return _mapper.Map<TalkModel[]>(talks);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TalkModel>> Get(string moniker, int id)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, id, true);

                if (talk == null)
                {
                    return NotFound("The specified talk is not found!");
                }

                return _mapper.Map<TalkModel>(talk);
            }
            catch (System.Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Database Failure!");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> Post(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null) return BadRequest("Camp Does Not Exist!");

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;

                if (model.Speaker == null) return BadRequest("Speaker ID is required!");
                var speaker = await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if (speaker == null) return BadRequest("Speaker could not be found!");
                talk.Speaker = speaker;

                _campRepository.Add(talk);

                string location = _linkGenerator.GetPathByAction(HttpContext, "Get",
                    values: new { moniker, id = model.TalkId });

                if (await _campRepository.SaveChangesAsync())
                {
                    return Created(location, _mapper.Map<TalkModel>(talk));
                }
                else
                {
                    return BadRequest("Failure To Save New Talk!");
                }

            }
            catch (System.Exception)
            {
                return BadRequest("Database Failure");
            }
        }

        [HttpPut("{talkId:int}")]
        public async Task<ActionResult<TalkModel>> Put(string moniker, int talkId, TalkModel model)
        {
            try
            {
                // check if there is a talk with the given data
                var oldTalk = await _campRepository.GetTalkByMonikerAsync(moniker, talkId, true);
                if (oldTalk == null) return NotFound("The specified talk does not exist!");

                // map all the data form model to db entity
                // camp and spealer should be mapping manually
                _mapper.Map(model, oldTalk);

                if (model.Speaker != null)
                {
                    var speaker = await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);

                    if (speaker != null)
                    {
                        oldTalk.Speaker = speaker;
                    }
                }

                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(oldTalk);
                }
                else
                {
                    return BadRequest("Failed to update database!");
                }
            }
            catch (System.Exception)
            {
                return BadRequest("Database Failure");
            }
        }

        [HttpDelete("{talkId:int}")]
        public async Task<IActionResult> Delete(string moniker, int talkId)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, talkId);
                if (talk == null) return NotFound("Failed to find talk to database!");

                _campRepository.Delete(talk);
                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok("The talk deleted successfully!");
                }
                else
                {
                    return BadRequest("Failed to delete the talk!");
                }
            }
            catch (System.Exception)
            {
                return BadRequest("Database Failure");
            }
        }
    }
}

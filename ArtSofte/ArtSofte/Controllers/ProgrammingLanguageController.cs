using System.Collections.Generic;
using System.Threading.Tasks;
using Artsofte.Models;
using Artsofte.Models.Requests;
using Artsofte.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Artsofte.Controllers
{
    [Route("api/languages")]
    [ApiController]
    public class ProgrammingLanguageController : ControllerBase
    {
        private readonly ProgrammingLanguagesRepository _languagesRepository;

        public ProgrammingLanguageController(ProgrammingLanguagesRepository languagesRepository)
        {
            _languagesRepository = languagesRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ProgrammingLanguage>> AddNewLanguage(UpdateLanguageRequest updateLanguageRequest)
        {
            try
            {
                var language = new ProgrammingLanguage();
                UpdateModelFromRequest(language, updateLanguageRequest);
                await _languagesRepository.AddAsync(language);
                return Ok(language);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<ProgrammingLanguage>>> GetAllLanguages()
        {
            var languages = await _languagesRepository.GetAllAsync();
            return Ok(languages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProgrammingLanguage>> GetLanguage(int id)
        {
            var language = await _languagesRepository.FindAsync(id);
            return language is null
                ? NotFound()
                : Ok(language);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProgrammingLanguage>> UpdateLanguage(int id,
            UpdateLanguageRequest updateLanguageRequest)
        {
            try
            {
                var language = await _languagesRepository.FindAsync(id);
                if (language == null)
                    return NotFound();

                UpdateModelFromRequest(language, updateLanguageRequest);
                await _languagesRepository.UpdateAsync(language);
                return language;
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLanguage(int id)
        {
            var language = await _languagesRepository.FindAsync(id);
            if (language == null)
                return NotFound();
            await _languagesRepository.DeleteAsync(language);
            return Ok();
        }

        private static void UpdateModelFromRequest(ProgrammingLanguage language, UpdateLanguageRequest request)
        {
            language.Name = request.Name;
        }
    }
}
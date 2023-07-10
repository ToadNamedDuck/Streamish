using Microsoft.AspNetCore.Mvc;
using Streamish.Models;
using Streamish.Repositories;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserProfileRepository _userRepository;
        public UserProfilesController(IUserProfileRepository userRepository)
        {
            _userRepository = userRepository;
        }
        // GET: api/<UserProfilesController>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_userRepository.GetAll());
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET api/<UserProfilesController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(_userRepository.GetById(id));
            }
            catch
            {
                return NotFound();
            }
        }

        // POST api/<UserProfilesController>
        [HttpPost]
        public IActionResult Post(UserProfile profile)
        {
            try
            {
                _userRepository.Add(profile);
                return CreatedAtAction("Get", new { id = profile.Id }, profile);
            }
            catch
            {
                return BadRequest();
            }
        }

        // PUT api/<UserProfilesController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, UserProfile profile)
        {
            if(id != profile.Id)
            {
                return BadRequest();
            }
            else
            {
                try
                {
                    _userRepository.Update(profile);
                    return NoContent();
                }
                catch
                {
                    return BadRequest();
                }
            }
        }

        // DELETE api/<UserProfilesController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _userRepository.Delete(id);
                return NoContent();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using Streamish.Repositories;
using Streamish.Models;
using System.Collections.Generic;

namespace Streamish.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoRepository _videoRepository;
        public VideoController(IVideoRepository videoRepository)
        {
            _videoRepository = videoRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_videoRepository.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var video = _videoRepository.GetById(id);
            if (video == null)
            {
                return NotFound();
            }
            return Ok(video);
        }

        [HttpPost]
        public IActionResult Post(Video video)
        {
            // NOTE: This is only temporary to set the UserProfileId until we implement login
            // TODO: After we implement login, use the id of the current user
            video.UserProfileId = 1;

            video.DateCreated = DateTime.Now;
            if (string.IsNullOrWhiteSpace(video.Description))
            {
                video.Description = null;
            }

            try
            {
                // Handle the video URL

                // A valid video link might look like this:
                //  https://www.youtube.com/watch?v=sstOXCQ-EG0&list=PLdo4fOcmZ0oVGRpRwbMhUA0KAvMA2mLyN
                // 
                // Our job is to pull out the "v=XXXXX" part to get the get the "code/id" of the video
                //  So we can construct an URL that's appropriate for embedding a video

                // An embeddable Video URL looks something like this:
                //  https://www.youtube.com/embed/sstOXCQ-EG0

                // If this isn't a YouTube video, we should just give up
                if (!video.Url.Contains("youtube"))
                {
                    return BadRequest();
                }

                // If it's not already an embeddable URL, we have some work to do
                if (!video.Url.Contains("embed"))
                {
                    var videoCode = video.Url.Split("v=")[1].Split("&")[0];//split the url in 2 pieces. one before and one after v=
                    //keep the second half (array index 1), and split that again on & - keep the first split, index 0. that is the id
                    video.Url = $"https://www.youtube.com/embed/{videoCode}";//append to an embed link.
                }
            }
            catch // Something went wrong while creating the embeddable url
            {
                return BadRequest();
            }

            _videoRepository.Add(video);

            return CreatedAtAction("Get", new { id = video.Id }, video);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Video video)
        {
            if (id != video.Id)
            {
                return BadRequest();
            }

            _videoRepository.Update(video);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _videoRepository.Delete(id);
            return NoContent();
        }

        [HttpGet("GetWithComments")]
        public IActionResult GetWithComments()
        {
            var videos = _videoRepository.GetAllWithComments();
            return Ok(videos);
        }

        [HttpGet("GetVideoByIdWithComments{id}")]
        public IActionResult GetVideoByIdWithComments(int id)
        {
            Video video = _videoRepository.GetVideoByIdWithComments(id);
            if(video == null)
            {
                return NotFound();
            }
            return Ok(video);
        }
        [HttpGet("search")]
        public IActionResult Search(string q, bool sortDesc)
        {
            return Ok(_videoRepository.Search(q, sortDesc));
        }

        [HttpGet("Hottest")]
        public IActionResult Hottest(DateTime q)
        {
            List<Video> videos = _videoRepository.Hottest(q);
            try
            {
                return Ok(videos);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}